local class = {}

function class:Extends(o)
    o = o or {}
    setmetatable(o, self)
    return Class(o, self)
end

function class:Constructor()
    -- Base constructor
end

function Class(this, base, isSingleton)
    this = this or {}
    base = base or class
    setmetatable(this, base)
    base.__index = base
    -- Call constructor only if it's not a singleton instance
    if not isSingleton then
        this:Constructor()
    end
    return this
end
