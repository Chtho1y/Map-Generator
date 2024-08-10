#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using System;


namespace XLua
{
    public partial class DelegateBridge : DelegateBridgeBase
    {
		
		public void __Gen_Delegate_Imp0()
		{
#if THREAD_SAFE || HOTFIX_ENABLE
            lock (luaEnv.luaEnvLock)
            {
#endif
                RealStatePtr L = luaEnv.rawL;
                int errFunc = LuaAPI.pcall_prepare(L, errorFuncRef, luaReference);
                
                
                PCall(L, 0, 0, errFunc);
                
                
                
                LuaAPI.lua_settop(L, errFunc - 1);
                
#if THREAD_SAFE || HOTFIX_ENABLE
            }
#endif
		}
        
		public void __Gen_Delegate_Imp1(string p0)
		{
#if THREAD_SAFE || HOTFIX_ENABLE
            lock (luaEnv.luaEnvLock)
            {
#endif
                RealStatePtr L = luaEnv.rawL;
                int errFunc = LuaAPI.pcall_prepare(L, errorFuncRef, luaReference);
                
                LuaAPI.lua_pushstring(L, p0);
                
                PCall(L, 1, 0, errFunc);
                
                
                
                LuaAPI.lua_settop(L, errFunc - 1);
                
#if THREAD_SAFE || HOTFIX_ENABLE
            }
#endif
		}
        
		public void __Gen_Delegate_Imp2(bool p0)
		{
#if THREAD_SAFE || HOTFIX_ENABLE
            lock (luaEnv.luaEnvLock)
            {
#endif
                RealStatePtr L = luaEnv.rawL;
                int errFunc = LuaAPI.pcall_prepare(L, errorFuncRef, luaReference);
                
                LuaAPI.lua_pushboolean(L, p0);
                
                PCall(L, 1, 0, errFunc);
                
                
                
                LuaAPI.lua_settop(L, errFunc - 1);
                
#if THREAD_SAFE || HOTFIX_ENABLE
            }
#endif
		}
        
        
		static DelegateBridge()
		{
		    Gen_Flag = true;
		}
		
		public override Delegate GetDelegateByType(Type type)
		{
		
		    if (type == typeof(DarlingEngine.Engine.Lua.LuaOnGameEnter))
			{
			    return new DarlingEngine.Engine.Lua.LuaOnGameEnter(__Gen_Delegate_Imp0);
			}
		
		    if (type == typeof(DarlingEngine.Engine.Lua.LuaOnCheckUpdateFinished))
			{
			    return new DarlingEngine.Engine.Lua.LuaOnCheckUpdateFinished(__Gen_Delegate_Imp0);
			}
		
		    if (type == typeof(DarlingEngine.Engine.Lua.LuaOnUpdate))
			{
			    return new DarlingEngine.Engine.Lua.LuaOnUpdate(__Gen_Delegate_Imp0);
			}
		
		    if (type == typeof(DarlingEngine.Engine.Lua.LuaOnFixedUpdate))
			{
			    return new DarlingEngine.Engine.Lua.LuaOnFixedUpdate(__Gen_Delegate_Imp0);
			}
		
		    if (type == typeof(DarlingEngine.Engine.Lua.LuaOnLateUpdate))
			{
			    return new DarlingEngine.Engine.Lua.LuaOnLateUpdate(__Gen_Delegate_Imp0);
			}
		
		    if (type == typeof(DarlingEngine.Engine.Lua.LuaOnLowMemory))
			{
			    return new DarlingEngine.Engine.Lua.LuaOnLowMemory(__Gen_Delegate_Imp0);
			}
		
		    if (type == typeof(DarlingEngine.Engine.Lua.LuaOnApplicationQuit))
			{
			    return new DarlingEngine.Engine.Lua.LuaOnApplicationQuit(__Gen_Delegate_Imp0);
			}
		
		    if (type == typeof(DarlingEngine.Engine.Lua.LuaOnSceneChanged))
			{
			    return new DarlingEngine.Engine.Lua.LuaOnSceneChanged(__Gen_Delegate_Imp1);
			}
		
		    if (type == typeof(DarlingEngine.Engine.Lua.LuaOnException))
			{
			    return new DarlingEngine.Engine.Lua.LuaOnException(__Gen_Delegate_Imp1);
			}
		
		    if (type == typeof(DarlingEngine.Engine.Lua.LuaOnApplicationFocus))
			{
			    return new DarlingEngine.Engine.Lua.LuaOnApplicationFocus(__Gen_Delegate_Imp2);
			}
		
		    if (type == typeof(DarlingEngine.Engine.Lua.LuaOnApplicationPause))
			{
			    return new DarlingEngine.Engine.Lua.LuaOnApplicationPause(__Gen_Delegate_Imp2);
			}
		
		    return null;
		}
	}
    
}