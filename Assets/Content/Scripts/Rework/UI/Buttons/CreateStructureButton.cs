using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateStructureButton : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    private void Awake()
    {
        Initialize();
    }
    private void Initialize()
    {
        GetComponentInChildren<Button>().onClick.AddListener(onClick);
    }

    public void onClick()
    {
        switch(dropdown.options[dropdown.value].text)
        {
            case "Array":
                print("Create array");
                break;
            case "Linked List":
                print("Create linked list");
                break;
            case "Tree":
                print("Create tree");
                break;
            
        }
    }
}
