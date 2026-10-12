using System.Collections;
using UnityEngine;

// Coloque este script num texto/aviso que aparece por alguns segundos e some com fade
// (usado para a mensagem "Preencher documentação").
// IMPORTANTE: este objeto fica LIGADO o tempo todo no editor. Ele fica invisível sozinho
// (Alpha 0) e não bloqueia cliques do mouse.
[RequireComponent(typeof(CanvasGroup))]
public class AvisoTemporario : MonoBehaviour
{
    [Header("Tempo que o aviso fica totalmente visível (segundos)")]
    public float tempoVisivel = 1.2f;

    [Header("Tempo que o aviso demora pra sumir (fade out, em segundos)")]
    public float tempoFade = 0.5f;

    private CanvasGroup grupo;

    private void Awake()
    {
        PrepararGrupo();
        grupo.alpha = 0f;
    }

    private void PrepararGrupo()
    {
        if (grupo != null)
            return;

        grupo = GetComponent<CanvasGroup>();

        // O aviso nunca atrapalha os cliques do jogo
        grupo.interactable = false;
        grupo.blocksRaycasts = false;
    }

    // Chame este método para mostrar o aviso
    public void Mostrar()
    {
        PrepararGrupo();

        // Se o objeto estiver desligado no editor, liga
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

        // Garante que o aviso fica na frente de tudo que está no mesmo grupo
        transform.SetAsLastSibling();

        grupo.alpha = 1f;

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
            grupo.alpha = Mathf.Lerp(1f, 0f, tempoDecorrido / tempoFade);
            yield return null;
        }

        grupo.alpha = 0f;
    }
}