local BattleScene = Class {}

function BattleScene:Init()

    WindowManager.RegisterWindow("Background", true, WindowLayer.Top)
    WindowManager.RegisterWindow("Battle", false, WindowLayer.Default)

    WindowManager.OpenWindow("Battle")

end

function BattleScene:Dispose()

end

function BattleScene:Update(dt)

end

return BattleScene
