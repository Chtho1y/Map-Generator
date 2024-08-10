---@type MainMenuModel
local MainMenuModel = require('Window.MainMenu.MainMenuModel')
---@type MainMenuMeta
local MainMenuMeta = require('Window.MainMenu.MainMenuMeta')
---@type MainMenuMeta
local MainMenu = MainMenuMeta:Extends{}

---初始化---
function MainMenu:OnInit()
    MainMenuModel:OnInit();
    MainMenuMeta.OnInit(self);

end

---销毁---
function MainMenu:OnDispose()
    MainMenuMeta.OnDispose(self);
    MainMenuModel:OnDispose();
end

---没有具体逻辑注释掉---
-- function MainMenu:OnUpdate(dt)

-- end

---打开执行一次---
function MainMenu:OnOpen(data)
    -- bg init
    WindowManager.OpenWindowOutStack("Background", "MainMenu")

end

---刷新执行一次---
function MainMenu:OnRefresh(data)

end

---关闭执行一次---
function MainMenu:OnClose()

end

---暂停---
function MainMenu:OnPause()
    WindowManager.CloseWindow()

end

---恢复---
function MainMenu:OnResume()

end

------------------------------
---事件函数---
function MainMenuMeta:On_DLButton_ButtonTestBattle_Click()
    LuaUtil.LoadScene("Battle")
end

---事件函数---
function MainMenuMeta:On_DLButton_ButtonTestMap_Click()
    LuaUtil.LoadScene("Map")
end

return MainMenu
