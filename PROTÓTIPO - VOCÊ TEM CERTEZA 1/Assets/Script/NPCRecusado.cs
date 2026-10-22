using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Coloque este componente em CADA um dos 24 GameObjects de NPC
// (as 3 variações dos 8 NPCs). Cada um tem seu próprio campo
// "Imagem Recusado" no Inspector, então cada NPC pode ter uma
// imagem diferente quando for barrado.
//
// Quando o jogador aperta o botão vermelho:
//   1) a imagem troca para a de "recusado" (sem achatar, mantendo a proporção);
//   2) o NPC vai embora andando para a esquerda, balançando como na entrada;
//   3) ao chegar no limite da esquerda, ele desaparece.
public class NPCRecusado : MonoBehaviour
{
    [Header("Imagem que aparece quando ESTE NPC é recusado")]
    [FormerlySerializedAs("spriteRecusado")]
    public Sprite imagemRecusado;

    [Header("Imagem do NPC (deixe em branco se estiver no mesmo objeto)")]
    public Image imagemNPC;

    [Header("Caminhada para fora")]
    [Tooltip("Velocidade com que ele vai embora")]
    public float velocidade = 300f;

    [Tooltip("Posição (X) da esquerda onde ele desaparece")]
    public float limiteEsquerda = -900f;

    [Tooltip("Altura do balanço de cada passo")]
    public float distanciaPasso = 10f;

    [Tooltip("Velocidade do balanço dos passos")]
    public float velocidadePasso = 8f;

    [Header("Proporção (evita a imagem achatada ou esticada)")]
    [Tooltip("Ligado: ao trocar para a imagem de recusado, a caixa muda para o tamanho certo daquele desenho, sem deformar")]
    public bool ajustarTamanho = true;

    [Tooltip("Ligado: os PÉS do NPC (meio da borda de baixo) ficam parados ao trocar a imagem. Desligado: o centro fica parado")]
    public bool manterOsPes = true;

    [Header("Mostra mensagens no Console")]
    public bool mostrarMensagens = false;

    private bool indoEmbora = false;
    private float posicaoYInicial;

    private RectTransform caixa;
    private Sprite spriteOriginal;
    private Vector2 tamanhoOriginal;
    private float escalaDaImagem = 1f;
    private bool escalaCalculada = false;

    private void Awake()
    {
        if (imagemNPC == null)
            imagemNPC = GetComponent<Image>();

        if (imagemNPC == null)
            return;

        caixa = imagemNPC.rectTransform;
        spriteOriginal = imagemNPC.sprite;
        tamanhoOriginal = caixa.rect.size;

        // Escala em que você deixou o NPC no editor (tamanho da caixa / tamanho do desenho)
        if (spriteOriginal != null && spriteOriginal.rect.width > 0f)
        {
            escalaDaImagem = caixa.rect.width / spriteOriginal.rect.width;
            escalaCalculada = true;
        }
    }

    // Se este NPC voltar a aparecer depois de ter sido recusado, ele volta ao normal
    private void OnEnable()
    {
        if (!indoEmbora)
            return;

        indoEmbora = false;

        if (imagemNPC != null && spriteOriginal != null)
            TrocarImagem(spriteOriginal, tamanhoOriginal);
    }

    // Chamado pelo NPCMovement quando o jogador aperta o botão vermelho (Recusar)
    public void Recusar()
    {
        if (imagemNPC == null || imagemRecusado == null)
        {
            // Sem imagem de recusado, ele só vai embora andando
            posicaoYInicial = transform.localPosition.y;
            indoEmbora = true;
            return;
        }

        Vector2 novoTamanho = tamanhoOriginal;

        if (ajustarTamanho && escalaCalculada)
        {
            novoTamanho = new Vector2(
                imagemRecusado.rect.width * escalaDaImagem,
                imagemRecusado.rect.height * escalaDaImagem);
        }

        TrocarImagem(imagemRecusado, novoTamanho);

        if (mostrarMensagens)
            Debug.Log("NPCRecusado: " + name + " recusado. Caixa " + tamanhoOriginal + " -> " + caixa.rect.size);

        // Depois da troca, o chão (Y) em que ele caminha é o atual
        posicaoYInicial = transform.localPosition.y;
        indoEmbora = true;
    }

    private void Update()
    {
        if (!indoEmbora)
            return;

        Vector3 posicao = transform.localPosition;

        posicao.x = Mathf.MoveTowards(posicao.x, limiteEsquerda, velocidade * Time.deltaTime);
        posicao.y = posicaoYInicial + Mathf.Sin(Time.time * velocidadePasso) * distanciaPasso;

        transform.localPosition = posicao;

        // Chegou no limite: o NPC desaparece
        if (Mathf.Abs(posicao.x - limiteEsquerda) <= 1f)
        {
            indoEmbora = false;
            gameObject.SetActive(false);
        }
    }

    // Troca a imagem sem deformar, mantendo os pés (ou o centro) no mesmo lugar
    private void TrocarImagem(Sprite sprite, Vector2 novoTamanho)
    {
        Vector2 pivot = caixa.pivot;
        Vector2 tamanhoAntes = caixa.rect.size;

        Vector2 pes = caixa.anchoredPosition
            + new Vector2((0.5f - pivot.x) * tamanhoAntes.x, -pivot.y * tamanhoAntes.y);

        imagemNPC.sprite = sprite;

        // Nunca deixa o desenho esticado: se a caixa não bater, ele mantém a proporção
        imagemNPC.preserveAspect = true;

        if (!ajustarTamanho || !escalaCalculada)
            return;

        caixa.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, novoTamanho.x);
        caixa.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, novoTamanho.y);

        if (manterOsPes)
        {
            Vector2 tamanhoDepois = caixa.rect.size;

            caixa.anchoredPosition = pes
                - new Vector2((0.5f - pivot.x) * tamanhoDepois.x, -pivot.y * tamanhoDepois.y);
        }
    }
}