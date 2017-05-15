using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

/// <summary>
/// GameObject 注入
/// </summary>
[System.Serializable]
public class Injection {
    public string name;
    public GameObject value;
}

/// <summary>
/// Lua behaviour.
/// </summary>
[CSharpCallLua]
public class LuaBehaviour : MonoBehaviour {

    public static LuaEnv luaEnv = new LuaEnv();

    public TextAsset luaScript;

    [CSharpCallLua]
    public Injection[] injections;

    [CSharpCallLua]
    delegate void LuaCallback();

    [CSharpCallLua]
    private LuaCallback lua_Awake;

    [CSharpCallLua]
    private LuaCallback lua_Start;

    [CSharpCallLua]
    private LuaCallback lua_Update;

    [CSharpCallLua]
    private LuaCallback lua_OnDestroy;

    private LuaTable scriptEnv;

    void Awake () {
        scriptEnv = luaEnv.NewTable ();

        LuaTable meta = luaEnv.NewTable ();
        meta.Set ("__index", luaEnv.Global);
        scriptEnv.SetMetaTable (meta);
        meta.Dispose ();

        scriptEnv.Set ("self", this);

        // 注入对象
        if (injections != null) {
            foreach (var i in injections) {
                scriptEnv.Set (i.name, i.value);
            }
        }

        if (luaScript != null) {
            // 执行脚本
            luaEnv.DoString (luaScript.text, "LuaBehaviour", scriptEnv);
            // 绑定函数
            scriptEnv.Get ("awake", out lua_Awake);
            scriptEnv.Get ("start", out lua_Start);
            scriptEnv.Get ("update", out lua_Update);
            scriptEnv.Get ("ondestroy", out lua_OnDestroy);

            // 按生命周期调用
            if (lua_Awake != null) {
                lua_Awake ();
            }
        }
	}
	
    void Start() {
        // 按生命周期调用
        if (lua_Start != null) {
            lua_Start ();
        }
    }

	void Update () {
        // 按生命周期调用
        if (lua_Update != null) {
            lua_Update ();
        }
	}

    void OnDestroy() {
        // 按生命周期调用
        if (lua_OnDestroy != null) {
            lua_OnDestroy ();
        }
        lua_OnDestroy = null;
        lua_Update = null;
        lua_Start = null;

        scriptEnv.Dispose ();
    }
}
