using UnityEngine;

// Coloque este script em CADA versão de NPC (Normal, Anomalia e DocErrado).
// Cada versão tem as SUAS PRÓPRIAS falas de abertura.
// Para adicionar mais falas no futuro, é só aumentar o "Size" da lista no Inspector.
public class DialogoNPC : MonoBehaviour
{
    // ================================================================
    // UMA FALA = uma lista de balões (páginas).
    // Fala curta  -> 1 balão.
    // Fala longa  -> vários balões, que aparecem um depois do outro.
    // ================================================================
    [System.Serializable]
    public class Fala
    {
        [Tooltip("Cada elemento é UM balão. Exemplo: Element 0 = 'Oi, eu estava indo ali...'  Element 1 = '...me deixar passar, né??'")]
        [TextArea(2, 6)]
        public string[] baloes;
    }

    // ================================================================
    // PREPARADO PARA O FUTURO: conversa em formato de chat.
    // Por enquanto NÃO é usado pelo jogo. Você já pode ir cadastrando
    // perguntas do guarda e as respostas do NPC, que depois ligamos.
    // ================================================================
    [System.Serializable]
    public class PerguntaResposta
    {
        [Tooltip("O que o GUARDA (jogador) pergunta. Exemplo: 'Para onde você vai?'")]
        public string pergunta;

        [Tooltip("As respostas possíveis do NPC (pode ter várias, cada uma com vários balões)")]
        public Fala[] respostas;
    }

    public enum ModoDeEscolha
    {
        Alternar,   // usa a fala 0, depois a 1, depois a 2... e volta pra 0
        Aleatorio   // sorteia, mas nunca repete a mesma duas vezes seguidas
    }

    [Header("Falas de ABERTURA (o que o NPC diz ao chegar na cabine)")]
    [Tooltip("Coloque pelo menos 2. Pode colocar quantas quiser.")]
    public Fala[] aberturas;

    [Header("Como escolher entre as aberturas")]
    public ModoDeEscolha modoDeEscolha = ModoDeEscolha.Alternar;

    [Header("(FUTURO) Conversa por chat: perguntas do guarda e respostas do NPC")]
    public PerguntaResposta[] conversa;

    // Qual abertura foi usada por último (-1 = ainda nenhuma)
    private int ultimaAbertura = -1;

    // O DialogueManager chama isto pra saber o que o NPC vai falar agora
    public Fala EscolherAbertura()
    {
        if (aberturas == null || aberturas.Length == 0)
            return null;

        if (aberturas.Length == 1)
        {
            ultimaAbertura = 0;
            return aberturas[0];
        }

        int escolhida;

        if (modoDeEscolha == ModoDeEscolha.Alternar)
        {
            escolhida = (ultimaAbertura + 1) % aberturas.Length;
        }
        else
        {
            // Sorteia entre as outras (nunca repete a anterior)
            escolhida = Random.Range(0, aberturas.Length - 1);

            if (ultimaAbertura >= 0 && escolhida >= ultimaAbertura)
                escolhida++;
        }

        ultimaAbertura = escolhida;
        return aberturas[escolhida];
    }
}