using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DSCreationManager : MonoBehaviour
{
    private RectTransform viewPanel;
    private RectTransform optionsPanel;
    private DSButton finalizeButton;

    public enum OptionType
    {
        USER_INPUT_TYPE,
        USER_INPUT_INT,
        USER_INPUT_FREE,
        BOOL,
        DISPLAY_ONLY,
        BUTTON
    }

    public struct CreationOption
    {
        public String text;
        public OptionType type;
        public UnityEngine.Events.UnityAction action;

        public CreationOption(String _text, OptionType _type, UnityEngine.Events.UnityAction _action)
        {
            text = _text;
            type = _type;
            action = _action;
        }
    }

    private void Initialize()
    {
    }

    // TODO: Make option button prefab
    public void setup(CreationOption[] options, CreationOption finalizeOption, Vector2 creationPanelsize)
    {
        var rect = GetComponent<RectTransform>();
        rect.anchorMax = rect.anchorMin = rect.pivot = new Vector2(0.5f, 0);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = creationPanelsize;

        float margin = Mathf.Ceil(rect.sizeDelta.y * .02f);
        
        print(GOLIB.GetTrueScreenSizeAndPosition(rect.gameObject));
        float size = Mathf.Min(Mathf.Ceil(rect.sizeDelta.x * (1f / 8f)), Mathf.Ceil(rect.sizeDelta.y * (1f / 6f)));
        Vector2 buttonSize = new Vector2(size, size);

        print(margin);
        print(size);

        // Add view panel
        viewPanel = new GameObject("DSCreationView", typeof(RectTransform)).GetComponent<RectTransform>();
        viewPanel.SetParent(gameObject.transform);

        viewPanel.AddComponent<Image>().color = Color.yellow;

        viewPanel.anchorMax = new Vector2(1,1);
        viewPanel.anchorMin = Vector2.zero;
        viewPanel.pivot = new Vector2(0.5f, 0.5f);

        viewPanel.sizeDelta = new Vector2(-margin*2, -(size + margin));
        viewPanel.anchoredPosition = new Vector2(0, (size + margin) * 0.5f);

        // Add options panel
        optionsPanel = new GameObject("DSCreationsOptions", typeof(RectTransform)).GetComponent<RectTransform>();
        optionsPanel.SetParent(gameObject.transform);

        optionsPanel.AddComponent<Image>().color = Color.blue;

        optionsPanel.anchorMax = new Vector2(1, 0);
        optionsPanel.anchorMin = Vector2.zero;
        optionsPanel.pivot = new Vector2(0.5f, 0);

        optionsPanel.sizeDelta = new Vector2(-(size + margin + margin),size);
        optionsPanel.anchoredPosition = new Vector2(-(size + margin + margin) * .5f + margin,0);


        // Add finalize button
        finalizeButton = GameObject.Instantiate(DSPrefabs.GetPrefab("Button"), gameObject.transform).GetComponent<DSButton>();
        finalizeButton.gameObject.name = "Button_" + finalizeOption.text;

        finalizeButton.rect.anchorMax = finalizeButton.rect.anchorMin = finalizeButton.rect.pivot = new Vector2(1,0);
        finalizeButton.rect.anchoredPosition = Vector2.zero;
        finalizeButton.rect.sizeDelta = buttonSize;

        finalizeButton.addListener(finalizeOption.action);
        finalizeButton.setText(finalizeOption.text);

        // Populate options panel
        float xOffset = margin;
        
        foreach (var option in options)
        {
            var newButton = GameObject.Instantiate(DSPrefabs.GetPrefab("Button"), optionsPanel).GetComponent<DSButton>();
            newButton.name = $"Button_{option.text}";
            newButton.addListener(option.action);
            newButton.setText(option.text);

            newButton.rect.anchorMin = newButton.rect.anchorMax = newButton.rect.pivot = new Vector2(0, .5f);
            newButton.rect.anchoredPosition = new Vector2(xOffset, 0);
            newButton.rect.sizeDelta = buttonSize * .75f;

            xOffset += margin + size;
        }
        

        // Set default view
    }
}