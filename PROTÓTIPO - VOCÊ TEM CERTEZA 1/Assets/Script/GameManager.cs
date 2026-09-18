using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("NPCs")]
    public GameObject[] npcObjects;

    public int currentNPC = 0;

    [Header("Decision Buttons")]
    public Button acceptButton;
    public Button denyButton;

    [Header("Documents")]
    public GameObject documentIdentity;

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

            NextNPC();
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
     if (documentIdentity != null)
    {
        documentIdentity.SetActive(true);
     }
    }

    public void HideDocumentIdentity()
    {
     if (documentIdentity != null)
     {
        documentIdentity.SetActive(false);
     }
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
