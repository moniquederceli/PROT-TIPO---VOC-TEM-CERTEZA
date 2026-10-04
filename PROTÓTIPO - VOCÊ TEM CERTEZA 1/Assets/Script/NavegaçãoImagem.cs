using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavegacaoVisao : MonoBehaviour
{
    private enum Visao
    {
        Esquerda,
        Centro,
        Direita
    }

    [Header("O painel que contém as imagens de fundo lado a lado")]
    public RectTransform panorama;

    [Header("Largura de UMA tela (a mesma largura do seu Canvas, ex: 1920)")]
    public float larguraTela = 1920f;

    [Header("Tudo que pertence SÓ à visão central (cenário da cabine, mesa, documentos, botões...)")]
    public GameObject[] elementosDaVisaoCentral;

    [Header("Setas de navegação")]
    public GameObject setaEsquerda;
    public GameObject setaDireita;

    [Header("Painel de 'flash' usado no efeito de borrão (Image branca ou preta, começa com Alpha 0)")]
    public Image flashTransicao;

    [Header("Configuração do efeito")]
    public float duracaoTransicao = 0.35f;
    [Range(0f, 1f)]
    public float intensidadeFlash = 0.6f;

    private Visao visaoAtual = Visao.Centro;
    private bool emTransicao = false;

    // Motivos para esconder as setas. Se QUALQUER um deles estiver ligado, as setas somem.
    private bool jogoIniciado = false;
    private bool diarioAberto = false;
    private bool relatorioAberto = false;
    private bool transicaoDiaAtiva = false;

    // Guardamos os elementos do centro e o quanto já os deslocamos
    private List<RectTransform> rectsCentrais = new List<RectTransform>();
    private List<CanvasGroup> gruposCentrais = new List<CanvasGroup>();
    private float deslocamentoAplicado = 0f;

    private void Start()
    {
        PrepararElementosCentrais();

        if (panorama != null)
            panorama.anchoredPosition = new Vector2(0f, panorama.anchoredPosition.y);

        deslocamentoAplicado = 0f;

        DefinirAlfaFlash(0f);

        DefinirInteracaoCentral(true);
        AtualizarSetas();
    }

    // Monta a lista dos elementos do centro e garante que cada um tem um CanvasGroup
    // (o CanvasGroup é o que permite bloquear os cliques sem desligar o objeto).
    private void PrepararElementosCentrais()
    {
        rectsCentrais.Clear();
        gruposCentrais.Clear();

        if (elementosDaVisaoCentral == null)
            return;

        foreach (GameObject elemento in elementosDaVisaoCentral)
        {
            if (elemento == null)
                continue;

            // Se este elemento é filho de OUTRO elemento da lista, ignora:
            // ele já acompanha o pai sozinho e não pode ser movido duas vezes.
            bool ehFilhoDeOutro = false;

            foreach (GameObject outro in elementosDaVisaoCentral)
            {
                if (outro != null && outro != elemento && elemento.transform.IsChildOf(outro.transform))
                {
                    ehFilhoDeOutro = true;
                    break;
                }
            }

            if (ehFilhoDeOutro)
                continue;

            RectTransform rt = elemento.GetComponent<RectTransform>();
            if (rt == null)
                continue;

            CanvasGroup grupo = elemento.GetComponent<CanvasGroup>();
            if (grupo == null)
                grupo = elemento.AddComponent<CanvasGroup>();

            rectsCentrais.Add(rt);
            gruposCentrais.Add(grupo);
        }
    }

    // ==========================================
    // CLIQUES DAS SETAS
    // ==========================================

    // Ligue este método no ONCLICK da seta esquerda
    public void SetaEsquerdaClicada()
    {
        if (emTransicao)
            return;

        if (visaoAtual == Visao.Direita)
            IniciarTransicao(Visao.Centro);
        else if (visaoAtual == Visao.Centro)
            IniciarTransicao(Visao.Esquerda);
    }

    // Ligue este método no ONCLICK da seta direita
    public void SetaDireitaClicada()
    {
        if (emTransicao)
            return;

        if (visaoAtual == Visao.Esquerda)
            IniciarTransicao(Visao.Centro);
        else if (visaoAtual == Visao.Centro)
            IniciarTransicao(Visao.Direita);
    }

    // ==========================================
    // CHAMADOS POR OUTROS SCRIPTS (Diário e GameManager)
    // ==========================================

    // O GameManager avisa quando o jogo realmente começou (true) ou voltou ao menu (false).
    // Antes disso (menu e animação da porta de entrada) as setas ficam escondidas.
    public void DefinirJogoIniciado(bool iniciado)
    {
        jogoIniciado = iniciado;
        AtualizarSetas();
    }

    // O Diário avisa quando abre (true) e quando fecha (false)
    public void DefinirDiarioAberto(bool aberto)
    {
        diarioAberto = aberto;
        AtualizarSetas();
    }

    // O GameManager avisa quando a tela de relatório/resumo está aberta (true) ou não (false)
    public void DefinirRelatorioAberto(bool aberto)
    {
        relatorioAberto = aberto;
        AtualizarSetas();
    }

    // O GameManager avisa quando a tela preta "DIA X" está na tela (true) ou não (false)
    public void DefinirTransicaoDiaAtiva(bool ativa)
    {
        transicaoDiaAtiva = ativa;
        AtualizarSetas();
    }

    // Volta IMEDIATAMENTE para a visão do centro, sem animação
    public void ResetarParaCentro()
    {
        // Se estava no meio de uma transição, cancela ela
        StopAllCoroutines();
        emTransicao = false;

        MoverTudo(0f);
        DefinirAlfaFlash(0f);

        visaoAtual = Visao.Centro;

        DefinirInteracaoCentral(true);
        AtualizarSetas();
    }

    // ==========================================
    // TRANSIÇÃO
    // ==========================================
    private void IniciarTransicao(Visao novaVisao)
    {
        StartCoroutine(Transicionar(novaVisao));
    }

    private IEnumerator Transicionar(Visao novaVisao)
    {
        emTransicao = true;

        // Durante o movimento ninguém pode clicar nos elementos do centro.
        // IMPORTANTE: nada é desligado (SetActive), então os NPCs continuam andando normalmente.
        DefinirInteracaoCentral(false);

        float posicaoInicial = panorama.anchoredPosition.x;
        float posicaoFinal = PosicaoXParaVisao(novaVisao);
        float tempoDecorrido = 0f;

        while (tempoDecorrido < duracaoTransicao)
        {
            tempoDecorrido += Time.deltaTime;
            float progresso = Mathf.Clamp01(tempoDecorrido / duracaoTransicao);

            // Move o fundo e TODOS os elementos do centro juntos, pelo mesmo valor
            float x = Mathf.SmoothStep(posicaoInicial, posicaoFinal, progresso);
            MoverTudo(x);

            // O flash sobe rápido na primeira metade e desce na segunda metade
            float alfaFlash;

            if (progresso < 0.5f)
                alfaFlash = Mathf.Lerp(0f, intensidadeFlash, progresso / 0.5f);
            else
                alfaFlash = Mathf.Lerp(intensidadeFlash, 0f, (progresso - 0.5f) / 0.5f);

            DefinirAlfaFlash(alfaFlash);

            yield return null;
        }

        // Garante que termina exatamente no lugar certo
        MoverTudo(posicaoFinal);
        DefinirAlfaFlash(0f);

        visaoAtual = novaVisao;

        // Só libera os cliques da mesa quando estiver de volta no centro
        DefinirInteracaoCentral(novaVisao == Visao.Centro);

        AtualizarSetas();

        emTransicao = false;
    }

    // Move o panorama e os elementos do centro pela mesma quantidade
    private void MoverTudo(float x)
    {
        if (panorama != null)
            panorama.anchoredPosition = new Vector2(x, panorama.anchoredPosition.y);

        float diferenca = x - deslocamentoAplicado;

        foreach (RectTransform rt in rectsCentrais)
        {
            if (rt != null)
                rt.anchoredPosition += new Vector2(diferenca, 0f);
        }

        deslocamentoAplicado = x;
    }

    private void DefinirAlfaFlash(float alfa)
    {
        if (flashTransicao == null)
            return;

        Color c = flashTransicao.color;
        c.a = alfa;
        flashTransicao.color = c;
    }

    // Liga/desliga os cliques nos elementos do centro (sem desligar os objetos)
    private void DefinirInteracaoCentral(bool permitido)
    {
        foreach (CanvasGroup grupo in gruposCentrais)
        {
            if (grupo != null)
            {
                grupo.interactable = permitido;
                grupo.blocksRaycasts = permitido;
            }
        }
    }

    private float PosicaoXParaVisao(Visao visao)
    {
        switch (visao)
        {
            case Visao.Esquerda:
                return larguraTela;
            case Visao.Direita:
                return -larguraTela;
            default:
                return 0f;
        }
    }

    private void AtualizarSetas()
    {
        // As setas somem por completo se o jogo ainda não começou (menu e porta de entrada)
        // ou se o diário, o relatório ou a tela "DIA X" estiverem abertos.
        bool ocultarTudo = !jogoIniciado || diarioAberto || relatorioAberto || transicaoDiaAtiva;

        // Fora disso: esconde a seta esquerda se já estiver no lado esquerdo,
        // e a seta direita se já estiver no lado direito.
        if (setaEsquerda != null)
            setaEsquerda.SetActive(!ocultarTudo && visaoAtual != Visao.Esquerda);

        if (setaDireita != null)
            setaDireita.SetActive(!ocultarTudo && visaoAtual != Visao.Direita);
    }
}