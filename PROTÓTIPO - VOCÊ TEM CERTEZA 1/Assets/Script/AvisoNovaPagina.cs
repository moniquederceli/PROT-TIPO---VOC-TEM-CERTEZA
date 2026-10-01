using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class AvisoNovaPagina : MonoBehaviour
{
    [Header("Tempo que o aviso fica totalmente visível")]
    public float tempoVisivel = 3f;

    [Header("Tempo que o aviso demora pra sumir (fade out)")]
    public float tempoFade = 1f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        gameObject.SetActive(false);
    }

    // Chame este método (pelo GameManager) para mostrar o aviso
    public void Mostrar()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;

        StopAllCoroutines();
        StartCoroutine(EsconderAposTempo());
    }

    private IEnumerator EsconderAposTempo()
    {
        yield return new WaitForSeconds(tempoVisivel);

        float tempoDecorrido = 0f;

        while (tempoDecorrido < tempoFade)
        {
            tempoDecorrido += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, tempoDecorrido / tempoFade);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}