using System;
using System.Collections.Generic;

namespace Area27.Hub.Helpers;

/// <summary>
/// Cálculos de custo, markup, lucro e preço de venda.
/// Replica as regras de negócio dos módulos Custo.dg, AnaliseLucroMarkup.dg
/// e CalculaPrecoVenda.dg do sistema DataFlex Integra.
/// </summary>
public static class CustoHelper
{
    // ────────────────────────────────────────────────────────────────
    // Markup e Lucro
    // ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Calcula o percentual de markup sobre o custo.
    /// Markup = (Preço de Venda - Custo) / Custo × 100
    /// </summary>
    /// <param name="precoVenda">Preço de venda do produto.</param>
    /// <param name="custo">Custo do produto (CustoZero).</param>
    /// <returns>Markup em percentual. Retorna 0 quando custo = 0.</returns>
    public static decimal CalcularMarkup(decimal precoVenda, decimal custo)
    {
        if (custo == 0) return 0;
        return Math.Round((precoVenda - custo) / custo * 100, 2);
    }

    /// <summary>
    /// Calcula o percentual de lucro (margem sobre o preço de venda).
    /// Lucro = (1 - Custo / Preço de Venda) × 100
    /// </summary>
    /// <param name="precoVenda">Preço de venda do produto.</param>
    /// <param name="custo">Custo do produto.</param>
    /// <returns>Margem de lucro em percentual. Retorna 0 quando preço de venda = 0.</returns>
    public static decimal CalcularLucro(decimal precoVenda, decimal custo)
    {
        if (precoVenda == 0) return 0;
        return Math.Round((1 - custo / precoVenda) * 100, 2);
    }

    /// <summary>
    /// Calcula o preço de venda a partir do custo e do markup desejado.
    /// Preço de Venda = Custo × (1 + Markup / 100)
    /// </summary>
    /// <param name="custo">Custo do produto.</param>
    /// <param name="markupPercent">Markup desejado em percentual.</param>
    /// <returns>Preço de venda sugerido.</returns>
    public static decimal CalcularPrecoVendaMarkup(decimal custo, decimal markupPercent)
    {
        return Math.Round(custo * (1 + markupPercent / 100), 2);
    }

    /// <summary>
    /// Calcula o preço de venda a partir do custo e da margem de lucro desejada.
    /// Preço de Venda = Custo / (1 - Margem / 100)
    /// </summary>
    /// <param name="custo">Custo do produto.</param>
    /// <param name="margemPercent">Margem de lucro desejada em percentual.</param>
    /// <returns>Preço de venda sugerido.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Quando margem ≥ 100%.</exception>
    public static decimal CalcularPrecoVendaMargem(decimal custo, decimal margemPercent)
    {
        if (margemPercent >= 100)
            throw new ArgumentOutOfRangeException(nameof(margemPercent), "Margem de lucro deve ser menor que 100%.");
        return Math.Round(custo / (1 - margemPercent / 100), 2);
    }

    // ────────────────────────────────────────────────────────────────
    // Composição de Custo (baseado em Custo.dg)
    // ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Calcula o custo total de um produto considerando frete, IPI, ICMS e despesas acessórias.
    /// </summary>
    /// <param name="valorCompra">Valor de compra do produto na NF.</param>
    /// <param name="desconto">Desconto comercial em valor.</param>
    /// <param name="ipiValor">Valor do IPI.</param>
    /// <param name="freteValor">Valor do frete (CIF).</param>
    /// <param name="freteTransporte">Frete de transporte complementar.</param>
    /// <param name="seguro">Valor do seguro.</param>
    /// <param name="descarga">Custo de descarga.</param>
    /// <param name="outrasDespesas">Outras despesas acessórias.</param>
    /// <param name="icmsCredito">Crédito de ICMS recuperável (abate no custo).</param>
    /// <param name="icmsFreteCredito">Crédito de ICMS sobre frete.</param>
    /// <param name="pisCredito">Crédito de PIS recuperável.</param>
    /// <param name="cofinsCredito">Crédito de COFINS recuperável.</param>
    /// <param name="icmsStValor">Valor do ICMS-ST (custo adicional).</param>
    /// <returns>Custo total do produto disponível para formação de preço.</returns>
    public static decimal CalcularCustoTotal(
        decimal valorCompra,
        decimal desconto = 0,
        decimal ipiValor = 0,
        decimal freteValor = 0,
        decimal freteTransporte = 0,
        decimal seguro = 0,
        decimal descarga = 0,
        decimal outrasDespesas = 0,
        decimal icmsCredito = 0,
        decimal icmsFreteCredito = 0,
        decimal pisCredito = 0,
        decimal cofinsCredito = 0,
        decimal icmsStValor = 0)
    {
        var custo = valorCompra
                    - desconto
                    + ipiValor
                    + freteValor
                    + freteTransporte
                    + seguro
                    + descarga
                    + outrasDespesas
                    - icmsCredito
                    - icmsFreteCredito
                    - pisCredito
                    - cofinsCredito
                    + icmsStValor;

        return Math.Round(custo, 4);
    }

    // ────────────────────────────────────────────────────────────────
    // Análise de Lucro por Item (baseado em AnaliseLucroMarkup.dg)
    // ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Realiza a análise de lucro e markup de um item de venda considerando descontos.
    /// </summary>
    /// <param name="custoZero">Custo do produto (CustoZero).</param>
    /// <param name="valorVenda">Preço de venda cheio.</param>
    /// <param name="percentualDesconto">Percentual de desconto concedido na venda.</param>
    /// <returns>Resultado da análise com valores líquidos e percentuais.</returns>
    public static AnaliseLucroResult AnalisarLucro(
        decimal custoZero,
        decimal valorVenda,
        decimal percentualDesconto = 0)
    {
        var desconto     = Math.Round(valorVenda * percentualDesconto / 100, 2);
        var valorLiquido = Math.Round(valorVenda - desconto, 2);
        var lucro        = custoZero == 0 ? 0 : CalcularLucro(valorLiquido, custoZero);
        var markup       = custoZero == 0 ? 0 : CalcularMarkup(valorLiquido, custoZero);

        return new AnaliseLucroResult
        {
            CustoZero         = custoZero,
            ValorVenda        = valorVenda,
            PercentualDesconto = percentualDesconto,
            ValorDesconto     = desconto,
            ValorLiquido      = valorLiquido,
            PercentualLucro   = lucro,
            PercentualMarkup  = markup,
            CustoZeroInvalido = custoZero == 0
        };
    }

    // ────────────────────────────────────────────────────────────────
    // Tabela de Preços (múltiplas margens)
    // ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Gera uma tabela de preços de venda para várias margens de markup.
    /// </summary>
    /// <param name="custo">Custo do produto.</param>
    /// <param name="margens">Lista de percentuais de markup a calcular.</param>
    /// <returns>Dicionário com markup → preço de venda sugerido.</returns>
    public static Dictionary<decimal, decimal> GerarTabelaPrecos(decimal custo, IEnumerable<decimal> margens)
    {
        var tabela = new Dictionary<decimal, decimal>();
        foreach (var margem in margens)
            tabela[margem] = CalcularPrecoVendaMarkup(custo, margem);
        return tabela;
    }
}

/// <summary>
/// Resultado da análise de lucro e markup de um item de venda.
/// </summary>
public class AnaliseLucroResult
{
    public decimal CustoZero          { get; init; }
    public decimal ValorVenda         { get; init; }
    public decimal PercentualDesconto { get; init; }
    public decimal ValorDesconto      { get; init; }
    public decimal ValorLiquido       { get; init; }
    public decimal PercentualLucro    { get; init; }
    public decimal PercentualMarkup   { get; init; }
    /// <summary>Alerta quando CustoZero não foi informado, tornando os percentuais inválidos.</summary>
    public bool CustoZeroInvalido     { get; init; }
}
