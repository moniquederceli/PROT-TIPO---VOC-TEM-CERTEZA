using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Coloque este script em CADA uma das 3 folhas do tutorial (Regras, Selos, Documentos).
// A folha funciona como os outros documentos da mesa:
//   - botão ESQUERDO arrasta a folha pela tela;
//   - clicar numa folha (sem arrastar) abre a versão GRANDE dela, e a pequena some do grupo;
//   - botão DIREITO fecha a folha;
//   - passar o mouse ilumina a folha e escurece as outras.
// Você NÃO precisa preencher nada neste componente: o TutorialEnvelopeManager configura sozinho.
public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IDragHandler, IPointerClickHandler
{
    private TutorialEnvelopeManager gerenciador;
    private int indice;

    private RectTransform rectTransform;
    private Canvas canvas;

    private Image[] imagens;
    private Color[] coresOriginais;
    private Vector3 escalaOriginal;
    private Vector2 posicaoInicial;

    // Guarda as cores, o tamanho e o lugar originais da folha
    private void Preparar()
    {
        if (imagens != null)
            return;

        rectTransform = GetComponent<RectTransform>();

        Canvas pai = GetComponentInParent<Canvas>();
        if (pai != null)
            canvas = pai.rootCanvas;

        imagens = GetComponentsInChildren<Image>(true);
        coresOriginais = new Color[imagens.Length];

        for (int i = 0; i < imagens.Length; i++)
            coresOriginais[i] = imagens[i].color;

        escalaOriginal = transform.localScale;
        posicaoInicial = rectTransform.anchoredPosition;
    }

    // Chamado pelo TutorialEnvelopeManager
    public void Configurar(TutorialEnvelopeManager novoGerenciador, int novoIndice)
    {
        Preparar();
        gerenciador = novoGerenciador;
        indice = novoIndice;
    }

    // Chamado pelo TutorialEnvelopeManager: devolve a folha ao lugar e tamanho originais
    public void Resetar()
    {
        Preparar();
        rectTransform.anchoredPosition = posicaoInicial;
        AplicarAparencia(Color.white, 1f);
    }

    // Chamado pelo TutorialEnvelopeManager: pinta a folha e ajusta o tamanho
    public void AplicarAparencia(Color cor, float escala)
    {
        Preparar();

        for (int i = 0; i < imagens.Length; i++)
        {
            if (imagens[i] != null)
                imagens[i].color = coresOriginais[i] * cor;
        }

        transform.localScale = escalaOriginal * escala;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gerenciador != null)
            gerenciador.AoEntrarNaFolha(indice);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (gerenciador != null)
            gerenciador.AoSairDaFolha(indice);
    }

    // Clicou na folha: ela vai para a frente das outras
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }

    // Arrastar com o botão esquerdo
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || canvas == null)
            return;

        Preparar();
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    // Clique ESQUERDO (sem arrastar): a folha some do grupo e abre a versão grande.
    // Clique DIREITO: fecha a folha.
    // (O Unity não chama este método quando você arrastou a folha, só quando foi um clique de verdade.)
    public void OnPointerClick(PointerEventData eventData)
    {
        if (gerenciador == null)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
            gerenciador.AbrirFolhaGrande(indice);
        else if (eventData.button == PointerEventData.InputButton.Right)
            gerenciador.FecharFolha(indice);
    }
}