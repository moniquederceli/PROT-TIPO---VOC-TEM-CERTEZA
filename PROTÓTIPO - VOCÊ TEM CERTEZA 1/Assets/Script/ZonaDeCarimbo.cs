using UnityEngine;
using UnityEngine.EventSystems;

// Coloque este script numa Image invisível que cobre SÓ o espaço do carimbo
// dentro do Registro de Entrada. É a única área do jogo que aceita o carimbo.
public class ZonaDeCarimbo : MonoBehaviour, IDropHandler
{
    [Header("Marca do carimbo APROVADO (verde). Começa desligada.")]
    public GameObject marcaAprovado;

    [Header("Marca do carimbo RECUSADO (vermelho). Começa desligada.")]
    public GameObject marcaRecusado;

    // O Unity chama este método sozinho quando algo é SOLTO em cima desta zona
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        // Só aceita se o que foi solto for um carimbo
        CarimboNaMao carimbo = eventData.pointerDrag.GetComponent<CarimboNaMao>();

        if (carimbo == null)
            return;

        Carimbar(carimbo.tipo);
    }

    // Mostra a marca do carimbo usado e apaga a outra (um carimbo substitui o outro)
    public void Carimbar(TipoCarimbo tipo)
    {
        bool aprovado = (tipo == TipoCarimbo.Aprovado);

        if (marcaAprovado != null)
            marcaAprovado.SetActive(aprovado);

        if (marcaRecusado != null)
            marcaRecusado.SetActive(!aprovado);
    }

    // Apaga qualquer carimbo do documento
    public void LimparMarcas()
    {
        if (marcaAprovado != null)
            marcaAprovado.SetActive(false);

        if (marcaRecusado != null)
            marcaRecusado.SetActive(false);
    }
}