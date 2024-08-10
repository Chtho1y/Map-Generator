---窗口基类
Window = Class {}

function Window:Init(name, node, info)
    self.Name = name
    ---@type UnityEngine.GameObject
    self.Node = node
    self.OutStack = info.outStack
    self.IsFull = info.isFull
    self.IsOpened = false
    self.CanvasGroup = GameUtil.GetOrAddComponent(self.Node, typeof(UnityEngine.CanvasGroup))
    --初始化
    self:OnInit()
    --注册控件
    self:OnRegister()
end

function Window:Dispose()
    --移除控件
    self:OnRemove()
    --释放
    self:OnDispose()
    --销毁节点
    UnityEngine.Object.Destroy(self.Node)

    --[[for k,v in pairs(self) do
        self[k] = nil
    end]]
end

function Window:Update(dt)
    if not (self.Node and self.IsOpened) then return end
    if self.OnUpdate ~= nil then self:OnUpdate(dt) end
end

function Window:CheckNode()
    if self.Node == nil then
        LOG("Node is nil")
        return false
    end
    return true
end

function Window:Open(data)
    if not self:CheckNode() then return end

    self.CanvasGroup.blocksRaycasts = true
    self.Node.transform:SetAsLastSibling()
    self.Node:SetActive(true)

    if not self.IsOpened then
        self.IsOpened = true
        self:OnOpen(data)
    end

    if data ~= nil then
        self:OnRefresh(data)
    end
end

function Window:Close()
    if not self:CheckNode() then return end

    self.Node.transform:SetAsFirstSibling()
    self.Node:SetActive(false)

    if self.IsOpened then
        self.IsOpened = false
        self:OnClose()
    end
end

function Window:Pause()
    --if self.Node == nil or self.IsOpened ~= true then return end
    if self.Node == nil then return end

    --暂停
    self.CanvasGroup.blocksRaycasts = false

    self:OnPause()
end

function Window:Resume()
    --if self.Node == nil or self.IsOpened ~= true then return end
    if self.Node == nil then return end

    --恢复
    self.CanvasGroup.blocksRaycasts = true

    self:OnResume()
end

--------------------------

function Window:OnInit()

end

function Window:OnDispose()

end

function Window:OnOpen(data)

end

function Window:OnRefresh(data)

end

function Window:OnClose()

end

function Window:OnPause()

end

function Window:OnResume()

end

function Window:OnRegister()

end

function Window:OnRemove()

end

return Window
