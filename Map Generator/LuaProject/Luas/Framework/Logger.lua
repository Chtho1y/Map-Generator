LOG = function(...)
    local msg = ""
    local arr = {...}
    for i = 1, #arr do
        msg = msg .. " " .. tostring(arr[i])
    end
    UnityEngine.Debug.Log(msg)
end

WARN = function(message)
    UnityEngine.Debug.LogWarning(message)
end

ERROR = function(message)
    UnityEngine.Debug.LogError(message)
end

EXCEPTION = function(message)
    local e = CS.System.Exception(message)
    UnityEngine.Debug.LogException(e)
end
