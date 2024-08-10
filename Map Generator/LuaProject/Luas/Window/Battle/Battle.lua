---@type BattleModel
local BattleModel = require('Window.Battle.BattleModel')
---@type BattleMeta
local BattleMeta = require('Window.Battle.BattleMeta')
---@type BattleMeta
local Battle = BattleMeta:Extends{}

---初始化---
function Battle:OnInit()
    BattleModel:OnInit();
    BattleMeta.OnInit(self);

end

---销毁---
function Battle:OnDispose()
    BattleMeta.OnDispose(self);
    BattleModel:OnDispose();
end

---没有具体逻辑注释掉---
-- function Battle:OnUpdate(dt)

-- end

---打开执行一次---
function Battle:OnOpen(data)
    -- bg init
    WindowManager.OpenWindowOutStack("Background", "Battle")

end

---刷新执行一次---
function Battle:OnRefresh(data)

end

---关闭执行一次---
function Battle:OnClose()

end

---暂停---
function Battle:OnPause()
    WindowManager.CloseWindow()

end

---恢复---
function Battle:OnResume()

end

return Battle
