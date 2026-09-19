using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleClickClose : MonoBehaviour, IPointerClickHandler
{
    [Header("Documento que será fechado")]
    public GameObject documentToClose;

    private void Awake()
    {
        // Se nenhum objeto for configurado,
        // fecha o próprio objeto.
        if (documentToClose == null)
        {
            documentToClose = gameObject;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount >= 2)
        {
            documentToClose.SetActive(false);
        }
    }
}