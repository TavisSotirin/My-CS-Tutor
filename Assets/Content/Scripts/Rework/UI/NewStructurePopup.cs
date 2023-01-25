using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.UI;

public class NewStructurePopup : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown structDropdown;
    [SerializeField]
    private TMP_Dropdown typeDropdown;
    [SerializeField]
    private DSCreationManager creationManager;

    public GameObject DSViewNodePrefab;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        // Always hidden by default
        //gameObject.SetActive(false);
        gameObject.name = "NewStructPopup";

        // Create dropdown lists?
        // Structure type {array, LL, tree, etc}
        // Valid data types {char, int, bool, string, etc.}

        DSCreationManager.CreationOption[] options = { 
            new DSCreationManager.CreationOption("Add node", addNode),
            new DSCreationManager.CreationOption("Clear all", clearAll)
        };

        var creationPanelSize = GetComponent<RectTransform>().sizeDelta;
        creationPanelSize.y -= Mathf.Abs(structDropdown.GetComponent<RectTransform>().anchoredPosition.y) * 2 + structDropdown.GetComponent<RectTransform>().sizeDelta.y;

        creationManager.setup(options, new DSCreationManager.CreationOption("Finalize", finalize), creationPanelSize);
    }

    private void finalize()
    {
        print("finalize called in " + gameObject.name);
    }

    // Reset creation structure to default
    private void clearAll()
    {
        print("clearAll called in " + gameObject.name);
    }

    private void addNode()
    {
        print("addNode called in " + gameObject.name);
    }
}