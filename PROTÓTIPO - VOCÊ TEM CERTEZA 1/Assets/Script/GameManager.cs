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

    [Header("Resultado Financeiro DO DIA (reseta a cada dia)")]
    private float vidasSalvas = 0f;
    private float mortesDecaidos = 0f;
    private float bonus = 0f;

    [Header("Contagem TOTAL do jogo (usada no Resumo Final)")]
    private int normalSalvos = 0;
    private int normalMortos = 0;
    private int docErradoSalvos = 0;
    private int docErradoMortos = 0;
    private int anomalicoSalvos = 0;
    private int anomalicoMortos = 0;

    [Header("Tela de Final de Dia")]
    public GameObject finalDeDia;
    public Button botaoConcluirDia;
    public TMP_Text textoBotaoConcluirDia;

    [Header("Tela Preta de Transição (DIA X)")]
    public GameObject telaDia;
    public TMP_Text textoTelaDia;

    [Header("Tela de Resumo Final (aparece só no fim do Dia 2)")]
    public GameObject telaResumoFinal;

    [Header("Aviso de Nova Página do Diário (aparece no início do Dia 2)")]
    public GameObject avisoNovaPagina;

    [Header("Tela Inicial (usada ao Voltar ao Menu)")]
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

    [Header("Documentos GRANDES (fecham sozinhos ao Aceitar/Recusar)")]
    public GameObject[] documentosGrandesAbertos;

    private void Start()
    {
        SetButtons(false);

        if (finalDeDia != null) finalDeDia.SetActive(false);
        if (telaDia != null) telaDia.SetActive(false);
        if (telaResumoFinal != null) telaResumoFinal.SetActive(false);
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

        ResetarValoresDoDia();
        ResetarContagemTotal();

        SortearNPCsDoDia();

        currentNPC = 0;
        ShowCurrentNPC();
    }

    private void ResetarValoresDoDia()
    {
        vidasSalvas = 0f;
        mortesDecaidos = 0f;
        bonus = 0f;
    }

    private void ResetarContagemTotal()
    {
        normalSalvos = 0;
        normalMortos = 0;
        docErradoSalvos = 0;
        docErradoMortos = 0;
        anomalicoSalvos = 0;
        anomalicoMortos = 0;
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

            // Código do documento: (Id do NPC x 10) + 0 [correto] ou +1 [incorreto]
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
    // ACEITAR (BOTÃO VERDE)
    // ==========================================
    public void AcceptCurrentNPC()
    {
        if (filaNPCs == null || currentNPC >= filaNPCs.Length || filaNPCs[currentNPC] == null)
            return;

        NPCMovement movement = filaNPCs[currentNPC].GetComponent<NPCMovement>();

        if (movement != null)
        {
            HideDocumentIdentity();
            FecharTodosDocumentosGrandes();
            SetButtons(false);

            switch (filaVariacoes[currentNPC])
            {
                case TipoVariacao.Normal:
                    // NPC Normal aceito: +20 de bônus (vida salva)
                    vidasSalvas += 20f;
                    normalSalvos++;
                    break;

                case TipoVariacao.DocumentoErrado:
                    // NPC com Documento Errado aceito: não acontece nada
                    docErradoSalvos++;
                    break;

                case TipoVariacao.AparenciaAnomala:
                    // NPC Anomálico aceito: -10 no bônus (deixou passar um decaído)
                    bonus -= 10f;
                    anomalicoSalvos++;
                    break;
            }

            movement.Accept();

            Invoke(nameof(NextNPC), 2f);
        }
    }

    // ==========================================
    // RECUSAR (BOTÃO VERMELHO)
    // ==========================================
    public void DenyCurrentNPC()
    {
        if (filaNPCs == null || currentNPC >= filaNPCs.Length || filaNPCs[currentNPC] == null)
            return;

        NPCMovement movement = filaNPCs[currentNPC].GetComponent<NPCMovement>();

        if (movement != null)
        {
            HideDocumentIdentity();
            FecharTodosDocumentosGrandes();
            SetButtons(false);

            switch (filaVariacoes[currentNPC])
            {
                case TipoVariacao.Normal:
                    // NPC Normal recusado: não acontece nada
                    break;

                case TipoVariacao.DocumentoErrado:
                    // Documento Errado recusado: +10 de bônus
                    bonus += 10f;
                    break;

                case TipoVariacao.AparenciaAnomala:
                    // Anomálico recusado: +10 de bônus
                    bonus += 10f;
                    break;
            }

            movement.Deny();

            Invoke(nameof(NextNPC), 2f);
        }
    }

    // Chamado 2 segundos depois de Aceitar ou Recusar
    public void NextNPC()
    {
        AvancarParaProximoNPC();
    }

    private void AvancarParaProximoNPC()
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
    // MANIVELA / PORTA (MATA O NPC ATUAL)
    // ==========================================

    // Chamado pela PortaAnimacao assim que o botão da mesa é apertado,
    // pra impedir o jogador de clicar em Aceitar/Recusar durante a animação.
    public void DesabilitarBotoesDecisao()
    {
        SetButtons(false);
    }

    // Chamado pela PortaAnimacao no instante em que a porta termina de fechar.
    public void EsconderNPCAtual()
    {
        Debug.Log("EsconderNPCAtual foi chamado!");

        if (filaNPCs == null)
        {
            Debug.LogWarning("EsconderNPCAtual: filaNPCs está vazia (null). O jogo foi iniciado corretamente?");
            return;
        }

        if (currentNPC >= filaNPCs.Length)
        {
            Debug.LogWarning("EsconderNPCAtual: currentNPC (" + currentNPC + ") está fora da fila (tamanho " + filaNPCs.Length + ").");
            return;
        }

        if (filaNPCs[currentNPC] == null)
        {
            Debug.LogWarning("EsconderNPCAtual: o NPC atual na fila está vazio (null). Confira se todos os campos do array 'Npcs' no GameManager estão preenchidos.");
            return;
        }

        Debug.Log("Escondendo o NPC: " + filaNPCs[currentNPC].name);

        RegistrarMortePorPorta(filaVariacoes[currentNPC]);

        filaNPCs[currentNPC].SetActive(false);

        HideDocumentIdentity();
        FecharTodosDocumentosGrandes();
        SetButtons(false);
    }

    private void RegistrarMortePorPorta(TipoVariacao variacao)
    {
        switch (variacao)
        {
            case TipoVariacao.Normal:
                // Morto por engano (era inocente)
                normalMortos++;
                break;

            case TipoVariacao.DocumentoErrado:
                // Morto por engano (era inocente)
                docErradoMortos++;
                break;

            case TipoVariacao.AparenciaAnomala:
                // Morte "correta" de um decaído
                anomalicoMortos++;
                mortesDecaidos += 1f;
                break;
        }
    }

    // Chamado pela PortaAnimacao assim que a porta termina de abrir de novo.
    public void ContinuarAposPorta()
    {
        AvancarParaProximoNPC();
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
            textoBotaoConcluirDia.text = ultimoDia ? "Finalizar Dia" : "Concluir Dia";
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

    // Único método ligado ao botão da tela de final de dia.
    public void ConcluirDia()
    {
        if (finalDeDia != null)
            finalDeDia.SetActive(false);

        if (ultimoDia)
        {
            MostrarResumoFinal();
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

        // Os valores financeiros do dia (vidas/bônus/mortes) recomeçam do zero.
        // O dinheiro acumulado (saldo total) NÃO é afetado por isso — ele fica
        // guardado dentro do CalculadoraFinalDia.
        ResetarValoresDoDia();

        SortearNPCsDoDia();

        currentNPC = 0;
        ShowCurrentNPC();

        // Mostra o aviso de nova página do diário só ao entrar no Dia 2
        if (diaAtual == 2 && avisoNovaPagina != null)
        {
            AvisoNovaPagina aviso = avisoNovaPagina.GetComponent<AvisoNovaPagina>();
            if (aviso != null)
                aviso.Mostrar();
        }
    }

    // ==========================================
    // RESUMO FINAL DO JOGO (depois do Dia 2)
    // ==========================================
    private void MostrarResumoFinal()
    {
        if (telaResumoFinal != null)
        {
            telaResumoFinal.SetActive(true);

            ResumoFinalJogo resumo = telaResumoFinal.GetComponent<ResumoFinalJogo>();
            if (resumo != null)
            {
                resumo.ConfigurarResumo(
                    normalSalvos, normalMortos,
                    docErradoSalvos, docErradoMortos,
                    anomalicoSalvos, anomalicoMortos
                );
            }
        }
    }

    // Ligue este método ao botão "Voltar ao Menu" da tela de Resumo Final.
    public void VoltarAoMenuPrincipal()
    {
        if (telaResumoFinal != null)
            telaResumoFinal.SetActive(false);

        ReiniciarJogo();
    }

    private void ReiniciarJogo()
    {
        diaAtual = 1;
        npcsJaUsados.Clear();

        ResetarValoresDoDia();
        ResetarContagemTotal();

        currentNPC = 0;
        filaNPCs = null;
        ultimoDia = false;

        if (finalDeDia != null) finalDeDia.SetActive(false);
        if (telaDia != null) telaDia.SetActive(false);
        if (telaResumoFinal != null) telaResumoFinal.SetActive(false);

        // Zera o saldo de dinheiro acumulado guardado na calculadora
        if (finalDeDia != null)
        {
            CalculadoraFinalDia calculadora = finalDeDia.GetComponent<CalculadoraFinalDia>();
            if (calculadora != null)
                calculadora.ReiniciarSaldo();
        }

        if (telaInicial != null) telaInicial.SetActive(true);
        if (fundoInicial != null) fundoInicial.SetActive(true);
    }

    // ==========================================
    // DOCUMENTOS E BOTÕES
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

    private void FecharTodosDocumentosGrandes()
    {
        if (documentosGrandesAbertos == null) return;

        foreach (GameObject documento in documentosGrandesAbertos)
        {
            if (documento != null)
                documento.SetActive(false);
        }
    }
}