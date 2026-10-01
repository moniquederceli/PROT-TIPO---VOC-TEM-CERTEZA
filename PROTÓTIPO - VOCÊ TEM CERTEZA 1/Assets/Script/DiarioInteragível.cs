using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiarioInterativo : MonoBehaviour
{
    // Uma "página" do diário: uma imagem + um texto de leitura
    [System.Serializable]
    public class PaginaDiario
    {
        public Sprite imagem;

        [TextArea(5, 20)]
        public string texto;
    }

    [Header("As páginas do diário (Element 0 = Página 1, Element 1 = Página 2...)")]
    public PaginaDiario[] paginas;

    [Header("Referência ao GameManager (pra saber em qual dia estamos)")]
    public GameManager gameManager;

    [Header("Fundo escurecido atrás do diário")]
    public GameObject overlayEscuro;

    [Header("O painel do diário (imagem fixa no centro)")]
    public GameObject painelDiario;
    public Image imagemPagina;

    [Header("Caixa de texto (aparece no segundo clique)")]
    public GameObject caixaTexto;
    public TMP_Text textoPagina;

    [Header("Setas de navegação entre páginas")]
    public GameObject setaEsquerda;
    public GameObject setaDireita;

    private int paginaAtual = 0;
    private bool modoLeitura = false;

    private void Awake()
    {
        if (overlayEscuro != null) overlayEscuro.SetActive(false);
        if (painelDiario != null) painelDiario.SetActive(false);
        if (caixaTexto != null) caixaTexto.SetActive(false);
    }

    // Ligue este método no ONCLICK do ícone do diário na mesa
    public void AbrirDiario()
    {
        paginaAtual = 0;
        modoLeitura = false;

        if (overlayEscuro != null) overlayEscuro.SetActive(true);
        if (painelDiario != null) painelDiario.SetActive(true);
        if (caixaTexto != null) caixaTexto.SetActive(false);

        MostrarPaginaAtual();
    }

    // Ligue este método no ONCLICK do fundo escurecido (clicar fora fecha o diário)
    public void FecharDiario()
    {
        if (overlayEscuro != null) overlayEscuro.SetActive(false);
        if (painelDiario != null) painelDiario.SetActive(false);
        if (caixaTexto != null) caixaTexto.SetActive(false);

        modoLeitura = false;
    }

    // Ligue este método no ONCLICK da imagem da página (alterna mostrar/esconder o texto)
    public void AlternarModoLeitura()
    {
        modoLeitura = !modoLeitura;

        if (caixaTexto != null)
            caixaTexto.SetActive(modoLeitura);
    }

    // Ligue este método no ONCLICK da seta direita
    public void ProximaPagina()
    {
        bool existeProximaPagina = (paginaAtual + 1) < paginas.Length;

        // A Página 2 (índice 1) só existe a partir do Dia 2, e assim por diante
        int diaNecessario = paginaAtual + 2;
        bool diaDesbloqueado = (gameManager == null) || (gameManager.diaAtual >= diaNecessario);

        if (existeProximaPagina && diaDesbloqueado)
        {
            paginaAtual++;
            MostrarPaginaAtual();
        }
    }

    // Ligue este método no ONCLICK da seta esquerda
    public void PaginaAnterior()
    {
        if (paginaAtual > 0)
        {
            paginaAtual--;
            MostrarPaginaAtual();
        }
    }

    private void MostrarPaginaAtual()
    {
        if (paginas == null || paginaAtual >= paginas.Length)
            return;

        PaginaDiario pagina = paginas[paginaAtual];

        if (imagemPagina != null)
            imagemPagina.sprite = pagina.imagem;

        if (textoPagina != null)
            textoPagina.text = pagina.texto;

        // Toda vez que troca de página, esconde a caixa de texto de novo
        modoLeitura = false;
        if (caixaTexto != null)
            caixaTexto.SetActive(false);

        AtualizarSetas();
    }

    private void AtualizarSetas()
    {
        bool existeProximaPagina = (paginaAtual + 1) < paginas.Length;
        int diaNecessario = paginaAtual + 2;
        bool proximaDesbloqueada = (gameManager == null) || (gameManager.diaAtual >= diaNecessario);

        if (setaDireita != null)
            setaDireita.SetActive(existeProximaPagina && proximaDesbloqueada);

        if (setaEsquerda != null)
            setaEsquerda.SetActive(paginaAtual > 0);
    }
}