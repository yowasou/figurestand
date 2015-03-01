using UnityEngine;
using System.Collections;

public static class GlobalFunctions 
{
    public static void SetVisible(GameObject go,bool status)
    {
        go.renderer.enabled = status;
        Transform[] tr_child = go.GetComponentsInChildren<Transform>();
        foreach (Transform tr in tr_child)
        {
            tr.renderer.enabled = status;
        }
    }
}
