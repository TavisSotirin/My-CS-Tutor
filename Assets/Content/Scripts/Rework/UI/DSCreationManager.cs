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
    private Button finalizeButton;

    public struct CreationOption
    {
        public String text;
        public UnityEngine.Events.UnityAction action;

        public CreationOption(String _text, UnityEngine.Events.UnityAction _action)
        {
            text = _text;
            action = _action;
        }
    }

    private void Initialize()
    {
    }

    public void setup(CreationOption[] options, CreationOption finalizeOption, Vector2 creationPanelsize)
    {
        var rect = GetComponent<RectTransform>();
        rect.anchorMax = rect.anchorMin = rect.pivot = new Vector2(0.5f, 0);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = creationPanelsize;

        float margin = Mathf.Ceil(rect.sizeDelta.y * .02f);
        float size = Mathf.Min(Mathf.Ceil(rect.sizeDelta.x * (1 / 8)), Mathf.Ceil(rect.sizeDelta.y * (1 / 6)));
        Vector2 buttonSize = new Vector2(size, size);

        // Add view panel
        viewPanel = new GameObject("DSCreationView", typeof(RectTransform)).GetComponent<RectTransform>();
        viewPanel.SetParent(gameObject.transform);

        viewPanel.anchorMax = new Vector2(1,1);
        viewPanel.anchorMin = Vector2.zero;
        viewPanel.pivot = new Vector2(0.5f, 0.5f);

        viewPanel.anchoredPosition = Vector2.zero;
        viewPanel.sizeDelta = new Vector2(0, size + margin);

        // Add options panel
        optionsPanel = new GameObject("DSCreationsOptions", typeof(RectTransform)).GetComponent<RectTransform>();
        optionsPanel.SetParent(gameObject.transform);
        
        optionsPanel.anchorMax = new Vector2(1, 0);
        optionsPanel.anchorMin = Vector2.zero;
        optionsPanel.pivot = new Vector2(0.5f, 0);
        
        optionsPanel.anchoredPosition = Vector2.zero;
        optionsPanel.sizeDelta = new Vector2(size + margin, size);

        // Add finalize button
        finalizeButton = new GameObject("Finalize", typeof(Button), typeof(RectTransform)).GetComponent<Button>();
        var finalizeRect = finalizeButton.GetComponent<RectTransform>();

        finalizeRect.anchorMax = finalizeRect.anchorMin = finalizeRect.pivot = new Vector2(1,0);
        finalizeRect.anchoredPosition = Vector2.zero;
        finalizeRect.sizeDelta = buttonSize;

        finalizeButton.onClick.AddListener(finalizeOption.action);
        finalizeButton.GetComponent<TextMeshProUGUI>().text = finalizeOption.text;

        // Populate options panel
        float xOffset = margin;

        foreach (var option in options)
        {
            var newButton = new GameObject($"{option.text}_opButton").AddComponent<Button>();
            newButton.GetComponent<TextMeshProUGUI>().text = option.text;
            newButton.onClick.AddListener(option.action);

            var bRect = newButton.GetComponent<RectTransform>();
            bRect.SetParent(gameObject.transform);
            bRect.anchorMin = bRect.anchorMax = bRect.pivot = new Vector2(0, .5f);
            bRect.anchoredPosition = new Vector2(xOffset, 0);
            bRect.sizeDelta = buttonSize * .9f;

            xOffset += margin + size;
        }

        // Set default view
    }
}