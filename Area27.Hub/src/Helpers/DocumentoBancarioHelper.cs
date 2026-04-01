using System;
using System.Text.RegularExpressions;

namespace Area27.Hub.Helpers;

/// <summary>
/// Utilitários para documentos bancários: cheques, borderô e compensação.
/// Replica as regras de negócio do módulo BanDoc.vw do sistema DataFlex Integra.
/// </summary>
public static class DocumentoBancarioHelper
{
    // ────────────────────────────────────────────────────────────────
    // Cheque
    // ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Valida os dados básicos de um cheque: agência, conta e número.
    /// </summary>
    /// <param name="agencia">Número da agência (apenas dígitos).</param>
    /// <param name="conta">Número da conta (apenas dígitos).</param>
    /// <param name="numeroCheque">Número do cheque.</param>
    /// <returns><see langword="true"/> quando todos os campos estão preenchidos corretamente.</returns>
    public static bool ValidarCheque(string? agencia, string? conta, string? numeroCheque)
    {
        if (string.IsNullOrWhiteSpace(agencia)     || !Regex.IsMatch(agencia,     @"^\d+$")) return false;
        if (string.IsNullOrWhiteSpace(conta)        || !Regex.IsMatch(conta,       @"^\d+$")) return false;
        if (string.IsNullOrWhiteSpace(numeroCheque) || !Regex.IsMatch(numeroCheque, @"^\d+$")) return false;
        return true;
    }

    /// <summary>
    /// Calcula os dias corridos até o vencimento de um cheque pré-datado.
    /// </summary>
    /// <param name="dataEmissao">Data de emissão do cheque.</param>
    /// <param name="dataVencimento">Data de vencimento (bom para).</param>
    /// <returns>Número de dias úteis/corridos até o vencimento. Pode ser negativo se vencido.</returns>
    public static int DiasAteVencimento(DateTime dataEmissao, DateTime dataVencimento)
    {
        return (dataVencimento.Date - dataEmissao.Date).Days;
    }

    /// <summary>
    /// Formata um cheque no padrão de exibição: "Banco XXXX — Ag. YYYY CC ZZZZZZ".
    /// </summary>
    public static string FormatarIdentificacaoCheque(
        string codigoBanco,
        string agencia,
        string conta,
        string numeroCheque)
    {
        return $"Banco {codigoBanco.PadLeft(3, '0')} — Ag. {agencia} CC {conta} N° {numeroCheque}";
    }

    // ────────────────────────────────────────────────────────────────
    // Borderô (agrupamento de documentos para cobrança/desconto)
    // ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Calcula o valor líquido do borderô após descontos bancários.
    /// </summary>
    /// <param name="valorBruto">Soma dos valores dos documentos do borderô.</param>
    /// <param name="taxaDesconto">Taxa de desconto cobrada pelo banco em percentual.</param>
    /// <param name="tarifas">Valor das tarifas e despesas bancárias.</param>
    /// <returns>Valor líquido a receber/pagar após encargos.</returns>
    public static decimal CalcularLiquidoBordero(
        decimal valorBruto,
        decimal taxaDesconto,
        decimal tarifas = 0)
    {
        var desconto = Math.Round(valorBruto * taxaDesconto / 100, 2);
        return Math.Round(valorBruto - desconto - tarifas, 2);
    }

    // ────────────────────────────────────────────────────────────────
    // Boleto / Linha Digitável (FEBRABAN)
    // ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Valida se uma linha digitável de boleto bancário (47 dígitos) tem formato válido.
    /// Não verifica o dígito verificador completo — apenas estrutura e comprimento.
    /// </summary>
    /// <param name="linhaDigitavel">Linha digitável com ou sem pontos e espaços.</param>
    /// <returns><see langword="true"/> quando a linha digitável tem 47 dígitos numéricos.</returns>
    public static bool ValidarLinhaDigitavel(string? linhaDigitavel)
    {
        if (string.IsNullOrWhiteSpace(linhaDigitavel)) return false;
        var digits = Regex.Replace(linhaDigitavel, @"\D", "");
        return digits.Length == 47 || digits.Length == 48; // 47 boleto bancário, 48 convênio
    }

    /// <summary>
    /// Extrai a data de vencimento embutida nos campos 6–9 do código de barras de um boleto.
    /// </summary>
    /// <param name="codigoBarras44">Código de barras com 44 dígitos.</param>
    /// <returns>Data de vencimento ou <see langword="null"/> quando o fator for 0000 (sem vencimento fixo).</returns>
    public static DateTime? ExtrairVencimentoBoleto(string codigoBarras44)
    {
        var digits = Regex.Replace(codigoBarras44, @"\D", "");
        if (digits.Length != 44)
            throw new ArgumentException("Código de barras deve ter 44 dígitos.", nameof(codigoBarras44));

        var fator = digits.Substring(5, 4);
        if (fator == "0000") return null;

        // Fator de vencimento: dias corridos a partir de 07/10/1997
        var dataBase = new DateTime(1997, 10, 7);
        return dataBase.AddDays(int.Parse(fator));
    }

    /// <summary>
    /// Extrai o valor nominal embutido nos campos 10–19 do código de barras de um boleto.
    /// </summary>
    /// <param name="codigoBarras44">Código de barras com 44 dígitos.</param>
    /// <returns>Valor nominal do boleto.</returns>
    public static decimal ExtrairValorBoleto(string codigoBarras44)
    {
        var digits = Regex.Replace(codigoBarras44, @"\D", "");
        if (digits.Length != 44)
            throw new ArgumentException("Código de barras deve ter 44 dígitos.", nameof(codigoBarras44));

        var valorStr = digits.Substring(9, 10);
        return decimal.Parse(valorStr) / 100m;
    }

    // ────────────────────────────────────────────────────────────────
    // Compensação / Devolução
    // ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Determina se uma data de compensação é válida (dia útil bancário).
    /// Exclui sábados e domingos. Feriados devem ser validados externamente.
    /// </summary>
    /// <param name="data">Data a validar.</param>
    /// <returns><see langword="true"/> quando a data é dia útil (seg–sex).</returns>
    public static bool IsDiaUtilBancario(DateTime data)
    {
        return data.DayOfWeek != DayOfWeek.Saturday && data.DayOfWeek != DayOfWeek.Sunday;
    }

    /// <summary>
    /// Obtém o próximo dia útil bancário a partir de uma data.
    /// </summary>
    /// <param name="data">Data de referência.</param>
    /// <returns>Próximo dia útil (seg–sex).</returns>
    public static DateTime ProximoDiaUtil(DateTime data)
    {
        var proximo = data.AddDays(1);
        while (!IsDiaUtilBancario(proximo))
            proximo = proximo.AddDays(1);
        return proximo;
    }
}
