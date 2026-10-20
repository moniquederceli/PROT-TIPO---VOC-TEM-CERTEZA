using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Coloque este script em CADA botão do menu que é uma IMAGEM (Jogar, Configuração, Créditos,
// Modo história, Voltar, setinhas...).
//
//  - Mouse em cima  -> troca para a imagem "grifada" (com o risquinho) e liga a setinha ▶
//  - Mouse sai      -> volta para a imagem normal e desliga a setinha
//  - Clique         -> executa o que você ligar em "Ao Clicar ()"
//  - Botão BLOQUEADO -> não faz nada, só dá uma tremidinha pra mostrar que não funciona
//
// IMPORTANTE: a imagem normal e a grifada devem ter o MESMO tamanho de arquivo.
[RequireComponent(typeof(Image))]
public class BotaoDeMenuImagem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Imagens do botão")]
    [Tooltip("Imagem normal (letra sem risquinho). Se deixar vazio, usa a imagem que já está no componente Image")]
    public Sprite imagemNormal;

    [Tooltip("Imagem com o risquinho embaixo (aparece quando o mouse passa em cima). Pode deixar vazio, ex: nas setinhas de lado")]
    public Sprite imagemGrifada;

    [Header("Setinha ▶ que aparece ao lado do botão (OPCIONAL)")]
    [Tooltip("Arraste aqui a imagem da setinha. Ela só aparece com o mouse em cima do botão")]
    public GameObject setinha;

    [Header("Texto explicativo que aparece com o mouse em cima (OPCIONAL)")]
    [Tooltip("Arraste aqui o texto (TextMeshPro) de descrição do modo. Ele só aparece com o mouse em cima do botão, igual à setinha")]
    public GameObject textoDeDescricao;

    [Header("Bloqueado?")]
    [Tooltip("Marque nos botões que ainda não funcionam (Modo Timer e Modo Difícil). Ao clicar, o botão treme")]
    public bool bloqueado = false;

    [Header("Tremidinha do botão bloqueado")]
    [Tooltip("Quantos pixels o botão balança pro lado")]
    public float forcaDoTremor = 12f;

    [Tooltip("Quanto tempo dura a tremida (segundos)")]
    public float duracaoDoTremor = 0.3f;

    [Tooltip("Quantas balançadas")]
    public float velocidadeDoTremor = 50f;

    [Header("O que acontece ao clicar (igual ao OnClick de um Button)")]
    public UnityEvent aoClicar;

    private Image imagem;
    private RectTransform rect;
    private Vector2 posicaoOriginal;
    private Coroutine tremor;

    private void Awake()
    {
        imagem = GetComponent<Image>();
        rect = GetComponent<RectTransform>();

        if (imagemNormal == null)
            imagemNormal = imagem.sprite;
    }

    private void OnEnable()
    {
        posicaoOriginal = rect.anchoredPosition;
        MostrarNormal();
    }

    private void OnDisable()
    {
        // Se desligar no meio da tremida ou do hover, volta tudo ao normal
        if (tremor != null)
        {
            StopCoroutine(tremor);
            tremor = null;
        }

        rect.anchoredPosition = posicaoOriginal;
        MostrarNormal();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (bloqueado || GameModeController.Iniciando)
            return;

        if (imagemGrifada != null)
            imagem.sprite = imagemGrifada;

        if (setinha != null)
            setinha.SetActive(true);

        if (textoDeDescricao != null)
            textoDeDescricao.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MostrarNormal();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Só o botão esquerdo do mouse
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        // Depois de escolher o modo, o menu fica travado
        if (GameModeController.Iniciando)
            return;

        if (bloqueado)
        {
            if (tremor != null)
            {
                StopCoroutine(tremor);
                rect.anchoredPosition = posicaoOriginal;
            }

            tremor = StartCoroutine(Tremer());
            return;
        }

        aoClicar.Invoke();
    }

    private void MostrarNormal()
    {
        if (imagem != null && imagemNormal != null)
            imagem.sprite = imagemNormal;

        if (setinha != null)
            setinha.SetActive(false);

        if (textoDeDescricao != null)
            textoDeDescricao.SetActive(false);
    }

    // Balança o botão pros lados, diminuindo até parar
    private IEnumerator Tremer()
    {
        float tempo = 0f;

        while (tempo < duracaoDoTremor)
        {
            tempo += Time.unscaledDeltaTime;

            float restante = 1f - (tempo / duracaoDoTremor);
            float deslocamento = Mathf.Sin(tempo * velocidadeDoTremor) * forcaDoTremor * restante;

            rect.anchoredPosition = posicaoOriginal + new Vector2(deslocamento, 0f);

            yield return null;
        }

        rect.anchoredPosition = posicaoOriginal;
        tremor = null;
    }
}