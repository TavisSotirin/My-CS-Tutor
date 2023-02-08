using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DSViewNode : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDSSelectable
{
    [SerializeField]
    private Button viewNode;
    [SerializeField]
    private TextMeshProUGUI displayText;
    [SerializeField]
    private TextMeshProUGUI captionText;


    public void OnPointerEnter(PointerEventData eventData)
    {
        viewNode.image.color = Color.green;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        viewNode.image.color = Color.white;
    }

    public Bounds bounds 
    { 
        get 
        {
            return new Bounds(this.transform.position, viewNode.GetComponent<RectTransform>().sizeDelta * viewNode.transform.localScale);
        } 
    }

    private bool isClicked = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        isClicked = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isClicked = false;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (isClicked)
        {
            isClicked = false;

            print($"{name} child calling {transform.parent.gameObject.name} as parent from pointerMove");
        }
    }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        viewNode.onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {
        Debug.Log($"DSViewNode '{displayText.text}' clicked");
    }

    public void setText(string _displayText, string _captionText)
    {
        displayText.text = _displayText;
        captionText.text = _captionText;
    }

    public void setText(string _displayText)
    {
        displayText.text = _displayText;
    }

    private void setCaptionVisibility(bool isVisible)
    {
        captionText.gameObject.SetActive(isVisible);
    }
}

public interface IDSSelectable : ISelectHandler, IDeselectHandler
{
    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
        Debug.Log($"OnSelect selectable interface called.\nObj: {eventData.selectedObject.name}");
    }

    void IDeselectHandler.OnDeselect(BaseEventData eventData)
    {
        Debug.Log($"OnDeselect selectable interface called.\nObj: {eventData.selectedObject.name}");
    }
}