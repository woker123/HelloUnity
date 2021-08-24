using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using System;

[LuaCallCSharp]
public class LuaStartup : MonoBehaviour
{
    public TextAsset luaScript;

    internal static LuaEnv luaEnv = new LuaEnv();
    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;

    private LuaTable scriptEnv;
    private Action luaStart;
    private Action LuaUpdate;
    private Action luaFixedUpdate;
    private Action luaOnDestroy;   

    void Awake()
    {
        scriptEnv = luaEnv.NewTable();

        LuaTable meta = luaEnv.NewTable();
        meta.Set("__index", luaEnv.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();

        luaEnv.DoString(luaScript.text, "LuaStartup", scriptEnv);

        Action luaAwake = scriptEnv.Get<Action>("awake");
        scriptEnv.Get("start", out luaStart);
        scriptEnv.Get("update", out LuaUpdate);
        scriptEnv.Get("ondestroy", out luaOnDestroy);
        scriptEnv.Get("fixed_update", out luaFixedUpdate);

        if(luaAwake != null)
        {
            luaAwake();
        }
    }

    void Start()
    {
        if(luaStart != null)
        {
            luaStart();
        }
    }

    void Update()
    {
        if(LuaUpdate != null)
        {
            LuaUpdate();
        }
        if(Time.time - lastGCTime > GCInterval)
        {
            luaEnv.Tick();
            lastGCTime = Time.time;
        }
    }

    void FixedUpdate()
    {
        if(luaFixedUpdate != null)
        {
            luaFixedUpdate();
        }
    }

    void OnDestroy()
    {
        if(luaOnDestroy != null)
        {
            luaOnDestroy();
        }
        luaOnDestroy = null;
        LuaUpdate = null;
        luaStart = null;
        scriptEnv.Dispose();
    }



}
