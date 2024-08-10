---事件系统(全局单例)
UnityEventHelper = Singleton:Extends { Singleton = "UnityEventHelper" }

--UI事件
UnityEventHelper.Click = DarlingEngine.UI.UIEvent.Click
UnityEventHelper.ValueChanged = DarlingEngine.UI.UIEvent.ValueChanged
UnityEventHelper.EndEdit = DarlingEngine.UI.UIEvent.EndEdit
UnityEventHelper.Submit = DarlingEngine.UI.UIEvent.Submit
UnityEventHelper.Drag = DarlingEngine.UI.UIEvent.Drag
UnityEventHelper.Down = DarlingEngine.UI.UIEvent.Down
UnityEventHelper.Up = DarlingEngine.UI.UIEvent.Up
UnityEventHelper.Selected = DarlingEngine.UI.UIEvent.Selected



function UnityEventHelper.RegisterEvent(owner, eventName, handler)
    DarlingEngine.Engine.UnityEventHelper.RegisterEvent(owner, eventName, handler);
end

function UnityEventHelper.RemoveEvent(owner, eventName)
    DarlingEngine.Engine.UnityEventHelper.RemoveEvent(owner, eventName);
end

function UnityEventHelper.RemoveEventsOn(owner)
    DarlingEngine.Engine.UnityEventHelper.RemoveEventsOn(owner);
end

function UnityEventHelper.SendEvent(owner, eventName, data)
    DarlingEngine.Engine.UnityEventHelper.SendEvent(owner, eventName, data);
end

function UnityEventHelper.RegisterGlobalEvent(eventName, handler)
    DarlingEngine.Engine.UnityEventHelper.RegisterGlobalEvent(eventName, handler);
end

function UnityEventHelper.RemoveGlobalEvent(eventName)
    DarlingEngine.Engine.UnityEventHelper.RemoveGlobalEvent(eventName);
end

function UnityEventHelper.SendGlobalEvent(eventName, data)
    DarlingEngine.Engine.UnityEventHelper.SendGlobalEvent(eventName, data);
end

function UnityEventHelper.Clear()
    DarlingEngine.Engine.UnityEventHelper.Clear()
end

return UnityEventHelper
