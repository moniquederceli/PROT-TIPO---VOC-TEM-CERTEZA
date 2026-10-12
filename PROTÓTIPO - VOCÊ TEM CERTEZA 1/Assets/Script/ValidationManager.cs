using UnityEngine;

// Coloque este script UMA VEZ SÓ, em um objeto vazio chamado "ValidationManager".
// Ele decide se o jogador já pode apertar Aprovar (verde) ou Recusar (vermelho):
// só pode depois de marcar TODAS as linhas do Checklist e aplicar o carimbo.
public class ValidationManager : MonoBehaviour
{
    [Header("O Checklist (arraste o objeto ChecklistInspect)")]
    public Transform checklist;

    [Header("Quantas linhas o Checklist tem (Check1, Check2... até Check5)")]
    public int quantidadeDeLinhas = 5;

    [Header("Nomes dos objetos (só mude se você renomeou algo)")]
    public string prefixoDaLinha = "Check";   // Check1 A, Check1 B...
    public string sufixoA = " A";
    public string sufixoB = " B";
    public string nomeDaMarca = "CheckMark";  // o objeto que aparece quando a caixinha está marcada

    [Header("A zona do carimbo (arraste a ZonaCarimbo). Se ficar vazia, o script acha sozinho")]
    public ZonaDeCarimbo zonaDeCarimbo;

    [Header("O aviso 'Preencher documentação' (arraste o objeto que tem o script AvisoTemporario)")]
    public AvisoTemporario aviso;

    [Header("Diagnóstico: escreve no Console o que está faltando. Desmarque quando tudo funcionar.")]
    public bool mostrarMensagens = true;

    private void Awake()
    {
        if (zonaDeCarimbo == null)
            zonaDeCarimbo = FindAnyObjectByType<ZonaDeCarimbo>(FindObjectsInactive.Include);
    }

    // O GameManager chama este método quando o jogador aperta Aprovar ou Recusar.
    // Se faltar algo, mostra o aviso e devolve "false" (o botão não faz nada).
    public bool PodeDecidir()
    {
        bool linhasOk = TodasAsLinhasMarcadas();
        bool carimboOk = CarimboAplicado();

        if (linhasOk && carimboOk)
            return true;

        if (mostrarMensagens)
        {
            Debug.Log("ValidationManager: decisão bloqueada. Checklist completo? " + linhasOk + " | Carimbo aplicado? " + carimboOk);
        }

        if (aviso != null)
            aviso.Mostrar();

        return false;
    }

    private bool TodasAsLinhasMarcadas()
    {
        if (checklist == null)
        {
            Debug.LogError("ValidationManager: o campo 'Checklist' está vazio. Arraste o ChecklistInspect para ele. Por enquanto a trava do checklist está DESLIGADA.");
            return true;
        }

        for (int i = 1; i <= quantidadeDeLinhas; i++)
        {
            Transform a = BuscarPorNome(prefixoDaLinha + i + sufixoA);
            Transform b = BuscarPorNome(prefixoDaLinha + i + sufixoB);

            if (a == null && b == null)
            {
                Debug.LogError("ValidationManager: não achei '" + prefixoDaLinha + i + sufixoA + "' nem '" + prefixoDaLinha + i + sufixoB + "' dentro do Checklist. Confira os nomes. Esta linha foi ignorada.");
                continue;
            }

            bool marcada = TemMarcaLigada(a) || TemMarcaLigada(b);

            if (!marcada)
            {
                if (mostrarMensagens)
                {
                    Debug.Log("ValidationManager: a linha " + i + " ainda não foi marcada.");
                    Debug.Log(RelatorioCompleto());
                }

                return false;
            }
        }

        return true;
    }

    private bool CarimboAplicado()
    {
        if (zonaDeCarimbo == null)
        {
            Debug.LogError("ValidationManager: não achei a ZonaCarimbo. Arraste ela para o campo 'Zona De Carimbo'. Por enquanto a trava do carimbo está DESLIGADA.");
            return true;
        }

        return zonaDeCarimbo.TemCarimbo();
    }

    // Procura um objeto pelo nome dentro do Checklist (inclusive objetos desligados).
    // A comparação ignora espaços (inclusive espaços escondidos), maiúsculas/minúsculas
    // e letras parecidas de outros alfabetos, para não falhar por um detalhe invisível no nome.
    private Transform BuscarPorNome(string nome)
    {
        string procurado = Normalizar(nome);
        Transform[] todos = checklist.GetComponentsInChildren<Transform>(true);

        foreach (Transform t in todos)
        {
            if (t != null && t != checklist && Normalizar(t.name) == procurado)
                return t;
        }

        return null;
    }

    private string Normalizar(string texto)
    {
        if (string.IsNullOrEmpty(texto))
            return "";

        System.Text.StringBuilder resultado = new System.Text.StringBuilder();

        foreach (char c in texto)
        {
            // Pula espaços normais, espaços "invisíveis" e caracteres de largura zero
            if (char.IsWhiteSpace(c) || c == '\u200B' || c == '\uFEFF' || c == '\u00A0')
                continue;

            char minusculo = char.ToLowerInvariant(c);

            // Letras do alfabeto cirílico que são iguais às do nosso alfabeto
            switch (minusculo)
            {
                case '\u0430': minusculo = 'a'; break;
                case '\u0432': minusculo = 'b'; break;
                case '\u0441': minusculo = 'c'; break;
                case '\u0435': minusculo = 'e'; break;
                case '\u043E': minusculo = 'o'; break;
                case '\u043A': minusculo = 'k'; break;
            }

            resultado.Append(minusculo);
        }

        return resultado.ToString();
    }

    // Só para diagnóstico: descreve, linha por linha, tudo o que existe dentro de cada caixinha
    private string RelatorioCompleto()
    {
        System.Text.StringBuilder texto = new System.Text.StringBuilder();
        texto.Append("ValidationManager RELATÓRIO (o que o script enxerga dentro de cada caixinha) >>> ");

        for (int i = 1; i <= quantidadeDeLinhas; i++)
        {
            texto.Append("LINHA " + i + ") ");
            texto.Append(prefixoDaLinha + i + sufixoA + " = " + Descrever(BuscarPorNome(prefixoDaLinha + i + sufixoA)));
            texto.Append(" | ");
            texto.Append(prefixoDaLinha + i + sufixoB + " = " + Descrever(BuscarPorNome(prefixoDaLinha + i + sufixoB)));
            texto.Append("  ///  ");
        }

        return texto.ToString();
    }

    // Só para diagnóstico: mostra cada nome com a quantidade de letras dele (revela espaços escondidos)
    private string ListarNomesDoChecklist()
    {
        System.Text.StringBuilder texto = new System.Text.StringBuilder();

        foreach (Transform filho in checklist)
            texto.Append("'" + filho.name + "'(" + filho.name.Length + " letras) ");

        return texto.ToString();
    }

    private string Descrever(Transform caixinha)
    {
        if (caixinha == null)
            return "NÃO ACHEI este objeto (nomes que existem no Checklist: " + ListarNomesDoChecklist() + ")";

        Transform[] filhos = caixinha.GetComponentsInChildren<Transform>(true);
        System.Text.StringBuilder texto = new System.Text.StringBuilder();
        int quantos = 0;

        foreach (Transform t in filhos)
        {
            if (t == caixinha)
                continue;

            quantos++;
            texto.Append("[" + t.name + ": " + (t.gameObject.activeSelf ? "LIGADO" : "desligado") + "] ");
        }

        if (quantos == 0)
            return "(não tem nenhum objeto dentro)";

        return texto.ToString();
    }

    // A caixinha está marcada se o objeto "CheckMark" dela estiver ligado
    private bool TemMarcaLigada(Transform caixinha)
    {
        if (caixinha == null)
            return false;

        Transform[] filhos = caixinha.GetComponentsInChildren<Transform>(true);

        foreach (Transform t in filhos)
        {
            // activeSelf: olha só se a marca em si está ligada,
            // mesmo que o Checklist esteja fechado no momento
            if (t != null && t != caixinha && Normalizar(t.name) == Normalizar(nomeDaMarca) && t.gameObject.activeSelf)
                return true;
        }

        return false;
    }
}