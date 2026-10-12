using UnityEngine;
using UnityEngine.UI;

// Coloque este script UMA VEZ SÓ, em um objeto vazio chamado "SistemaTutorial".
// Ele controla o tutorial inteiro:
//   envelope fechado -> envelope aberto -> papel -> menu dos 3 papéis -> papel grande.
public class TutorialEnvelopeManager : MonoBehaviour
{
    [Header("Navegação de visão (esconde as setas enquanto o tutorial está aberto). Opcional: se ficar vazio, o script acha sozinho")]
    public NavegacaoVisao navegacaoVisao;

    [Header("=== 1) O ENVELOPE NA PAREDE ===")]
    [Tooltip("A imagem do envelope na parede (a que troca entre fechado e aberto)")]
    public Image imagemEnvelope;
    public Sprite spriteEnvelopeFechado;
    public Sprite spriteEnvelopeAberto;

    [Tooltip("Área invisível em cima da tampa do envelope. Só existe com o envelope ABERTO.")]
    public GameObject areaDaTampa;

    [Tooltip("O papel que aparece dentro do envelope aberto.")]
    public GameObject papelDoEnvelope;

    [Header("=== 2) O MENU DOS 3 PAPÉIS ===")]
    [Tooltip("Fundo escurecido atrás dos papéis (clicar nele fecha). Opcional, mas recomendado.")]
    public GameObject fundoEscuro;

    [Tooltip("O objeto 'pai' que contém os 3 papéis sobrepostos")]
    public GameObject menuDosPapeis;

    [Tooltip("Os 3 papéis do menu, NA ORDEM: Element 0 = Regras, Element 1 = Selos, Element 2 = Documentos")]
    public HoverEffect[] papeisDoMenu;

    [Header("Efeito de destaque ao passar o mouse")]
    [Tooltip("Cor do papel que está com o mouse em cima (branco = brilho normal)")]
    public Color corDestacada = Color.white;

    [Tooltip("Cor dos OUTROS papéis enquanto um está destacado (mais escura = mais apagado)")]
    public Color corEscurecida = new Color(0.4f, 0.4f, 0.4f, 1f);

    [Tooltip("O papel destacado cresce um pouquinho (1 = não cresce, 1.05 = 5% maior)")]
    public float escalaDestacada = 1.05f;

    [Header("=== 3) OS 3 PAPÉIS GRANDES DE LEITURA ===")]
    [Tooltip("NA MESMA ORDEM: Element 0 = Regras, Element 1 = Selos, Element 2 = Documentos")]
    public GameObject[] papeisGrandes;

    private bool envelopeAberto = false;
    private bool menuAberto = false;
    private int papelGrandeAberto = -1;
    private Vector2[] posicoesIniciais;

    private void Awake()
    {
        if (navegacaoVisao == null)
            navegacaoVisao = FindAnyObjectByType<NavegacaoVisao>();

        // Guarda onde você deixou cada papel grande no editor.
        // Toda vez que ele abrir, volta para este lugar.
        if (papeisGrandes != null)
        {
            posicoesIniciais = new Vector2[papeisGrandes.Length];

            for (int i = 0; i < papeisGrandes.Length; i++)
            {
                if (papeisGrandes[i] != null)
                {
                    RectTransform rt = papeisGrandes[i].GetComponent<RectTransform>();
                    if (rt != null)
                        posicoesIniciais[i] = rt.anchoredPosition;
                }
            }
        }

        // Avisa cada papel do menu qual é o número dele (0, 1 ou 2)
        if (papeisDoMenu != null)
        {
            for (int i = 0; i < papeisDoMenu.Length; i++)
            {
                if (papeisDoMenu[i] != null)
                    papeisDoMenu[i].Configurar(this, i);
            }
        }

        EsconderMenuEPapeis();
        AtualizarEnvelope();
    }

    // ==========================================
    // ETAPA 1: ENVELOPE
    // ==========================================

    // Ligue no ONCLICK do envelope (a imagem principal do envelope)
    public void CliqueNoEnvelope()
    {
        if (menuAberto || envelopeAberto)
            return;

        envelopeAberto = true;
        AtualizarEnvelope();
    }

    // Ligue no ONCLICK da área da tampa
    public void FecharEnvelope()
    {
        if (menuAberto || !envelopeAberto)
            return;

        envelopeAberto = false;
        AtualizarEnvelope();
    }

    // Ligue no ONCLICK do papel que está dentro do envelope
    public void PegarPapel()
    {
        if (menuAberto || !envelopeAberto)
            return;

        menuAberto = true;
        papelGrandeAberto = -1;

        AtualizarEnvelope();

        if (fundoEscuro != null) fundoEscuro.SetActive(true);
        if (menuDosPapeis != null) menuDosPapeis.SetActive(true);

        AplicarDestaque(-1);

        // O jogador não pode virar a cabeça com o tutorial aberto
        if (navegacaoVisao != null)
            navegacaoVisao.DefinirDiarioAberto(true);
    }

    // Deixa o envelope e o papel dentro dele do jeito certo
    private void AtualizarEnvelope()
    {
        if (imagemEnvelope != null)
        {
            Sprite sprite = envelopeAberto ? spriteEnvelopeAberto : spriteEnvelopeFechado;

            if (sprite != null)
                imagemEnvelope.sprite = sprite;
        }

        if (areaDaTampa != null)
            areaDaTampa.SetActive(envelopeAberto);

        // O papel só aparece com o envelope aberto e enquanto você NÃO está lendo o tutorial
        if (papelDoEnvelope != null)
            papelDoEnvelope.SetActive(envelopeAberto && !menuAberto);
    }

    // ==========================================
    // ETAPA 2: MENU DOS 3 PAPÉIS
    // ==========================================

    // Chamado pelo HoverEffect quando o mouse ENTRA num papel
    public void AoEntrarNoPapel(int indice)
    {
        AplicarDestaque(indice);
    }

    // Chamado pelo HoverEffect quando o mouse SAI de um papel
    public void AoSairDoPapel(int indice)
    {
        AplicarDestaque(-1);
    }

    // indiceDestacado = -1 significa "nenhum": todos ficam normais
    private void AplicarDestaque(int indiceDestacado)
    {
        if (papeisDoMenu == null)
            return;

        for (int i = 0; i < papeisDoMenu.Length; i++)
        {
            if (papeisDoMenu[i] == null)
                continue;

            if (indiceDestacado < 0)
                papeisDoMenu[i].AplicarAparencia(Color.white, 1f);
            else if (i == indiceDestacado)
                papeisDoMenu[i].AplicarAparencia(corDestacada, escalaDestacada);
            else
                papeisDoMenu[i].AplicarAparencia(corEscurecida, 1f);
        }
    }

    // Ligue no ONCLICK do fundo escurecido (clicar fora)
    public void CliqueNoFundo()
    {
        if (papelGrandeAberto >= 0)
            FecharPapelGrande();
        else
            FecharMenu();
    }

    // Fecha o menu dos 3 papéis (botão direito num papel, ou clique no fundo)
    public void FecharMenu()
    {
        if (!menuAberto)
            return;

        menuAberto = false;
        papelGrandeAberto = -1;

        EsconderMenuEPapeis();
        AtualizarEnvelope();   // o papel volta para dentro do envelope

        if (navegacaoVisao != null)
            navegacaoVisao.DefinirDiarioAberto(false);
    }

    // ==========================================
    // ETAPA 3: PAPEL GRANDE
    // ==========================================

    // Chamado pelo HoverEffect quando você clica com o botão esquerdo num dos 3 papéis
    public void AbrirPapelGrande(int indice)
    {
        if (!menuAberto || papelGrandeAberto >= 0)
            return;

        if (papeisGrandes == null || indice < 0 || indice >= papeisGrandes.Length || papeisGrandes[indice] == null)
        {
            Debug.LogError("TutorialEnvelopeManager: o papel grande número " + indice + " não está configurado em 'Papeis Grandes'.");
            return;
        }

        papelGrandeAberto = indice;

        // O menu dos 3 papéis some enquanto você lê (o fundo escuro continua)
        if (menuDosPapeis != null)
            menuDosPapeis.SetActive(false);

        GameObject papel = papeisGrandes[indice];

        // Abre sempre no lugar original
        RectTransform rt = papel.GetComponent<RectTransform>();
        if (rt != null && posicoesIniciais != null && indice < posicoesIniciais.Length)
            rt.anchoredPosition = posicoesIniciais[indice];

        papel.SetActive(true);
        papel.transform.SetAsLastSibling();
    }

    // Fecha o papel grande e volta para o menu dos 3 papéis (botão direito no papel)
    public void FecharPapelGrande()
    {
        if (papelGrandeAberto < 0)
            return;

        papelGrandeAberto = -1;

        if (papeisGrandes != null)
        {
            foreach (GameObject papel in papeisGrandes)
            {
                if (papel != null)
                    papel.SetActive(false);
            }
        }

        if (menuDosPapeis != null)
            menuDosPapeis.SetActive(true);

        AplicarDestaque(-1);
    }

    private void EsconderMenuEPapeis()
    {
        if (fundoEscuro != null) fundoEscuro.SetActive(false);
        if (menuDosPapeis != null) menuDosPapeis.SetActive(false);

        if (papeisGrandes != null)
        {
            foreach (GameObject papel in papeisGrandes)
            {
                if (papel != null)
                    papel.SetActive(false);
            }
        }
    }
}