using UnityEngine;
using UnityEngine.UI;

public class NPCDocuments : MonoBehaviour
{
    [Header("Documentos na mesa")]
    public Image documentoRG;
    public Image documentoLM;
    public Image documentoRM;

    [Header("NPC 1")]
    public Sprite rg1;
    public Sprite lm1;
    public Sprite rm1;

    [Header("NPC 2")]
    public Sprite rg2;
    public Sprite lm2;
    public Sprite rm2;

    [Header("NPC 3")]
    public Sprite rg3;
    public Sprite lm3;
    public Sprite rm3;

    public void MostrarDocumentos(int numeroNPC)
    {
        Debug.Log("Mostrando documentos do NPC: " + numeroNPC);

        if (numeroNPC == 1)
        {
            documentoRG.sprite = rg1;
            documentoLM.sprite = lm1;
            documentoRM.sprite = rm1;
        }

        if (numeroNPC == 2)
        {
            documentoRG.sprite = rg2;
            documentoLM.sprite = lm2;
            documentoRM.sprite = rm2;

            Debug.Log("NPC 2 - RG: " + documentoRG.sprite.name);
            Debug.Log("NPC 2 - LM: " + documentoLM.sprite.name);
            Debug.Log("NPC 2 - RM: " + documentoRM.sprite.name);
        }

        if (numeroNPC == 3)
        {
            documentoRG.sprite = rg3;
            documentoLM.sprite = lm3;
            documentoRM.sprite = rm3;
        }
    }
}