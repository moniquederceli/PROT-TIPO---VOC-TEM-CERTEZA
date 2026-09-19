using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DocumentInteraction : MonoBehaviour, IPointerClickHandler
{
    [Header("RG - Documento Grande Especial")]
    public GameObject documentoGrande;

    [Header("Sistema antigo - LM e RM")]
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
        // SE tiver um Documento Grande configurado,
        // abre ele.
        // Isso será usado pelo RG.
        if (documentoGrande != null)
        {
            documentoGrande.SetActive(true);
            return;
        }

        // Se não tiver Documento Grande,
        // usa o sistema antigo.
        // LM e RM continuam funcionando assim.
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
        if (documentoGrande != null)
        {
            documentoGrande.SetActive(false);
        }

        if (inspectionPanel != null)
        {
            inspectionPanel.SetActive(false);
        }
    }
}