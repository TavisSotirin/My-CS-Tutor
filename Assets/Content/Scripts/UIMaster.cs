using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

//[ExecuteInEditMode]
public class UIMaster : UIPanelLib
{
    [System.Serializable]
    private struct UIPanels
    {
        [SerializeField]
        public RectTransform MemoryPanel;
        [SerializeField]
        public RectTransform ToolPanel;
        [SerializeField]
        public RectTransform WorkPanel;
        [SerializeField]
        public RectTransform MenuPanel;
        [SerializeField]
        public RectTransform InfoPanel;
        [SerializeField]
        public RectTransform CodePanel;
    }

    // System vars set purposefully
    [Header("DO NOT TOUCH-------------------")]
    [Tooltip("Preset panels in prefab")]
    [SerializeField]
    private UIPanels m_UIPanels;
    [Header("-----------------------------------")]
    [Space(30)]

    // Kept debug vars
    [Header("DEBUG")]
    //[SerializeField]
    public bool rebuildUI = false;
    [Space(15)]

    // Random working vars. Delete or comment at end of sessions
    [Header("TEMPORARY DEBUG VALUES - DELETE ME!")]
    public bool bAnim = false; // Starts anim to reset UI to default ratios
    [Space(15)]

    // Editable inspector variables
    [Header("New variables")]
    public int test2;

    public float UIPCT_MENU = .1f;
    public float UIPCT_TOOL = .1f;
    public float UIPCT_MEM = .19f;
    public float UIPCT_INFO = .19f;
    public float UIPCT_WORK = .6f;
    
    // Will run in edit mode as vars change or cursor overlaps scene
    private void Update()
    {
        if (rebuildUI)
        {
            print("REBUILD UI");
            rebuildUI = false;
            autoLayout();
        }

        if (bAnim)
        {
            bAnim = false;
            startAnim();
        }
    }

    private void Awake()
    {
        Initialize();
    }

    public override void Initialize()
    {
        autoLayout();

        // UPDATE: Add Initialize to inhierited library to avoid this
        m_UIPanels.CodePanel.gameObject.GetComponent<CodePanel>().Initialize();
        m_UIPanels.MemoryPanel.gameObject.GetComponent<MemoryPanel>().Initialize();

        // DEBUG - REMOVE
        DEBUG_AddWatermarks();
        Manager.testStaticFunc();
    }

    public void startAnim()
    {
        StartCoroutine(AnimateScaleTest());
    }

    public IEnumerator AnimateScaleTest(float animTime = .3f, float TGT_MENU = .1f, float TGT_TOOL = .1f, float TGT_MEM = .19f, float TGT_INFO = .19f, bool elastic = false, float elasticTime = 0.2f, float elasticPercent = 0.2f)
    {
        elastic = true;

        float START_UIPCT_MENU = UIPCT_MENU;
        float START_UIPCT_TOOL = UIPCT_TOOL;
        float START_UIPCT_MEM = UIPCT_MEM;
        float START_UIPCT_INFO = UIPCT_INFO;

        float startTime = Time.fixedTime;
        float oneOverAnimTime = 1 / animTime;
        ScalarLerp scalarLerp = new();
        float alpha;

        while (Time.fixedTime - startTime < animTime)
        {
            alpha = (Time.fixedTime - startTime) * oneOverAnimTime;

            UIPCT_MENU = scalarLerp.Operation(START_UIPCT_MENU, TGT_MENU, alpha);
            UIPCT_TOOL = scalarLerp.Operation(START_UIPCT_TOOL, TGT_TOOL, alpha);
            UIPCT_MEM = scalarLerp.Operation(START_UIPCT_MEM, TGT_MEM, alpha);
            UIPCT_INFO = scalarLerp.Operation(START_UIPCT_INFO, TGT_INFO, alpha);

            autoLayout();
            yield return null;
        }

        UIPCT_MENU = TGT_MENU;
        UIPCT_TOOL = TGT_TOOL;
        UIPCT_MEM = TGT_MEM;
        UIPCT_INFO = TGT_INFO;

        if (elastic)
        {
            TGT_MENU = (UIPCT_MENU - START_UIPCT_MENU > 0) ? UIPCT_MENU + UIPCT_MENU * elasticPercent : UIPCT_MENU - UIPCT_MENU * elasticPercent;
            TGT_TOOL = (UIPCT_TOOL - START_UIPCT_TOOL > 0) ? UIPCT_TOOL + UIPCT_TOOL * elasticPercent : UIPCT_TOOL - UIPCT_TOOL * elasticPercent;
            TGT_MEM = (UIPCT_MEM - START_UIPCT_MEM > 0) ? UIPCT_MEM + UIPCT_MEM * elasticPercent : UIPCT_MEM - UIPCT_MEM * elasticPercent;
            TGT_INFO = (UIPCT_INFO - START_UIPCT_INFO > 0) ? UIPCT_INFO + UIPCT_INFO * elasticPercent : UIPCT_INFO - UIPCT_INFO * elasticPercent;

            START_UIPCT_MENU = UIPCT_MENU;
            START_UIPCT_TOOL = UIPCT_TOOL;
            START_UIPCT_MEM = UIPCT_MEM;
            START_UIPCT_INFO = UIPCT_INFO;

            startTime = Time.fixedTime;
            oneOverAnimTime = 1 / elasticTime * 0.5f;

            while (Time.fixedTime - startTime < elasticTime * 0.5f)
            {
                alpha = (Time.fixedTime - startTime) * oneOverAnimTime;

                UIPCT_MENU = scalarLerp.Operation(START_UIPCT_MENU, TGT_MENU, alpha);
                UIPCT_TOOL = scalarLerp.Operation(START_UIPCT_TOOL, TGT_TOOL, alpha);
                UIPCT_MEM = scalarLerp.Operation(START_UIPCT_MEM, TGT_MEM, alpha);
                UIPCT_INFO = scalarLerp.Operation(START_UIPCT_INFO, TGT_INFO, alpha);

                autoLayout();
                yield return null;
            }

            TGT_MENU = START_UIPCT_MENU;
            TGT_TOOL = START_UIPCT_TOOL;
            TGT_MEM = START_UIPCT_MEM;
            TGT_INFO = START_UIPCT_INFO;

            START_UIPCT_MENU = UIPCT_MENU;
            START_UIPCT_TOOL = UIPCT_TOOL;
            START_UIPCT_MEM = UIPCT_MEM;
            START_UIPCT_INFO = UIPCT_INFO;

            startTime = Time.fixedTime;

            while (Time.fixedTime - startTime < elasticTime * 0.5f)
            {
                alpha = (Time.fixedTime - startTime) * oneOverAnimTime;

                UIPCT_MENU = scalarLerp.Operation(START_UIPCT_MENU, TGT_MENU, alpha);
                UIPCT_TOOL = scalarLerp.Operation(START_UIPCT_TOOL, TGT_TOOL, alpha);
                UIPCT_MEM = scalarLerp.Operation(START_UIPCT_MEM, TGT_MEM, alpha);
                UIPCT_INFO = scalarLerp.Operation(START_UIPCT_INFO, TGT_INFO, alpha);

                autoLayout();
                yield return null;
            }
        }

        UIPCT_MENU = TGT_MENU;
        UIPCT_TOOL = TGT_TOOL;
        UIPCT_MEM = TGT_MEM;
        UIPCT_INFO = TGT_INFO;
    }

    private void autoLayout(bool reset = false)
    {
        int memPanelW = (int)(Screen.width * UIPCT_MEM);
        int infoPanelW = (int)(Screen.width * UIPCT_INFO);
        int toolPanelH = (int)(Screen.height * UIPCT_TOOL);
        int menuPanelH = (int)(Screen.height * UIPCT_MENU);

        int innerPanelHeight = (int)((Screen.height - toolPanelH - menuPanelH));
        int innerPanelWidth = (int)(Screen.width - infoPanelW - memPanelW);
        int innerVertOffset = (int)((-menuPanelH + toolPanelH) * 0.5f);

        m_UIPanels.MemoryPanel.sizeDelta = new Vector2(memPanelW, 0);
        m_UIPanels.MemoryPanel.anchoredPosition = Vector2.zero;
        m_UIPanels.InfoPanel.sizeDelta = new Vector2(infoPanelW, -menuPanelH-toolPanelH);
        m_UIPanels.InfoPanel.anchoredPosition = new Vector2(0, innerVertOffset);

        m_UIPanels.MenuPanel.sizeDelta = new Vector2(-memPanelW, menuPanelH);
        m_UIPanels.MenuPanel.anchoredPosition = new Vector2(memPanelW/2, 0);
        m_UIPanels.ToolPanel.sizeDelta = new Vector2(-memPanelW, toolPanelH);
        m_UIPanels.ToolPanel.anchoredPosition = new Vector2(memPanelW/2, 0);

        //m_UIPanels.WorkPanel.sizeDelta = new Vector2(innerPanelWidth, innerPanelHeight * UIPCT_WORK);
        //m_UIPanels.WorkPanel.anchoredPosition = new Vector2((memPanelW - infoPanelW) * 0.5f, (innerPanelHeight * UIPCT_WORK) * 0.5f - () - innerVertOffset);

        //m_UIPanels.CodePanel.sizeDelta = new Vector2(innerPanelWidth, innerPanelHeight - innerPanelHeight * UIPCT_WORK);
        //m_UIPanels.CodePanel.anchoredPosition = new Vector2((memPanelW - infoPanelW) * 0.5f, -((innerPanelHeight - innerPanelHeight * UIPCT_WORK) - innerVertOffset));

        float innerScreenUIPCT = 1f - UIPCT_TOOL - UIPCT_MENU;

        m_UIPanels.WorkPanel.anchorMin = new Vector2(UIPCT_MEM, UIPCT_TOOL + innerScreenUIPCT * (1-UIPCT_WORK));
        m_UIPanels.WorkPanel.anchorMax = new Vector2(1 - UIPCT_INFO, 1 - UIPCT_MENU);
        m_UIPanels.WorkPanel.sizeDelta = Vector2.zero;
        m_UIPanels.WorkPanel.anchoredPosition = Vector2.zero;

        m_UIPanels.CodePanel.anchorMin = new Vector2(UIPCT_MEM, UIPCT_TOOL);
        m_UIPanels.CodePanel.anchorMax = new Vector2(1 - UIPCT_INFO, 1 - UIPCT_MENU - innerScreenUIPCT * UIPCT_WORK);
        m_UIPanels.CodePanel.sizeDelta = Vector2.zero;
        m_UIPanels.CodePanel.anchoredPosition = Vector2.zero;
    }

    private void DEBUG_AddWatermarks()
    {
        DEBUG_AddTextIdentifiers(ref m_UIPanels.MemoryPanel);
        DEBUG_AddTextIdentifiers(ref m_UIPanels.ToolPanel);
        DEBUG_AddTextIdentifiers(ref m_UIPanels.WorkPanel);
        DEBUG_AddTextIdentifiers(ref m_UIPanels.MenuPanel);
        DEBUG_AddTextIdentifiers(ref m_UIPanels.InfoPanel);
        DEBUG_AddTextIdentifiers(ref m_UIPanels.CodePanel);
    }

    // Watermark UI elements for debugging
    private void DEBUG_AddTextIdentifiers(ref RectTransform curPanel)
    {
        var obj = new GameObject();
        obj.transform.parent = curPanel;

        var bounds = new Vector2(Screen.height, Screen.width) * (curPanel.anchorMax - curPanel.anchorMin);

        var objRect = obj.AddComponent<RectTransform>();
        objRect.anchorMin = new Vector2(0.5f, 0.5f);
        objRect.anchorMax = new Vector2(0.5f, 0.5f);
        objRect.sizeDelta = new Vector2(Mathf.Abs(0.8f * bounds.x),0);
        objRect.anchoredPosition = Vector2.zero;
        

        var textObj = obj.AddComponent<TextMeshProUGUI>();
        string _text = curPanel.gameObject.name;

        textObj.gameObject.name = "DEBUG_" + _text + "_WATERMARK";
        textObj.text = _text;
        textObj.color = Color.black;
        textObj.alpha = 0.2f;
        //textObj.enableAutoSizing = true;
        textObj.fontSizeMin = 2;
        textObj.alignment = TextAlignmentOptions.Center;
        textObj.fontStyle = FontStyles.Bold;
    }
}
