using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DocumentInteraction : MonoBehaviour, IPointerClickHandler
{
    [Header("RG - Documento Grande Especial")]
    public GameObject documentoGrande;
    public Image imagemDocumentoGrande;

    public Sprite rgGrande1;
    public Sprite rgGrande2;
    public Sprite rgGrande3;

    public Sprite lmGrande1;
    public Sprite lmGrande2;
    public Sprite lmGrande3;

    public Sprite rmGrande1;
    public Sprite rmGrande2;
    public Sprite rmGrande3;

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
        if (documentoGrande != null)
{
    GameManager gameManager = FindAnyObjectByType<GameManager>();

    if (gameManager != null && imagemDocumentoGrande != null)
    {
        if (gameObject.name == "RG")
        {
            if (gameManager.currentNPC == 0)
                imagemDocumentoGrande.sprite = rgGrande1;
            else if (gameManager.currentNPC == 1)
                imagemDocumentoGrande.sprite = rgGrande2;
            else if (gameManager.currentNPC == 2)
                imagemDocumentoGrande.sprite = rgGrande3;
        }
        else if (gameObject.name == "LM")
        {
            if (gameManager.currentNPC == 0)
                imagemDocumentoGrande.sprite = lmGrande1;
            else if (gameManager.currentNPC == 1)
                imagemDocumentoGrande.sprite = lmGrande2;
            else if (gameManager.currentNPC == 2)
                imagemDocumentoGrande.sprite = lmGrande3;
        }
        else if (gameObject.name == "RM")
        {
            if (gameManager.currentNPC == 0)
                imagemDocumentoGrande.sprite = rmGrande1;
            else if (gameManager.currentNPC == 1)
                imagemDocumentoGrande.sprite = rmGrande2;
            else if (gameManager.currentNPC == 2)
                imagemDocumentoGrande.sprite = rmGrande3;
        }
    }

    documentoGrande.SetActive(true);
    return;
}    
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