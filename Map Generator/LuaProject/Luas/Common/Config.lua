function Readonly(tab)
    local this = {}
    local meta = { __index = tab, __newindex = function (t, k, v) error('can not write readonly: ' .. k) end }
    setmetatable(this, meta)
    return this
end

Config = Class {}

function Config:Extends(o)
    local tab = {}
    for i, v in ipairs(o) do
        table.insert(tab, Readonly(v))
    end
    setmetatable(tab, self)
    self.__index = self
    return Readonly(tab)
end

function Config:Get(k)
    for i, v in ipairs(self) do
        if tostring(v.Id) == tostring(k) then
            return v
        end
    end
    return nil
end

function Config:Foreach(handler)
    for i, v in ipairs(self) do
        handler(i, v)
    end
end

function Config:Count()
    local count = 0
    for i, v in ipairs(self) do
        count = count + 1
    end
    return count
end
