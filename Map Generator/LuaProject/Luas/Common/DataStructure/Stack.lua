--Lua Stack

---@class Stack
Stack = Class {}

function Stack:Constructor()
    self.Data = {}
end

function Stack:Push(e)
    if e == nil then
        LOG("Error: Stack element is nil!")
        return
    end
    local size = self:Count()
    self.Data[size + 1] = e
end

function Stack:Pop()
    if self:IsEmpty() then
        LOG("Error: Stack is empty!")
        return nil;
    end
    local size = self:Count()
    return table.remove(self.Data, size)
end

function Stack:Peek()
    if self:IsEmpty() then
        LOG("Error: Stack is empty!")
        return nil;
    end
    local size = self:Count()
    return self.Data[size]
end

function Stack:IsEmpty()
    local size = self:Count()
    return size == 0;
end

function Stack:Count()
    return #self.Data or 0
end

function Stack:Contains(e)
    for k, v in pairs(self.Data) do
        if v == e then
            return true
        end
    end
    return false
end

function Stack:Clear()
    self.Data = nil
    self.Data = {}
end

function Stack:LOG()
    if self:IsEmpty() then
        LOG("Error: Stack is empty!")
        return
    end
    local size = self:Count()
    local i = 1;

    local str = "{ " .. self.Data[i]
    i = i + 1;

    while i <= size do
        str = str .. ", " .. self.Data[i]
        i = i + 1;
    end
    str = str .. " }"

    LOG(str)
end

--------------------

function Stack:Foreach(handler)
    if self:Count() <= 0 then return end

    for i = 1, self:Count() do
        handler(i, self.Data[i])
    end
end

return Stack
