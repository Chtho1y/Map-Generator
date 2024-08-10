---@type LoginModel
local LoginModel = require('Window.Login.LoginModel')
---@type LoginMeta
local LoginMeta = require('Window.Login.LoginMeta')
---@type LoginMeta
local Login = LoginMeta:Extends{}

---初始化---
function Login:OnInit()
    LoginModel:OnInit();
    LoginMeta.OnInit(self);

end

---销毁---
function Login:OnDispose()
    LoginMeta.OnDispose(self);
    LoginModel:OnDispose();
end

---没有具体逻辑注释掉---
-- function Login:OnUpdate(dt)

-- end

---打开执行一次---
function Login:OnOpen(data)
    -- bg init
    WindowManager.OpenWindowOutStack("Background", "Login")

    TimerManager:Delay(3, function()
        LuaUtil.LoadScene("Main")
    end)

end

---刷新执行一次---
function Login:OnRefresh(data)

end

---关闭执行一次---
function Login:OnClose()

end

---暂停---
function Login:OnPause()
    WindowManager.CloseWindow()

end

---恢复---
function Login:OnResume()

end

return Login
