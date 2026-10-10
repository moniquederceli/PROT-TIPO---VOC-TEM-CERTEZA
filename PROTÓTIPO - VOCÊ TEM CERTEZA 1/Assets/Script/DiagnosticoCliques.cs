using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// SCRIPT TEMPORÁRIO DE DIAGNÓSTICO.
// A cada clique com o botão esquerdo, escreve no Console quais objetos da interface
// estão embaixo do mouse, começando pelo que está mais na frente.
// Quando o problema for resolvido, remova este componente do EventSystem.
public class DiagnosticoCliques : MonoBehaviour
{
    private readonly List<RaycastResult> resultados = new List<RaycastResult>();

    private void Update()
    {
        bool clicou = false;
        Vector2 posicao = Vector2.zero;

#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            clicou = true;
            posicao = Mouse.current.position.ReadValue();
        }
#elif ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetMouseButtonDown(0))
        {
            clicou = true;
            posicao = Input.mousePosition;
        }
#endif

        if (!clicou || EventSystem.current == null)
            return;

        PointerEventData dados = new PointerEventData(EventSystem.current);
        dados.position = posicao;

        resultados.Clear();
        EventSystem.current.RaycastAll(dados, resultados);

        StringBuilder texto = new StringBuilder();
        texto.AppendLine("DIAGNÓSTICO: clique em " + posicao + ". Objetos embaixo do mouse (o 1º é o que está mais na frente):");

        if (resultados.Count == 0)
            texto.AppendLine("   (nenhum objeto da interface foi encontrado nesse ponto)");

        for (int i = 0; i < resultados.Count && i < 8; i++)
        {
            texto.AppendLine("   " + (i + 1) + ") " + resultados[i].gameObject.name);
        }

        Debug.Log(texto.ToString());
    }
}