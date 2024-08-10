---@class Timer
Timer = {}

local Timer = Timer
local mt = { __index = Timer }

---@param func fun():void @被定时执行的函数
---@param duration number @每次执行间隔时间
---@param remainTimes number @执行次数
---@param unscaled boolean @是否使用无缩放时间
function Timer.New(func, duration, remainTimes, unscaled)
    unscaled = unscaled or false and true
    remainTimes = remainTimes or 1
    local untilNextInvoke = duration
    return setmetatable(
        {
            func = func,
            duration = duration,
            untilNextInvoke = untilNextInvoke,
            remainTimes = remainTimes,
            unscaled =
                unscaled,
            running = false
        },
        mt)
end

function Timer:Start()
    self.running = true
end

---@param func fun():void @被定时执行的函数
---@param duration number @每次执行间隔时间
---@param remainTimes number @执行次数
---@param unscaled boolean @是否使用无缩放时间
function Timer:Reset(func, duration, remainTimes, unscaled)
    self.duration        = duration
    self.remainTimes     = remainTimes or 1
    self.unscaled        = unscaled
    self.func            = func
    self.untilNextInvoke = duration
end

function Timer:Stop()
    self.running = false
end

function Timer:Update()
    if not self.running then
        return
    end

    local delta = self.unscaled and UnityEngine.Time.unscaledDeltaTime or UnityEngine.Time.deltaTime
    self.untilNextInvoke = self.untilNextInvoke - delta

    if self.untilNextInvoke <= 0 then
        self.func()

        if self.remainTimes > 0 then
            self.remainTimes = self.remainTimes - 1
            self.untilNextInvoke = self.untilNextInvoke + self.duration
        end

        if self.remainTimes == 0 then
            self:Stop()
        elseif self.remainTimes < 0 then
            self.untilNextInvoke = self.untilNextInvoke + self.duration
        end
    end
end

---@class FrameTimer
FrameTimer = {}

local FrameTimer = FrameTimer
local mt2 = { __index = FrameTimer }

---@param func fun():void @被定时执行的函数
---@param frameLength number @每次执行间隔帧数
---@param remainTimes number @执行次数
function FrameTimer.New(func, frameLength, remainTimes)
    local nextInokeAtFrame = UnityEngine.Time.frameCount + frameLength
    remainTimes = remainTimes or 1
    return setmetatable(
        { func = func, remainTimes = remainTimes, frameLength = frameLength, nextInokeAtFrame = nextInokeAtFrame, running = false },
        mt2)
end

---@param func fun():void @被定时执行的函数
---@param frameLength number @每次执行间隔帧数
---@param remainTimes number @执行次数
function FrameTimer:Reset(func, frameLength, remainTimes)
    self.func = func
    self.frameLength = frameLength
    self.remainTimes = remainTimes
    self.nextInokeAtFrame = UnityEngine.Time.frameCount + frameLength
end

function FrameTimer:Start()
    self.running = true
end

function FrameTimer:Stop()
    self.running = false
end

function FrameTimer:LateUpdate()
    if not self.running then
        return
    end

    if UnityEngine.Time.frameCount >= self.nextInokeAtFrame then
        self.func()

        if self.remainTimes > 0 then
            self.remainTimes = self.remainTimes - 1
        end

        if self.remainTimes == 0 then
            self:Stop()
        elseif self.remainTimes < 0 then
            self.nextInokeAtFrame = UnityEngine.Time.frameCount + self.frameLength
        end
    end
end
