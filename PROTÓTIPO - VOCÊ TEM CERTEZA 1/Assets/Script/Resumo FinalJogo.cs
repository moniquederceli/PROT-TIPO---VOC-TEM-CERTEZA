using UnityEngine;
using TMPro;

public class ResumoFinalJogo : MonoBehaviour
{
    [Header("NPCs Normais")]
    public TMP_Text textoNormalSalvos;
    public TMP_Text textoNormalMortos;

    [Header("NPCs com Documento Errado")]
    public TMP_Text textoDocErradoSalvos;
    public TMP_Text textoDocErradoMortos;

    [Header("NPCs Anomálicos")]
    public TMP_Text textoAnomalicoSalvos;
    public TMP_Text textoAnomalicoMortos;

    [Header("Total morto por engano (Normal + Documento Errado)")]
    public TMP_Text textoMortosPorEngano;

    // Chamado pelo GameManager assim que a tela de resumo é aberta.
    public void ConfigurarResumo(
        int normalSalvos, int normalMortos,
        int docErradoSalvos, int docErradoMortos,
        int anomalicoSalvos, int anomalicoMortos)
    {
        if (textoNormalSalvos != null)
            textoNormalSalvos.text = "NPCs Normais salvos: " + normalSalvos;

        if (textoNormalMortos != null)
            textoNormalMortos.text = "NPCs Normais mortos: " + normalMortos;

        if (textoDocErradoSalvos != null)
            textoDocErradoSalvos.text = "NPCs com Documento Errado salvos: " + docErradoSalvos;

        if (textoDocErradoMortos != null)
            textoDocErradoMortos.text = "NPCs com Documento Errado mortos: " + docErradoMortos;

        if (textoAnomalicoSalvos != null)
            textoAnomalicoSalvos.text = "NPCs Anomálicos salvos: " + anomalicoSalvos;

        if (textoAnomalicoMortos != null)
            textoAnomalicoMortos.text = "NPCs Anomálicos mortos: " + anomalicoMortos;

        int mortosPorEngano = normalMortos + docErradoMortos;

        if (textoMortosPorEngano != null)
            textoMortosPorEngano.text = "Total mortos por engano (Normais + Doc. Errado): " + mortosPorEngano;
    }
}