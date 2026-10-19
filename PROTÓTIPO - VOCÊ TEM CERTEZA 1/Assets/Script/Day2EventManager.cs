using System.Collections;
using UnityEngine;

// Coloque este script UMA VEZ SÓ, em um objeto vazio chamado "SistemaEventos"
// (um objeto que fica SEMPRE LIGADO).
//
// O que ele faz:
//  - Quando o jogador abre o post-it especial do Dia 2, o jogo "joga uma moeda" (50%).
//  - Se der SIM, a figura misteriosa aparece na paisagem da janela (visão da esquerda).
//  - Depois que o jogador fica olhando pra ela por alguns segundos, ela DESAPARECE
//    e a tela dá uma tremidinha de susto.
//  - A cada novo dia (ou jogo reiniciado), tudo recomeça.
public class Day2EventManager : MonoBehaviour
{
    public enum ModoDeTeste
    {
        Aleatorio,       // normal: sorteia de verdade
        SempreAparece,   // só pra TESTAR: a figura sempre aparece
        NuncaAparece     // só pra TESTAR: a figura nunca aparece
    }

    [Header("Referências (se deixar vazio, o script acha sozinho)")]
    public GameManager gameManager;
    public NavegacaoVisao navegacaoVisao;

    [Header("A figura misteriosa (a imagem que fica entre a paisagem e a moldura da janela)")]
    public GameObject figuraMisteriosa;

    [Header("Configuração do sorteio")]
    [Tooltip("Chance de a figura aparecer. 0.5 = 50%. 1 = sempre. 0 = nunca")]
    [Range(0f, 1f)]
    public float chanceDeAparecer = 0.5f;

    [Tooltip("Em qual dia o evento pode acontecer")]
    public int diaDoEvento = 2;

    [Header("Quando a figura some")]
    [Tooltip("Quantos segundos o jogador precisa ficar olhando a janela até a figura sumir")]
    public float segundosOlhandoAteSumir = 2f;

    [Header("Tremida de susto")]
    [Tooltip("O que vai tremer. Se deixar vazio, treme o Panorama (o cenário inteiro da visão)")]
    public RectTransform objetoQueTreme;

    [Tooltip("Força da tremida em pixels. 10 a 20 é sutil, 30 ou mais é forte")]
    public float forcaDoTremor = 15f;

    [Tooltip("Quanto tempo dura a tremida (segundos)")]
    public float duracaoDoTremor = 0.5f;

    [Header("Para testar (deixe em Aleatorio no jogo final)")]
    public ModoDeTeste modoDeTeste = ModoDeTeste.Aleatorio;

    [Header("Mostra no Console o que está acontecendo")]
    public bool mostrarMensagens = true;

    private int diaJaSorteado = -1;
    private int ultimoDiaConferido = -1;
    private float tempoOlhando = 0f;
    private bool tremendo = false;

    private void Awake()
    {
        if (gameManager == null)
            gameManager = FindAnyObjectByType<GameManager>();

        if (navegacaoVisao == null)
            navegacaoVisao = FindAnyObjectByType<NavegacaoVisao>();

        EsconderFigura();
    }

    private void Update()
    {
        // Se o dia mudou (novo dia ou jogo reiniciado), a figura some e o sorteio recomeça
        if (gameManager != null && gameManager.diaAtual != ultimoDiaConferido)
        {
            ultimoDiaConferido = gameManager.diaAtual;
            diaJaSorteado = -1;
            tempoOlhando = 0f;
            EsconderFigura();
        }

        // Conta quanto tempo o jogador está olhando pra janela com a figura visível
        if (figuraMisteriosa == null || !figuraMisteriosa.activeSelf || navegacaoVisao == null)
        {
            tempoOlhando = 0f;
            return;
        }

        if (navegacaoVisao.OlhandoParaEsquerda)
        {
            tempoOlhando += Time.deltaTime;

            if (tempoOlhando >= segundosOlhandoAteSumir)
                Assustar();
        }
        else
        {
            // Olhou pra outro lado: a contagem recomeça
            tempoOlhando = 0f;
        }
    }

    // O LeitorDaParede chama isto quando o jogador abre o post-it especial
    public void SortearEvento()
    {
        int dia = (gameManager != null) ? gameManager.diaAtual : 1;

        // Só vale no dia certo
        if (dia != diaDoEvento)
            return;

        // Só sorteia UMA vez por dia (abrir o post-it de novo não muda o resultado)
        if (diaJaSorteado == dia)
            return;

        diaJaSorteado = dia;
        tempoOlhando = 0f;

        bool aparece;

        switch (modoDeTeste)
        {
            case ModoDeTeste.SempreAparece:
                aparece = true;
                break;

            case ModoDeTeste.NuncaAparece:
                aparece = false;
                break;

            default:
                aparece = Random.value < chanceDeAparecer;
                break;
        }

        if (figuraMisteriosa != null)
            figuraMisteriosa.SetActive(aparece);

        if (mostrarMensagens)
            Debug.Log("Day2EventManager: sorteio feito. A figura na janela " + (aparece ? "APARECEU." : "NÃO apareceu."));
    }

    // A figura some e a tela treme
    private void Assustar()
    {
        tempoOlhando = 0f;

        if (figuraMisteriosa != null)
            figuraMisteriosa.SetActive(false);

        if (mostrarMensagens)
            Debug.Log("Day2EventManager: a figura sumiu! Tremida de susto.");

        if (!tremendo)
            StartCoroutine(TremerTela());
    }

    private IEnumerator TremerTela()
    {
        tremendo = true;

        // Quem treme: o objeto escolhido ou, se vazio, o Panorama
        RectTransform alvo = objetoQueTreme;

        if (alvo == null && navegacaoVisao != null)
            alvo = navegacaoVisao.panorama;

        if (alvo == null)
        {
            tremendo = false;
            yield break;
        }

        // Durante a tremida o jogador não consegue virar a cabeça (evita bagunçar a posição)
        if (navegacaoVisao != null)
            navegacaoVisao.BloquearNavegacao(true);

        Vector2 posicaoBase = alvo.anchoredPosition;
        Vector2 deslocamentoAtual = Vector2.zero;
        float tempo = 0f;

        while (tempo < duracaoDoTremor)
        {
            tempo += Time.unscaledDeltaTime;

            // A tremida começa forte e vai diminuindo até parar
            float restante = 1f - Mathf.Clamp01(tempo / duracaoDoTremor);
            deslocamentoAtual = Random.insideUnitCircle * forcaDoTremor * restante;

            alvo.anchoredPosition = posicaoBase + deslocamentoAtual;

            yield return null;
        }

        // Volta exatamente para onde estava
        alvo.anchoredPosition = posicaoBase;

        if (navegacaoVisao != null)
            navegacaoVisao.BloquearNavegacao(false);

        tremendo = false;
    }

    private void EsconderFigura()
    {
        if (figuraMisteriosa != null)
            figuraMisteriosa.SetActive(false);
    }
}