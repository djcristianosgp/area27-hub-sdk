namespace Area27.Hub.Entities;

/// <summary>
/// Representa um cheque recebido ou emitido.
/// Baseado nos campos de BanDoc.vw / CadChe do sistema DataFlex Integra.
/// </summary>
public class Cheque
{
    public int Id { get; set; }
    public StatusCheque Status { get; set; } = StatusCheque.Recebido;

    // ── Dados do banco ────────────────────────────────────────────────
    /// <summary>Código do banco (3 dígitos, ex: "001" Banco do Brasil).</summary>
    public string BancoCodigo  { get; set; } = string.Empty;
    public string BancoNome    { get; set; } = string.Empty;
    public string Agencia      { get; set; } = string.Empty;
    /// <summary>Praça da agência (cidade).</summary>
    public string Praca        { get; set; } = string.Empty;
    public string Conta        { get; set; } = string.Empty;
    public string NumeroCheque { get; set; } = string.Empty;

    // ── Dados do emitente ─────────────────────────────────────────────
    public string Emitente  { get; set; } = string.Empty;
    public string Nominal   { get; set; } = string.Empty;
    /// <summary>CPF ou CNPJ do emitente.</summary>
    public string Documento { get; set; } = string.Empty;

    // ── Valores e datas ───────────────────────────────────────────────
    public decimal Valor         { get; set; }
    public DateTime DataEmissao  { get; set; } = DateTime.Today;
    /// <summary>Data "bom para" (cheque pré-datado).</summary>
    public DateTime DataVencimento { get; set; }

    // ── Rastreabilidade ───────────────────────────────────────────────
    public int? ClienteId     { get; set; }
    public int? FornecedorId  { get; set; }
    public int? BorderoId     { get; set; }
    public int FilialId       { get; set; }

    public string Observacao { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; } = DateTime.Now;
}

/// <summary>
/// Borderô bancário — agrupamento de cheques ou títulos para cobrança, desconto ou repasse.
/// Baseado em BanDoc.vw do sistema DataFlex Integra.
/// </summary>
public class Bordero
{
    public int Id { get; set; }
    public DateTime DataEmissao { get; set; } = DateTime.Today;
    public int BancoId    { get; set; }
    public string BancoCodigo { get; set; } = string.Empty;
    public string Agencia { get; set; } = string.Empty;
    public string Conta   { get; set; } = string.Empty;
    public int FilialId   { get; set; }

    public decimal ValorBruto  { get; set; }
    public decimal TaxaDesconto { get; set; }
    public decimal Tarifas      { get; set; }
    public decimal ValorLiquido => Math.Round(ValorBruto * (1 - TaxaDesconto / 100) - Tarifas, 2);

    public List<Cheque> Cheques { get; set; } = new();
    public string Observacao { get; set; } = string.Empty;
}
