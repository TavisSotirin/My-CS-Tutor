using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DS2;

public class CreateStructureButton : MonoBehaviour
{
    public DSLIB.Structures structureType;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(onClick);
    }

    public void onClick()
    {
        // Parent under canvas
        GameObject creationManagerGO = new("DSCreationManager_"+structureType.ToString());
        creationManagerGO.transform.SetParent(transform.parent);
        var creationManager = creationManagerGO.AddComponent<DSCreationManager>();

        ADataStructure structure = null;

        switch(structureType)
        {
            case DSLIB.Structures.ARRAY:
                creationManager.setup(DSArray.Instantiate(ref structure), new Vector2(500, 500));
                break;
            case DSLIB.Structures.LINKED_LIST:
                print("Implement LL creation menu");
                //creationManager.setup(DSArray.Instantiate(ref structure), null, new Vector2(500, 500));
                break;
        }
        
    }
}
