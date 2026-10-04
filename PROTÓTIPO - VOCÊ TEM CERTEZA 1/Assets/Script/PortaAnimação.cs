using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PortaAnimacao : MonoBehaviour
{
    [Header("Imagem da Porta")]
    public Image porta;

    [Header("Frames da Animação")]
    public Sprite frame1;
    public Sprite frame2;
    public Sprite frame3;
    public Sprite frame4;
    public Sprite frame5;

    [Header("Velocidade")]
    public float tempoEntreFrames = 0.25f;

    [Header("Tempo com a porta fechada")]
    public float tempoPortaFechada = 1.5f;

    [Header("Luz vermelha de alarme (pisca enquanto a porta está fechada)")]
    public Image luzVermelha;
    public float piscadasPorSegundo = 3f;
    [Range(0f, 1f)]
    public float intensidadeMaxima = 0.5f;

    [Header("Imagem do Botão")]
    public Image imagemBotao;

    [Header("Sprites do Botão")]
    public Sprite botaoNormal;
    public Sprite botaoApertado;

    [Header("Game Manager")]
    public GameManager gameManager;

    private bool animando = false;

    private void Start()
    {
        // =================================
        // PORTA COMEÇA INVISÍVEL
        // =================================

        Color cor = porta.color;
        cor.a = 0f;
        porta.color = cor;

        // =================================
        // LUZ VERMELHA COMEÇA APAGADA
        // =================================

        DefinirAlfaLuz(0f);

        // =================================
        // BOTÃO COMEÇA COM A IMAGEM NORMAL
        // =================================

        if (imagemBotao != null && botaoNormal != null)
        {
            imagemBotao.sprite = botaoNormal;
        }
    }

    public void FecharPorta()
    {
        // Impede clicar novamente durante a animação
        if (animando)
            return;

        // =================================
        // BOTÃO FOI APERTADO
        // =================================

        if (imagemBotao != null && botaoApertado != null)
        {
            imagemBotao.sprite = botaoApertado;
        }

        // Trava os botões Aceitar/Recusar enquanto a porta faz sua animação
        if (gameManager != null)
        {
            gameManager.DesabilitarBotoesDecisao();
        }

        // =================================
        // COMEÇA A ANIMAÇÃO DA PORTA
        // =================================

        StartCoroutine(AnimarPorta());
    }

    IEnumerator AnimarPorta()
    {
        animando = true;

        // =========================
        // PORTA APARECE
        // =========================

        Color cor = porta.color;
        cor.a = 1f;
        porta.color = cor;

        // =========================
        // FECHANDO
        // =========================

        porta.sprite = frame1;
        yield return new WaitForSeconds(tempoEntreFrames);

        porta.sprite = frame2;
        yield return new WaitForSeconds(tempoEntreFrames);

        porta.sprite = frame3;
        yield return new WaitForSeconds(tempoEntreFrames);

        porta.sprite = frame4;
        yield return new WaitForSeconds(tempoEntreFrames);

        porta.sprite = frame5;

        // =========================
        // PORTA ESTÁ TOTALMENTE FECHADA AGORA.
        // O NPC que estava na tela "morre" imediatamente.
        // =========================

        if (gameManager != null)
        {
            gameManager.EsconderNPCAtual();
        }

        // =========================
        // PORTA FICA FECHADA E A LUZ VERMELHA PISCA
        // (a luz para de piscar assim que este tempo acaba,
        // logo antes da porta começar a subir)
        // =========================

        yield return StartCoroutine(PiscarLuz(tempoPortaFechada));

        // =========================
        // ABRINDO
        // =========================

        porta.sprite = frame4;
        yield return new WaitForSeconds(tempoEntreFrames);

        porta.sprite = frame3;
        yield return new WaitForSeconds(tempoEntreFrames);

        porta.sprite = frame2;
        yield return new WaitForSeconds(tempoEntreFrames);

        porta.sprite = frame1;
        yield return new WaitForSeconds(tempoEntreFrames);

        // =========================
        // PORTA TOTALMENTE ABERTA:
        // o próximo NPC pode entrar na tela agora.
        // =========================

        if (gameManager != null)
        {
            gameManager.ContinuarAposPorta();
        }

        // =========================
        // PORTA SOME
        // =========================

        cor = porta.color;
        cor.a = 0f;
        porta.color = cor;

        // =================================
        // BOTÃO VOLTA A FICAR NORMAL
        // =================================

        if (imagemBotao != null && botaoNormal != null)
        {
            imagemBotao.sprite = botaoNormal;
        }

        animando = false;
    }

    // Faz a luz vermelha pulsar (acende e apaga suavemente) durante o tempo informado.
    // Se não houver luz configurada, ela simplesmente espera esse tempo.
    private IEnumerator PiscarLuz(float duracao)
    {
        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;

            // Onda que começa apagada (0), vai até o máximo (1) e volta
            float onda = (1f - Mathf.Cos(tempo * piscadasPorSegundo * 2f * Mathf.PI)) / 2f;
            DefinirAlfaLuz(onda * intensidadeMaxima);

            yield return null;
        }

        // Apaga a luz no fim
        DefinirAlfaLuz(0f);
    }

    private void DefinirAlfaLuz(float alfa)
    {
        if (luzVermelha == null)
            return;

        Color c = luzVermelha.color;
        c.a = alfa;
        luzVermelha.color = c;
    }
}