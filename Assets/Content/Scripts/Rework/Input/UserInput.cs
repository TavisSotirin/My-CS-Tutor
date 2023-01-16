using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UserInput : MonoBehaviour
{
    private EventSystem eventSystem;
    // Start is called before the first frame update
    void Start()
    {
        eventSystem = GetComponent<EventSystem>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            print("W pressed");
        }
    }

    // Distinguish between drag, single click, double click, hover, scroll on panning panel vs node objects
    // Panning panel only drags and scrolls, should pass all other down to children
}
