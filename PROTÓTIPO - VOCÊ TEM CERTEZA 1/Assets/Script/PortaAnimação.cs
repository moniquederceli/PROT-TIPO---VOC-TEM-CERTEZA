using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PortaAnimacao : MonoBehaviour
{
    [Header("Imagem da Porta")]
    public Image porta;

    [Header("Frames da Animação")]
    public Sprite frame1;
    public Sprite frame2;
    public Sprite frame3;
    public Sprite frame4;
    public Sprite frame5;

    [Header("Velocidade")]
    public float tempoEntreFrames = 0.25f;

    [Header("Tempo com a porta fechada")]
    public float tempoPortaFechada = 1.5f;

    private bool animando = false;

    private void Start()
    {
        // A porta começa invisível
        Color cor = porta.color;
        cor.a = 0f;
        porta.color = cor;
    }

    public void FecharPorta()
    {
        // Impede clicar novamente durante a animação
        if (animando)
            return;

        StartCoroutine(AnimarPorta());
    }

    IEnumerator AnimarPorta()
    {
        animando = true;

        // =========================
        // PORTA APARECE
        // =========================

        Color cor = porta.color;
        cor.a = 1f;
        porta.color = cor;

        // =========================
        // FECHANDO
        // =========================

        porta.sprite = frame1;
        yield return new WaitForSeconds(tempoEntreFrames);

        porta.sprite = frame2;
        yield return new WaitForSeconds(tempoEntreFrames);

        porta.sprite = frame3;
        yield return new WaitForSeconds(tempoEntreFrames);

        porta.sprite = frame4;
        yield return new WaitForSeconds(tempoEntreFrames);

        porta.sprite = frame5;

        // =========================
        // PORTA FICA FECHADA
        // =========================

        yield return new WaitForSeconds(tempoPortaFechada);

        // =========================
        // ABRINDO
        // =========================

        porta.sprite = frame4;
        yield return new WaitForSeconds(tempoEntreFrames);

        porta.sprite = frame3;
        yield return new WaitForSeconds(tempoEntreFrames);

        porta.sprite = frame2;
        yield return new WaitForSeconds(tempoEntreFrames);

        porta.sprite = frame1;
        yield return new WaitForSeconds(tempoEntreFrames);

        // =========================
        // PORTA SOME
        // =========================

        cor = porta.color;
        cor.a = 0f;
        porta.color = cor;

        animando = false;
    }
}