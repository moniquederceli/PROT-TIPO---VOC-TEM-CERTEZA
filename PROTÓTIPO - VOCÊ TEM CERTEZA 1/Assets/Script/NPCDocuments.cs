using UnityEngine;
using UnityEngine.UI;

public class NPCDocuments : MonoBehaviour
{
    [Header("Documentos na mesa")]
    public Image documentoRG;
    public Image documentoLM;
    public Image documentoRM;

    // ==========================================
    // NPC 1
    // ==========================================

    [Header("NPC 1")]
    public Sprite rg1;
    public Sprite lm1;
    public Sprite rm1;

    // ==========================================
    // NPC 1.5
    // ==========================================

    [Header("NPC 1.5")]
    public Sprite rg1_5;
    public Sprite lm1_5;
    public Sprite rm1_5;

    // ==========================================
    // NPC 2
    // ==========================================

    [Header("NPC 2")]
    public Sprite rg2;
    public Sprite lm2;
    public Sprite rm2;

    // ==========================================
    // NPC 2.5
    // ==========================================

    [Header("NPC 2.5")]
    public Sprite rg2_5;
    public Sprite lm2_5;
    public Sprite rm2_5;

    // ==========================================
    // NPC 3
    // ==========================================

    [Header("NPC 3")]
    public Sprite rg3;
    public Sprite lm3;
    public Sprite rm3;

    // ==========================================
    // NPC 3.5
    // ==========================================

    [Header("NPC 3.5")]
    public Sprite rg3_5;
    public Sprite lm3_5;
    public Sprite rm3_5;


    // ==========================================
    // DOCUMENTOS GRANDES
    // ==========================================

    [Header("RG Grande")]
    public Sprite rgGrande1;
    public Sprite rgGrande1_5;
    public Sprite rgGrande2;
    public Sprite rgGrande2_5;
    public Sprite rgGrande3;
    public Sprite rgGrande3_5;


    [Header("LM Grande")]
    public Sprite lmGrande1;
    public Sprite lmGrande1_5;
    public Sprite lmGrande2;
    public Sprite lmGrande2_5;
    public Sprite lmGrande3;
    public Sprite lmGrande3_5;


    [Header("RM Grande")]
    public Sprite rmGrande1;
    public Sprite rmGrande1_5;
    public Sprite rmGrande2;
    public Sprite rmGrande2_5;
    public Sprite rmGrande3;
    public Sprite rmGrande3_5;


    // ==========================================
    // DOCUMENTOS DA MESA
    // ==========================================

    public void MostrarDocumentos(int numeroNPC)
    {
        Debug.Log("Mostrando documentos do NPC: " + numeroNPC);

        if (numeroNPC == 1)
        {
            documentoRG.sprite = rg1;
            documentoLM.sprite = lm1;
            documentoRM.sprite = rm1;
        }

        else if (numeroNPC == 15)
        {
            documentoRG.sprite = rg1_5;
            documentoLM.sprite = lm1_5;
            documentoRM.sprite = rm1_5;
        }

        else if (numeroNPC == 2)
        {
            documentoRG.sprite = rg2;
            documentoLM.sprite = lm2;
            documentoRM.sprite = rm2;
        }

        else if (numeroNPC == 25)
        {
            documentoRG.sprite = rg2_5;
            documentoLM.sprite = lm2_5;
            documentoRM.sprite = rm2_5;
        }

        else if (numeroNPC == 3)
        {
            documentoRG.sprite = rg3;
            documentoLM.sprite = lm3;
            documentoRM.sprite = rm3;
        }

        else if (numeroNPC == 35)
        {
            documentoRG.sprite = rg3_5;
            documentoLM.sprite = lm3_5;
            documentoRM.sprite = rm3_5;
        }
    }


    // ==========================================
    // PEGAR DOCUMENTO GRANDE
    // ==========================================

    public Sprite GetRGGrande(int numeroNPC)
    {
        if (numeroNPC == 1)
            return rgGrande1;

        if (numeroNPC == 15)
            return rgGrande1_5;

        if (numeroNPC == 2)
            return rgGrande2;

        if (numeroNPC == 25)
            return rgGrande2_5;

        if (numeroNPC == 3)
            return rgGrande3;

        if (numeroNPC == 35)
            return rgGrande3_5;

        return null;
    }


    public Sprite GetLMGrande(int numeroNPC)
    {
        if (numeroNPC == 1)
            return lmGrande1;

        if (numeroNPC == 15)
            return lmGrande1_5;

        if (numeroNPC == 2)
            return lmGrande2;

        if (numeroNPC == 25)
            return lmGrande2_5;

        if (numeroNPC == 3)
            return lmGrande3;

        if (numeroNPC == 35)
            return lmGrande3_5;

        return null;
    }


    public Sprite GetRMGrande(int numeroNPC)
    {
        if (numeroNPC == 1)
            return rmGrande1;

        if (numeroNPC == 15)
            return rmGrande1_5;

        if (numeroNPC == 2)
            return rmGrande2;

        if (numeroNPC == 25)
            return rmGrande2_5;

        if (numeroNPC == 3)
            return rmGrande3;

        if (numeroNPC == 35)
            return rmGrande3_5;

        return null;
    }
}