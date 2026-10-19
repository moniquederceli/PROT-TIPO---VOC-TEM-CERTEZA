using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Coloque este script na tela "TelaResumoFinal" (a mesma que aparece depois do Dia 2).
//
// O GameManager conta tudo durante a partida e, no fim, entrega os números para este script.
// Ele calcula a pontuação, descobre a NOTA (S+ até E-) e escreve tudo na tela.
public class EndGameReportManager : MonoBehaviour
{
    // ==========================================
    // TABELA DE PONTOS (quanto vale cada decisão)
    // Você pode mudar esses números no Inspector quando quiser.
    // ==========================================
    [System.Serializable]
    public class TabelaDePontos
    {
        [Header("NPC NORMAL (inocente)")]
        [Tooltip("Deixou entrar (certo)")]
        public int normalEntrou = 10;
        [Tooltip("Negou a entrada (errado)")]
        public int normalNegado = -10;
        [Tooltip("Matou na porta (errado, penalidade alta)")]
        public int normalMorto = -20;

        [Header("NPC COM DOCUMENTO ERRADO")]
        [Tooltip("Deixou entrar (muito ruim)")]
        public int docErradoEntrou = -20;
        [Tooltip("Negou a entrada (certo)")]
        public int docErradoNegado = 10;
        [Tooltip("Matou na porta (ruim)")]
        public int docErradoMorto = -10;

        [Header("NPC ANOMALIA (decaído)")]
        [Tooltip("Deixou entrar (péssimo, penalidade gravíssima)")]
        public int anomaliaEntrou = -40;
        [Tooltip("Negou a entrada (risco à segurança)")]
        public int anomaliaNegada = -10;
        [Tooltip("Matou na porta (excelente)")]
        public int anomaliaMorta = 20;
    }

    // ==========================================
    // TABELA DE NOTAS
    // A nota depende da PORCENTAGEM da pontuação máxima possível:
    // 100% = jogou perfeito. A primeira faixa que o jogador alcança é a nota dele.
    // ==========================================
    [System.Serializable]
    public class FaixaDeNota
    {
        [Tooltip("O texto da nota. Ex: S+")]
        public string nota = "S+";

        [Tooltip("Porcentagem MÍNIMA da pontuação máxima para ganhar esta nota")]
        public float porcentagemMinima = 100f;

        [Tooltip("Cor da nota na tela")]
        public Color cor = Color.white;

        public FaixaDeNota(string nota, float porcentagemMinima, Color cor)
        {
            this.nota = nota;
            this.porcentagemMinima = porcentagemMinima;
            this.cor = cor;
        }
    }

    [Header("TEXTOS DA TELA")]
    [Tooltip("A nota GRANDE no topo (ex: S+)")]
    public TMP_Text textoNota;

    [Tooltip("O relatório com as 4 linhas de contagem (parte de baixo)")]
    public TMP_Text textoRelatorio;

    [Tooltip("OPCIONAL: uma linha pequena com a pontuação (ex: Pontuação: 70 de 100 (70%))")]
    public TMP_Text textoPontuacao;

    [Header("FRASES DO RELATÓRIO ({0} vira o número)")]
    public string frasePessoasCorretas = "Pessoas corretas que você deixou entrar: {0}";
    public string frasePessoasErradas = "Pessoas com documento incorreto que você deixou entrar: {0}";
    public string fraseAnomaliasMortas = "Anomalias que você matou na porta: {0}";
    public string fraseInocentesMortos = "Inocentes que você matou por engano: {0}";
    public string frasePontuacao = "Pontuação: {0} de {1} ({2}%)";

    [Header("PONTUAÇÃO")]
    public TabelaDePontos pontos = new TabelaDePontos();

    [Header("NOTAS (da melhor para a pior)")]
    public List<FaixaDeNota> faixas = CriarFaixasPadrao();

    [Header("EFEITO")]
    [Tooltip("Ligado: a nota aparece 'crescendo' na tela")]
    public bool animarNota = true;

    [Header("Mostra a conta no Console (ajuda a ajustar as notas)")]
    public bool mostrarMensagens = true;

    // Valores iniciais das notas. Se quiser voltar a eles: no Inspector, clique nos 3 pontinhos
    // do componente e escolha "Reset".
    private void Reset()
    {
        faixas = CriarFaixasPadrao();
        pontos = new TabelaDePontos();
    }

    private static List<FaixaDeNota> CriarFaixasPadrao()
    {
        Color ouro = new Color(1f, 0.85f, 0.25f);
        Color verde = new Color(0.45f, 0.9f, 0.45f);
        Color azul = new Color(0.45f, 0.8f, 0.95f);
        Color amarelo = new Color(1f, 0.75f, 0.3f);
        Color laranja = new Color(1f, 0.5f, 0.2f);
        Color vermelho = new Color(0.9f, 0.2f, 0.2f);

        List<FaixaDeNota> lista = new List<FaixaDeNota>();

        lista.Add(new FaixaDeNota("S+", 100f, ouro));
        lista.Add(new FaixaDeNota("S", 80f, ouro));
        lista.Add(new FaixaDeNota("A+", 70f, verde));
        lista.Add(new FaixaDeNota("A", 60f, verde));
        lista.Add(new FaixaDeNota("B+", 50f, azul));
        lista.Add(new FaixaDeNota("B", 40f, azul));
        lista.Add(new FaixaDeNota("B-", 30f, azul));
        lista.Add(new FaixaDeNota("C", 20f, amarelo));
        lista.Add(new FaixaDeNota("C-", 10f, amarelo));
        lista.Add(new FaixaDeNota("D", 0f, laranja));
        lista.Add(new FaixaDeNota("D-", -15f, laranja));
        lista.Add(new FaixaDeNota("E", -35f, vermelho));
        lista.Add(new FaixaDeNota("E-", -1000f, vermelho));

        return lista;
    }

    // ==========================================
    // CHAMADO PELO GAMEMANAGER NO FIM DO JOGO
    // ==========================================
    public void MostrarRelatorio(
        int normalEntrou, int normalNegado, int normalMorto,
        int docErradoEntrou, int docErradoNegado, int docErradoMorto,
        int anomaliaEntrou, int anomaliaNegada, int anomaliaMorta)
    {
        // 1) Pontuação conseguida
        int total =
            normalEntrou * pontos.normalEntrou +
            normalNegado * pontos.normalNegado +
            normalMorto * pontos.normalMorto +
            docErradoEntrou * pontos.docErradoEntrou +
            docErradoNegado * pontos.docErradoNegado +
            docErradoMorto * pontos.docErradoMorto +
            anomaliaEntrou * pontos.anomaliaEntrou +
            anomaliaNegada * pontos.anomaliaNegada +
            anomaliaMorta * pontos.anomaliaMorta;

        // 2) Pontuação MÁXIMA possível (se cada NPC tivesse recebido a melhor decisão)
        int quantidadeNormais = normalEntrou + normalNegado + normalMorto;
        int quantidadeDocErrado = docErradoEntrou + docErradoNegado + docErradoMorto;
        int quantidadeAnomalias = anomaliaEntrou + anomaliaNegada + anomaliaMorta;

        int maximo =
            quantidadeNormais * Mathf.Max(pontos.normalEntrou, pontos.normalNegado, pontos.normalMorto) +
            quantidadeDocErrado * Mathf.Max(pontos.docErradoEntrou, pontos.docErradoNegado, pontos.docErradoMorto) +
            quantidadeAnomalias * Mathf.Max(pontos.anomaliaEntrou, pontos.anomaliaNegada, pontos.anomaliaMorta);

        // 3) Porcentagem e nota
        float porcentagem = (maximo > 0) ? (total * 100f / maximo) : 0f;
        FaixaDeNota faixa = EscolherFaixa(porcentagem);

        if (mostrarMensagens)
        {
            Debug.Log("EndGameReportManager: pontuação " + total + " de " + maximo
                + " (" + Mathf.RoundToInt(porcentagem) + "%) -> nota "
                + (faixa != null ? faixa.nota : "?"));
        }

        // 4) Escreve a nota grande
        if (textoNota != null && faixa != null)
        {
            textoNota.text = faixa.nota;
            textoNota.color = faixa.cor;

            if (animarNota && gameObject.activeInHierarchy)
                StartCoroutine(AnimarNota());
        }

        // 5) Escreve o relatório detalhado
        if (textoRelatorio != null)
        {
            textoRelatorio.text =
                string.Format(frasePessoasCorretas, normalEntrou) + "\n" +
                string.Format(frasePessoasErradas, docErradoEntrou) + "\n" +
                string.Format(fraseAnomaliasMortas, anomaliaMorta) + "\n" +
                string.Format(fraseInocentesMortos, normalMorto);
        }

        // 6) Linha da pontuação (opcional)
        if (textoPontuacao != null)
        {
            textoPontuacao.text = string.Format(frasePontuacao, total, maximo, Mathf.RoundToInt(porcentagem));
        }
    }

    // Escolhe a melhor nota que a porcentagem alcança
    private FaixaDeNota EscolherFaixa(float porcentagem)
    {
        FaixaDeNota escolhida = null;
        FaixaDeNota pior = null;

        foreach (FaixaDeNota faixa in faixas)
        {
            if (faixa == null)
                continue;

            if (pior == null || faixa.porcentagemMinima < pior.porcentagemMinima)
                pior = faixa;

            // +0.001 evita erros de arredondamento nas contas
            if (porcentagem + 0.001f >= faixa.porcentagemMinima)
            {
                if (escolhida == null || faixa.porcentagemMinima > escolhida.porcentagemMinima)
                    escolhida = faixa;
            }
        }

        // Se nenhuma faixa serviu (porcentagem baixíssima), vale a pior nota da lista
        return (escolhida != null) ? escolhida : pior;
    }

    // A nota "cresce" na tela, com um leve exagero no final
    private IEnumerator AnimarNota()
    {
        RectTransform rect = textoNota.rectTransform;

        float duracao = 0.45f;
        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(tempo / duracao);

            // Cresce até 1.15 e assenta em 1.0
            float escala;

            if (t < 0.7f)
                escala = Mathf.Lerp(0.2f, 1.15f, t / 0.7f);
            else
                escala = Mathf.Lerp(1.15f, 1f, (t - 0.7f) / 0.3f);

            rect.localScale = new Vector3(escala, escala, 1f);

            yield return null;
        }

        rect.localScale = Vector3.one;
    }
}