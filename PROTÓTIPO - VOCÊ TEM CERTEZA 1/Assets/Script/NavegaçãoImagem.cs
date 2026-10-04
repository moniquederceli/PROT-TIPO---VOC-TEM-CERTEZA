using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NavegacaoVisao : MonoBehaviour
{
    private enum Visao
    {
        Esquerda,
        Centro,
        Direita
    }

    [Header("O painel que contém as 3 imagens lado a lado")]
    public RectTransform panorama;

    [Header("Largura de UMA tela (a mesma largura do seu Canvas, ex: 1920)")]
    public float larguraTela = 1920f;

    [Header("Grupo da mesa principal (documentos, NPCs, porta, diário...)")]
    public GameObject mesaPrincipal;

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

    private void Start()
    {
        if (panorama != null)
            panorama.anchoredPosition = new Vector2(0f, panorama.anchoredPosition.y);

        if (flashTransicao != null)
        {
            Color c = flashTransicao.color;
            c.a = 0f;
            flashTransicao.color = c;
        }

        AtualizarSetas();
    }

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

    private void IniciarTransicao(Visao novaVisao)
    {
        StartCoroutine(Transicionar(novaVisao));
    }

    private IEnumerator Transicionar(Visao novaVisao)
    {
        emTransicao = true;

        // Assim que começa a mover, já esconde a mesa principal (se estiver saindo do centro)
        if (novaVisao != Visao.Centro && mesaPrincipal != null)
            mesaPrincipal.SetActive(false);

        float posicaoInicial = panorama.anchoredPosition.x;
        float posicaoFinal = PosicaoXParaVisao(novaVisao);
        float tempoDecorrido = 0f;

        while (tempoDecorrido < duracaoTransicao)
        {
            tempoDecorrido += Time.deltaTime;
            float progresso = Mathf.Clamp01(tempoDecorrido / duracaoTransicao);

            // Move o panorama suavemente
            float x = Mathf.SmoothStep(posicaoInicial, posicaoFinal, progresso);
            panorama.anchoredPosition = new Vector2(x, panorama.anchoredPosition.y);

            // O flash sobe rápido na primeira metade e desce na segunda metade
            if (flashTransicao != null)
            {
                float alfaFlash;

                if (progresso < 0.5f)
                    alfaFlash = Mathf.Lerp(0f, intensidadeFlash, progresso / 0.5f);
                else
                    alfaFlash = Mathf.Lerp(intensidadeFlash, 0f, (progresso - 0.5f) / 0.5f);

                Color c = flashTransicao.color;
                c.a = alfaFlash;
                flashTransicao.color = c;
            }

            yield return null;
        }

        // Garante que termina exatamente no lugar certo
        panorama.anchoredPosition = new Vector2(posicaoFinal, panorama.anchoredPosition.y);

        if (flashTransicao != null)
        {
            Color c = flashTransicao.color;
            c.a = 0f;
            flashTransicao.color = c;
        }

        visaoAtual = novaVisao;

        // Só mostra a mesa principal de novo quando voltar pro centro
        if (novaVisao == Visao.Centro && mesaPrincipal != null)
            mesaPrincipal.SetActive(true);

        AtualizarSetas();

        emTransicao = false;
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
        // Esconde a seta esquerda se já estiver no lado esquerdo,
        // e a seta direita se já estiver no lado direito.
        if (setaEsquerda != null)
            setaEsquerda.SetActive(visaoAtual != Visao.Esquerda);

        if (setaDireita != null)
            setaDireita.SetActive(visaoAtual != Visao.Direita);
    }
}