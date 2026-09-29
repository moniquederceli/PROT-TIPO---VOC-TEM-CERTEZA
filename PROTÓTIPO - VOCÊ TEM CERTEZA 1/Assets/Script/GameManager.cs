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
    [System.Serializable]
    public class NPCEntry
    {
        [Tooltip("Número do NPC: use 1, 2, 3, 4, 5, 6, 7 ou 8 (um número diferente para cada um dos 8 NPCs)")]
        public int npcId;

        [Header("As 3 variações deste NPC (arraste os GameObjects aqui)")]
        public GameObject npcNormal;
        public GameObject npcDocumentoErrado;
        public GameObject npcAparenciaAnomala;
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
    private bool ultimoDia = false;

    public int currentNPC = 0;

    [Header("Resultado Financeiro do Dia")]
    private float vidasSalvas = 0f;
    private float mortesDecaidos = 0f;
    private float bonus = 0f;

    [Header("Tela de Final de Dia")]
    public GameObject finalDeDia;
    public Button botaoConcluirDia;
    public TMP_Text textoBotaoConcluirDia;

    [Header("Tela Preta de Transição (DIA X)")]
    public GameObject telaDia;
    public TMP_Text textoTelaDia;

    [Header("Tela Inicial (usada ao Finalizar Jogo)")]
    public GameObject telaInicial;
    public GameObject fundoInicial;

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

            switch (variacaoEscolhida)
            {
                case TipoVariacao.Normal:
                    npcEscolhido = entrada.npcNormal;
                    break;
                case TipoVariacao.DocumentoErrado:
                    npcEscolhido = entrada.npcDocumentoErrado;
                    break;
                case TipoVariacao.AparenciaAnomala:
                    npcEscolhido = entrada.npcAparenciaAnomala;
                    break;
            }

            // ==========================================
            // CÓDIGO DO DOCUMENTO
            // Fórmula: (Id do NPC x 10) + 0 [correto] ou +1 [incorreto]
            // Normal e Aparência Anômala usam o MESMO documento correto,
            // só a variação Documento Errado usa o documento incorreto.
            // ==========================================
            int codigoDocumento = entrada.npcId * 10;

            if (variacaoEscolhida == TipoVariacao.DocumentoErrado)
                codigoDocumento += 1;

            filaNPCs[slot] = npcEscolhido;
            filaDocumentos[slot] = codigoDocumento;
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
                npcDocuments.MostrarDocumentos();

            NPCMovement movement = npc.GetComponent<NPCMovement>();
            if (movement != null)
                movement.StartNPC();
        }
    }

    // ==========================================
    // FINAL DO DIA
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

        // Verifica se ainda sobram NPCs novos para um próximo dia
        int restantes = npcs.Length - npcsJaUsados.Count;
        ultimoDia = restantes <= 0;

        if (textoBotaoConcluirDia != null)
        {
            textoBotaoConcluirDia.text = ultimoDia ? "Finalizar Jogo" : "Concluir Dia";
        }

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

    // Este é o único método ligado ao botão da tela de final de dia.
    // Ele decide sozinho se deve ir para o próximo dia ou finalizar o jogo.
    public void ConcluirDia()
    {
        if (finalDeDia != null)
            finalDeDia.SetActive(false);

        if (ultimoDia)
        {
            ReiniciarJogo();
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
    // FIM DE JOGO -> VOLTA PARA O MENU INICIAL
    // ==========================================
    private void ReiniciarJogo()
    {
        diaAtual = 1;
        npcsJaUsados.Clear();
        vidasSalvas = 0f;
        mortesDecaidos = 0f;
        bonus = 0f;
        currentNPC = 0;
        filaNPCs = null;
        ultimoDia = false;

        if (finalDeDia != null) finalDeDia.SetActive(false);
        if (telaDia != null) telaDia.SetActive(false);

        if (telaInicial != null) telaInicial.SetActive(true);
        if (fundoInicial != null) fundoInicial.SetActive(true);
    }

    // ==========================================
    // DOCUMENTOS E BOTÕES (sem alterações)
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