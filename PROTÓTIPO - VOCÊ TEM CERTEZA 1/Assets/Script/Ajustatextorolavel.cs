using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Coloque este script no TEXTO (TextMeshPro) que fica dentro do Scroll View
// do jornal e também no do diário (um script em cada texto).
//
// O que ele faz: toda vez que a caixa de texto abre, ele mede o tamanho real do texto
// e aumenta a área rolável (Content) para caber tudo. Sem isso, o Content fica
// pequeno e a rolagem "volta" pro meio.
[RequireComponent(typeof(TMP_Text))]
public class AjustaTextoRolavel : MonoBehaviour
{
    [Header("Folga extra no final do texto (pixels)")]
    public float folgaNoFinal = 30f;

    [Header("Velocidade da rodinha do mouse")]
    public float sensibilidadeDaRodinha = 30f;

    private TMP_Text texto;
    private ScrollRect scroll;

    private void OnEnable()
    {
        texto = GetComponent<TMP_Text>();
        scroll = GetComponentInParent<ScrollRect>();

        Ajustar();

        // Repete no frame seguinte, depois do Unity terminar de montar o layout
        StartCoroutine(AjustarDepois());
    }

    private IEnumerator AjustarDepois()
    {
        yield return null;
        Ajustar();
    }

    public void Ajustar()
    {
        if (texto == null)
            return;

        // Nunca corta o texto
        texto.overflowMode = TextOverflowModes.Overflow;

        RectTransform textoRect = texto.rectTransform;

        // O "Content" é a área que rola. Se não tiver ScrollRect, usamos o próprio texto.
        RectTransform content = textoRect;
        RectTransform viewport = null;

        if (scroll != null)
        {
            if (scroll.content != null)
                content = scroll.content;

            viewport = (scroll.viewport != null) ? scroll.viewport : scroll.GetComponent<RectTransform>();

            // Configura a rolagem do jeito certo
            scroll.vertical = true;
            scroll.horizontal = false;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = sensibilidadeDaRodinha;
        }

        Canvas.ForceUpdateCanvases();

        // Largura disponível pro texto
        float largura = (viewport != null) ? viewport.rect.width : content.rect.width;

        if (largura <= 0f)
            largura = textoRect.rect.width;

        // Altura que o texto realmente precisa nessa largura
        float alturaDoTexto = texto.GetPreferredValues(texto.text, largura, 0f).y;
        float alturaTotal = alturaDoTexto + folgaNoFinal;

        // O Content estica na largura e fica preso no topo
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(1f, 1f);
        content.pivot = new Vector2(0.5f, 1f);
        content.sizeDelta = new Vector2(0f, alturaTotal);
        content.anchoredPosition = new Vector2(0f, 0f);

        // Se o texto é um filho do Content, ele preenche o Content inteiro
        if (content != textoRect)
        {
            textoRect.anchorMin = new Vector2(0f, 1f);
            textoRect.anchorMax = new Vector2(1f, 1f);
            textoRect.pivot = new Vector2(0.5f, 1f);
            textoRect.sizeDelta = new Vector2(0f, alturaTotal);
            textoRect.anchoredPosition = new Vector2(0f, 0f);
        }

        // Sempre começa lendo do topo
        if (scroll != null)
            scroll.verticalNormalizedPosition = 1f;
    }
}