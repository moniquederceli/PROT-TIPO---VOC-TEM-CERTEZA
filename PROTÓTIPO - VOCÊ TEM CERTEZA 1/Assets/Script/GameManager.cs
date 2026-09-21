using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("NPCs")]
    public GameObject[] npcObjects;

    [Header("NPCs do Sorteio")]
    public GameObject npc1;
    public GameObject npc1_5;

    public GameObject npc2;
    public GameObject npc2_5;

    public GameObject npc3;
    public GameObject npc3_5;

    private GameObject[] filaNPCs = new GameObject[3];
    private int[] filaDocumentos = new int[3];

    public int currentNPC = 0;

    [Header("Resultado Financeiro do Dia")]
    private float vidasSalvas = 0f;
    private float mortesDecaidos = 0f;
    private float bonus = 0f;

    [Header("Final de Dia")]
    public GameObject finalDeDia;

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

        if (finalDeDia != null)
        {
            finalDeDia.SetActive(false);
        }
    }


    public void IniciarJogo()
    {
        currentNPC = 0;

        vidasSalvas = 0f;
        mortesDecaidos = 0f;
        bonus = 0f;

        SortearNPCs();

        ShowCurrentNPC();
    }


    public void AcceptCurrentNPC()
    {
        if (currentNPC >= filaNPCs.Length)
            return;

        NPCMovement movement = filaNPCs[currentNPC].GetComponent<NPCMovement>();

        if (movement != null)
        {
            HideDocumentIdentity();

            SetButtons(false);

            if (filaNPCs[currentNPC] == npc1 ||
                filaNPCs[currentNPC] == npc2 ||
                filaNPCs[currentNPC] == npc3)
            {
                vidasSalvas += 20f;
            }

            movement.Accept();

            Invoke(nameof(NextNPC), 2f);
        }
    }


    public void DenyCurrentNPC()
    {
        if (currentNPC >= filaNPCs.Length)
            return;

        NPCMovement movement = filaNPCs[currentNPC].GetComponent<NPCMovement>();

        if (movement != null)
        {
            HideDocumentIdentity();

            SetButtons(false);

            if (filaNPCs[currentNPC] == npc1_5 ||
                filaNPCs[currentNPC] == npc2_5 ||
                filaNPCs[currentNPC] == npc3_5)
            {
                bonus += 10f;
            }

            if (filaNPCs[currentNPC] == npc3_5)
            {
                mortesDecaidos += 1f;
            }

            movement.Deny();

            Invoke(nameof(NextNPC), 2f);
        }
    }


    public void NextNPC()
    {
        currentNPC++;

        // Terminou os 3 NPCs
        if (currentNPC >= filaNPCs.Length)
        {
            MostrarFinalDeDia();
            return;
        }

        ShowCurrentNPC();
    }


    private void ShowCurrentNPC()
    {
        // Desativa todos os NPCs sorteados
        for (int i = 0; i < filaNPCs.Length; i++)
        {
            if (filaNPCs[i] != null)
            {
                filaNPCs[i].SetActive(false);
            }
        }

        // Verifica se ainda existem NPCs
        if (currentNPC < filaNPCs.Length)
        {
            GameObject npc = filaNPCs[currentNPC];

            npc.SetActive(true);

            if (npcDocuments != null)
            {
                npcDocuments.MostrarDocumentos(filaDocumentos[currentNPC]);
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


    private void MostrarFinalDeDia()
    {
        Debug.Log("FINAL DE DIA: começou");

        // Esconde todos os NPCs
        for (int i = 0; i < filaNPCs.Length; i++)
        {
            if (filaNPCs[i] != null)
            {
                filaNPCs[i].SetActive(false);
            }
        }

        // Esconde os documentos
        HideDocumentIdentity();

        // Desativa os botões
        SetButtons(false);

        // Mostra a imagem de final de dia
        if (finalDeDia != null)
        {
        Debug.Log("FINAL DE DIA: mostrando imagem");

        finalDeDia.SetActive(true);

        CalculadoraFinalDia calculadora =
        finalDeDia.GetComponent<CalculadoraFinalDia>();

        if (calculadora != null)
        {
        calculadora.ConfigurarDia(
            vidasSalvas,
            mortesDecaidos,
            bonus
        );
        }
        }
        else
        {
        Debug.LogError("FINAL DE DIA: a imagem não foi colocada no campo Final De Dia do Inspector!");
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


    private void SortearNPCs()
    {
        // Primeiro escolhe qual NPC NORMAL será o primeiro
        int primeiro = Random.Range(0, 3);

        if (primeiro == 0)
        {
            filaNPCs[0] = npc1;
            filaDocumentos[0] = 1;

            filaNPCs[1] = Random.Range(0, 2) == 0 ? npc2 : npc2_5;
            filaDocumentos[1] = filaNPCs[1] == npc2 ? 2 : 25;

            filaNPCs[2] = Random.Range(0, 2) == 0 ? npc3 : npc3_5;
            filaDocumentos[2] = 3;
        }
        else if (primeiro == 1)
        {
            filaNPCs[0] = npc2;
            filaDocumentos[0] = 2;

            filaNPCs[1] = Random.Range(0, 2) == 0 ? npc1 : npc1_5;
            filaDocumentos[1] = filaNPCs[1] == npc1 ? 1 : 15;

            filaNPCs[2] = Random.Range(0, 2) == 0 ? npc3 : npc3_5;
            filaDocumentos[2] = 3;
        }
        else
        {
            filaNPCs[0] = npc3;
            filaDocumentos[0] = 3;

            filaNPCs[1] = Random.Range(0, 2) == 0 ? npc1 : npc1_5;
            filaDocumentos[1] = filaNPCs[1] == npc1 ? 1 : 15;

            filaNPCs[2] = Random.Range(0, 2) == 0 ? npc2 : npc2_5;
            filaDocumentos[2] = filaNPCs[2] == npc2 ? 2 : 25;
        }

        // Embaralha o segundo e o terceiro
        if (Random.Range(0, 2) == 0)
        {
            GameObject npcTemporario = filaNPCs[1];
            filaNPCs[1] = filaNPCs[2];
            filaNPCs[2] = npcTemporario;

            int documentoTemporario = filaDocumentos[1];
            filaDocumentos[1] = filaDocumentos[2];
            filaDocumentos[2] = documentoTemporario;
        }
    }
}