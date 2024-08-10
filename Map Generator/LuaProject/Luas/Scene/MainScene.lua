local MainScene = Class {}

function MainScene:Init()

    WindowManager.RegisterWindow("Background", true, WindowLayer.Top)
    WindowManager.RegisterWindow("MainMenu", false, WindowLayer.Default)

    WindowManager.OpenWindow("MainMenu")

end

function MainScene:Dispose()

end

function MainScene:Update(dt)

end

return MainScene
