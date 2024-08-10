---@class GameController
local GameController = Class {}

local currentScene = nil

function GameController:Init()

end

function GameController:OnCheckUpdateFinished()
    local scene = require("Scene.InitScene")
    currentScene = scene:Extends{}
    currentScene:Init()
end

function GameController:Dispose()

end

function GameController:Update()
    local dt = UnityEngine.Time.deltaTime
    assert(type(dt) == "number", "GameController:Update: dt is not number")

    WindowManager.Update(dt)
    if currentScene ~= nil then
        currentScene:Update(dt)
    end
end

function GameController:FixedUpdate()
    local dt = UnityEngine.Time.fixedDeltaTime
end

function GameController:LateUpdate()

end

function GameController:OnException(appException)
    -- LOG(appException)
end

function GameController:OnLowMemory()
    -- LOG("Low Memory")
end

function GameController:OnApplicationFocus(isFocus)

end

function GameController:OnApplicationPause(isPause)

end

function GameController:OnSceneChanged(sceneName)
    WindowManager.OnSceneChanged(sceneName)

    if currentScene ~= nil then
        currentScene:Dispose()
    end

    GameRoot.Instance.LoaderProgress:End()

    local scene = nil
    if sceneName == "Init" then
        scene = require("Scene.InitScene")
    elseif sceneName == "Main" then
        scene = require("Scene.MainScene")
    elseif sceneName == "Battle" then
        scene = require("Scene.BattleScene")
    elseif sceneName == "Map" then
        scene = require("Scene.MapScene")
    end

    if scene ~= nil then
        currentScene = scene:Extends{}
        currentScene:Init()
    end
end

return GameController
