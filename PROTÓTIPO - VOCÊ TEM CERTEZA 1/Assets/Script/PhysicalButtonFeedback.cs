using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Coloque este script na IMAGEM do botão Verde e na IMAGEM do botão Vermelho da mesa
// (o objeto filho chamado "Image", dentro de AcceptButton / DenyButton).
// O componente Button pode estar nesse mesmo objeto ou no objeto pai: o script acha sozinho.
//
// O que ele faz: quando o jogador clica, a imagem troca rapidinho para a do botão
// "apertado" e depois volta para a normal, como um botão físico.
// Ele NÃO mexe no que o botão já faz (aprovar/recusar continua igual).
[RequireComponent(typeof(Image))]
public class PhysicalButtonFeedback : MonoBehaviour
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

    [Header("OPCIONAL: o Button deste botão (se vazio, o script procura neste objeto e nos pais)")]
    public Button botaoDoJogo;

    private Image imagem;
    private Button botao;
    private float escalaDaImagem = 1f;
    private Coroutine rotina;

    private void Awake()
    {
        imagem = GetComponent<Image>();
        botao = (botaoDoJogo != null) ? botaoDoJogo : GetComponentInParent<Button>();

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

        // Escuta o clique do Button (só dispara quando o botão está ligado/interactable)
        if (botao != null)
            botao.onClick.AddListener(AoClicar);
    }

    private void OnDisable()
    {
        if (botao != null)
            botao.onClick.RemoveListener(AoClicar);

        // Se o botão for desligado no meio do clique, ele volta ao normal
        rotina = null;
        MostrarNormal();
    }

    private void AoClicar()
    {
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