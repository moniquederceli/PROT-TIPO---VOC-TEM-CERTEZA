using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Os 3 "tipos" que um NPC pode ser quando aparece na cabine
    private enum TipoVariacao
    {
        Normal,
        DocumentoErrado,
        AparenciaAnomala
    }

    // Isso representa UM dos 8 NPCs, com suas 3 variações.
    // Vai aparecer no Inspector como uma "caixinha" que você preenche.
    [System.Serializable]
    public class NPCEntry
    {
        [Tooltip("Número do NPC: use 1, 2, 3, 4, 5, 6, 7 ou 8 (um número diferente para cada um dos 8 NPCs)")]
        public int npcId;

        [Header("As 3 variações deste NPC (arraste os GameObjects aqui)")]
        public GameObject npcNormal;
        public GameObject npcDocumentoErrado;
        public GameObject npcAparenciaAnomala;

        [Header("Códigos de documento (ligados ao NPCDocuments) - pode deixar 0 por enquanto")]
        public int documentoNormal;
        public int documentoErrado;
        public int documentoAnomalo;
    }

    [Header("=== Os 8 NPCs do jogo ===")]
    public NPCEntry[] npcs = new NPCEntry[8];

    [Header("=== Configuração dos Dias ===")]
    public int npcsPorDia = 4;
    public int diaAtual = 1;
    public float tempoTelaDia = 2f;

    private List<int> npcsJaUsados = new List<int>();
    private GameObject[] filaNPCs;
    private int[] filaDocumentos;
    private TipoVariacao[] filaVariacoes;

    public int currentNPC = 0;

    [Header("Resultado Financeiro do Dia")]
    private float vidasSalvas = 0f;
    private float mortesDecaidos = 0f;
    private float bonus = 0f;

    [Header("Telas")]
    public GameObject finalDeDia;
    public GameObject telaDia;
    public TMP_Text textoTelaDia;
    public GameObject fimDeJogo;

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

        if (finalDeDia != null) finalDeDia.SetActive(false);
        if (telaDia != null) telaDia.SetActive(false);
        if (fimDeJogo != null) fimDeJogo.SetActive(false);
    }

    public int GetDocumentoAtual()
    {
        if (filaDocumentos == null || currentNPC >= filaDocumentos.Length)
            return 0;

        return filaDocumentos[currentNPC];
    }

    public void IniciarJogo()
    {
        diaAtual = 1;
        npcsJaUsados.Clear();

        vidasSalvas = 0f;
        mortesDecaidos = 0f;
        bonus = 0f;

        SortearNPCsDoDia();

        currentNPC = 0;
        ShowCurrentNPC();
    }

    // ==========================================
    // SORTEIO DO DIA, SEM REPETIR NPCS DE DIAS ANTERIORES
    // ==========================================
    private void SortearNPCsDoDia()
    {
        // Monta a lista de IDs que ainda não apareceram em nenhum dia
        List<int> disponiveis = new List<int>();

        foreach (NPCEntry entrada in npcs)
        {
            if (entrada != null && !npcsJaUsados.Contains(entrada.npcId))
                disponiveis.Add(entrada.npcId);
        }

        int quantidade = Mathf.Min(npcsPorDia, disponiveis.Count);

        filaNPCs = new GameObject[quantidade];
        filaDocumentos = new int[quantidade];
        filaVariacoes = new TipoVariacao[quantidade];

        for (int slot = 0; slot < quantidade; slot++)
        {
            // Escolhe um ID aleatório entre os que sobraram
            int indiceEscolhido = Random.Range(0, disponiveis.Count);
            int idEscolhido = disponiveis[indiceEscolhido];
            disponiveis.RemoveAt(indiceEscolhido);

            NPCEntry entrada = BuscarEntradaPorId(idEscolhido);
            if (entrada == null) continue;

            TipoVariacao variacaoEscolhida;

            if (slot == 0)
            {
                // REGRA OBRIGATÓRIA: o primeiro NPC do dia é sempre normal
                variacaoEscolhida = TipoVariacao.Normal;
            }
            else
            {
                int sorteio = Random.Range(0, 3);
                variacaoEscolhida = (TipoVariacao)sorteio;
            }

            GameObject npcEscolhido = null;
            int documentoEscolhido = 0;

            switch (variacaoEscolhida)
            {
                case TipoVariacao.Normal:
                    npcEscolhido = entrada.npcNormal;
                    documentoEscolhido = entrada.documentoNormal;
                    break;
                case TipoVariacao.DocumentoErrado:
                    npcEscolhido = entrada.npcDocumentoErrado;
                    documentoEscolhido = entrada.documentoErrado;
                    break;
                case TipoVariacao.AparenciaAnomala:
                    npcEscolhido = entrada.npcAparenciaAnomala;
                    documentoEscolhido = entrada.documentoAnomalo;
                    break;
            }

            filaNPCs[slot] = npcEscolhido;
            filaDocumentos[slot] = documentoEscolhido;
            filaVariacoes[slot] = variacaoEscolhida;

            npcsJaUsados.Add(idEscolhido);
        }
    }

    private NPCEntry BuscarEntradaPorId(int id)
    {
        foreach (NPCEntry entrada in npcs)
        {
            if (entrada != null && entrada.npcId == id)
                return entrada;
        }

        Debug.LogError("GameManager: não encontrei nenhum NPC com o Npc Id " + id + ". Confira o array 'Npcs' no Inspector.");
        return null;
    }

    // ==========================================
    // ACEITAR / RECUSAR
    // ==========================================
    public void AcceptCurrentNPC()
    {
        if (filaNPCs == null || currentNPC >= filaNPCs.Length || filaNPCs[currentNPC] == null)
            return;

        NPCMovement movement = filaNPCs[currentNPC].GetComponent<NPCMovement>();

        if (movement != null)
        {
            HideDocumentIdentity();
            SetButtons(false);

            if (filaVariacoes[currentNPC] == TipoVariacao.Normal)
            {
                vidasSalvas += 20f;
            }

            movement.Accept();

            Invoke(nameof(NextNPC), 2f);
        }
    }

    public void DenyCurrentNPC()
    {
        if (filaNPCs == null || currentNPC >= filaNPCs.Length || filaNPCs[currentNPC] == null)
            return;

        NPCMovement movement = filaNPCs[currentNPC].GetComponent<NPCMovement>();

        if (movement != null)
        {
            HideDocumentIdentity();
            SetButtons(false);

            if (filaVariacoes[currentNPC] == TipoVariacao.DocumentoErrado ||
                filaVariacoes[currentNPC] == TipoVariacao.AparenciaAnomala)
            {
                bonus += 10f;
            }

            if (filaVariacoes[currentNPC] == TipoVariacao.AparenciaAnomala)
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

        if (currentNPC >= filaNPCs.Length)
        {
            MostrarFinalDeDia();
            return;
        }

        ShowCurrentNPC();
    }

    private void ShowCurrentNPC()
    {
        foreach (GameObject npc in filaNPCs)
        {
            if (npc != null)
                npc.SetActive(false);
        }

        if (currentNPC < filaNPCs.Length && filaNPCs[currentNPC] != null)
        {
            GameObject npc = filaNPCs[currentNPC];
            npc.SetActive(true);

            if (npcDocuments != null)
                npcDocuments.MostrarDocumentos(filaDocumentos[currentNPC]);

            NPCMovement movement = npc.GetComponent<NPCMovement>();
            if (movement != null)
                movement.StartNPC();
        }
    }

    // ==========================================
    // FINAL DO DIA / TRANSIÇÃO "DIA X"
    // ==========================================
    private void MostrarFinalDeDia()
    {
        foreach (GameObject npc in filaNPCs)
        {
            if (npc != null)
                npc.SetActive(false);
        }

        HideDocumentIdentity();
        SetButtons(false);

        if (finalDeDia != null)
        {
            finalDeDia.SetActive(true);

            CalculadoraFinalDia calculadora = finalDeDia.GetComponent<CalculadoraFinalDia>();
            if (calculadora != null)
            {
                calculadora.ConfigurarDia(vidasSalvas, mortesDecaidos, bonus);
            }
        }
    }

    // Chame esta função no botão "Concluir Dia" da tela de final de dia
    public void ConcluirDia()
    {
        if (finalDeDia != null)
            finalDeDia.SetActive(false);

        bool aindaTemNPCsNovos = (npcs.Length - npcsJaUsados.Count) >= 1;

        if (!aindaTemNPCsNovos)
        {
            if (fimDeJogo != null)
                fimDeJogo.SetActive(true);

            return;
        }

        diaAtual++;

        if (telaDia != null)
        {
            telaDia.SetActive(true);

            if (textoTelaDia != null)
                textoTelaDia.text = "DIA " + diaAtual;
        }

        Invoke(nameof(ComecarProximoDia), tempoTelaDia);
    }

    private void ComecarProximoDia()
    {
        if (telaDia != null)
            telaDia.SetActive(false);

        SortearNPCsDoDia();

        currentNPC = 0;
        ShowCurrentNPC();
    }

    // ==========================================
    // DOCUMENTOS E BOTÕES (igual antes)
    // ==========================================
    public void ShowDocumentIdentity()
    {
        if (documentoRG != null) documentoRG.SetActive(true);
        if (documentoLM != null) documentoLM.SetActive(true);
        if (documentoRM != null) documentoRM.SetActive(true);
    }

    public void HideDocumentIdentity()
    {
        if (documentoRG != null) documentoRG.SetActive(false);
        if (documentoLM != null) documentoLM.SetActive(false);
        if (documentoRM != null) documentoRM.SetActive(false);
    }

    public void EnableDecisionButtons()
    {
        SetButtons(true);
    }

    private void SetButtons(bool enabled)
    {
        if (acceptButton != null) acceptButton.interactable = enabled;
        if (denyButton != null) denyButton.interactable = enabled;
    }
}
