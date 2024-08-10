Basic = {}

function Basic.Enum(t)
    start_index = 1
    for i, v in ipairs(t) do
        t[v] = start_index
        start_index = start_index + 1
    end
    return t;
end

return Basic
