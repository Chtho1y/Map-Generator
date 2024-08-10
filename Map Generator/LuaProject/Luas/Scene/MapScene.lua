local MapScene = Class {}

function MapScene:Init()

    WindowManager.RegisterWindow("Background", true, WindowLayer.Top)
    WindowManager.RegisterWindow("Map", false, WindowLayer.Default)

    WindowManager.OpenWindow("Map")

end

function MapScene:Dispose()

end

function MapScene:Update(dt)

end

return MapScene
