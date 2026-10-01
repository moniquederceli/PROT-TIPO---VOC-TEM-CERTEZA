using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DocumentInteraction : MonoBehaviour, IPointerClickHandler
{
    [Header("Documento Grande")]
    public GameObject documentoGrande;

    public Image imagemDocumentoGrande;

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
        // ==========================================
        // DOCUMENTO GRANDE
        // ==========================================

        if (documentoGrande != null)
        {
            GameManager gameManager =
                FindAnyObjectByType<GameManager>();

            if (gameManager != null &&
                gameManager.npcDocuments != null &&
                imagemDocumentoGrande != null)
            {
                // Pega QUAL NPC realmente está sendo atendido
                int documentoAtual =
                    gameManager.GetDocumentoAtual();

                Sprite documentoGrande = null;


                // ==========================================
                // RG
                // ==========================================

                if (gameObject.name == "RG")
                {
                    documentoGrande =
                        gameManager.npcDocuments.GetRGGrande(
                            documentoAtual
                        );
                }


                // ==========================================
                // LM
                // ==========================================

                else if (gameObject.name == "LM")
                {
                    documentoGrande =
                        gameManager.npcDocuments.GetLMGrande(
                            documentoAtual
                        );
                }


                // ==========================================
                // RM
                // ==========================================

                else if (gameObject.name == "RM")
                {
                    documentoGrande =
                        gameManager.npcDocuments.GetRMGrande(
                            documentoAtual
                        );
                }


                // ==========================================
                // COLOCA A IMAGEM
                // ==========================================

                if (documentoGrande != null)
                {
                    imagemDocumentoGrande.sprite =
                        documentoGrande;
                }
                else
                {
                    Debug.LogWarning(
                        "Documento grande não configurado para: "
                        + documentoAtual
                    );
                }
            }


            // Abre o documento grande
            documentoGrande.SetActive(true);

            return;
        }


        // ==========================================
        // SISTEMA ANTIGO
        // ==========================================

        if (inspectionPanel == null)
        {
            Debug.LogError(
                "Inspection Panel não foi configurado."
            );

            return;
        }


        if (bigDocumentImage == null)
        {
            Debug.LogError(
                "Big Document Image não foi configurado."
            );

            return;
        }


        bigDocumentImage.sprite =
            documentImage.sprite;

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