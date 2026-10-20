using System.Collections.Generic;
using UnityEngine;

// Coloque este script UMA VEZ SÓ, em um objeto vazio chamado "GabaritoManager".
//
// Ele guarda o GABARITO do Checklist e compara com o que o jogador marcou.
// Tudo acontece por trás dos panos: o jogador não vê nada disso na tela.
//
// Como o gabarito é montado:
//  1) Por padrão é AUTOMÁTICO, pelo tipo do NPC:
//       Normal           -> todas as linhas válidas
//       Documento Errado -> todas válidas, EXCETO as linhas marcadas "Errada No Documento Errado"
//       Anomalia         -> todas válidas, EXCETO as linhas marcadas "Errada Na Anomalia"
//  2) Se um NPC tiver o script GabaritoNPC com "Usar Gabarito Personalizado" marcado,
//     vale o gabarito dele.
public class GabaritoManager : MonoBehaviour
{
    public enum Opcao
    {
        A,
        B
    }

    [System.Serializable]
    public class LinhaDoChecklist
    {
        [Tooltip("Nome da linha, só pra você se organizar. Ex: RG, LM, RM, Aparência")]
        public string nome = "Linha";

        [Tooltip("Qual caixinha (A ou B) o jogador marca quando o item está CORRETO / VÁLIDO")]
        public Opcao opcaoQuandoValido = Opcao.A;

        [Tooltip("Marque se esta linha está ERRADA nos NPCs de Documento Errado (ex: a linha do RM)")]
        public bool erradaNoDocumentoErrado = false;

        [Tooltip("Marque se esta linha está ERRADA nos NPCs Anomalia (ex: a linha da Aparência)")]
        public bool erradaNaAnomalia = false;
    }

    [Header("O ValidationManager (ele sabe ler o que o jogador marcou). Se vazio, o script acha sozinho")]
    public ValidationManager validationManager;

    [Header("As linhas do Checklist, na ordem (Element 0 = Linha 1 = Check1, ...)")]
    public List<LinhaDoChecklist> linhas = CriarLinhasPadrao();

    [Header("Escreve no Console a comparação de cada decisão (desmarque quando tudo funcionar)")]
    public bool mostrarMensagens = true;

    public int QuantidadeDeLinhas
    {
        get { return linhas != null ? linhas.Count : 0; }
    }

    private static List<LinhaDoChecklist> CriarLinhasPadrao()
    {
        List<LinhaDoChecklist> lista = new List<LinhaDoChecklist>();

        for (int i = 1; i <= 5; i++)
        {
            LinhaDoChecklist linha = new LinhaDoChecklist();
            linha.nome = "Linha " + i;
            lista.Add(linha);
        }

        return lista;
    }

    private void Awake()
    {
        if (validationManager == null)
            validationManager = FindAnyObjectByType<ValidationManager>();
    }

    // O GameManager chama isto quando o jogador decide.
    // tipoDeNpc: 0 = Normal, 1 = Documento Errado, 2 = Anomalia.
    // Devolve quantas linhas o jogador preencheu ERRADO (de 0 até a quantidade de linhas).
    public int ContarLinhasErradas(GameObject npc, int tipoDeNpc)
    {
        if (validationManager == null || linhas == null || linhas.Count == 0)
            return 0;

        GabaritoNPC personalizado = (npc != null) ? npc.GetComponent<GabaritoNPC>() : null;
        bool usarPersonalizado = (personalizado != null && personalizado.usarGabaritoPersonalizado);

        if (mostrarMensagens && !usarPersonalizado)
            Debug.LogWarning("GabaritoManager: " + (npc != null ? npc.name : "NPC") + " NÃO tem o script GabaritoNPC (ou ele está desmarcado). Usando o gabarito automático.");

        int erradas = 0;
        System.Text.StringBuilder relatorio = new System.Text.StringBuilder();

        for (int i = 0; i < linhas.Count; i++)
        {
            Opcao esperada = usarPersonalizado
                ? ConverterOpcao(personalizado.RespostaDaLinha(i))
                : EsperadaAutomatica(i, tipoDeNpc);

            // 0 = nada marcado | 1 = A | 2 = B | 3 = as duas
            int marcada = validationManager.LerMarcacao(i + 1);

            bool certa = (marcada == 1 && esperada == Opcao.A) || (marcada == 2 && esperada == Opcao.B);

            if (!certa)
                erradas++;

            relatorio.Append(linhas[i].nome + ": esperado " + esperada + ", marcado " + NomeDaMarcacao(marcada)
                + (certa ? " (ok)" : " (ERRADO)") + " | ");
        }

        if (mostrarMensagens)
        {
            Debug.Log("GabaritoManager: " + (npc != null ? npc.name : "NPC") + " -> " + erradas + " linha(s) errada(s). " + relatorio);
        }

        return erradas;
    }

    // Gabarito automático, pelo tipo do NPC
    private Opcao EsperadaAutomatica(int indiceDaLinha, int tipoDeNpc)
    {
        LinhaDoChecklist linha = linhas[indiceDaLinha];

        bool linhaQuebrada =
            (tipoDeNpc == 1 && linha.erradaNoDocumentoErrado) ||
            (tipoDeNpc == 2 && linha.erradaNaAnomalia);

        if (!linhaQuebrada)
            return linha.opcaoQuandoValido;

        // Linha errada: a resposta certa é marcar a OUTRA caixinha
        return (linha.opcaoQuandoValido == Opcao.A) ? Opcao.B : Opcao.A;
    }

    private Opcao ConverterOpcao(GabaritoNPC.Opcao opcao)
    {
        return (opcao == GabaritoNPC.Opcao.A) ? Opcao.A : Opcao.B;
    }

    private string NomeDaMarcacao(int marcada)
    {
        switch (marcada)
        {
            case 1: return "A";
            case 2: return "B";
            case 3: return "A e B";
            default: return "nada";
        }
    }
}