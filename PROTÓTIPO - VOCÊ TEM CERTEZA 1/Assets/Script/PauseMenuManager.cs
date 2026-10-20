using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Coloque este script UMA VEZ SÓ em um objeto vazio chamado "SistemaPausa"
// (um objeto que fica SEMPRE LIGADO, fora do painel de pausa).
//
// ESC durante a partida: pausa o jogo e abre o painel de pausa.
// ESC de novo (ou botão Continuar): fecha o painel e o jogo continua.
// Botão Sair: volta para o Menu Principal (o jogo recomeça do zero).
public class PauseMenuManager : MonoBehaviour
{
    [Header("O painel de pausa (começa DESLIGADO no Hierarchy)")]
    public GameObject painelDePausa;

    [Header("O objeto MenuPrincipal (o ESC não faz nada enquanto o menu estiver aparecendo)")]
    public GameObject menuRaiz;

    [Header("OPCIONAL: tela de Resumo Final (o ESC não pausa nela)")]
    public GameObject telaResumoFinal;

    [Header("OPCIONAL: se o Menu Principal for OUTRA cena, escreva o nome dela aqui. Vazio = recarrega esta mesma cena")]
    public string nomeDaCenaDoMenu = "";

    [Header("Mostra mensagens no Console")]
    public bool mostrarMensagens = true;

    private bool pausado = false;

    public bool EstaPausado
    {
        get { return pausado; }
    }

    private void Start()
    {
        // Garante que o jogo nunca começa congelado
        Time.timeScale = 1f;

        if (painelDePausa != null)
            painelDePausa.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            Alternar();
    }

    private void OnDestroy()
    {
        // Se este objeto sumir, o jogo não pode ficar congelado
        Time.timeScale = 1f;
    }

    public void Alternar()
    {
        if (pausado)
            Continuar();
        else
            Pausar();
    }

    public void Pausar()
    {
        if (pausado)
            return;

        // Não pausa no menu principal, durante a porta de entrada, nem no resumo final
        if (menuRaiz != null && menuRaiz.activeInHierarchy)
            return;

        if (GameModeController.Iniciando)
            return;

        if (telaResumoFinal != null && telaResumoFinal.activeInHierarchy)
            return;

        pausado = true;
        Time.timeScale = 0f;

        if (painelDePausa != null)
            painelDePausa.SetActive(true);

        if (mostrarMensagens)
            Debug.Log("PauseMenuManager: jogo PAUSADO.");
    }

    // Ligue ao botão "Continuar"
    public void Continuar()
    {
        pausado = false;
        Time.timeScale = 1f;

        if (painelDePausa != null)
            painelDePausa.SetActive(false);

        if (mostrarMensagens)
            Debug.Log("PauseMenuManager: jogo continua.");
    }

    // Ligue ao botão "Sair"
    public void SairParaOMenu()
    {
        pausado = false;
        Time.timeScale = 1f;

        if (mostrarMensagens)
            Debug.Log("PauseMenuManager: voltando ao Menu Principal.");

        string cena = string.IsNullOrEmpty(nomeDaCenaDoMenu)
            ? SceneManager.GetActiveScene().name
            : nomeDaCenaDoMenu;

        // Recarregar a cena zera TUDO (NPCs, dia, dinheiro...) e o jogo volta ao Menu Principal
        SceneManager.LoadScene(cena);
    }
}