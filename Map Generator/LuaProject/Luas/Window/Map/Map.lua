---@type MapModel
local MapModel = require('Window.Map.MapModel')
---@type MapMeta
local MapMeta = require('Window.Map.MapMeta')
---@type MapMeta
local Map = MapMeta:Extends{}

---初始化---
function Map:OnInit()
    MapModel:OnInit();
    MapMeta.OnInit(self);

    -- self.CameraRoot = GameUtil.Find("CameraRoot", typeof(UnityEngine.Transform))
    -- self.MainCamera = LuaUtil.GetMainCamera()
    -- self.MainCamera.transform:SetParent(self.CameraRoot)
    -- self.MainCamera.transform.localRotation = UnityEngine.Quaternion.Euler(UnityEngine.Vector3(90, 0, 0))
    -- self.MainCamera.transform.localPosition = UnityEngine.Vector3(0, 10, 0)

end

---销毁---
function Map:OnDispose()
    MapMeta.OnDispose(self);
    MapModel:OnDispose();
end

---没有具体逻辑注释掉---
-- function Map:OnUpdate(dt)

-- end

---打开执行一次---
function Map:OnOpen(data)
    -- bg init
    WindowManager.OpenWindowOutStack("Background", "Battle")

end

---刷新执行一次---
function Map:OnRefresh(data)

end

---关闭执行一次---
function Map:OnClose()

end

---暂停---
function Map:OnPause()
    WindowManager.CloseWindow()

end

---恢复---
function Map:OnResume()

end

return Map
