using UnityEngine;
using UnityEngine.UI;

public class NPCDocuments : MonoBehaviour
{
    // Um "conjunto" de documentos GRANDES de UM NPC: os corretos (usados
    // quando o NPC é Normal OU Aparência Anômala) e os incorretos (usados
    // só quando o NPC é Documento Errado).
    [System.Serializable]
    public class NPCDocumentSet
    {
        [Tooltip("Precisa ser o MESMO número usado no GameManager, no campo Npc Id")]
        public int npcId;

        [Header("Documento GRANDE (zoom) quando o NPC é Normal ou Aparência Anômala")]
        public Sprite rgCorretoGrande;
        public Sprite lmCorretoGrande;
        public Sprite rmCorretoGrande;

        [Header("Documento GRANDE (zoom) quando o NPC é Documento Errado")]
        public Sprite rgIncorretoGrande;
        public Sprite lmIncorretoGrande;
        public Sprite rmIncorretoGrande;
    }

    [Header("Documentos na mesa (SEMPRE iguais, para qualquer NPC)")]
    public Image documentoRG;
    public Image documentoLM;
    public Image documentoRM;

    public Sprite rgGenerico;
    public Sprite lmGenerico;
    public Sprite rmGenerico;

    [Header("Documentos GRANDES (zoom) de cada um dos 8 NPCs")]
    public NPCDocumentSet[] documentos = new NPCDocumentSet[8];

    // Chamado toda vez que um novo NPC entra na cabine.
    // A imagem da mesa é sempre a mesma, não importa o NPC nem a variação.
    public void MostrarDocumentos()
    {
        if (documentoRG != null) documentoRG.sprite = rgGenerico;
        if (documentoLM != null) documentoLM.sprite = lmGenerico;
        if (documentoRM != null) documentoRM.sprite = rmGenerico;
    }

    // ==========================================
    // O código chega assim: (Id do NPC x 10) + 0 [correto] ou +1 [incorreto]
    // Exemplo: NPC 3 correto = 30 | NPC 3 incorreto = 31
    // Usado só quando o jogador AMPLIA o documento.
    // ==========================================
    public Sprite GetRGGrande(int codigoDocumento)
    {
        NPCDocumentSet set = BuscarSetPorId(codigoDocumento / 10);
        if (set == null) return null;

        return (codigoDocumento % 10) == 1 ? set.rgIncorretoGrande : set.rgCorretoGrande;
    }

    public Sprite GetLMGrande(int codigoDocumento)
    {
        NPCDocumentSet set = BuscarSetPorId(codigoDocumento / 10);
        if (set == null) return null;

        return (codigoDocumento % 10) == 1 ? set.lmIncorretoGrande : set.lmCorretoGrande;
    }

    public Sprite GetRMGrande(int codigoDocumento)
    {
        NPCDocumentSet set = BuscarSetPorId(codigoDocumento / 10);
        if (set == null) return null;

        return (codigoDocumento % 10) == 1 ? set.rmIncorretoGrande : set.rmCorretoGrande;
    }

    private NPCDocumentSet BuscarSetPorId(int npcId)
    {
        foreach (NPCDocumentSet set in documentos)
        {
            if (set != null && set.npcId == npcId)
                return set;
        }

        Debug.LogError("NPCDocuments: não encontrei nenhum conjunto de documentos com o Npc Id " + npcId + ". Confira o array 'Documentos' no Inspector.");
        return null;
    }
}