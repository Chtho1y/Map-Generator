local InitScene = Class {}

function InitScene:Init()
    -- e.g.
    -- WindowManager.RegisterWindow("BackGround", true, WindowLayer.Top)
    -- WindowManager.RegisterWindow("NewUIWindow", false)

    -- WindowManager.OpenWindow("NewUIWindow")

    WindowManager.RegisterWindow("Background", true, WindowLayer.Top)
    WindowManager.RegisterWindow("Login", false, WindowLayer.Default)

    WindowManager.OpenWindow("Login")

end

function InitScene:Dispose()

end

function InitScene:Update(dt)

end

return InitScene
