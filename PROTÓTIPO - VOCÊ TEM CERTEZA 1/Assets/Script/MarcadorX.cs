using UnityEngine;

public class MarcadorX : MonoBehaviour
{
    [Header("Imagem do X")]
    public GameObject X;

    [Header("Calculadora")]
    public CalculadoraFinalDia calculadora;

    [Header("Valor deste gasto")]
    public float valorGasto;

    public void AlternarX()
    {
        bool estavaMarcado = X.activeSelf;

        if (!estavaMarcado)
        {
            // O jogador está tentando MARCAR (gastar) este item.
            // Só deixa se ele tiver saldo suficiente.
            if (calculadora != null && !calculadora.TentarAdicionarGasto(valorGasto))
            {
                // Sem saldo suficiente: não marca o X e não gasta nada.
                return;
            }

            X.SetActive(true);
        }
        else
        {
            // O jogador está DESMARCANDO: devolve o dinheiro.
            X.SetActive(false);

            if (calculadora != null)
                calculadora.RemoverGasto(valorGasto);
        }
    }
}