using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleClickClose : MonoBehaviour, IPointerClickHandler
{
    public GameObject documentToClose;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            documentToClose.SetActive(false);
        }
    }
}