using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Coloque este script em CADA um dos 3 papéis do menu do tutorial (Regras, Selos, Documentos).
// Ele avisa o TutorialEnvelopeManager quando o mouse entra, sai ou clica no papel.
// Você NÃO precisa preencher nada neste componente: o TutorialEnvelopeManager configura sozinho.
public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private TutorialEnvelopeManager gerenciador;
    private int indice;

    private Image[] imagens;
    private Color[] coresOriginais;
    private Vector3 escalaOriginal;

    // Guarda as cores e o tamanho originais do papel (e do post-it que está dentro dele)
    private void Preparar()
    {
        if (imagens != null)
            return;

        imagens = GetComponentsInChildren<Image>(true);
        coresOriginais = new Color[imagens.Length];

        for (int i = 0; i < imagens.Length; i++)
            coresOriginais[i] = imagens[i].color;

        escalaOriginal = transform.localScale;
    }

    // Chamado pelo TutorialEnvelopeManager
    public void Configurar(TutorialEnvelopeManager novoGerenciador, int novoIndice)
    {
        Preparar();
        gerenciador = novoGerenciador;
        indice = novoIndice;
    }

    // Chamado pelo TutorialEnvelopeManager: pinta o papel (e o post-it) e ajusta o tamanho
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
            gerenciador.AoEntrarNoPapel(indice);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (gerenciador != null)
            gerenciador.AoSairDoPapel(indice);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gerenciador == null)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
            gerenciador.AbrirPapelGrande(indice);
        else if (eventData.button == PointerEventData.InputButton.Right)
            gerenciador.FecharMenu();
    }
}