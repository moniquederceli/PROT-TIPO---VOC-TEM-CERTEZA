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

        X.SetActive(!estavaMarcado);

        if (calculadora == null)
            return;

        if (!estavaMarcado)
        {
            // Marcou o gasto
            calculadora.AdicionarGasto(valorGasto);
        }
        else
        {
            // Desmarcou o gasto
            calculadora.RemoverGasto(valorGasto);
        }
    }
}