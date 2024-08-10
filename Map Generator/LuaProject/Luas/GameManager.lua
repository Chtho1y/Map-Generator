UnityEngine = CS.UnityEngine
DarlingEngine = CS.DarlingEngine
TMPro = CS.TMPro

GameManager = DarlingEngine.Engine.GameManager
GM = GameManager.Instance

---@type DarlingEngine.Engine.GameUtil
GameUtil = DarlingEngine.Engine.GameUtil
---@type DarlingEngine.Main.GameRoot
GameRoot = DarlingEngine.Main.GameRoot
---@type DarlingEngine.Engine.Random
Random = DarlingEngine.Engine.Random
---@type DarlingEngine.Engine.TimeUtil
TimeUtil = DarlingEngine.Engine.TimeUtil
---@type DarlingEngine.Engine.StringUtil
StringUtil = DarlingEngine.Engine.StringUtil
---@type DarlingEngine.Engine.TweenUtil
TweenUtil = DarlingEngine.Engine.TweenUtil
---@type DarlingEngine.UI.UIUtil
UIUtil = DarlingEngine.UI.UIUtil
---@type DarlingEngine.Engine.Pool.GameObjectPoolManager
PoolManager = DarlingEngine.Engine.Pool.GameObjectPoolManager

require('Common.Basic')
require('Common.Class')
require('Common.Singleton')
require('Common.Config')

require('Common.DataStructure.Dictionary')
require('Common.DataStructure.List')
require('Common.DataStructure.Queue')
require('Common.DataStructure.Stack')

require("Define.Game")

require("Framework.GameCache")
require("Framework.LuaUtil")
require("Framework.UnityEventHelper")
require("Framework.Window")
require("Framework.WindowManager")
require("Framework.TimerManager")
require("Framework.Logger")

---@type GameController
local gameController = require('Framework.GameController')

function OnGameEnter()
    TimerManager:Constructor()
    gameController:Init()
end

-- Csharp层检查更新结束之后执行
function OnCheckUpdateFinished()
    gameController:OnCheckUpdateFinished()
end

function OnGameUpdate()
    TimerManager:Update()
    gameController:Update()
end

function OnGameFixedUpdate()
    if gameController.FixedUpdate ~= nil then
        gameController:FixedUpdate()
    end
end

function OnGameLateUpdate()
    TimerManager:LateUpdate()
    if gameController.LateUpdate ~= nil then
        gameController:LateUpdate()
    end
end

function OnGameSceneChanged(name)
    gameController:OnSceneChanged(name)
end

function OnGameException(appException)
    if gameController.OnException ~= nil then
        gameController:OnException(appException)
    end
end

function OnGameLowMemory()
    if gameController.OnLowMemory ~= nil then
        gameController:OnLowMemory()
    end
end

function OnApplicationFocus(focus)
    if gameController.OnApplicationFocus ~= nil then
        gameController:OnApplicationFocus(focus)
    end
end

function OnApplicationPause(pause)
    if gameController.OnApplicationPause ~= nil then
        gameController:OnApplicationPause(pause)
    end
end

function OnApplicationQuit()
    TimerManager:Dispose()
    gameController:Dispose()
end

return 'GM'
