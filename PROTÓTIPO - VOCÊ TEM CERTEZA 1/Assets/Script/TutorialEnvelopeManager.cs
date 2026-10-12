using UnityEngine;

// Coloque este script UMA VEZ SÓ, em um objeto vazio chamado "SistemaTutorial".
// Fluxo do tutorial:
//   envelope fechado --(clique)--> envelope aberto (fundo + todas as folhas + frente)
//   envelope aberto --(clique na tampa)--> envelope fechado
//   envelope aberto --(clique em "todas as folhas")--> 3 folhas soltas na tela (grupo)
//   folha do grupo --(clique)--> some do grupo e abre a folha GRANDE (igual ao RG, LM e RM)
//   folha grande --(botão direito, ou Aprovar/Recusar)--> fecha e a folha volta ao grupo
//   folha do grupo --(botão direito)--> fecha a folha; quando as 3 fecham, "todas as folhas" volta pro envelope
public class TutorialEnvelopeManager : MonoBehaviour
{
    [Header("=== O ENVELOPE ===")]
    [Tooltip("O envelope FECHADO (a imagem padrão, que você clica para abrir)")]
    public GameObject envelopeFechado;

    [Tooltip("O envelope ABERTO: o objeto 'pai' que contém Fundo, Area Da Tampa, Todas As Folhas e Frente")]
    public GameObject envelopeAberto;

    [Tooltip("A imagem 'todas as folhas' que aparece dentro do envelope aberto (some enquanto as folhas estão fora do envelope)")]
    public GameObject todasAsFolhasNoEnvelope;

    [Header("=== AS 3 FOLHAS PEQUENAS DO GRUPO (com o efeito de hover) ===")]
    [Tooltip("NA ORDEM: Element 0 = Regras, Element 1 = Selos, Element 2 = Documentos")]
    public HoverEffect[] folhas;

    [Header("=== AS 3 FOLHAS GRANDES (iguais ao RG, LM e RM) ===")]
    [Tooltip("NA MESMA ORDEM: Element 0 = Regras, Element 1 = Selos, Element 2 = Documentos")]
    public FolhaGrandeTutorial[] folhasGrandes;

    [Header("Efeito de destaque ao passar o mouse")]
    [Tooltip("Cor da folha que está com o mouse em cima (branco = brilho normal)")]
    public Color corDestacada = Color.white;

    [Tooltip("Cor das OUTRAS folhas enquanto uma está destacada (mais escura = mais apagada)")]
    public Color corEscurecida = new Color(0.4f, 0.4f, 0.4f, 1f);

    [Tooltip("A folha destacada cresce um pouquinho (1 = não cresce, 1.05 = 5% maior)")]
    public float escalaDestacada = 1.05f;

    // Onde cada folha está agora
    private enum Estado
    {
        Fechada,   // dentro do envelope
        NoGrupo,   // folha pequena solta na tela, no grupo de 3
        Grande     // aberta em tamanho grande
    }

    private bool envelopeEstaAberto = false;
    private Estado[] estados;
    private int[] ordemInicial;

    private void Awake()
    {
        int quantidade = (folhas != null) ? folhas.Length : 0;
        estados = new Estado[quantidade];

        // Descobre qual folha fica atrás e qual fica na frente (pela ordem na Hierarchy),
        // para sempre reabrir as folhas empilhadas do mesmo jeito.
        ordemInicial = new int[quantidade];
        int[] chaves = new int[quantidade];

        for (int i = 0; i < quantidade; i++)
        {
            ordemInicial[i] = i;
            chaves[i] = (folhas[i] != null) ? folhas[i].transform.GetSiblingIndex() : int.MaxValue;

            if (folhas[i] != null)
                folhas[i].Configurar(this, i);
        }

        System.Array.Sort(chaves, ordemInicial);

        if (folhasGrandes != null)
        {
            for (int i = 0; i < folhasGrandes.Length; i++)
            {
                if (folhasGrandes[i] != null)
                    folhasGrandes[i].Configurar(this, i);
            }
        }

        // Começa tudo fechado
        for (int i = 0; i < quantidade; i++)
        {
            estados[i] = Estado.Fechada;

            if (folhas[i] != null)
                folhas[i].gameObject.SetActive(false);

            if (folhasGrandes != null && i < folhasGrandes.Length && folhasGrandes[i] != null)
                folhasGrandes[i].gameObject.SetActive(false);
        }

        envelopeEstaAberto = false;
        AtualizarEnvelope();
    }

    // ==========================================
    // ENVELOPE
    // ==========================================

    // Ligue no ONCLICK do envelope FECHADO
    public void AbrirEnvelope()
    {
        if (envelopeEstaAberto)
            return;

        envelopeEstaAberto = true;
        AtualizarEnvelope();
    }

    // Ligue no ONCLICK da área da tampa (dentro do envelope aberto)
    public void FecharEnvelope()
    {
        if (!envelopeEstaAberto)
            return;

        envelopeEstaAberto = false;
        AtualizarEnvelope();
    }

    // Ligue no ONCLICK da imagem "todas as folhas" (dentro do envelope aberto)
    public void PegarFolhas()
    {
        if (!envelopeEstaAberto || !TodasFechadas())
            return;

        // Abre as 3 folhas empilhadas, cada uma no seu lugar original
        for (int k = 0; k < ordemInicial.Length; k++)
        {
            int i = ordemInicial[k];

            if (folhas[i] == null)
                continue;

            folhas[i].Resetar();
            folhas[i].gameObject.SetActive(true);
            folhas[i].transform.SetAsLastSibling();
            estados[i] = Estado.NoGrupo;
        }

        AtualizarEnvelope();
    }

    // Mostra o envelope fechado OU o aberto, e esconde "todas as folhas" quando elas estão fora do envelope
    private void AtualizarEnvelope()
    {
        if (envelopeFechado != null)
            envelopeFechado.SetActive(!envelopeEstaAberto);

        if (envelopeAberto != null)
            envelopeAberto.SetActive(envelopeEstaAberto);

        if (todasAsFolhasNoEnvelope != null)
            todasAsFolhasNoEnvelope.SetActive(envelopeEstaAberto && TodasFechadas());
    }

    // ==========================================
    // FOLHAS DO GRUPO (PEQUENAS)
    // ==========================================

    // Chamado pelo HoverEffect quando o mouse ENTRA numa folha
    public void AoEntrarNaFolha(int indice)
    {
        AplicarDestaque(indice);
    }

    // Chamado pelo HoverEffect quando o mouse SAI de uma folha
    public void AoSairDaFolha(int indice)
    {
        AplicarDestaque(-1);
    }

    // Chamado pelo HoverEffect quando você clica (com o botão esquerdo, sem arrastar) numa folha do grupo:
    // ela some do grupo e abre a folha grande.
    public void AbrirFolhaGrande(int indice)
    {
        if (!IndiceValido(indice) || estados[indice] != Estado.NoGrupo)
            return;

        if (folhasGrandes == null || indice >= folhasGrandes.Length || folhasGrandes[indice] == null)
        {
            Debug.LogError("TutorialEnvelopeManager: a folha grande número " + indice + " não está configurada em 'Folhas Grandes'.");
            return;
        }

        estados[indice] = Estado.Grande;

        // Some do grupo
        if (folhas[indice] != null)
            folhas[indice].gameObject.SetActive(false);

        AplicarDestaque(-1);

        // Abre a folha grande no lugar original, na frente dos outros documentos (sem passar do GrennFilter)
        FolhaGrandeTutorial grande = folhasGrandes[indice];
        grande.Resetar();
        grande.gameObject.SetActive(true);
        OrdemDeCamadas.TrazerParaFrente(grande.transform);

        AtualizarEnvelope();
    }

    // Chamado pelo FolhaGrandeTutorial quando a folha grande fecha (botão direito, ou Aprovar/Recusar):
    // a folha pequena volta para o grupo.
    public void AoFecharFolhaGrande(int indice)
    {
        if (!IndiceValido(indice) || estados[indice] != Estado.Grande)
            return;

        estados[indice] = Estado.NoGrupo;

        if (folhas[indice] != null)
        {
            folhas[indice].Resetar();
            folhas[indice].gameObject.SetActive(true);
        }

        // Refaz a pilha na ordem original (a folha que você clicou tinha passado para a frente)
        RestaurarOrdemDasFolhas();

        AtualizarEnvelope();
    }

    // Coloca as folhas de volta na ordem de empilhamento original da Hierarchy
    // (a primeira da lista fica atrás, e a última na frente)
    private void RestaurarOrdemDasFolhas()
    {
        for (int k = 0; k < ordemInicial.Length; k++)
        {
            int i = ordemInicial[k];

            if (folhas[i] != null)
                folhas[i].transform.SetAsLastSibling();
        }
    }

    // Chamado pelo HoverEffect quando você clica com o botão direito numa folha do grupo
    public void FecharFolha(int indice)
    {
        if (!IndiceValido(indice) || estados[indice] != Estado.NoGrupo)
            return;

        estados[indice] = Estado.Fechada;

        if (folhas[indice] != null)
            folhas[indice].gameObject.SetActive(false);

        AplicarDestaque(-1);

        // Se foi a última folha, "todas as folhas" volta para dentro do envelope
        AtualizarEnvelope();
    }

    // indiceDestacado = -1 significa "nenhuma": todas ficam normais
    private void AplicarDestaque(int indiceDestacado)
    {
        if (folhas == null)
            return;

        for (int i = 0; i < folhas.Length; i++)
        {
            if (folhas[i] == null || estados[i] != Estado.NoGrupo)
                continue;

            if (indiceDestacado < 0)
                folhas[i].AplicarAparencia(Color.white, 1f);
            else if (i == indiceDestacado)
                folhas[i].AplicarAparencia(corDestacada, escalaDestacada);
            else
                folhas[i].AplicarAparencia(corEscurecida, 1f);
        }
    }

    private bool IndiceValido(int indice)
    {
        return folhas != null && estados != null && indice >= 0 && indice < folhas.Length;
    }

    private bool TodasFechadas()
    {
        if (estados == null)
            return true;

        foreach (Estado estado in estados)
        {
            if (estado != Estado.Fechada)
                return false;
        }

        return true;
    }
}