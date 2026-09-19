using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("NPCs")]
    public GameObject[] npcObjects;

    public int currentNPC = 0;

    [Header("Documentos dos NPCs")]
    public NPCDocuments npcDocuments;

    [Header("Decision Buttons")]
    public Button acceptButton;
    public Button denyButton;

    [Header("Documents")]
    public GameObject documentoRG;
    public GameObject documentoLM;
    public GameObject documentoRM;

    private void Start()
    {
        SetButtons(false);
        ShowCurrentNPC();
    }

    public void AcceptCurrentNPC()
    {
        if (currentNPC >= npcObjects.Length)
            return;

        NPCMovement movement = npcObjects[currentNPC].GetComponent<NPCMovement>();

        if (movement != null)
        {
            HideDocumentIdentity();

            SetButtons(false);

            movement.Accept();

            Invoke(nameof(NextNPC), 2f);
        }
    }

    public void DenyCurrentNPC()
   {
    if (currentNPC >= npcObjects.Length)
        return;

    NPCMovement movement = npcObjects[currentNPC].GetComponent<NPCMovement>();

    if (movement != null)
    {
        HideDocumentIdentity();

        SetButtons(false);

        movement.Deny();

        Invoke(nameof(NextNPC), 2f);
    }
    }

    public void NextNPC()
    {
        currentNPC++;

        ShowCurrentNPC();
    }

    private void ShowCurrentNPC()
    {
        // Desativa todos os NPCs
        for (int i = 0; i < npcObjects.Length; i++)
        {
            npcObjects[i].SetActive(false);
        }

        // Verifica se ainda existem NPCs
        if (currentNPC < npcObjects.Length)
        {
            GameObject npc = npcObjects[currentNPC];

            npc.SetActive(true);

            if (npcDocuments != null)
            {
                npcDocuments.MostrarDocumentos(currentNPC + 1);
            }

            NPCMovement movement = npc.GetComponent<NPCMovement>();

            if (movement != null)
            {
                movement.StartNPC();
            }
        }
        else
        {
            Debug.Log("Todos os NPCs foram atendidos!");
        }
    }

    public void ShowDocumentIdentity()
    {
        if (documentoRG != null)
        documentoRG.SetActive(true);

        if (documentoLM != null)
        documentoLM.SetActive(true);

        if (documentoRM != null)
        documentoRM.SetActive(true);
    }

    public void HideDocumentIdentity()
    {
         if (documentoRG != null)
        documentoRG.SetActive(false);

        if (documentoLM != null)
        documentoLM.SetActive(false);

        if (documentoRM != null)
        documentoRM.SetActive(false);
    }
    public void EnableDecisionButtons()
    {
    SetButtons(true);
    }
    private void SetButtons(bool enabled)
    {
    if (acceptButton != null)
    {
        acceptButton.interactable = enabled;
    }

    if (denyButton != null)
    {
        denyButton.interactable = enabled;
    }
    }
}
