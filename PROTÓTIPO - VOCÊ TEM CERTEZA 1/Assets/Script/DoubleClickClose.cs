using UnityEngine;
using UnityEngine.EventSystems;

// O nome do arquivo continua "DoubleClickClose" para não perder as
// configurações que você já tinha feito no Inspector, mas agora ele
// fecha com o BOTÃO DIREITO do mouse em vez de clique duplo.
public class DoubleClickClose : MonoBehaviour, IPointerClickHandler
{
    [Header("Documento que será fechado")]
    public GameObject documentToClose;

    private void Awake()
    {
        // Se nenhum objeto for configurado, fecha o próprio objeto.
        if (documentToClose == null)
        {
            documentToClose = gameObject;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Só fecha se o clique foi com o BOTÃO DIREITO do mouse
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            documentToClose.SetActive(false);
        }
    }
}