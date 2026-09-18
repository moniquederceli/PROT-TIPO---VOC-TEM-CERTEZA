using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DocumentInteraction : MonoBehaviour, IPointerClickHandler
{
    [Header("Document Inspection")]
    public GameObject inspectionPanel;
    public Image bigDocumentImage;

    private Image documentImage;

    private void Awake()
    {
        documentImage = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OpenDocument();
    }

    public void OpenDocument()
    {
        if (inspectionPanel == null)
        {
            Debug.LogError("Inspection Panel não foi configurado.");
            return;
        }

        if (bigDocumentImage == null)
        {
            Debug.LogError("Big Document Image não foi configurado.");
            return;
        }

        bigDocumentImage.sprite = documentImage.sprite;

        inspectionPanel.SetActive(true);
    }

    public void CloseDocument()
    {
        if (inspectionPanel != null)
        {
            inspectionPanel.SetActive(false);
        }
    }
}