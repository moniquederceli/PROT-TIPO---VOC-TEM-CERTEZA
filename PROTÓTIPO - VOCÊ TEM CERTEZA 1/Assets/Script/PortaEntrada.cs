using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PortaEntrada : MonoBehaviour
{
    [Header("Imagem da Porta")]
    public Image porta;

    [Header("Frames da Animação")]
    public Sprite imagem5;
    public Sprite imagem4;
    public Sprite imagem3;
    public Sprite imagem2;
    public Sprite imagem1;

    [Header("Velocidade")]
    public float tempoEntreFrames = 0.25f;

    private bool animando = false;

    private void Awake()
    {
        if (porta != null)
        {
            porta.enabled = false;
        }
    }

    public IEnumerator AbrirPortao()
    {
        if (animando)
            yield break;

        animando = true;

        Debug.Log("PORTA: começou");

        // Garante que a porta fique NA FRENTE do fundo
        porta.transform.SetAsLastSibling();

        // Mostra a porta
        porta.enabled = true;

        // IMAGEM 5
        porta.sprite = imagem5;
        Debug.Log("PORTA: imagem 5");
        yield return new WaitForSeconds(tempoEntreFrames);

        // IMAGEM 4
        porta.sprite = imagem4;
        Debug.Log("PORTA: imagem 4");
        yield return new WaitForSeconds(tempoEntreFrames);

        // IMAGEM 3
        porta.sprite = imagem3;
        Debug.Log("PORTA: imagem 3");
        yield return new WaitForSeconds(tempoEntreFrames);

        // IMAGEM 2
        porta.sprite = imagem2;
        Debug.Log("PORTA: imagem 2");
        yield return new WaitForSeconds(tempoEntreFrames);

        // IMAGEM 1
        porta.sprite = imagem1;
        Debug.Log("PORTA: imagem 1");
        yield return new WaitForSeconds(tempoEntreFrames);

        // Faz a porta desaparecer
        porta.enabled = false;

        Debug.Log("PORTA: terminou");

        animando = false;
    }
}