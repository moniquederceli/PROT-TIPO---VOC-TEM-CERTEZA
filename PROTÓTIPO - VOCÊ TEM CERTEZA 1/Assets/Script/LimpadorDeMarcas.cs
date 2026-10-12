using UnityEngine;
using UnityEngine.UI;

// Coloque este script no objeto "pai" de uma tela ou documento que tem caixinhas para marcar
// (o Checklist de inspeção, ou a tela de gastos do final do dia).
// Quando o GameManager chama Limpar(), todas as caixinhas desse objeto voltam a ficar desmarcadas.
public class LimpadorDeMarcas : MonoBehaviour
{
    [Header("Procurar sozinho dentro deste objeto (deixe marcado, é o mais fácil)")]
    [Tooltip("Acha todas as caixinhas feitas com o script MarcadorX e desliga o X de cada uma.")]
    public bool procurarMarcadoresX = true;

    [Tooltip("Acha todas as caixinhas feitas com o componente Toggle do Unity e desmarca cada uma.")]
    public bool procurarToggles = true;

    [Header("Procurar marcas pelo NOME (é o que o Checklist usa)")]
    [Tooltip("Desliga todo objeto com este nome dentro deste objeto. No Checklist o nome é CheckMark. Deixe vazio para não usar.")]
    public string nomeDoObjetoDeMarca = "CheckMark";

    [Header("Extra (opcional): outros objetos que devem ser DESLIGADOS na limpeza (um X, um risco, uma marca...)")]
    public GameObject[] objetosParaDesligar;

    [Header("Diagnóstico: escreve no Console o que foi encontrado. Desmarque quando tudo funcionar.")]
    public bool mostrarMensagens = true;

    // Chamado pelo GameManager. Também pode ser ligado ao OnClick de um botão.
    public void Limpar()
    {
        int totalMarcadores = 0;
        int totalToggles = 0;
        int totalExtras = 0;
        int totalPorNome = 0;

        // O "true" faz o script procurar também em objetos que estão desligados
        if (procurarMarcadoresX)
        {
            MarcadorX[] marcadores = GetComponentsInChildren<MarcadorX>(true);

            totalMarcadores = marcadores.Length;

            foreach (MarcadorX marcador in marcadores)
            {
                // Só desliga o desenho do X.
                // IMPORTANTE: não devolve dinheiro. O que já foi gasto continua gasto;
                // o jogador só pode escolher de novo com o saldo que sobrou.
                if (marcador != null && marcador.X != null)
                    marcador.X.SetActive(false);
            }
        }

        if (procurarToggles)
        {
            Toggle[] toggles = GetComponentsInChildren<Toggle>(true);

            totalToggles = toggles.Length;

            foreach (Toggle toggle in toggles)
            {
                if (toggle != null)
                    toggle.SetIsOnWithoutNotify(false);
            }
        }

        if (!string.IsNullOrEmpty(nomeDoObjetoDeMarca))
        {
            // Procura em TODOS os objetos filhos, inclusive os desligados
            Transform[] todos = GetComponentsInChildren<Transform>(true);

            foreach (Transform t in todos)
            {
                if (t != null && t != transform && t.name == nomeDoObjetoDeMarca)
                {
                    t.gameObject.SetActive(false);
                    totalPorNome++;
                }
            }
        }

        if (objetosParaDesligar != null)
        {
            totalExtras = objetosParaDesligar.Length;

            foreach (GameObject objeto in objetosParaDesligar)
            {
                if (objeto != null)
                    objeto.SetActive(false);
            }
        }

        if (mostrarMensagens)
        {
            Debug.Log("LimpadorDeMarcas (" + name + "): limpei " + totalMarcadores + " caixinha(s) MarcadorX, "
                + totalToggles + " Toggle(s), " + totalPorNome + " marca(s) pelo nome e "
                + totalExtras + " objeto(s) da lista extra.");
        }
    }
}