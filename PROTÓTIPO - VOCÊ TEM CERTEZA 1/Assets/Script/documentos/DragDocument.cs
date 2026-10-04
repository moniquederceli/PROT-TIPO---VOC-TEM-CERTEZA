using UnityEngine;
using UnityEngine.EventSystems;

public class DragDocument : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private LimiteDeCamada limite;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    // Roda assim que o jogador clica no documento, mesmo que não arraste.
    public void OnPointerDown(PointerEventData eventData)
    {
        TrazerParaFrente();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        TrazerParaFrente();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null)
            return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    private void TrazerParaFrente()
    {
        // Procura a "linha invisível" (objeto com o script LimiteDeCamada)
        if (limite == null)
            limite = FindAnyObjectByType<LimiteDeCamada>();

        // Se a linha existe e está no MESMO grupo (mesmo pai) que este objeto,
        // coloca este objeto logo ABAIXO da linha: na frente das outras folhas,
        // mas sem nunca passar por cima do GrennFilter e das telas.
        if (limite != null && limite.transform.parent == rectTransform.parent)
        {
            int indiceLimite = limite.transform.GetSiblingIndex();
            int meuIndice = rectTransform.GetSiblingIndex();

            if (meuIndice < indiceLimite)
                rectTransform.SetSiblingIndex(indiceLimite - 1);
            else
                rectTransform.SetSiblingIndex(indiceLimite);
        }
        else
        {
            // Documento dentro de um grupo próprio (ex: dentro do DocumentInspect):
            // só passa por cima dos "irmãos" dele, sem afetar o resto do Canvas.
            rectTransform.SetAsLastSibling();
        }
    }
}