using System;
using System.Collections.Generic;

namespace Area27.Hub.Helpers;

/// <summary>
/// Cálculos financeiros: geração de parcelas, juros, descontos e acréscimos.
/// Replica as regras de negócio dos módulos BaxFin, BaxCdc e CadPed do sistema DataFlex Integra.
/// </summary>
public static class FinanceiroHelper
{
    // ────────────────────────────────────────────────────────────────
    // Geração de Parcelas (ForPar — DataFlex)
    // ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Gera parcelas com vencimentos. A última parcela absorve diferença de arredondamento.
    /// </summary>
    /// <param name="valorTotal">Valor total a parcelar.</param>
    /// <param name="quantidadeParcelas">Quantidade de parcelas.</param>
    /// <param name="primeiroVencimento">Data de vencimento da primeira parcela.</param>
    /// <param name="intervaloDias">Número de dias entre parcelas (padrão: 30).</param>
    /// <returns>Lista de parcelas geradas.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Quando quantidade de parcelas &lt; 1.</exception>
    public static List<ParcelaItem> GerarParcelas(
        decimal valorTotal,
        int quantidadeParcelas,
        DateTime primeiroVencimento,
        int intervaloDias = 30)
    {
        if (quantidadeParcelas < 1)
            throw new ArgumentOutOfRangeException(nameof(quantidadeParcelas), "Deve haver pelo menos 1 parcela.");

        var valorParcela = Math.Round(valorTotal / quantidadeParcelas, 2);
        var parcelas     = new List<ParcelaItem>(quantidadeParcelas);
        var somaAcum     = 0m;

        for (int i = 1; i <= quantidadeParcelas; i++)
        {
            var valor      = i < quantidadeParcelas ? valorParcela : Math.Round(valorTotal - somaAcum, 2);
            var vencimento = primeiroVencimento.AddDays(intervaloDias * (i - 1));

            parcelas.Add(new ParcelaItem
            {
                Numero     = i,
                Valor      = valor,
                Vencimento = vencimento,
                Status     = StatusParcela.Aberta
            });

            somaAcum += valor;
        }

        return parcelas;
    }

    /// <summary>
    /// Gera parcelas com vencimentos mensais na mesma data (ciclo mensal).
    /// </summary>
    /// <param name="valorTotal">Valor total a parcelar.</param>
    /// <param name="quantidadeParcelas">Quantidade de parcelas.</param>
    /// <param name="primeiroVencimento">Data do primeiro vencimento.</param>
    /// <returns>Lista de parcelas com vencimentos mensais.</returns>
    public static List<ParcelaItem> GerarParcelasMensais(
        decimal valorTotal,
        int quantidadeParcelas,
        DateTime primeiroVencimento)
    {
        if (quantidadeParcelas < 1)
            throw new ArgumentOutOfRangeException(nameof(quantidadeParcelas), "Deve haver pelo menos 1 parcela.");

        var valorParcela = Math.Round(valorTotal / quantidadeParcelas, 2);
        var parcelas     = new List<ParcelaItem>(quantidadeParcelas);
        var somaAcum     = 0m;

        for (int i = 1; i <= quantidadeParcelas; i++)
        {
            var valor      = i < quantidadeParcelas ? valorParcela : Math.Round(valorTotal - somaAcum, 2);
            var vencimento = primeiroVencimento.AddMonths(i - 1);

            parcelas.Add(new ParcelaItem
            {
                Numero     = i,
                Valor      = valor,
                Vencimento = vencimento,
                Status     = StatusParcela.Aberta
            });

            somaAcum += valor;
        }

        return parcelas;
    }

    // ────────────────────────────────────────────────────────────────
    // Juros
    // ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Calcula juros simples.
    /// Juros = Principal × Taxa × Dias / 30
    /// </summary>
    /// <param name="principal">Valor principal.</param>
    /// <param name="taxaMensalPercent">Taxa de juros mensal em percentual.</param>
    /// <param name="dias">Número de dias de atraso/período.</param>
    /// <returns>Valor dos juros simples.</returns>
    public static decimal CalcularJurosSimples(decimal principal, decimal taxaMensalPercent, int dias)
    {
        return Math.Round(principal * taxaMensalPercent / 100 * dias / 30, 2);
    }

    /// <summary>
    /// Calcula juros compostos por dias (taxa ao mês).
    /// Montante = Principal × (1 + Taxa)^(Dias/30)
    /// </summary>
    /// <param name="principal">Valor principal.</param>
    /// <param name="taxaMensalPercent">Taxa de juros mensal em percentual.</param>
    /// <param name="dias">Número de dias do período.</param>
    /// <returns>Valor dos juros compostos.</returns>
    public static decimal CalcularJurosCompostos(decimal principal, decimal taxaMensalPercent, int dias)
    {
        var fator    = Math.Pow(1 + (double)taxaMensalPercent / 100, (double)dias / 30);
        var montante = Math.Round(principal * (decimal)fator, 2);
        return montante - principal;
    }

    /// <summary>
    /// Calcula multa por atraso (valor fixo em percentual sobre o principal).
    /// </summary>
    /// <param name="principal">Valor da parcela em atraso.</param>
    /// <param name="percentualMulta">Percentual de multa (ex: 2 para 2%).</param>
    /// <returns>Valor da multa.</returns>
    public static decimal CalcularMulta(decimal principal, decimal percentualMulta)
    {
        return Math.Round(principal * percentualMulta / 100, 2);
    }

    /// <summary>
    /// Calcula o total de encargos (multa + juros simples) para uma parcela em atraso.
    /// </summary>
    /// <param name="valorParcela">Valor da parcela.</param>
    /// <param name="diasAtraso">Dias de atraso.</param>
    /// <param name="taxaJurosMensalPercent">Taxa de juros mensal em percentual.</param>
    /// <param name="percentualMulta">Percentual de multa por atraso.</param>
    /// <returns>Valor total dos encargos.</returns>
    public static decimal CalcularEncargosAtraso(
        decimal valorParcela,
        int diasAtraso,
        decimal taxaJurosMensalPercent,
        decimal percentualMulta)
    {
        if (diasAtraso <= 0) return 0;
        var multa  = CalcularMulta(valorParcela, percentualMulta);
        var juros  = CalcularJurosSimples(valorParcela, taxaJurosMensalPercent, diasAtraso);
        return Math.Round(multa + juros, 2);
    }

    // ────────────────────────────────────────────────────────────────
    // Desconto / Acréscimo
    // ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Aplica desconto percentual sobre um valor.
    /// </summary>
    public static decimal AplicarDesconto(decimal valor, decimal percentualDesconto)
    {
        return Math.Round(valor * (1 - percentualDesconto / 100), 2);
    }

    /// <summary>
    /// Aplica acréscimo percentual sobre um valor.
    /// </summary>
    public static decimal AplicarAcrescimo(decimal valor, decimal percentualAcrescimo)
    {
        return Math.Round(valor * (1 + percentualAcrescimo / 100), 2);
    }

    // ────────────────────────────────────────────────────────────────
    // Prorrogação de Vencimento
    // ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Prorroga o vencimento de todas as parcelas em aberto por um número de dias.
    /// </summary>
    /// <param name="parcelas">Lista de parcelas.</param>
    /// <param name="dias">Quantidade de dias a prorrogar.</param>
    public static void ProrrogarVencimentos(IEnumerable<ParcelaItem> parcelas, int dias)
    {
        foreach (var p in parcelas)
        {
            if (p.Status == StatusParcela.Aberta || p.Status == StatusParcela.Vencida)
                p.Vencimento = p.Vencimento.AddDays(dias);
        }
    }
}

/// <summary>
/// Representa uma parcela gerada pelo <see cref="FinanceiroHelper"/>.
/// </summary>
public class ParcelaItem
{
    public int           Numero     { get; set; }
    public decimal       Valor      { get; set; }
    public DateTime      Vencimento { get; set; }
    public StatusParcela Status     { get; set; }
    public decimal       ValorPago  { get; set; }
    public DateTime?     DataPagamento { get; set; }
}

/// <summary>Status de uma parcela.</summary>
public enum StatusParcela
{
    Aberta,
    Paga,
    Vencida,
    Cancelada,
    Prorrogada
}
