using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

//一些内置XLua无法直接调用的特殊C#函数
[LuaCallCSharp]
public class CustomFuncs : MonoBehaviour
{
    static public bool Raycast(Ray ray, out RaycastHit hitResult, float distance)
    {
        return Physics.Raycast(ray, out hitResult, Mathf.Infinity);
    }
}


