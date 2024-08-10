---@type BackgroundModel
local BackgroundModel = require('Window.Background.BackgroundModel')
---@type BackgroundMeta
local BackgroundMeta = require('Window.Background.BackgroundMeta')
---@type BackgroundMeta
local Background = BackgroundMeta:Extends{}

---初始化---
function Background:OnInit()
    BackgroundModel:OnInit();
    BackgroundMeta.OnInit(self);

    self.BgRoot = GameUtil.Find("BgRoot", typeof(UnityEngine.Transform))
    self.Node.transform:SetParent(self.BgRoot.transform)
    self.NowBG = nil
end

---销毁---
function Background:OnDispose()
    BackgroundMeta.OnDispose(self);
    BackgroundModel:OnDispose();
end

---没有具体逻辑注释掉---
-- function Background:OnUpdate(dt)

-- end

---打开执行一次---
function Background:OnOpen(data)

end

---刷新执行一次---
function Background:OnRefresh(data)
    if self.NowBG ~= nil then
        self.NowBG.gameObject:SetActive(false)
        self.NowBG = nil
    end

    self.NowBG = GameUtil.FindChild(self.Node, data, typeof(UnityEngine.Transform))

    self.NowBG.gameObject:SetActive(true)
    self:ResetRectTransform(self.NowBG)
end

---关闭执行一次---
function Background:OnClose()

end

---暂停---
function Background:OnPause()

end

---恢复---
function Background:OnResume()

end

function Background:ResetRectTransform(rectTrans)
    rectTrans.localPosition = UnityEngine.Vector3.zero
    rectTrans.localScale = UnityEngine.Vector3.one
    rectTrans.sizeDelta = UnityEngine.Vector2.zero
    rectTrans.anchorMin = UnityEngine.Vector2.zero
    rectTrans.anchorMax = UnityEngine.Vector2.one
    rectTrans.pivot = UnityEngine.Vector2.one / 2
end

return Background
