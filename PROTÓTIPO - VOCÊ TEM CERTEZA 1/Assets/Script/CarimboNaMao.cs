using UnityEngine;
using UnityEngine.EventSystems;

// Os dois tipos de carimbo do jogo
public enum TipoCarimbo
{
    Aprovado,   // verde
    Recusado    // vermelho
}

// Coloque este script na imagem GRANDE do carimbo (a que aparece "na mão").
[RequireComponent(typeof(CanvasGroup))]
public class CarimboNaMao : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("Qual carimbo é este?")]
    public TipoCarimbo tipo;

    [Header("O carimbo pequeno que fica no suporte da mesa (some enquanto este estiver na mão)")]
    public GameObject carimboNaMesa;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup grupo;
    private Vector2 posicaoInicial;
    private bool encerrando = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        grupo = GetComponent<CanvasGroup>();

        // Guarda o lugar onde você deixou o carimbo no editor.
        // Toda vez que ele for pego, aparece de novo neste lugar.
        posicaoInicial = rectTransform.anchoredPosition;
    }

    // Roda sempre que o carimbo grande é ligado (aparece na tela)
    private void OnEnable()
    {
        rectTransform.anchoredPosition = posicaoInicial;
        grupo.blocksRaycasts = true;

        // O carimbo pequeno do suporte some enquanto o grande está na mão
        if (carimboNaMesa != null)
            carimboNaMesa.SetActive(false);
    }

    private void OnApplicationQuit()
    {
        encerrando = true;
    }

    // Roda sempre que o carimbo grande é desligado (por qualquer motivo):
    // o carimbo pequeno volta para o suporte.
    private void OnDisable()
    {
        if (encerrando)
            return;

        if (carimboNaMesa != null)
            carimboNaMesa.SetActive(true);
    }

    // Ligue este método no ONCLICK do carimbo pequeno da mesa
    public void PegarCarimbo()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    public void Fechar()
    {
        gameObject.SetActive(false);
    }

    // Clicou no carimbo: fica na frente do outro carimbo (se os dois estiverem abertos)
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        // Durante o arrasto o carimbo "fica transparente" para o mouse,
        // assim o Unity consegue enxergar o que está embaixo (a zona de carimbo).
        grupo.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || canvas == null)
            return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        grupo.blocksRaycasts = true;
    }

    // Botão direito fecha, igual aos outros documentos
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Fechar();
        }
    }
}