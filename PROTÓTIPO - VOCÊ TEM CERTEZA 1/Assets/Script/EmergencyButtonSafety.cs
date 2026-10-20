using UnityEngine;

public class EmergencyButtonSafety : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Coloque este script NO PRÓPRIO botão amarelo (o objeto com a Image do botão + tampa de vidro).
//
// Funciona em 2 cliques:
//   1) Clique com a tampa FECHADA  -> a tampa abre.
//   2) Clique com a tampa ABERTA   -> o botão é apertado, a porta fecha, a luz vermelha pisca
//                                     e a tampa fecha sozinha de novo.
[RequireComponent(typeof(Image))]
public class EmergencyButtonSafety : MonoBehaviour, IPointerClickHandler
{
    private enum Estado
    {
        TampaFechada,
        TampaAberta
    }

    [Header("A porta (arraste o objeto que tem o script PortaAnimacao)")]
    public PortaAnimacao portaAnimacao;

    [Header("Imagens")]
    [Tooltip("Botão amarelo coberto pela caixa de vidro FECHADA")]
    public Sprite spriteTampaFechada;

    [Tooltip("Caixa de vidro ABERTA (botão amarelo à mostra)")]
    public Sprite spriteTampaAberta;

    [Tooltip("Botão amarelo sendo APERTADO")]
    public Sprite spriteBotaoApertado;

    [Header("Tempo")]
    [Tooltip("Quantos segundos o botão fica apertado antes da tampa fechar")]
    public float duracaoDoClique = 0.25f;

    [Tooltip("Se o jogador abrir a tampa e não apertar, ela fecha sozinha depois disso (segundos). 0 = nunca fecha sozinha")]
    public float fecharTampaSozinhaDepoisDe = 0f;

    [Header("Proporção")]
    [Tooltip("Ligado: a imagem muda de tamanho junto com o desenho e nunca fica esticada")]
    public bool ajustarTamanho = true;

    [Header("Mostra mensagens no Console")]
    public bool mostrarMensagens = true;

    private Image imagem;
    private Estado estado = Estado.TampaFechada;
    private bool ocupado = false;
    private float escalaDaImagem = 1f;
    private Coroutine rotinaAutoFechar;

    private void Awake()
    {
        imagem = GetComponent<Image>();
        imagem.preserveAspect = true;

        // Escala em que você deixou a imagem no editor (deixe a tampa FECHADA na Image)
        if (imagem.sprite != null && imagem.sprite.rect.width > 0f)
            escalaDaImagem = imagem.rectTransform.rect.width / imagem.sprite.rect.width;

        if (spriteTampaFechada == null)
            spriteTampaFechada = imagem.sprite;
    }

    private void OnEnable()
    {
        FecharTampa();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        rotinaAutoFechar = null;
        ocupado = false;
        FecharTampa();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        // Enquanto o botão está apertado ou a porta está em ação, ignora cliques
        if (ocupado)
            return;

        if (portaAnimacao != null && portaAnimacao.Animando)
            return;

        if (estado == Estado.TampaFechada)
        {
            AbrirTampa();
        }
        else
        {
            StartCoroutine(Acionar());
        }
    }

    private void AbrirTampa()
    {
        estado = Estado.TampaAberta;
        Mostrar(spriteTampaAberta);

        if (mostrarMensagens)
            Debug.Log("EmergencyButtonSafety: tampa ABERTA.");

        if (fecharTampaSozinhaDepoisDe > 0f)
        {
            if (rotinaAutoFechar != null)
                StopCoroutine(rotinaAutoFechar);

            rotinaAutoFechar = StartCoroutine(FecharTampaDepoisDeUmTempo());
        }
    }

    private IEnumerator FecharTampaDepoisDeUmTempo()
    {
        yield return new WaitForSeconds(fecharTampaSozinhaDepoisDe);

        rotinaAutoFechar = null;

        if (!ocupado)
            FecharTampa();
    }

    private IEnumerator Acionar()
    {
        ocupado = true;

        if (rotinaAutoFechar != null)
        {
            StopCoroutine(rotinaAutoFechar);
            rotinaAutoFechar = null;
        }

        // 1) Botão apertado
        Mostrar(spriteBotaoApertado);

        if (mostrarMensagens)
            Debug.Log("EmergencyButtonSafety: botão APERTADO, fechando a porta.");

        // 2) A porta fecha e a luz vermelha começa a piscar
        if (portaAnimacao != null)
            portaAnimacao.FecharPorta();

        yield return new WaitForSeconds(duracaoDoClique);

        // 3) A tampa de vidro fecha sozinha
        FecharTampa();

        ocupado = false;
    }

    private void FecharTampa()
    {
        estado = Estado.TampaFechada;
        Mostrar(spriteTampaFechada);
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
}
