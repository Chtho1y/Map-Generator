require('Framework.Timer')

---@class TimerManager
TimerManager = Singleton:Extends { Singleton = "TimerManager" }

function TimerManager:Constructor()
    ---@type Timer[]
    self.timers = {}
    ---@type FrameTimer[]
    self.frametimers = {}

    -- 下面两个是对象池
    self.timerPool = {}
    self.frametimerPool = {}
end

function TimerManager:Update()
    for i = 1, #self.timers do
        if self.timers[i] then
            self.timers[i]:Update()
        end
    end
end

function TimerManager:LateUpdate()
    for i = 1, #self.frametimers do
        if self.frametimers[i] then
            self.frametimers[i]:LateUpdate()
        end
    end
end

function TimerManager:Dispose()
    self:ClearAllTimers()
end

---@param delaySeconds number
---@param func fun():void
function TimerManager:Delay(delaySeconds, func)
    return self:Loop(delaySeconds, 1, func)
end

---@param delayFrame number
---@param func fun():void
function TimerManager:DelayFrame(delayFrame, func)
    return self:LoopFrame(delayFrame, 1, func)
end

---@param intervalSeconds number @每次执行间隔时间
---@param loopCount number @执行次数, 负数为无限循环
---@param func fun():void @被执行的函数
function TimerManager:Loop(intervalSeconds, loopCount, func)
    -- 注意一下第一次调用是在intervalSeconds后
    local _count = #self.timerPool
    local _timer = nil
    if (_count > 0) then
        _timer = self.timerPool[_count]
        self.timerPool[_count] = nil
        _timer:Reset(func, intervalSeconds, loopCount)
    else
        _timer = Timer.New(func, intervalSeconds, loopCount)
    end
    table.insert(self.timers, _timer)
    _timer:Start();
    return _timer;
end

---@param intervalFrameCount number @每次执行间隔帧数
---@param loopCount number @执行次数
---@param func fun():void @被执行的函数
function TimerManager:LoopFrame(intervalFrameCount, loopCount, func)
    local _count = #self.frametimerPool
    local _frametimer = nil
    if (_count > 0) then
        _frametimer = self.frametimerPool[_count]
        self.frametimerPool[_count] = nil
        _frametimer:Reset(func, intervalFrameCount, loopCount)
    else
        _frametimer = FrameTimer.New(func, intervalFrameCount, loopCount)
    end
    table.insert(self.timers, _frametimer)
    _frametimer:Start();
    return _frametimer;
end

function TimerManager:GCTime(_timer)
    self.timerPool[#self.timerPool + 1] = _timer
end

function TimerManager:GCFrameTime(_frametimer)
    self.frametimerPool[#self.frametimerPool + 1] = _frametimer
end

function TimerManager:ClearAllTimers()
    for i = 1, #self.timers do
        if self.timers[i] and self.timers[i].running then
            self.timers[i]:Stop()
        end
    end
    self.timers = {}

    for i = 1, #self.frametimers do
        if self.frametimers[i] and self.frametimers[i].running then
            self.frametimers[i]:Stop()
        end
    end
    self.frametimers = {}
end

return TimerManager
