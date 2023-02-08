using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// TODO: set up option panel generation given list from DataStructure class
[RequireComponent(typeof(RectTransform))]
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

    [AllowsNull]
    public struct CreationOptionSet
    {
        public CreationOption[] options;
        public CreationOption finalize;
    }

    public class CreationOption
    {
        public abstract class OptionAction
        {
            public abstract void Invoke();
        }
        public class OANoParam : OptionAction
        {
            public UnityAction action;

            private OANoParam() { }
            public OANoParam(UnityAction _action)
            {
                action = _action;
            }

            override public void Invoke()
            {
                action.Invoke();
            }
        }
        public class OAParam<T> : OptionAction
        {
            public T param;
            public UnityAction<T> action;

            private OAParam() { }
            public OAParam(UnityAction<T> _action)
            {
                action = _action;
            }
            public OAParam(UnityAction<T> _action, T _startParamValue)
            {
                action = _action;
                param = _startParamValue;
            }

            override public void Invoke()
            {
                action.Invoke(param);
            }
        }

        public String displayText;
        public OptionType type;
        private OptionAction _action;

        public CreationOption(string displayText, OptionType type, OptionAction action)
        {
            this.displayText = displayText;
            this.type = type;
            this._action = action;
        }

        public UnityAction tryGetAction()
        {
            if (_action is OANoParam)
                return ((OANoParam)_action).action;
            return null;
        }

        public UnityAction<T> tryGetAction<T>()
        {
            if (_action is OAParam<T>)
                return ((OAParam<T>)_action).action;
            return null;
        }

        public bool tryUpdateParam<T>(T value)
        {
            if (_action is OAParam<T>)
            {
                ((OAParam<T>)_action).param = value;
                return true;
            }

            return false;
        }
    }

    public static CreationOption NewOption(String _optionDisplayText, OptionType _optiontype, UnityAction _action)
    {
        return new CreationOption(_optionDisplayText, _optiontype, new CreationOption.OANoParam(_action));
    }

    public static CreationOption NewOption<T>(String _optionDisplayText, OptionType _optiontype, UnityAction<T> _action)
    {
        return new CreationOption(_optionDisplayText, _optiontype, new CreationOption.OAParam<T>(_action));
    }

    private void Initialize()
    {
    }

    public void setup(CreationOptionSet optionSet, Vector2 creationPanelsize)
    {
        var rect = GetComponent<RectTransform>();
        rect.anchorMax = rect.anchorMin = rect.pivot = new Vector2(0.5f, 0);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = creationPanelsize;

        float margin = Mathf.Ceil(rect.sizeDelta.y * .02f);
        float size = Mathf.Min(Mathf.Ceil(rect.sizeDelta.x * (1f / 8f)), Mathf.Ceil(rect.sizeDelta.y * (1f / 6f)));
        Vector2 buttonSize = new Vector2(size, size);

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
        finalizeButton.gameObject.name = "Button_" + optionSet.finalize.displayText;

        finalizeButton.rect.anchorMax = finalizeButton.rect.anchorMin = finalizeButton.rect.pivot = new Vector2(1,0);
        finalizeButton.rect.anchoredPosition = Vector2.zero;
        finalizeButton.rect.sizeDelta = buttonSize;

        finalizeButton.addListener(optionSet.finalize.tryGetAction());
        finalizeButton.setText(optionSet.finalize.displayText);

        // Populate options panel
        float xOffset = margin;
        
        foreach (var option in optionSet.options)
        {
            if (option.type == OptionType.BUTTON)
            {
                var newButton = GameObject.Instantiate(DSPrefabs.GetPrefab("Button"), optionsPanel).GetComponent<DSButton>();
                newButton.name = $"Button_{option.displayText}";
                newButton.addListener(option.tryGetAction());
                newButton.setText(option.displayText);

                newButton.rect.anchorMin = newButton.rect.anchorMax = newButton.rect.pivot = new Vector2(0, .5f);
                newButton.rect.anchoredPosition = new Vector2(xOffset, 0);
                newButton.rect.sizeDelta = buttonSize * .75f;

                xOffset += margin + size;
            }
        }

        // Set default view
        optionsViewSetup(optionSet.options, margin, size);
    }

    private void optionsViewSetup(CreationOption[] options, float margin, float size)
    {
        foreach (var option in options)
        {
            switch (option.type)
            {
                case OptionType.USER_INPUT_INT:
                    print("USER_INPUT_INT option type");
                    break;
                case OptionType.BOOL:
                    var toggle = GameObject.Instantiate(DSPrefabs.GetPrefab(DSPrefabs.PrefabEnums.TOGGLE), viewPanel.transform).GetComponent<OPToggle>();
                    toggle.gameObject.name = "Toggle option";
                    toggle.setText(option.displayText);
                    toggle.addListener(onToggle);
                    toggle.rect.anchoredPosition = Vector2.zero;
                    break;
            }
        }
    }

    public void onToggle(bool value)
    {
        print((value ? "Toggled on" : "Toggle off") + " - called on " + this.gameObject.name);
    }
}

