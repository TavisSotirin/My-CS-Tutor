using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// TODO: work on DS core classes. Add option panel settings (valid types, creation manager input, display input, etc.) to each structure to pass here
public class NewStructurePopup : MonoBehaviour
{
    [SerializeField]
    private DSDropdown structDropdown;
    [SerializeField]
    private DSDropdown typeDropdown;
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

        DSCreationManager.CreationOption[] options = { new("Option1", DSCreationManager.OptionType.BUTTON, clearAll) };
        
        var creationPanelSize = GetComponent<RectTransform>().sizeDelta;
        creationPanelSize.y -= Mathf.Abs(structDropdown.GetComponent<RectTransform>().anchoredPosition.y) * 2 + structDropdown.GetComponent<RectTransform>().sizeDelta.y;
        creationManager.setup(options, new DSCreationManager.CreationOption("Finalize", DSCreationManager.OptionType.BUTTON, finalize), creationPanelSize);

        structDropdown.onValueChanged.AddListener(onStructChange);
        structDropdown.setDropdownOptions(new string[] { "Choose Structure" });
        structDropdown.onClickExternal = () =>
            {
                structDropdown.setDropdownOptions(Enum.GetNames(typeof(DSLIB.Structures)));
                onStructChange(0);
                structDropdown.onClickExternal = null;
            };

        typeDropdown.onValueChanged.AddListener(onTypeChange);
        typeDropdown.setDropdownOptions(new string[] { "Choose Type" });
        typeDropdown.interactable = false;
    }

    private void finalize()
    {
        print("finalize called in " + gameObject.name);
        GameObject.Destroy(gameObject);
    }

    // TODO: Default selection not counting as change and not triggering type dropdown activation
    // Add on select extension?
    private void onStructChange(int value)
    {
        if (!typeDropdown.interactable)
        {
            typeDropdown.setDropdownOptions(Enum.GetNames(typeof(DSLIB.DataTypes)));
            typeDropdown.interactable = true;
        }

        typeDropdown.setDropdownOptions(DSLIB.GetValidDataTypes(structDropdown.options.ToArray()[value].text));
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