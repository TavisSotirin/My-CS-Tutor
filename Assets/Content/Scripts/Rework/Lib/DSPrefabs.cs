using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

public class DSPrefabs : MonoBehaviour
{
    [Serializable]
    public enum PrefabEnums
    {
        BUTTON,
        PANNING_PANEL,
        VIEWNODE_LINEAR,
        TOGGLE,
        DROPDOWN,
        DEBUG_COLOR_PANEL
    }

    [Serializable]
    public struct PrefabStruct
    {
        public string name { get { return prefabEnum.ToString(); } }
        public PrefabEnums prefabEnum;
        public GameObject prefab;
    }

    [SerializeField]
    private PrefabStruct[] prefabs;

    // MonoBehavior singleton class with static operations
    private static DSPrefabs _instance;
    public static DSPrefabs Instance 
    { 
        get 
        {
            if (_instance == null)
            {
                var instSearch = GameObject.FindGameObjectWithTag("PrefabManager");
                if (instSearch == null)
                {
                    string path = "Prefabs/PrefabManager";
                    var prefabManagerPrefab = Resources.Load(path) as GameObject;

                    if (prefabManagerPrefab == null)
                        throw new Exception("Prefab manager failed to load correctly from resources");
                    else
                        _instance = Instantiate(prefabManagerPrefab).GetComponent<DSPrefabs>();
                }
                else
                    _instance = instSearch.GetComponent<DSPrefabs>();
            }
            
            return _instance; 
        } 
    }
    private DSPrefabs() { }

    public static GameObject GetPrefab(int _PF_index)
    {
        if (_PF_index >= DSPrefabs.Instance.prefabs.Length || _PF_index < 0)
            return null;
        return DSPrefabs.Instance.prefabs[_PF_index].prefab;
    }

    public static GameObject GetPrefab(string name)
    {
        foreach (var pStruct in DSPrefabs.Instance.prefabs)
            if (pStruct.name.ToLower() == name.ToLower())
                return pStruct.prefab;
        return null;
    }

    public static GameObject GetPrefab(PrefabEnums _prefabEnum)
    {
        foreach (var pStruct in DSPrefabs.Instance.prefabs)
            if (pStruct.prefabEnum == _prefabEnum)
                return pStruct.prefab;
        return null;
    }

    private void Awake()
    {
        DSPrefabs._instance = this;
    }
}

public static class GOLIB
{
    public static GameObject GetRootParent(GameObject child)
    {
        var parent = child.transform.parent?.gameObject;

        while (parent != null)
        {
            child = parent;
            parent = child.transform.parent?.gameObject;
        }
        return child;
    }

    public static string GetTrueScreenSizeAndPosition(GameObject go)
    {
        var goRect = go.GetComponent<RectTransform>();
        if (goRect == null)
            return "Invalid GO";

        var root = GetRootParent(go);
        if (root == go)
            return $"root: {go.name} \t {goRect.position.ToString()}";

        string ret = "";
        string indent = "";
        foreach(var rect in GetTransformChain(go))
        {
            ret += $"{indent}-{rect.gameObject.name} \t {rect.position.ToString()}\n";
            indent += "\t";
        }
        return ret;
    }

    private static Queue<RectTransform> GetTransformChain(GameObject go)
    {
        Queue<RectTransform> q = new();

        q.Enqueue(go.GetOrAddComponent<RectTransform>());
        var parent = go.transform.parent?.gameObject;

        while (parent != null)
        {
            go = parent;
            q.Enqueue(go.GetOrAddComponent<RectTransform>());
            parent = go.transform.parent?.gameObject;
        }

        return q;
    }
}
