using UnityEngine;
using TMPro;

public class CalculadoraFinalDia : MonoBehaviour
{
    [Header("Textos da tela")]
    public TMP_Text textoVidas;
    public TMP_Text textoMortes;
    public TMP_Text textoSalario;
    public TMP_Text textoBonus;
    public TMP_Text textoRendimentoDia;
    public TMP_Text textoRendimentoTotal;

    [Header("Valores")]
    public float vidasSalvas = 0f;
    public float mortesDecaidos = 0f;
    public float salario = 200f;
    public float bonus = 0f;

    private float gastos = 0f;

    public void ConfigurarDia(
        float vidas,
        float mortes,
        float bonusDoDia)
    {
        vidasSalvas = vidas;
        mortesDecaidos = mortes;
        bonus = bonusDoDia;
        salario = 200f;

        gastos = 0f;

        AtualizarTela();
    }

    public void AdicionarGasto(float valor)
    {
        gastos += valor;

        AtualizarTela();
    }

    public void RemoverGasto(float valor)
    {
        gastos -= valor;

        AtualizarTela();
    }

    private void AtualizarTela()
    {
        float rendimentoDia =
            vidasSalvas +
            salario +
            bonus;

        float rendimentoTotal =
            rendimentoDia - gastos;

        if (textoVidas != null)
            textoVidas.text = "Vidas salvas: R$ " + vidasSalvas.ToString("0.00");

        if (textoMortes != null)
            textoMortes.text = "Morte de decaídos: " + mortesDecaidos.ToString("0");

        if (textoSalario != null)
            textoSalario.text = "Salário: R$ " + salario.ToString("0.00");

        if (textoBonus != null)
            textoBonus.text = "Bônus: R$ " + bonus.ToString("0.00");

        if (textoRendimentoDia != null)
            textoRendimentoDia.text = "Rendimento do dia: R$ " + rendimentoDia.ToString("0.00");

        if (textoRendimentoTotal != null)
            textoRendimentoTotal.text = "Rendimento total: R$ " + rendimentoTotal.ToString("0.00");
    }
}
