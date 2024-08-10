---@class MainMenuMeta
local MainMenuMeta = Window:Extends {}

---初始化控件---
function MainMenuMeta:OnInit()
    ---@type DarlingEngine.UI.DLButton
    self.DLButton_ButtonTestBattle = GameUtil.FindChild(self.Node, 'ButtonTestBattle', typeof(DarlingEngine.UI.DLButton));
    ---@type DarlingEngine.UI.DLButton
    self.DLButton_ButtonTestMap = GameUtil.FindChild(self.Node, 'ButtonTestMap', typeof(DarlingEngine.UI.DLButton));

end

---销毁控件---
function MainMenuMeta:OnDispose()
    self.DLButton_ButtonTestBattle:Dispose();
    self.DLButton_ButtonTestBattle = nil;
    self.DLButton_ButtonTestMap:Dispose();
    self.DLButton_ButtonTestMap = nil;

end

---注册控件---
function MainMenuMeta:OnRegister()
    UnityEventHelper.RegisterEvent(self.DLButton_ButtonTestBattle, UnityEventHelper.Click, function (data) self:On_DLButton_ButtonTestBattle_Click(data) end);
    UnityEventHelper.RegisterEvent(self.DLButton_ButtonTestMap, UnityEventHelper.Click, function (data) self:On_DLButton_ButtonTestMap_Click(data) end);

end

---移除控件---
function MainMenuMeta:OnRemove()
    UnityEventHelper.RemoveEvent(self.DLButton_ButtonTestBattle, UnityEventHelper.Click);
    UnityEventHelper.RemoveEvent(self.DLButton_ButtonTestMap, UnityEventHelper.Click);

end

------------------------------
---事件函数---
function MainMenuMeta:On_DLButton_ButtonTestBattle_Click()
end
---事件函数---
function MainMenuMeta:On_DLButton_ButtonTestMap_Click()
end


return MainMenuMeta
