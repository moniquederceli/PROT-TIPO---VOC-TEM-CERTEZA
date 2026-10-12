using UnityEngine;

// Coloque este script em CADA folha GRANDE do tutorial (GrandeRegras, GrandeSelos, GrandeDocumentos).
// Ele avisa o TutorialEnvelopeManager quando a folha grande fecha (por qualquer motivo:
// botão direito, ou ao Aprovar/Recusar), para a folha pequena voltar ao grupo de 3.
// Você NÃO precisa preencher nada neste componente: o TutorialEnvelopeManager configura sozinho.
public class FolhaGrandeTutorial : MonoBehaviour
{
    private TutorialEnvelopeManager gerenciador;
    private int indice;

    private RectTransform rectTransform;
    private Vector2 posicaoInicial;
    private bool preparado = false;
    private bool encerrando = false;

    private void Preparar()
    {
        if (preparado)
            return;

        rectTransform = GetComponent<RectTransform>();
        posicaoInicial = rectTransform.anchoredPosition;
        preparado = true;
    }

    // Chamado pelo TutorialEnvelopeManager
    public void Configurar(TutorialEnvelopeManager novoGerenciador, int novoIndice)
    {
        Preparar();
        gerenciador = novoGerenciador;
        indice = novoIndice;
    }

    // Chamado pelo TutorialEnvelopeManager: a folha abre sempre no lugar em que você a deixou no editor
    public void Resetar()
    {
        Preparar();
        rectTransform.anchoredPosition = posicaoInicial;
    }

    private void OnApplicationQuit()
    {
        encerrando = true;
    }

    // Roda sempre que a folha grande é desligada (por qualquer motivo)
    private void OnDisable()
    {
        if (encerrando || gerenciador == null)
            return;

        gerenciador.AoFecharFolhaGrande(indice);
    }
}