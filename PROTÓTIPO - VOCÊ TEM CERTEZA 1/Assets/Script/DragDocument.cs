using UnityEngine;
using UnityEngine.EventSystems;

public class DragDocument : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    // Roda assim que o jogador clica no documento, mesmo que não arraste.
    // Isso garante que o documento vá para a frente dos outros.
    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.SetAsLastSibling();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Garante de novo, caso o clique inicial não tenha disparado o OnPointerDown
        rectTransform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null)
            return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}