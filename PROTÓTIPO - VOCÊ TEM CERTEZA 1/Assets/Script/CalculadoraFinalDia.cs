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

    [Header("Valores do dia atual (mostrados na tela)")]
    private float vidasSalvasHoje = 0f;
    private float mortesDecaidosHoje = 0f;
    private float salarioHoje = 200f;
    private float bonusHoje = 0f;

    // Este é o "cofre" do jogador: guarda o dinheiro de verdade,
    // e continua existindo de um dia para o outro (não reseta em ConfigurarDia).
    private float saldoAcumulado = 0f;
    private float gastosAcumulados = 0f;

    // Chamado pelo GameManager no final de cada dia, com os números daquele dia.
    public void ConfigurarDia(float vidas, float mortes, float bonusDoDia)
    {
        vidasSalvasHoje = vidas;
        mortesDecaidosHoje = mortes;
        bonusHoje = bonusDoDia;
        salarioHoje = 200f;

        // Soma o rendimento de HOJE ao saldo total acumulado
        float rendimentoHoje = vidasSalvasHoje + salarioHoje + bonusHoje;
        saldoAcumulado += rendimentoHoje;

        // O saldo nunca pode ficar negativo
        if (saldoAcumulado < 0f)
            saldoAcumulado = 0f;

        AtualizarTela();
    }

    // Chamado pelo MarcadorX quando o jogador tenta marcar um gasto.
    // Retorna "true" se deu certo, e "false" se não tinha saldo suficiente.
    public bool TentarAdicionarGasto(float valor)
    {
        if (SaldoDisponivel() < valor)
        {
            // Não deixa gastar mais do que tem
            return false;
        }

        gastosAcumulados += valor;
        AtualizarTela();
        return true;
    }

    // Chamado pelo MarcadorX quando o jogador desmarca um gasto.
    public void RemoverGasto(float valor)
    {
        gastosAcumulados -= valor;

        if (gastosAcumulados < 0f)
            gastosAcumulados = 0f;

        AtualizarTela();
    }

    // Quanto dinheiro o jogador realmente tem disponível agora
    public float SaldoDisponivel()
    {
        return saldoAcumulado - gastosAcumulados;
    }

    // Chamado pelo GameManager quando o jogo é reiniciado do zero (volta ao menu)
    public void ReiniciarSaldo()
    {
        saldoAcumulado = 0f;
        gastosAcumulados = 0f;
        vidasSalvasHoje = 0f;
        mortesDecaidosHoje = 0f;
        bonusHoje = 0f;

        AtualizarTela();
    }

    private void AtualizarTela()
    {
        float rendimentoHoje = vidasSalvasHoje + salarioHoje + bonusHoje;
        float rendimentoTotal = SaldoDisponivel();

        if (textoVidas != null)
            textoVidas.text = "Vidas salvas: R$ " + vidasSalvasHoje.ToString("0.00");

        if (textoMortes != null)
            textoMortes.text = "Morte de decaídos: " + mortesDecaidosHoje.ToString("0");

        if (textoSalario != null)
            textoSalario.text = "Salário: R$ " + salarioHoje.ToString("0.00");

        if (textoBonus != null)
            textoBonus.text = "Bônus: R$ " + bonusHoje.ToString("0.00");

        if (textoRendimentoDia != null)
            textoRendimentoDia.text = "Rendimento do dia: R$ " + rendimentoHoje.ToString("0.00");

        if (textoRendimentoTotal != null)
            textoRendimentoTotal.text = "Rendimento total: R$ " + rendimentoTotal.ToString("0.00");
    }
}