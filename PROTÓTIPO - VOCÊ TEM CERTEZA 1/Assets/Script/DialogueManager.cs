using System.Collections;
using UnityEngine;
using TMPro;

// Coloque este script UMA VEZ SÓ, em um objeto vazio chamado "SistemaDialogo".
// IMPORTANTE: este objeto precisa ficar SEMPRE LIGADO. Só o BALÃO (outro objeto) é ligado e desligado.
public class DialogueManager : MonoBehaviour
{
    // Atalho para os outros scripts chamarem: DialogueManager.Instance
    public static DialogueManager Instance { get; private set; }

    [Header("O balão de fala")]
    [Tooltip("O objeto do balão inteiro (a Image). O script liga e desliga ele.")]
    public GameObject balao;

    [Tooltip("O texto (TextMeshPro) que fica DENTRO do balão")]
    public TMP_Text textoDoBalao;

    [Header("Balão que cresce junto com o texto")]
    [Tooltip("Ligado: a ALTURA do balão aumenta linha por linha enquanto o texto é digitado")]
    public bool balaoCresceComTexto = true;

    [Tooltip("Altura mínima do balão (quando só tem a 1ª linha). Aumente se quiser o balão maior")]
    public float alturaMinima = 80f;

    [Tooltip("Folga extra somada na altura (use valores pequenos, ex: 0 a 20)")]
    public float alturaExtra = 0f;

    [Header("Efeito de digitação")]
    [Tooltip("Quantas letras aparecem por segundo. Maior = mais rápido. Sugestão: 25 a 40")]
    public float letrasPorSegundo = 30f;

    [Tooltip("Pausa extra (em segundos) depois de . ! ? , para a fala ficar mais natural. Use 0 para desligar")]
    public float pausaNaPontuacao = 0.15f;

    [Header("Tempos")]
    [Tooltip("Espera depois que um balão termina de digitar, ANTES de passar pro próximo balão")]
    public float esperaEntreBaloes = 1.2f;

    [Tooltip("Depois do ÚLTIMO balão, quanto tempo ele continua na tela antes de sumir sozinho")]
    public float tempoVisivelNoFinal = 2.5f;

    [Tooltip("Pequena espera entre a chegada do NPC e o começo da fala")]
    public float atrasoParaComecar = 0.1f;

    private Coroutine falaAtual;
    private GameObject quemEstaFalando;
    private RectTransform balaoRect;
    private RectTransform textoRect;

    public bool EstaFalando
    {
        get { return falaAtual != null; }
    }

    private void Awake()
    {
        Instance = this;

        if (balao != null)
        {
            balaoRect = balao.GetComponent<RectTransform>();
            balao.SetActive(false);
        }

        if (textoDoBalao != null)
        {
            textoRect = textoDoBalao.rectTransform;

            // Sem cortar letras que passam do tamanho
            textoDoBalao.overflowMode = TextOverflowModes.Overflow;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Update()
    {
        // Segurança: se o NPC que estava falando foi desligado (recusado, morto pela porta,
        // foi embora...), o balão some na hora.
        if (falaAtual != null && (quemEstaFalando == null || !quemEstaFalando.activeInHierarchy))
            Parar();
    }

    // ==========================================
    // CHAMADOS POR OUTROS SCRIPTS
    // ==========================================

    // O NPCMovement chama isto quando o NPC chega e para na frente do vidro
    public void FalarAbertura(DialogoNPC dialogo)
    {
        if (dialogo == null)
            return;

        DialogoNPC.Fala fala = dialogo.EscolherAbertura();
        Falar(fala, dialogo.gameObject);
    }

    // Fala qualquer "Fala" (abertura agora; no futuro também respostas do chat)
    public void Falar(DialogoNPC.Fala fala, GameObject falante)
    {
        Parar();

        // Modo (in)Finito: os NPCs ficam calados
        if (!GameModeController.FalasAtivas)
            return;

        if (fala == null || fala.baloes == null || fala.baloes.Length == 0)
            return;

        quemEstaFalando = falante;
        falaAtual = StartCoroutine(RotinaDeFala(fala));
    }

    // Para tudo e esconde o balão
    public void Parar()
    {
        if (falaAtual != null)
        {
            StopCoroutine(falaAtual);
            falaAtual = null;
        }

        quemEstaFalando = null;

        if (textoDoBalao != null)
            textoDoBalao.text = "";

        if (balao != null)
            balao.SetActive(false);
    }

    // ==========================================
    // A ROTINA QUE FAZ A FALA ACONTECER
    // ==========================================
    private IEnumerator RotinaDeFala(DialogoNPC.Fala fala)
    {
        if (atrasoParaComecar > 0f)
            yield return new WaitForSeconds(atrasoParaComecar);

        if (balao != null)
            balao.SetActive(true);

        for (int i = 0; i < fala.baloes.Length; i++)
        {
            string pagina = fala.baloes[i];

            if (string.IsNullOrEmpty(pagina))
                continue;

            yield return Digitar(pagina);

            bool ehUltimo = (i == fala.baloes.Length - 1);

            if (ehUltimo)
                yield return new WaitForSeconds(tempoVisivelNoFinal);
            else
                yield return new WaitForSeconds(esperaEntreBaloes);
        }

        // Acabou: some sozinho
        falaAtual = null;
        Parar();
    }

    // Efeito de máquina de escrever: o texto completo já existe,
    // mas só mostramos uma letra a mais a cada passo.
    // O balão cresce linha por linha conforme as letras vão aparecendo.
    private IEnumerator Digitar(string pagina)
    {
        if (textoDoBalao == null)
            yield break;

        textoDoBalao.text = pagina;
        textoDoBalao.maxVisibleCharacters = 0;

        // Faz o TextMeshPro calcular o texto agora, pra sabermos quantas letras e linhas ele tem
        textoDoBalao.ForceMeshUpdate();

        TMP_TextInfo info = textoDoBalao.textInfo;
        int total = info.characterCount;
        int totalLinhas = Mathf.Max(1, info.lineCount);

        // Guarda "quanta altura o texto precisa até a linha N" (soma da altura das linhas)
        float[] alturaAteLinha = new float[totalLinhas];
        float soma = 0f;

        for (int l = 0; l < totalLinhas; l++)
        {
            soma += info.lineInfo[l].lineHeight;
            alturaAteLinha[l] = soma;
        }

        // Começa com o balão do tamanho de 1 linha
        AplicarAlturaDoBalao(alturaAteLinha[0]);

        float tempoPorLetra = 1f / Mathf.Max(1f, letrasPorSegundo);

        for (int visiveis = 1; visiveis <= total; visiveis++)
        {
            // Descobre em qual linha está a letra que vai aparecer agora e ajusta o balão ANTES de mostrá-la
            int linha = Mathf.Clamp(info.characterInfo[visiveis - 1].lineNumber, 0, totalLinhas - 1);
            AplicarAlturaDoBalao(alturaAteLinha[linha]);

            textoDoBalao.maxVisibleCharacters = visiveis;

            float espera = tempoPorLetra;

            // Pausinha em pontuação
            if (pausaNaPontuacao > 0f)
            {
                char c = info.characterInfo[visiveis - 1].character;

                if (c == '.' || c == '!' || c == '?' || c == ',')
                    espera += pausaNaPontuacao;
            }

            yield return new WaitForSeconds(espera);
        }

        // Garante que o texto inteiro está aparecendo
        textoDoBalao.maxVisibleCharacters = 99999;
    }

    // Muda a altura do balão para caber o texto (usa as margens que você já deixou no TextoFala)
    private void AplicarAlturaDoBalao(float alturaDoTexto)
    {
        if (!balaoCresceComTexto || balaoRect == null || textoRect == null)
            return;

        // Margens de cima + embaixo do texto dentro do balão (vêm do Top/Bottom do TextoFala)
        float margens = textoRect.offsetMin.y + (-textoRect.offsetMax.y);

        float altura = Mathf.Max(alturaMinima, alturaDoTexto + margens + alturaExtra);

        balaoRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, altura);
    }
}