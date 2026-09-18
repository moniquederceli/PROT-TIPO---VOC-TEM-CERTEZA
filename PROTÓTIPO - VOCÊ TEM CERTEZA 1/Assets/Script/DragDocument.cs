using UnityEngine;
using UnityEngine.EventSystems;

public class DragDocument : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Começou a arrastar
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }
}