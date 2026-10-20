using UnityEngine;

// OPCIONAL. Coloque este script em um NPC (qualquer das 24 versões) SÓ se quiser um gabarito
// especial para ele, diferente do automático.
//
// Sem este script, o jogo usa o gabarito AUTOMÁTICO, que depende do tipo do NPC:
//   Normal           -> tudo válido
//   Documento Errado -> tudo válido, EXCETO as linhas marcadas como "errada no Documento Errado"
//   Anomalia         -> tudo válido, EXCETO as linhas marcadas como "errada na Anomalia"
// (isso é configurado UMA VEZ SÓ no GabaritoManager)
public class GabaritoNPC : MonoBehaviour
{
    public enum Opcao
    {
        A,
        B
    }

    [Header("Usar o gabarito DESTE NPC (em vez do automático)?")]
    [Tooltip("Marcado: vale o que você escolher abaixo (o normal). Desmarcado: este NPC usa o gabarito automático do GabaritoManager")]
    public bool usarGabaritoPersonalizado = true;

    [Header("Resposta CORRETA de cada linha do Checklist (A ou B)")]
    [Tooltip("Linha 1 = objetos 'Check1 A' / 'Check1 B'")]
    public Opcao linha1 = Opcao.A;

    [Tooltip("Linha 2 = objetos 'Check2 A' / 'Check2 B'")]
    public Opcao linha2 = Opcao.A;

    [Tooltip("Linha 3 = objetos 'Check3 A' / 'Check3 B'")]
    public Opcao linha3 = Opcao.A;

    [Tooltip("Linha 4 = objetos 'Check4 A' / 'Check4 B'")]
    public Opcao linha4 = Opcao.A;

    [Tooltip("Linha 5 = objetos 'Check5 A' / 'Check5 B'")]
    public Opcao linha5 = Opcao.A;

    // Devolve a resposta correta da linha (indice 0 = linha 1)
    public Opcao RespostaDaLinha(int indice)
    {
        switch (indice)
        {
            case 0: return linha1;
            case 1: return linha2;
            case 2: return linha3;
            case 3: return linha4;
            case 4: return linha5;
            default: return Opcao.A;
        }
    }
}