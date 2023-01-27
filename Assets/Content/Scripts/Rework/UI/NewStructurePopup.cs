using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.UI;

// TODO: work on DS core classes. Add option panel settings (valid types, creation manager input, display input, etc.) to each structure to pass here
public class NewStructurePopup : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown structDropdown;
    [SerializeField]
    private TMP_Dropdown typeDropdown;
    [SerializeField]
    private DSCreationManager creationManager;

    [SerializeField]
    public TMP_Dropdown.OptionDataList list;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        // Always hidden by default
        //gameObject.SetActive(false);
        gameObject.name = "NewStructurePopup";

        // Create dropdown lists?
        // Structure type {array, LL, tree, etc}
        // Valid data types {char, int, bool, string, etc.}

        //DSCreationManager.CreationOption[] options = { 
            //new DSCreationManager.CreationOption("Add node", addNode),
            //new DSCreationManager.CreationOption("Clear all", clearAll)
        //};

        var creationPanelSize = GetComponent<RectTransform>().sizeDelta;
        creationPanelSize.y -= Mathf.Abs(structDropdown.GetComponent<RectTransform>().anchoredPosition.y) * 2 + structDropdown.GetComponent<RectTransform>().sizeDelta.y;

        creationManager.setup(options, new DSCreationManager.CreationOption("Finalize", finalize), creationPanelSize);

        structDropdown.onValueChanged.AddListener(onStructChange);
        typeDropdown.onValueChanged.AddListener(onTypeChange);
        
        typeDropdown.interactable = false;
        typeDropdown.ClearOptions();
        TMP_Dropdown.OptionDataList blankOption =  new();
        blankOption.options.Add(new TMP_Dropdown.OptionData("Choose Type"));
        typeDropdown.options = blankOption.options;
        typeDropdown.value = 0;
    }

    private void finalize()
    {
        print("finalize called in " + gameObject.name);
        GameObject.Destroy(gameObject);
    }

    private void onStructChange(int value)
    {
        if (!typeDropdown.interactable)
        {
            typeDropdown.interactable = true;
            typeDropdown.ClearOptions();
            TMP_Dropdown.OptionDataList blankOption = new();
            blankOption.options.Add(new TMP_Dropdown.OptionData("Choose Type"));
            typeDropdown.options = blankOption.options;
            typeDropdown.value = 0;
        }

        if (typeDropdown.value < 0)
            typeDropdown.value = 0;

        switch (value)
        {

        }
    }

    private void onTypeChange(int value)
    {
        print("Type changed");
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