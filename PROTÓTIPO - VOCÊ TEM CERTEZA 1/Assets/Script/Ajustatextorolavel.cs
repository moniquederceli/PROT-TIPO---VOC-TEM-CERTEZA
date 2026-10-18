using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Coloque este script no TEXTO (TextMeshPro) que fica dentro do Scroll View
// (um script em cada texto: jornal e diário).
//
// Estrutura esperada no Hierarchy:
//   CaixaTexto      (tem o componente Scroll Rect)
//     Viewport      (a "janela" que recorta o texto)
//       Content     (a área que anda pra cima e pra baixo)
//         Texto     (este script fica aqui)
//
// O script religa essas peças sozinho, mede o texto e ajusta a área rolável.
[RequireComponent(typeof(TMP_Text))]
public class AjustaTextoRolavel : MonoBehaviour
{
    [Header("Folga extra no final do texto (pixels)")]
    public float folgaNoFinal = 30f;

    [Header("Velocidade da rodinha do mouse")]
    public float sensibilidadeDaRodinha = 30f;

    [Header("Escreve um relatório no Console quando abre (desligue depois que funcionar)")]
    public bool mostrarRelatorio = true;

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

        if (scroll == null)
        {
            Debug.LogError("AjustaTextoRolavel (" + name + "): não achei nenhum componente 'Scroll Rect' acima deste texto. Selecione o objeto CaixaTexto e adicione o componente Scroll Rect.");
            return;
        }

        // Nunca corta o texto, e precisa ser clicável para a rodinha funcionar em cima dele
        texto.overflowMode = TextOverflowModes.Overflow;
        texto.raycastTarget = true;

        RectTransform textoRect = texto.rectTransform;

        // 1) Descobre quem é o Content (o pai do texto) e quem é o Viewport (o pai do Content)
        RectTransform content;

        if (scroll.content != null && textoRect.IsChildOf(scroll.content) && scroll.content != textoRect)
            content = scroll.content;
        else if (textoRect.parent != null && textoRect.parent != scroll.transform)
            content = textoRect.parent as RectTransform;
        else
            content = textoRect;

        RectTransform viewport = null;

        if (content != textoRect && content.parent != null && content.parent != scroll.transform)
            viewport = content.parent as RectTransform;
        else if (scroll.viewport != null)
            viewport = scroll.viewport;
        else
            viewport = scroll.GetComponent<RectTransform>();

        // 2) Liga o Scroll Rect nas peças certas
        scroll.content = content;
        scroll.viewport = viewport;
        scroll.enabled = true;
        scroll.vertical = true;
        scroll.horizontal = false;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = sensibilidadeDaRodinha;

        // 3) O Viewport precisa recortar o que passa da janela
        if (viewport != null && viewport.GetComponent<Mask>() == null && viewport.GetComponent<RectMask2D>() == null)
            viewport.gameObject.AddComponent<RectMask2D>();

        // 4) Desliga componentes de layout que brigariam com a altura calculada aqui
        DesligarLayouts(content);
        DesligarLayouts(textoRect);

        Canvas.ForceUpdateCanvases();

        // 5) Mede o texto
        float largura = viewport.rect.width;

        if (largura <= 0f)
            largura = textoRect.rect.width;

        float alturaDoTexto = texto.GetPreferredValues(texto.text, largura, 0f).y;
        float alturaTotal = alturaDoTexto + folgaNoFinal;

        // 6) O Content estica na largura, fica preso no topo e tem a altura do texto
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(1f, 1f);
        content.pivot = new Vector2(0.5f, 1f);
        content.sizeDelta = new Vector2(0f, alturaTotal);
        content.anchoredPosition = Vector2.zero;

        if (content != textoRect)
        {
            textoRect.anchorMin = new Vector2(0f, 1f);
            textoRect.anchorMax = new Vector2(1f, 1f);
            textoRect.pivot = new Vector2(0.5f, 1f);
            textoRect.sizeDelta = new Vector2(0f, alturaTotal);
            textoRect.anchoredPosition = Vector2.zero;
        }

        // 7) Sempre começa lendo do topo
        scroll.verticalNormalizedPosition = 1f;

        if (mostrarRelatorio)
        {
            float alturaJanela = viewport.rect.height;
            Debug.Log("AjustaTextoRolavel (" + name + "): texto precisa de " + Mathf.RoundToInt(alturaTotal)
                + " px | janela (Viewport) tem " + Mathf.RoundToInt(alturaJanela) + " px | "
                + (alturaTotal > alturaJanela ? "VAI ROLAR" : "cabe inteiro, não precisa rolar")
                + " | Content=" + content.name + " | Viewport=" + viewport.name);
        }
    }

    private void DesligarLayouts(RectTransform alvo)
    {
        foreach (ContentSizeFitter f in alvo.GetComponents<ContentSizeFitter>())
            f.enabled = false;

        foreach (LayoutGroup g in alvo.GetComponents<LayoutGroup>())
            g.enabled = false;
    }
}