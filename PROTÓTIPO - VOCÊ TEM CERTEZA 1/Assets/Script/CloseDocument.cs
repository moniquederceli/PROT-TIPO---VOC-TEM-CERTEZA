using UnityEngine;
using UnityEngine.EventSystems;

public class CloseDocument : MonoBehaviour, IPointerClickHandler
{
    public GameObject inspectionPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (inspectionPanel != null)
        {
            inspectionPanel.SetActive(false);
        }
    }
}