using UnityEngine;
using UnityEngine.EventSystems;

public class DragDocument : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    // Roda assim que o jogador clica no documento, mesmo que não arraste.
    // O último documento tocado passa por cima dos outros, sem nunca passar do GrennFilter.
    public void OnPointerDown(PointerEventData eventData)
    {
        OrdemDeCamadas.TrazerParaFrente(transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null)
            return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}