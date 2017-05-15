using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class HelloWorld : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var lua_env = new LuaEnv ();
        lua_env.DoString ("CS.UnityEngine.Debug.Log('hello')");
        lua_env.Dispose ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
