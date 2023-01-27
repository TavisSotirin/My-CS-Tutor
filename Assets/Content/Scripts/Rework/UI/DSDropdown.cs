using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DSDropdown : TMP_Dropdown
{
    public UnityEngine.Events.UnityAction onClickExternal = null;
    override public void OnPointerClick(PointerEventData eventData)
    {
        if (onClickExternal != null)
            onClickExternal();
        Show();
    }

    public void setDropdownOptions(string[] listOptions, bool notify = true)
    {
        TMP_Dropdown.OptionDataList optionList = new();
        foreach (string option in listOptions)
            optionList.options.Add(new TMP_Dropdown.OptionData(option));

        ClearOptions();
        options = optionList.options;

        if (notify)
            value = 0;
        else
            SetValueWithoutNotify(0);
    }

    public void setDropdownOptions(DSLIB.DataTypes[] listOptions, bool notify = true)
    {
        string[] listOptionsStr = new string[listOptions.Length];
        for (int i = 0; i < listOptions.Length; i++)
            listOptionsStr[i] = listOptions[i].ToString();

        setDropdownOptions(listOptionsStr, notify);
    }
}
