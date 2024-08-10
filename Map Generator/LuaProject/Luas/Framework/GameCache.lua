GameCache = {}


local _cache = {}

function GameCache.Dispose()
    _cache = nil
end

local loginCacheKeys = Basic.Enum({
    "loginType", "loginData", "playerID", "playerName", "playerAvatar"
})

---@param loginType number
---@param data table
function GameCache.SetPlayerLoginData(loginType, data)
    _cache[loginCacheKeys.loginType] = loginType
    _cache[loginCacheKeys.loginData] = data
end

---@return number, table
function GameCache.GetPlayerLoginData()
    return _cache[loginCacheKeys.loginType], _cache[loginCacheKeys.loginData]
end

---@param playerID number
function GameCache.SetPlayerID(playerID)
    _cache[loginCacheKeys.playerID] = playerID
end

---@return number
function GameCache.GetPlayerID()
    return _cache[loginCacheKeys.playerID]
end

---@param playerName string
function GameCache.SetPlayerName(playerName)
    _cache[loginCacheKeys.playerName] = playerName
end

---@return string
function GameCache.GetPlayerName()
    return _cache[loginCacheKeys.playerName]
end

---@param playerAvatar string
function GameCache.SetPlayerAvatar(playerAvatar)
    _cache[loginCacheKeys.playerAvatar] = playerAvatar
end

---@return string
function GameCache.GetPlayerAvatar()
    return _cache[loginCacheKeys.playerAvatar]
end

return GameCache
