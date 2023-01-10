using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CodePanel : UIPanelLib
{
    private TextMeshProUGUI textBox;
    // TOP BOTTOM LEFT RIGHT
    public Vector4 margins = Vector4.zero;
    public float ScrollViewDividerPercent = .01f;
    public float codeDocRatio = 0.75f;

    public ScrollRect CodeScrollView;
    public ScrollRect DocumentOverviewScrollView;

    public override void Initialize()
    {
        var codeRect = CodeScrollView.GetComponent<RectTransform>();
        var docRect = DocumentOverviewScrollView.GetComponent<RectTransform>();

        codeRect.anchorMin = new Vector2(0, 0);
        codeRect.anchorMax = new Vector2(codeDocRatio, 1);

        docRect.anchorMin = new Vector2(codeDocRatio + ScrollViewDividerPercent, 0);
        docRect.anchorMax = new Vector2(1, 1);

        codeRect.sizeDelta = new Vector2(-margins[2], -margins[1] - margins[0]);
        docRect.sizeDelta = new Vector2(-margins[3], -margins[1] - margins[0]);

        codeRect.anchoredPosition = new Vector2(margins[2], 0.5f * (margins[1] - margins[0]));
        docRect.anchoredPosition = new Vector2(-margins[3], 0.5f * (margins[1] - margins[0]));
    }
}
