using UnityEngine;
using UnityEngine.UI;

// Coloque este script em imagens PNG que têm partes transparentes (folhas, "todas as folhas"...).
// Com ele, o clique só funciona em cima do DESENHO. Nas partes transparentes o clique "atravessa"
// e vai para o que estiver atrás. Isso permite empilhar folhas do mesmo tamanho sem uma bloquear a outra.
//
// IMPORTANTE: no Project, selecione o arquivo PNG, marque "Read/Write" no Inspector e clique em Apply.
[RequireComponent(typeof(Image))]
public class ClicaSoNoDesenho : MonoBehaviour
{
    [Header("Partes MAIS transparentes que isto não recebem clique (0.1 = quase invisível)")]
    [Range(0.01f, 1f)]
    public float transparenciaMinima = 0.1f;

    private void Awake()
    {
        Image imagem = GetComponent<Image>();
        imagem.alphaHitTestMinimumThreshold = transparenciaMinima;
    }
}