using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Coloque este script UMA VEZ SÓ, em um objeto vazio chamado "SistemaParede".
// Ele cuida de:
//  1) mostrar/esconder os jornais e post-its conforme o dia do jogo;
//  2) abrir a imagem grande (e o texto do jornal) quando o jogador clica num item.
public class LeitorDaParede : MonoBehaviour
{
    [Header("Referência ao GameManager (pra saber em qual dia estamos)")]
    public GameManager gameManager;

    [Header("Navegação de visão (esconde as setas enquanto a leitura está aberta). Opcional: se ficar vazio, o script acha sozinho")]
    public NavegacaoVisao navegacaoVisao;

    [Header("Fundo escurecido atrás da leitura (clicar nele FECHA)")]
    public GameObject overlayEscuro;

    [Header("Imagem grande no centro (clicar nela: Jornal abre/fecha o texto | Post-it fecha)")]
    public GameObject painelImagem;
    public Image imagemGrande;

    [Header("Caixa de texto do jornal (aparece no segundo clique)")]
    public GameObject caixaTexto;
    public TMP_Text textoLeitura;

    [Header("Scroll View do texto (opcional: faz o texto sempre começar do topo)")]
    public ScrollRect scrollTexto;

    [Header("Escurecer a imagem enquanto a caixa de texto está aberta")]
    [Tooltip("Cor da imagem normal (branco = sem mudança)")]
    public Color corNormal = Color.white;

    [Tooltip("Cor da imagem escurecida. Quanto mais perto de preto, mais escura. Ex: cinza escuro")]
    public Color corEscurecida = new Color(0.35f, 0.35f, 0.35f, 1f);

    private ItemDeParede[] todosOsItens;
    private ItemDeParede itemAberto = null;
    private bool textoVisivel = false;
    private int ultimoDiaConferido = -1;

    private void Awake()
    {
        if (navegacaoVisao == null)
            navegacaoVisao = FindAnyObjectByType<NavegacaoVisao>();

        // Procura TODOS os jornais e post-its da cena, inclusive os que estão desligados
        todosOsItens = FindObjectsByType<ItemDeParede>(FindObjectsInactive.Include);

        EsconderTudo();
    }

    private void Start()
    {
        AtualizarItensDoDia();
    }

    private void Update()
    {
        // Se o dia mudou (novo dia ou jogo reiniciado), troca os itens da parede
        if (gameManager != null && gameManager.diaAtual != ultimoDiaConferido)
            AtualizarItensDoDia();
    }

    // Liga os itens que devem aparecer neste dia e desliga os outros
    private void AtualizarItensDoDia()
    {
        int dia = (gameManager != null) ? gameManager.diaAtual : 1;
        ultimoDiaConferido = dia;

        // Se tinha algo aberto quando o dia mudou, fecha
        if (itemAberto != null)
            Fechar();

        if (todosOsItens == null)
            return;

        foreach (ItemDeParede item in todosOsItens)
        {
            if (item != null)
                item.gameObject.SetActive(item.EstaVisivelNoDia(dia));
        }
    }

    // Chamado pelo ItemDeParede quando o jogador clica nele
    public void Abrir(ItemDeParede item)
    {
        // Se já tem uma leitura aberta, ignora
        if (item == null || itemAberto != null)
            return;

        itemAberto = item;
        textoVisivel = false;

        if (overlayEscuro != null) overlayEscuro.SetActive(true);
        if (painelImagem != null) painelImagem.SetActive(true);
        if (caixaTexto != null) caixaTexto.SetActive(false);

        if (imagemGrande != null)
        {
            imagemGrande.sprite = item.imagemGrande;
            imagemGrande.preserveAspect = true;
            imagemGrande.color = corNormal;
        }

        if (textoLeitura != null)
            textoLeitura.text = item.texto;

        // Esconde as setas de virar a cabeça enquanto a leitura está aberta
        if (navegacaoVisao != null)
            navegacaoVisao.DefinirDiarioAberto(true);

        // Post-it especial: ao abrir, sorteia o evento da figura na janela
        if (item.sorteiaEventoDaJanela)
        {
            Day2EventManager evento = FindAnyObjectByType<Day2EventManager>();

            if (evento != null)
                evento.SortearEvento();
        }
    }

    // Ligue este método no ONCLICK do fundo escurecido (clicar fora fecha)
    public void Fechar()
    {
        EsconderTudo();

        itemAberto = null;
        textoVisivel = false;

        if (imagemGrande != null)
            imagemGrande.color = corNormal;

        if (navegacaoVisao != null)
            navegacaoVisao.DefinirDiarioAberto(false);
    }

    // Ligue este método no ONCLICK da imagem grande
    public void CliqueNaImagemGrande()
    {
        if (itemAberto == null)
            return;

        // Post-it: não tem texto, então clicar na imagem fecha
        if (itemAberto.tipo == ItemDeParede.TipoItem.PostIt)
        {
            Fechar();
            return;
        }

        // Jornal: alterna mostrar/esconder o texto
        textoVisivel = !textoVisivel;

        if (caixaTexto != null)
            caixaTexto.SetActive(textoVisivel);

        // Texto aberto: imagem escurece. Texto fechado: imagem volta ao normal
        if (imagemGrande != null)
            imagemGrande.color = textoVisivel ? corEscurecida : corNormal;

        if (textoVisivel && scrollTexto != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollTexto.verticalNormalizedPosition = 1f;
        }
    }

    private void EsconderTudo()
    {
        if (overlayEscuro != null) overlayEscuro.SetActive(false);
        if (painelImagem != null) painelImagem.SetActive(false);
        if (caixaTexto != null) caixaTexto.SetActive(false);
    }
}