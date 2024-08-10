--Lua Dictionary

---@class Dictionary
Dictionary = Class {}

function Dictionary:Constructor()
    self.Data = {}
end

function Dictionary:Add(key, value)
    if key == nil or value == nil then
        LOG("Error: Dictionary key or value is nil!")
        return
    end

    if self.Data[key] ~= nil then
        LOG("Error: Dictionary key has already exists!")
        return
    end

    self.Data[key] = value;
end

function Dictionary:Remove(key)
    if key == nil then
        LOG("Error: Dictionary key is nil!")
        return
    end

    if self.Data[key] ~= nil then
        self.Data[key] = nil
    end
end

function Dictionary:ContainsKey(key)
    if key == nil then
        LOG("Error: Dictionary key is nil!")
        return
    end

    return self.Data[key] ~= nil;
end

function Dictionary:ContainsValue(value)
    if value == nil then
        LOG("Error: Dictionary value is nil!")
        return
    end

    for k, v in pairs(self.Data) do
        if v == value then
            return true
        end
    end
    return false
end

function Dictionary:IsEmpty()
    local size = self:Count()
    return size == 0;
end

function Dictionary:Count()
    local size = 0;
    for k, v in pairs(self.Data) do
        size = size + 1
    end
    return size
end

function Dictionary:Clear()
    self.Data = {}
end

--------------------

function Dictionary:Get(key)
    if key == nil then
        LOG("Error: Dictionary key is nil!")
        return nil
    end

    return self.Data[key]
end

function Dictionary:Foreach(handler)
    for k, v in pairs(self.Data) do
        handler(k, v)
    end
end

return Dictionary
