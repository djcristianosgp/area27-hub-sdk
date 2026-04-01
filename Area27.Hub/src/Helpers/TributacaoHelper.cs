using System;

namespace Area27.Hub.Helpers;

/// <summary>
/// Cálculos tributários brasileiros: ICMS, IPI, PIS/COFINS, ICMS-ST/MVA.
/// Replica as regras de negócio do módulo fiscal do sistema DataFlex Integra.
/// </summary>
public static class TributacaoHelper
{
    // ────────────────────────────────────────────────────────────────
    // ICMS
    // ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Calcula o valor do ICMS sobre uma base de cálculo.
    /// </summary>
    /// <param name="baseCalculo">Base de cálculo do ICMS.</param>
    /// <param name="aliquota">Alíquota em percentual (ex: 12 para 12%).</param>
    /// <returns>Valor do ICMS.</returns>
    public static decimal CalcularIcms(decimal baseCalculo, decimal aliquota)
    {
        return Math.Round(baseCalculo * aliquota / 100, 2);
    }

    /// <summary>
    /// Calcula a base de cálculo do ICMS aplicando redução de base.
    /// </summary>
    /// <param name="valorProduto">Valor bruto do produto.</param>
    /// <param name="percentualReducao">Percentual de redução (ex: 33.33 para redução de 1/3).</param>
    /// <returns>Base de cálculo reduzida.</returns>
    public static decimal CalcularBaseIcmsReduzida(decimal valorProduto, decimal percentualReducao)
    {
        return Math.Round(valorProduto * (1 - percentualReducao / 100), 2);
    }

    /// <summary>
    /// Calcula o ICMS embutido no preço (por dentro).
    /// </summary>
    /// <param name="precoComIcms">Preço já com ICMS embutido.</param>
    /// <param name="aliquota">Alíquota em percentual.</param>
    /// <returns>Valor do ICMS embutido no preço.</returns>
    public static decimal CalcularIcmsPorDentro(decimal precoComIcms, decimal aliquota)
    {
        return Math.Round(precoComIcms * aliquota / 100, 2);
    }

    /// <summary>
    /// Calcula o preço bruto (sem ICMS) dado um preço desejado de venda.
    /// Útil para back-calculation: dado o preço líquido desejado, qual deve ser o preço de venda
    /// de forma que, após deduzir o ICMS, reste o valor desejado.
    /// </summary>
    /// <param name="precoLiquido">Preço líquido desejado (sem ICMS).</param>
    /// <param name="aliquota">Alíquota de ICMS em percentual.</param>
    /// <returns>Preço bruto de venda que comporta o ICMS.</returns>
    public static decimal CalcularPrecoBrutoComIcms(decimal precoLiquido, decimal aliquota)
    {
        if (aliquota >= 100)
            throw new ArgumentOutOfRangeException(nameof(aliquota), "Alíquota deve ser menor que 100%.");
        return Math.Round(precoLiquido / (1 - aliquota / 100), 2);
    }

    // ────────────────────────────────────────────────────────────────
    // ICMS-ST (Substituição Tributária)
    // ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Calcula a base de cálculo do ICMS-ST usando MVA (Margem de Valor Agregado).
    /// </summary>
    /// <param name="valorProduto">Valor do produto (base operação própria).</param>
    /// <param name="ipi">Valor do IPI da operação.</param>
    /// <param name="frete">Valor do frete CIF.</param>
    /// <param name="outrasDespesas">Outras despesas acessórias.</param>
    /// <param name="mvaPadrao">MVA Padrão em percentual (ex: 35 para 35%).</param>
    /// <returns>Base de cálculo do ICMS-ST.</returns>
    public static decimal CalcularBaseST(
        decimal valorProduto,
        decimal ipi,
        decimal frete,
        decimal outrasDespesas,
        decimal mvaPadrao)
    {
        var baseOrigem = valorProduto + ipi + frete + outrasDespesas;
        return Math.Round(baseOrigem * (1 + mvaPadrao / 100), 2);
    }

    /// <summary>
    /// Calcula o valor do ICMS-ST a recolher (retenção).
    /// </summary>
    /// <param name="baseSt">Base de cálculo do ICMS-ST.</param>
    /// <param name="aliquotaDestino">Alíquota interna do estado de destino em percentual.</param>
    /// <param name="icmsProprio">Valor do ICMS próprio já destacado na NF.</param>
    /// <returns>Valor do ICMS-ST a recolher.</returns>
    public static decimal CalcularIcmsST(decimal baseSt, decimal aliquotaDestino, decimal icmsProprio)
    {
        return Math.Round(baseSt * aliquotaDestino / 100 - icmsProprio, 2);
    }

    /// <summary>
    /// Calcula o MVA ajustado para operações interestaduais (convênio ICMS 35/2011).
    /// </summary>
    /// <param name="mvaPadrao">MVA padrão em percentual.</param>
    /// <param name="aliquotaInterna">Alíquota interna do estado destino em percentual.</param>
    /// <param name="aliquotaInterestadual">Alíquota interestadual aplicável (4%, 7% ou 12%).</param>
    /// <returns>MVA ajustado em percentual.</returns>
    public static decimal CalcularMvaAjustado(
        decimal mvaPadrao,
        decimal aliquotaInterna,
        decimal aliquotaInterestadual)
    {
        var mva = (1 + mvaPadrao / 100)
                  * (1 - aliquotaInterestadual / 100)
                  / (1 - aliquotaInterna / 100)
                  - 1;
        return Math.Round(mva * 100, 2);
    }

    // ────────────────────────────────────────────────────────────────
    // IPI
    // ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Calcula o valor do IPI sobre o valor do produto.
    /// </summary>
    /// <param name="valorProduto">Valor do produto na NF.</param>
    /// <param name="aliquota">Alíquota do IPI em percentual.</param>
    /// <returns>Valor do IPI.</returns>
    public static decimal CalcularIpi(decimal valorProduto, decimal aliquota)
    {
        return Math.Round(valorProduto * aliquota / 100, 2);
    }

    // ────────────────────────────────────────────────────────────────
    // PIS / COFINS
    // ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Calcula o valor do PIS sobre a base de cálculo.
    /// </summary>
    /// <param name="baseCalculo">Base de cálculo.</param>
    /// <param name="aliquota">Alíquota do PIS (ex: 0.65 para 0,65%).</param>
    /// <returns>Valor do PIS.</returns>
    public static decimal CalcularPis(decimal baseCalculo, decimal aliquota)
    {
        return Math.Round(baseCalculo * aliquota / 100, 2);
    }

    /// <summary>
    /// Calcula o valor do COFINS sobre a base de cálculo.
    /// </summary>
    /// <param name="baseCalculo">Base de cálculo.</param>
    /// <param name="aliquota">Alíquota do COFINS (ex: 3.0 para 3%).</param>
    /// <returns>Valor do COFINS.</returns>
    public static decimal CalcularCofins(decimal baseCalculo, decimal aliquota)
    {
        return Math.Round(baseCalculo * aliquota / 100, 2);
    }

    // ────────────────────────────────────────────────────────────────
    // Reforma Tributária — IBS / CBS
    // ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Calcula o valor do CBS (Contribuição sobre Bens e Serviços) — reforma tributária.
    /// Substitui PIS e COFINS a partir de 2026 (transição gradual).
    /// </summary>
    /// <param name="baseCalculo">Base de cálculo.</param>
    /// <param name="aliquota">Alíquota do CBS em percentual.</param>
    /// <returns>Valor do CBS.</returns>
    public static decimal CalcularCbs(decimal baseCalculo, decimal aliquota)
    {
        return Math.Round(baseCalculo * aliquota / 100, 2);
    }

    /// <summary>
    /// Calcula o valor do IBS (Imposto sobre Bens e Serviços) — reforma tributária.
    /// Substitui ICMS e ISS a partir de 2026 (transição gradual).
    /// </summary>
    /// <param name="baseCalculo">Base de cálculo.</param>
    /// <param name="aliquota">Alíquota do IBS em percentual (federal + estadual + municipal).</param>
    /// <returns>Valor do IBS.</returns>
    public static decimal CalcularIbs(decimal baseCalculo, decimal aliquota)
    {
        return Math.Round(baseCalculo * aliquota / 100, 2);
    }

    // ────────────────────────────────────────────────────────────────
    // Resumo de NF
    // ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Calcula o valor total da nota fiscal com todos os tributos.
    /// </summary>
    /// <param name="valorProdutos">Soma dos valores dos produtos.</param>
    /// <param name="valorIpi">Valor total do IPI.</param>
    /// <param name="valorIcmsSt">Valor total do ICMS-ST retido.</param>
    /// <param name="valorFrete">Valor do frete.</param>
    /// <param name="valorSeguro">Valor do seguro.</param>
    /// <param name="outrasDespesas">Outras despesas acessórias.</param>
    /// <param name="totalDesconto">Total de desconto sobre os produtos.</param>
    /// <returns>Valor total da nota fiscal.</returns>
    public static decimal CalcularTotalNF(
        decimal valorProdutos,
        decimal valorIpi,
        decimal valorIcmsSt,
        decimal valorFrete,
        decimal valorSeguro,
        decimal outrasDespesas,
        decimal totalDesconto)
    {
        return Math.Round(
            valorProdutos + valorIpi + valorIcmsSt + valorFrete + valorSeguro + outrasDespesas - totalDesconto,
            2);
    }
}
