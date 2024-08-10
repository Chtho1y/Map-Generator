Singleton = Class {}

local singletonInstances = {}

function Singleton:Extends(o)
    o = o or {}
    o.isSingleton = true

    if o.Singleton then
        -- Return existing instance if available
        if singletonInstances[o.Singleton] then
            return singletonInstances[o.Singleton]
        else
            -- Create and store new instance
            setmetatable(o, self)
            local instance = Class(o, self)
            singletonInstances[o.Singleton] = instance
            return instance
        end
    else
        -- Regular class extension
        setmetatable(o, self)
        return Class(o, self)
    end
end
