--Lua List

---@class List
List = Class {}

function List:Constructor()
    self.Data = {}
end

function List:Add(e)
    if e == nil then
        LOG("Error: List element is nil!")
        return
    end

    local size = self:Count()
    self.Data[size + 1] = e
end

function List:Remove(e)
    if e == nil then
        LOG("Error: List element is nil!")
        return
    end

    if self:IsEmpty() then
        LOG("Error: List is empty!")
        return
    end

    for k, v in pairs(self.Data) do
        if v == e then
            table.remove(self.Data, k);
            break;
        end
    end
end

function List:RemoveAt(i)
    if self:IsEmpty() then
        LOG("Error: List is empty!")
        return
    end

    local size = self:Count();
    if i <= 0 or i > size then
        LOG("Error: List index was out of range!")
        return
    end
    table.remove(self.Data, i);
end

function List:IsEmpty()
    local size = self:Count()
    return size == 0;
end

function List:Count()
    return #self.Data or 0;
end

function List:Contains(e)
    for k, v in pairs(self.Data) do
        if v == e then
            return true
        end
    end
    return false
end

function List:Clear()
    self.Data = nil
    self.Data = {}
end

function List:LOG()
    if self:IsEmpty() then
        LOG("Error: List is empty!")
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

function List:Get(index)
    if index > self:Count() then
        LOG("Error: List index is out of scope!")
        return nil
    end

    return self.Data[index]
end

function List:Foreach(handler)
    if self:Count() <= 0 then return end

    for i = 1, self:Count() do
        handler(i, self.Data[i])
    end
end

return List
