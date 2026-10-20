using UnityEngine;

// Coloque este script no objeto "SistemaMenu" (um objeto vazio que fica FORA do MenuPrincipal
// e que NUNCA é desligado).
//
// Ele cuida de qual painel do menu está aparecendo:
//   Principal  (Jogar / Configuração / Créditos)
//   Modos de jogo
//   Configurações
//   Créditos
public class MainMenuManager : MonoBehaviour
{
    [Header("O objeto que contém TODO o menu (o MenuPrincipal). Serve pra saber quando o menu volta a aparecer")]
    public GameObject menuRaiz;

    [Header("Os 4 painéis do menu")]
    public GameObject painelPrincipal;
    public GameObject painelModosDeJogo;
    public GameObject painelConfiguracoes;
    public GameObject painelCreditos;

    [Header("Controlador dos modos de jogo (arraste o próprio SistemaMenu aqui)")]
    public GameModeController gameModeController;

    private bool menuEstavaVisivel = false;

    private void Update()
    {
        // Toda vez que o menu APARECE (começo do jogo, ou ao voltar da partida),
        // ele reinicia mostrando o painel principal.
        bool visivel = (menuRaiz != null) && menuRaiz.activeInHierarchy;

        if (visivel && !menuEstavaVisivel)
        {
            MostrarSo(painelPrincipal);

            // O menu apareceu: música do menu
            AudioManager.MusicaDoMenu();

            if (gameModeController != null)
                gameModeController.PrepararMenu();
        }

        menuEstavaVisivel = visivel;
    }

    // ==========================================
    // MÉTODOS PARA LIGAR NOS BOTÕES (Ao Clicar)
    // ==========================================

    // Botão "Jogar"
    public void AbrirModosDeJogo()
    {
        MostrarSo(painelModosDeJogo);
    }

    // Botão "Configuração"
    public void AbrirConfiguracoes()
    {
        MostrarSo(painelConfiguracoes);
    }

    // Botão "Créditos"
    public void AbrirCreditos()
    {
        MostrarSo(painelCreditos);
    }

    // Todos os botões "Voltar"
    public void VoltarAoMenuPrincipal()
    {
        MostrarSo(painelPrincipal);
    }

    // Liga um painel e desliga os outros três
    private void MostrarSo(GameObject painel)
    {
        if (painelPrincipal != null) painelPrincipal.SetActive(painelPrincipal == painel);
        if (painelModosDeJogo != null) painelModosDeJogo.SetActive(painelModosDeJogo == painel);
        if (painelConfiguracoes != null) painelConfiguracoes.SetActive(painelConfiguracoes == painel);
        if (painelCreditos != null) painelCreditos.SetActive(painelCreditos == painel);
    }
}