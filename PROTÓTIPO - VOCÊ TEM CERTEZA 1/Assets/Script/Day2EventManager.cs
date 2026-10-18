using UnityEngine;

// Coloque este script UMA VEZ SÓ, em um objeto vazio chamado "SistemaEventos"
// (um objeto que fica SEMPRE LIGADO).
//
// O que ele faz:
//  - Quando o jogador abre o post-it especial do Dia 2, o jogo "joga uma moeda" (50%).
//  - Se der SIM, a figura misteriosa aparece na paisagem da janela (visão da esquerda).
//  - Se der NÃO, a janela fica normal.
//  - A cada novo dia (ou jogo reiniciado), a figura some e o sorteio recomeça.
public class Day2EventManager : MonoBehaviour
{
    public enum ModoDeTeste
    {
        Aleatorio,       // normal: sorteia de verdade
        SempreAparece,   // só pra TESTAR: a figura sempre aparece
        NuncaAparece     // só pra TESTAR: a figura nunca aparece
    }

    [Header("Referência ao GameManager (pra saber em qual dia estamos)")]
    public GameManager gameManager;

    [Header("A figura misteriosa (a imagem que fica entre a paisagem e a moldura da janela)")]
    public GameObject figuraMisteriosa;

    [Header("Configuração do sorteio")]
    [Tooltip("Chance de a figura aparecer. 0.5 = 50%. 1 = sempre. 0 = nunca")]
    [Range(0f, 1f)]
    public float chanceDeAparecer = 0.5f;

    [Tooltip("Em qual dia o evento pode acontecer")]
    public int diaDoEvento = 2;

    [Header("Para testar (deixe em Aleatorio no jogo final)")]
    public ModoDeTeste modoDeTeste = ModoDeTeste.Aleatorio;

    [Header("Mostra no Console o resultado do sorteio")]
    public bool mostrarMensagens = true;

    private int diaJaSorteado = -1;
    private int ultimoDiaConferido = -1;

    private void Awake()
    {
        if (gameManager == null)
            gameManager = FindAnyObjectByType<GameManager>();

        EsconderFigura();
    }

    private void Update()
    {
        // Se o dia mudou (novo dia ou jogo reiniciado), a figura some e o sorteio recomeça
        if (gameManager != null && gameManager.diaAtual != ultimoDiaConferido)
        {
            ultimoDiaConferido = gameManager.diaAtual;
            diaJaSorteado = -1;
            EsconderFigura();
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

    private void EsconderFigura()
    {
        if (figuraMisteriosa != null)
            figuraMisteriosa.SetActive(false);
    }
}