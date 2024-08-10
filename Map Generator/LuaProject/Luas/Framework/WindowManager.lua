---窗口管理器
WindowManager = Singleton:Extends { Singleton = "WindowManager" }

WindowLayer = {}
WindowLayer.Default = 0
WindowLayer.Front = 1
WindowLayer.High = 2
WindowLayer.Top = 3

local WindowTable = Dictionary:Extends {}
local WindowStack = Stack:Extends {}
local WindowInfo = Dictionary:Extends {}


function WindowManager.Update(dt)
    for k, v in pairs(WindowTable.Data) do
        --只更新当前激活或可见的窗口
        if v ~= nil and v.IsOpened then
            v:Update(dt)
        end
    end
end

function WindowManager.OnSceneChanged(name)
    for k, v in pairs(WindowTable.Data) do
        if v ~= nil then
            v:Close()
            v:Dispose()
        end
    end

    WindowTable:Clear()
    WindowStack:Clear()
    WindowInfo:Clear()
end

--注册窗口(窗户是否需要显示由其上层窗口是否全屏决定)
function WindowManager.RegisterWindow(name, outStack, layer)
    if not WindowInfo:ContainsKey(name) then
        outStack = outStack or false
        layer = layer or WindowLayer.Default

        local info = { outStack = outStack, layer = layer }

        WindowInfo:Add(name, info)
    end
end

function WindowManager.OpenWindow(name, data)
    if not WindowInfo:ContainsKey(name) then
        LOG("The window is not exist to open: " .. name)
        return
    end

    if WindowInfo:Get(name).outStack then
        LOG("The window is wrong type to open: " .. name)
        return
    end

    local window = WindowTable:Get(name)
    if window == nil then
        WindowManager.CreateAndOpenWindow(name, data)
        return
    end

    local isStackContains = WindowManager.IsWindowStackContains(name)
    if isStackContains then
        --重新排列窗口
        WindowManager.RankWindow(window, data)
    else
        --未入栈
        if not WindowStack:IsEmpty() then
            local lastWindow = WindowStack:Peek()
            lastWindow:Pause()
        end
        WindowStack:Push(window)

        --隐藏窗口
        window:Open(data)
    end
end

function WindowManager.OpenWindowOutStack(name, data)
    if not WindowInfo:ContainsKey(name) then
        LOG("The window is not exist to open: " .. name)
        return
    end

    if not WindowInfo:Get(name).outStack then
        LOG("the window is wrong type to open: "..name)
        return
    end

    local window = WindowTable:Get(name)
    if window ~= nil then
        window:Open(data)
    else
        WindowManager.CreateAndOpenWindow(name, data)
    end
end

function WindowManager.CloseWindowOutStack(name, dispose)
    if not WindowInfo:ContainsKey(name) then
        LOG("The window is not exit to close: " .. name)
        return
    end

    local info = WindowInfo:Get(name)

    if not info.outStack then
        LOG("The window is wrong type to close: " .. name)
        return
    end

    local window = WindowTable:Get(name)
    if window ~= nil then
        window:Close()
        if dispose then
            window:Dispose()
            WindowTable:Remove(name)
        end
    else
        LOG("the window instance is not exit to close: " .. name)
    end
end

--关闭栈窗口
function WindowManager.CloseWindow(dispose)
    if WindowStack:Count() < 1 then
        LOG("The window stack has no window to close !")
        return
    end

    local window = WindowStack:Pop()
    window:Close()

    if dispose then
        window:Dispose()
        WindowTable:Remove(window.Name)
    end

    if not WindowStack:IsEmpty() then
        WindowStack:Peek():Resume()
    end
end

function WindowManager.GetNodeParentByLayer(layer)
    local layerMappings = {
        [WindowLayer.Default] = "DefaultLayer",
        [WindowLayer.Front] = "FrontLayer",
        [WindowLayer.High] = "HighLayer",
        [WindowLayer.Top] = "TopLayer"
    }
    local layerName = layerMappings[layer] or "DefaultLayer"
    return GameUtil.Find(layerName)
end

function WindowManager.CreateAndOpenWindow(name, data)
    local info = WindowInfo:Get(name)
    local nodeParent = WindowManager.GetNodeParentByLayer(info.layer)

    if nodeParent == nil then
        LOG("The window layer is not exist: " .. name)
        return
    end

    -- UI 节点
    local prefab = LuaUtil.LoadAsset("Prefabs/Window/" .. name .. ".prefab", typeof(UnityEngine.GameObject))
    if prefab == nil then LOG("Failed to load prefab for window: " .. name) return end
    local node = UnityEngine.Object.Instantiate(prefab)
    node.transform:SetParent(nodeParent.transform, false)
    node.name = name

    -- Window
    local windowScript = require('Window.' .. name .. '.' .. name)
    local window = windowScript:Extends {}
    WindowTable:Add(name, window)

    --入栈
    if not info.outStack then
        if not WindowStack:IsEmpty() then
            local lastWindow = WindowStack:Peek()
            lastWindow:Pause()
        end
        WindowStack:Push(window)
    end

    window:Init(name, node, info)
    window:Open(data)
end

function WindowManager.PauseWindowOutStack(name)
    local window = WindowTable:Get(name)
    if window == nil then
        LOG("The window is not exit to pause: " .. name)
        return
    end

    if WindowInfo:Get(name).outStack ~= true then
        LOG("The window is wrong type to pause: " .. name)
        return
    end

    window:Pause()
end

function WindowManager.ResumeWindowOutStack(name)
    local window = WindowTable:Get(name)
    if window == nil then
        LOG("The window is not exit to resume: " .. name)
        return
    end

    if WindowInfo:Get(name).outStack ~= true then
        LOG("The window is wrong type to resume: " .. name)
        return
    end

    window:Resume()
end

function WindowManager.IsWindowOpened(name)
    local window = WindowTable:Get(name)
    return window and window.IsOpened == true or false
end

function WindowManager.GetTopWindow()
    local window = WindowStack:Peek()
    return window and window.Name or nil
end

function WindowManager.GetTopWindowName()
    local window = WindowStack:Peek()
    return window.Name or nil
end

--返回窗口
function WindowManager.BackWindow(name, data, dispose)
    local info = WindowInfo:Get(name)
    if info ~= nil and not info.outStack then
        if WindowManager.GetTopWindow() == name then
            LOG("The window is the top to back: " .. name)
            return
        end

        local isContains = WindowManager.IsWindowStackContains(name)
        if isContains then
            local count = WindowStack:Count()
            for i = count, 1, -1 do
                local topWindow = WindowStack:Peek()
                if topWindow.Name ~= name then
                    --删除窗口
                    topWindow:Close()
                    WindowStack:Pop()
                    if dispose then
                        topWindow:Dispose()
                        WindowTable:Remove(topWindow.Name)
                    end
                else
                    topWindow:Open(data)
                    topWindow:Resume()
                    break
                end
            end
        else
            LOG("The window is not in stack to back: " .. name)
        end
    else
        LOG("The window is not exit or wrong type to back: " .. name)
    end
end

--排列栈窗口
function WindowManager.RankWindow(window, data)
    local lowList = List:Extends {}
    local highList = List:Extends {}
    local tempStack = Stack:Extends {}

    local isHigh = false

    WindowStack:Foreach(function (i, v)
        if v.Name == window.Name then
            isHigh = true
        end
        if isHigh then
            highList:Add(v)
        else
            lowList:Add(v)
        end
    end)

    lowList:Foreach(function (i, v)
        tempStack:Push(v)
    end)

    highList:Foreach(function (i, v)
        if v ~= window then
            tempStack:Push(v)
        end
    end)

    if not tempStack:IsEmpty() then
        tempStack:Peek():Pause()
    end
    tempStack:Push(window)
    WindowStack = tempStack

    --隐藏窗口
    window:Open(data)
    window:Resume()
end

--是否入栈
function WindowManager.IsWindowStackContains(name)
    local isContains = false
    for i, v in ipairs(WindowStack.Data) do
        if v.Name == name then
            isContains = true
            break
        end
    end
    return isContains
end

return WindowManager
