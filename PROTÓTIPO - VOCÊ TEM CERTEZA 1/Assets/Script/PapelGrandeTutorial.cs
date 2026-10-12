using UnityEngine;
using UnityEngine.EventSystems;

// Coloque este script em CADA um dos 3 papéis GRANDES de leitura (Regras, Selos, Documentos).
// Botão esquerdo arrasta o papel pela tela. Botão direito fecha (e volta para o menu dos 3 papéis).
public class PapelGrandeTutorial : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private TutorialEnvelopeManager gerenciador;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        Canvas pai = GetComponentInParent<Canvas>();
        if (pai != null)
            canvas = pai.rootCanvas;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || canvas == null)
            return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        if (gerenciador == null)
            gerenciador = FindAnyObjectByType<TutorialEnvelopeManager>(FindObjectsInactive.Include);

        if (gerenciador != null)
            gerenciador.FecharPapelGrande();
    }
}