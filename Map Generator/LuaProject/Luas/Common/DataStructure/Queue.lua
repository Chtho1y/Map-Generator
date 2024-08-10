--Lua Queue

---@class Queue
Queue = Class {}

function Queue:Constructor()
    self.Data = {}
end

function Queue:Enqueue(e)
    if e == nil then
        print("Error: Queue element is nil!")
        return
    end

    local size = self:Count()
    self.Data[size + 1] = e
end

function Queue:Dequeue()
    if self:IsEmpty() then
        print("Error: Queue is empty.")
        return nil
    end

    return table.remove(self.Data, 1)
end

function Queue:IsEmpty()
    local size = self:Count()
    return size == 0;
end

function Queue:Count()
    return #self.Data or 0;
end

function Queue:Contains(e)
    for k, v in pairs(self.Data) do
        if v == e then
            return true
        end
    end
    return false
end

function Queue:Clear()
    self.Data = nil
    self.Data = {}
end

function Queue:Print()
    if self:IsEmpty() then
        print("Error: Queue is empty!")
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

    print(str)
end

--------------------

function Queue:Foreach(handler)
    if self:Count() <= 0 then return end

    for i = 1, self:Count() do
        handler(i, self.Data[i])
    end
end

return Queue
