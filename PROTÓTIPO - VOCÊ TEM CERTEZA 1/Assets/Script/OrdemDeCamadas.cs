using UnityEngine;

// Ajudante usado pelo DragDocument. Não vai em nenhum objeto da cena.
public static class OrdemDeCamadas
{
    private static LimiteDeCamada limite;
    private static bool avisouSemLimite = false;

    // Traz uma folha para a frente das outras folhas, mas NUNCA passa do LimiteFolhas.
    public static void TrazerParaFrente(Transform objeto)
    {
        if (objeto == null)
            return;

        if (limite == null)
            limite = Object.FindAnyObjectByType<LimiteDeCamada>();

        // Sem o LimiteFolhas na cena não dá para proteger o GrennFilter:
        // faz o básico (sobe só dentro do próprio grupo) e avisa uma vez.
        if (limite == null)
        {
            if (!avisouSemLimite)
            {
                Debug.LogWarning("OrdemDeCamadas: não achei o objeto LimiteFolhas (com o script Limite De Camada). Os documentos podem passar por cima do GrennFilter.");
                avisouSemLimite = true;
            }

            objeto.SetAsLastSibling();
            return;
        }

        Transform atual = objeto;

        // Sobe a "escada" de pais: a folha, o grupo dela, o grupo do grupo...
        while (atual != null && atual.parent != null)
        {
            Transform pai = atual.parent;

            // Chegamos no andar onde mora o LimiteFolhas (o Canvas):
            // coloca este grupo logo ABAIXO da linha, e para por aqui.
            if (pai == limite.transform.parent)
            {
                int indiceLimite = limite.transform.GetSiblingIndex();
                int indiceAtual = atual.GetSiblingIndex();

                if (indiceAtual < indiceLimite)
                    atual.SetSiblingIndex(indiceLimite - 1);
                else
                    atual.SetSiblingIndex(indiceLimite);

                return;
            }

            // Andares de baixo (dentro de grupos): só passa por cima dos "irmãos".
            atual.SetAsLastSibling();

            atual = pai;
        }
    }
}