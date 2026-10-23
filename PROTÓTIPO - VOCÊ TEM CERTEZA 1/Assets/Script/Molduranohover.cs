using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Coloca uma moldurinha BRANCA em volta de um item clicável quando o mouse passa por cima.
// Não precisa de nenhuma imagem: o script desenha a moldura sozinho.
//
// Normalmente você NÃO adiciona este script na mão: o SistemaMolduras adiciona nos itens
// que você escolher. Mas também dá para usar direto (Add Component > MolduraNoHover).
public class MolduraNoHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Aparência da moldura")]
    public Color cor = Color.white;

    [Tooltip("Grossura da linha, em pixels do Canvas")]
    public float espessura = 3f;

    [Tooltip("Quanto a moldura fica para FORA do item (pixels). 0 = colada na borda")]
    public float margem = 3f;

    [Header("Onde desenhar")]
    [Tooltip("OPCIONAL: o objeto cuja borda recebe a moldura. Vazio = o script escolhe a imagem do item sozinho")]
    public RectTransform alvoDaMoldura;

    [Tooltip("Ligado: se o item for um Button desligado (sem NPC para decidir), não mostra a moldura")]
    public bool respeitarBotaoDesligado = true;

    [Header("Mostra mensagens no Console")]
    public bool mostrarMensagens = false;

    // A imagem da moldura é criada UMA vez e compartilhada por todos os itens
    private const int TamanhoDaTextura = 32;
    private const int BordaEmPixels = 4;
    private static Sprite spriteDaMoldura;

    private GameObject moldura;
    private Button botao;

    private void Awake()
    {
        botao = GetComponentInParent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!PodeMostrar())
            return;

        CriarSeNecessario();

        if (moldura != null)
        {
            moldura.transform.SetAsLastSibling();
            moldura.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Esconder();
    }

    private void OnDisable()
    {
        Esconder();
    }

    // Se o botão ficar desligado com o mouse em cima (o NPC foi embora), a moldura some
    private void Update()
    {
        if (moldura != null && moldura.activeSelf && !PodeMostrar())
            Esconder();
    }

    private bool PodeMostrar()
    {
        if (respeitarBotaoDesligado && botao != null && !botao.interactable)
            return false;

        return true;
    }

    private void Esconder()
    {
        if (moldura != null)
            moldura.SetActive(false);
    }

    private void CriarSeNecessario()
    {
        if (moldura != null)
            return;

        RectTransform alvo = (alvoDaMoldura != null) ? alvoDaMoldura : EncontrarAlvo();

        if (alvo == null)
        {
            if (mostrarMensagens)
                Debug.LogWarning("MolduraNoHover: " + name + " não tem uma Image para receber a moldura.");
            return;
        }

        moldura = new GameObject("MolduraHover", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        moldura.transform.SetParent(alvo, false);

        // Não deixa Layout Groups mexerem na moldura
        LayoutElement layout = moldura.AddComponent<LayoutElement>();
        layout.ignoreLayout = true;

        // A moldura cobre o item inteiro (esticada nos 4 lados), com a margem para fora
        RectTransform rect = moldura.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(-margem, -margem);
        rect.offsetMax = new Vector2(margem, margem);
        rect.localScale = Vector3.one;

        Image imagem = moldura.GetComponent<Image>();
        imagem.sprite = PegarSpriteDaMoldura();
        imagem.type = Image.Type.Sliced;
        imagem.fillCenter = false;     // só a borda, o meio fica vazio
        imagem.color = cor;
        imagem.raycastTarget = false;  // nunca rouba o clique do item

        // Controla a grossura da linha
        Canvas canvas = alvo.GetComponentInParent<Canvas>();
        float pixelsPorUnidadeDoCanvas = (canvas != null) ? canvas.referencePixelsPerUnit : 100f;
        imagem.pixelsPerUnitMultiplier =
            BordaEmPixels * pixelsPorUnidadeDoCanvas / (spriteDaMoldura.pixelsPerUnit * Mathf.Max(0.1f, espessura));

        moldura.SetActive(false);

        if (mostrarMensagens)
            Debug.Log("MolduraNoHover: moldura criada em " + alvo.name);
    }

    // Procura a imagem do item: primeiro neste objeto, depois nos filhos
    private RectTransform EncontrarAlvo()
    {
        Image propria = GetComponent<Image>();

        if (propria != null && propria.sprite != null)
            return propria.rectTransform;

        Image[] filhas = GetComponentsInChildren<Image>(true);

        foreach (Image filha in filhas)
        {
            if (filha != null && filha.sprite != null && filha.gameObject.name != "MolduraHover")
                return filha.rectTransform;
        }

        if (propria != null)
            return propria.rectTransform;

        return GetComponent<RectTransform>();
    }

    // Desenha uma imagem de moldura (só as bordas brancas) por código
    private static Sprite PegarSpriteDaMoldura()
    {
        if (spriteDaMoldura != null)
            return spriteDaMoldura;

        Texture2D textura = new Texture2D(TamanhoDaTextura, TamanhoDaTextura, TextureFormat.RGBA32, false);
        textura.filterMode = FilterMode.Bilinear;
        textura.wrapMode = TextureWrapMode.Clamp;

        Color transparente = new Color(1f, 1f, 1f, 0f);

        for (int y = 0; y < TamanhoDaTextura; y++)
        {
            for (int x = 0; x < TamanhoDaTextura; x++)
            {
                bool naBorda =
                    x < BordaEmPixels || y < BordaEmPixels ||
                    x >= TamanhoDaTextura - BordaEmPixels || y >= TamanhoDaTextura - BordaEmPixels;

                textura.SetPixel(x, y, naBorda ? Color.white : transparente);
            }
        }

        textura.Apply();

        spriteDaMoldura = Sprite.Create(
            textura,
            new Rect(0, 0, TamanhoDaTextura, TamanhoDaTextura),
            new Vector2(0.5f, 0.5f),
            100f,
            0,
            SpriteMeshType.FullRect,
            new Vector4(BordaEmPixels, BordaEmPixels, BordaEmPixels, BordaEmPixels));

        return spriteDaMoldura;
    }
}