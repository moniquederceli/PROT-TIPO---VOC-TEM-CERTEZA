using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Coloque este script no objeto "SistemaMenu" (o mesmo do MainMenuManager).
//
// Ele cuida de:
//  - escolher o modo de jogo (História = com falas | (in)Finito = SEM falas dos NPCs);
//  - tocar a animação da porta grande abrindo;
//  - esconder o menu e começar a partida de verdade.
public class GameModeController : MonoBehaviour
{
    // ==========================================
    // OUTROS SCRIPTS CONSULTAM ISTO
    // ==========================================

    // false no Modo (in)Finito: os NPCs ficam calados
    public static bool FalasAtivas = true;

    // true enquanto a porta está abrindo (o menu fica travado)
    public static bool Iniciando = false;

    [Header("Referência ao GameManager")]
    public GameManager gameManager;

    [Header("O que ESCONDER quando a partida começa (ex: o MenuPrincipal inteiro)")]
    public GameObject[] esconderAoIniciar;

    [Header("PORTA GRANDE de entrada")]
    [Tooltip("O objeto da porta grande. Ele aparece cobrindo a tela, abre, e some")]
    public GameObject portaEntrada;

    [Header("Jeito 1: porta feita com FRAMES (imagens)")]
    [Tooltip("A Image da porta. Se deixar vazio, o script procura sozinho dentro da PortaEntrada")]
    public Image imagemDaPorta;

    [Tooltip("Os frames da porta, na ordem em que VOCÊ tem: Element 0 = porta ABERTA ... último = porta FECHADA")]
    public Sprite[] framesAbrindo;

    [Tooltip("Marcado: os frames estão do ABERTO para o FECHADO (como você montou). O script toca de trás pra frente, fechada -> aberta. Desmarque se montou do fechado para o aberto")]
    public bool framesVaoDoAbertoAteFechado = true;

    [Tooltip("Tempo de cada frame (segundos)")]
    public float tempoEntreFrames = 0.25f;

    [Tooltip("Quanto tempo a porta fica FECHADA antes de começar a abrir (segundos)")]
    public float tempoPortaFechada = 0.6f;

    [Header("Jeito 2: porta feita com ANIMATOR (animação do Unity)")]
    [Tooltip("O Animator da porta (deixe vazio se usar frames)")]
    public Animator animatorDaPorta;

    [Tooltip("Nome do Trigger que faz a porta abrir. Deixe vazio se a animação já toca sozinha quando a porta liga")]
    public string nomeDoTrigger = "";

    [Tooltip("Quanto tempo dura a animação (segundos). Só vale para o Animator")]
    public float duracaoDaAnimacao = 2f;

    [Header("Espera depois da porta abrir, antes da partida começar (segundos)")]
    public float esperaFinal = 0.2f;

    [Header("Mostra mensagens no Console para descobrir problemas")]
    public bool mostrarMensagens = true;

    private void Awake()
    {
        if (gameManager == null)
            gameManager = FindAnyObjectByType<GameManager>();
    }

    // Garante que o menu está pronto: destravado e porta escondida
    public void PrepararMenu()
    {
        Iniciando = false;

        if (portaEntrada != null)
            portaEntrada.SetActive(false);
    }

    // ==========================================
    // MÉTODOS PARA LIGAR NOS BOTÕES (Ao Clicar)
    // ==========================================

    // Botão "Modo história": jogo completo, com falas
    public void IniciarModoHistoria()
    {
        ComecarPartida(true);
    }

    // Botão "Modo (in)finito": jogo normal, SEM falas dos NPCs
    public void IniciarModoInfinito()
    {
        ComecarPartida(false);
    }

    // Botões ainda não prontos (por enquanto não fazem nada)
    public void IniciarModoTimer()
    {
    }

    public void IniciarModoDificil()
    {
    }

    // ==========================================
    // COMEÇO DA PARTIDA
    // ==========================================
    private void ComecarPartida(bool comFalas)
    {
        if (Iniciando)
            return;

        FalasAtivas = comFalas;
        StartCoroutine(RotinaDeInicio());
    }

    private IEnumerator RotinaDeInicio()
    {
        Iniciando = true;

        // Se o campo da imagem estiver vazio, procura a Image dentro da PortaEntrada
        if (imagemDaPorta == null && portaEntrada != null)
        {
            imagemDaPorta = portaEntrada.GetComponent<Image>();

            if (imagemDaPorta == null)
                imagemDaPorta = portaEntrada.GetComponentInChildren<Image>(true);
        }

        bool temFrames = (imagemDaPorta != null && framesAbrindo != null && framesAbrindo.Length > 0);

        if (mostrarMensagens)
        {
            Debug.Log("GameModeController: porta=" + (portaEntrada != null ? portaEntrada.name : "VAZIO")
                + " | imagem da porta=" + (imagemDaPorta != null ? imagemDaPorta.name : "VAZIO")
                + " | frames=" + (framesAbrindo != null ? framesAbrindo.Length : 0)
                + " | animator=" + (animatorDaPorta != null ? animatorDaPorta.name : "nenhum"));
        }

        // 1) A porta aparece, fechada. A posição dela no Hierarchy NÃO é mexida:
        //    ela fica exatamente onde você deixou (abaixo do GrennFilter).
        if (portaEntrada != null)
            portaEntrada.SetActive(true);

        if (temFrames)
        {
            imagemDaPorta.enabled = true;
            imagemDaPorta.raycastTarget = true;   // bloqueia cliques no que está atrás da porta
            MostrarFrame(PrimeiroFrame());
        }

        // 2) Com a porta cobrindo tudo, esconde o menu
        if (esconderAoIniciar != null)
        {
            foreach (GameObject objeto in esconderAoIniciar)
            {
                if (objeto != null)
                    objeto.SetActive(false);
            }
        }

        // O jogador saiu do menu: troca para a música do jogo
        AudioManager.MusicaDoJogo();

        // Espera 1 frame: se a porta tinha algum outro script que muda a imagem ao ligar,
        // ele já rodou. Aí reforçamos nossa imagem.
        yield return null;

        if (temFrames)
            MostrarFrame(PrimeiroFrame());

        // 3) A porta fica fechada um tempinho e depois abre
        if (temFrames)
        {
            if (tempoPortaFechada > 0f)
                yield return new WaitForSeconds(tempoPortaFechada);

            // Som mecânico da porta de entrada
            AudioManager.SomDePorta();

            // Toca do FECHADO até o ABERTO
            if (framesVaoDoAbertoAteFechado)
            {
                for (int i = framesAbrindo.Length - 1; i >= 0; i--)
                {
                    MostrarFrame(framesAbrindo[i]);
                    yield return new WaitForSeconds(tempoEntreFrames);
                }
            }
            else
            {
                for (int i = 0; i < framesAbrindo.Length; i++)
                {
                    MostrarFrame(framesAbrindo[i]);
                    yield return new WaitForSeconds(tempoEntreFrames);
                }
            }
        }
        else
        {
            if (animatorDaPorta != null && !string.IsNullOrEmpty(nomeDoTrigger))
                animatorDaPorta.SetTrigger(nomeDoTrigger);

            yield return new WaitForSeconds(duracaoDaAnimacao);
        }

        if (esperaFinal > 0f)
            yield return new WaitForSeconds(esperaFinal);

        // 4) A porta some e a partida começa
        if (portaEntrada != null)
            portaEntrada.SetActive(false);

        Iniciando = false;

        if (gameManager != null)
            gameManager.IniciarJogo();
    }

    // O frame que mostra a porta FECHADA (o primeiro a aparecer)
    private Sprite PrimeiroFrame()
    {
        if (framesVaoDoAbertoAteFechado)
            return framesAbrindo[framesAbrindo.Length - 1];

        return framesAbrindo[0];
    }

    // Troca a imagem da porta e garante que ela está visível (alpha 1)
    private void MostrarFrame(Sprite frame)
    {
        if (imagemDaPorta == null)
            return;

        imagemDaPorta.sprite = frame;

        Color cor = imagemDaPorta.color;
        cor.a = 1f;
        imagemDaPorta.color = cor;
    }
}