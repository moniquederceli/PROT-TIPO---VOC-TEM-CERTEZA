using UnityEngine;
using System.Collections;

public class InicioJogo : MonoBehaviour
{
    [Header("Tela Inicial")]
    public GameObject telaInicial;

    [Header("Fundo da Tela Inicial")]
    public GameObject fundoInicial;

    [Header("Porta de Entrada")]
    public PortaEntrada portaEntrada;

    [Header("Game Manager")]
    public GameManager gameManager;

    public void Jogar()
    {
        StartCoroutine(IniciarJogo());
    }

    private IEnumerator IniciarJogo()
    {
        Debug.Log("INICIO: botão JOGAR foi pressionado");

        // =========================================
        // 1. ESCONDE A TELA INICIAL IMEDIATAMENTE
        // =========================================

        if (telaInicial != null)
        {
            telaInicial.SetActive(false);
        }

        // =========================================
        // 2. ESCONDE O FUNDO INICIAL IMEDIATAMENTE
        // =========================================

        if (fundoInicial != null)
        {
            fundoInicial.SetActive(false);
        }

        // =========================================
        // 3. ABRE A PORTA
        // =========================================

        Debug.Log("INICIO: chamando animação da porta");

        if (portaEntrada != null)
        {
            yield return StartCoroutine(portaEntrada.AbrirPortao());
        }

        // =========================================
        // 4. ANIMAÇÃO DA PORTA TERMINOU
        // =========================================

        Debug.Log("INICIO: animação terminou");

        // =========================================
        // 5. COMEÇA O JOGO
        // =========================================

        if (gameManager != null)
        {
            gameManager.IniciarJogo();
        }
    }
}