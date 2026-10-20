using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Coloque este script NO PRÓPRIO botão Verde e NO PRÓPRIO botão Vermelho da mesa
// (o objeto que tem a Image do botão).
//
// O que ele faz: quando o jogador clica, a imagem troca rapidinho para a do botão
// "apertado" e depois volta para a normal, como um botão físico.
// Ele NÃO mexe no que o botão já faz (aprovar/recusar continua igual).
[RequireComponent(typeof(Image))]
public class PhysicalButtonFeedback : MonoBehaviour, IPointerClickHandler
{
    [Header("Imagens do botão")]
    [Tooltip("Imagem do botão normal (solto)")]
    public Sprite spriteNormal;

    [Tooltip("Imagem do botão apertado")]
    public Sprite spriteApertado;

    [Header("Tempo")]
    [Tooltip("Quantos segundos o botão fica apertado antes de voltar")]
    public float duracaoDoClique = 0.15f;

    [Header("Proporção")]
    [Tooltip("Ligado: a imagem muda de tamanho junto com o desenho e nunca fica esticada")]
    public bool ajustarTamanho = true;

    [Header("Mostra mensagens no Console")]
    public bool mostrarMensagens = false;

    private Image imagem;
    private Button botao;
    private float escalaDaImagem = 1f;
    private Coroutine rotina;

    private void Awake()
    {
        imagem = GetComponent<Image>();
        botao = GetComponent<Button>();

        // Evita qualquer deformação: a imagem sempre mantém a proporção do desenho
        imagem.preserveAspect = true;

        // Descobre em que escala você deixou a imagem no editor
        if (imagem.sprite != null && imagem.sprite.rect.width > 0f)
            escalaDaImagem = imagem.rectTransform.rect.width / imagem.sprite.rect.width;

        if (spriteNormal == null)
            spriteNormal = imagem.sprite;
    }

    private void OnEnable()
    {
        MostrarNormal();
    }

    private void OnDisable()
    {
        // Se o botão for desligado no meio do clique, ele volta ao normal
        rotina = null;
        MostrarNormal();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        // Se o botão está desligado (sem NPC para decidir), não mostra o clique
        if (botao != null && !botao.interactable)
            return;

        if (spriteApertado == null)
            return;

        if (rotina != null)
            StopCoroutine(rotina);

        rotina = StartCoroutine(Apertar());
    }

    private IEnumerator Apertar()
    {
        if (mostrarMensagens)
            Debug.Log("PhysicalButtonFeedback: " + name + " apertado.");

        Mostrar(spriteApertado);

        yield return new WaitForSecondsRealtime(duracaoDoClique);

        MostrarNormal();
        rotina = null;
    }

    private void MostrarNormal()
    {
        if (spriteNormal != null)
            Mostrar(spriteNormal);
    }

    private void Mostrar(Sprite sprite)
    {
        if (imagem == null || sprite == null)
            return;

        imagem.sprite = sprite;

        if (ajustarTamanho)
        {
            RectTransform caixa = imagem.rectTransform;
            caixa.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sprite.rect.width * escalaDaImagem);
            caixa.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sprite.rect.height * escalaDaImagem);
        }
    }
}