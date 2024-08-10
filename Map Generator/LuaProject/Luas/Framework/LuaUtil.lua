---LuaUtil(全局单例)
LuaUtil = Singleton:Extends{
    Singleton = "LuaUtil"
}

---加载资源(同步)
function LuaUtil.LoadAsset(path, type)
    return GM:Load(path, type)
end

---加载资源(异步)
function LuaUtil.LoadAssetAsync(path, type, handler)
    GM:LoadAsync(path, type, handler)
end

---加载场景(异步)
function LuaUtil.LoadScene(name, isBundle, onLoaded, onLoading)
    isBundle = isBundle or false
    GM:LoadScene(name, isBundle, onLoaded, onLoading)
end

---优点，使用Unity事件调度函数Invoke()，性能优秀，高效
---缺陷，只能同时完成一个动作的调用，有新的调用会清空正在进行的任务
---@param delayTime float
function LuaUtil.WaitInvoke(delayTime, handler)
    return GM:WaitInvoke(delayTime, handler)
end

---@param delayTime float
---@param repeatRate float 重复调用的时间间隔
function LuaUtil.RepeatInvoke(delayTime, repeatRate, handler)
    return GM:WaitInvoke(delayTime, repeatRate, handler)
end

---等待x秒执行
function LuaUtil.WaitTime2Do(seconds, handler)
    return GM:WaitTime2Do(seconds, handler)
end

---等待x个帧执行
function LuaUtil.WaitFrame2Do(frameCount, handler)
    return GM:WaitFrame2Do(frameCount, handler)
end

---取消等待执行
function LuaUtil.CancelWait2Do(waitCoroutine)
    GM:CancelWait2Do(waitCoroutine)
end

---取消所有等待执行
function LuaUtil.CancelAllWait2Do()
    GM:CancelAllWait2Do()
end

---获取UI相机
function LuaUtil.GetUICamera()
    return GM:GetUICamera()
end

---获取主相机
function LuaUtil.GetMainCamera()
    return GM:GetMainCamera()
end

return LuaUtil
