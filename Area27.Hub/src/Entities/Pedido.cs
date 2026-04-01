namespace Area27.Hub.Entities;

/// <summary>
/// Representa um pedido de venda ou compra.
/// Baseado nos módulos CadPed.vw e CadPed1.VW do sistema DataFlex Integra.
/// </summary>
public class Pedido
{
    public int Id { get; set; }
    public TipoPedido Tipo { get; set; }
    public StatusPedido Status { get; set; }

    /// <summary>Número do pedido (para filiais administradas, ≥ 500.000 no Integra).</summary>
    public int Numero { get; set; }

    public DateTime DataEmissao { get; set; } = DateTime.Today;
    public DateTime? DataEntrega { get; set; }

    public int ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;

    public int VendedorId { get; set; }
    public string VendedorNome { get; set; } = string.Empty;

    public int FilialId { get; set; }

    // ── Valores ─────────────────────────────────────────────────────
    public decimal SubtotalProdutos { get; set; }
    public decimal TotalDesconto    { get; set; }
    public decimal TotalFrete       { get; set; }
    public decimal TotalIpi         { get; set; }
    public decimal TotalIcmsSt      { get; set; }
    public decimal TotalNota        { get; set; }

    // ── Frete ────────────────────────────────────────────────────────
    public ModalidadeFrete ModalidadeFrete { get; set; } = ModalidadeFrete.Cif;
    public int? TransportadoraId { get; set; }

    // ── Itens e Parcelas ─────────────────────────────────────────────
    public List<ItemPedido> Itens { get; set; } = new();
    public List<ParcelaPedido> Parcelas { get; set; } = new();

    // ── Auditoria (replicado de CadPed.vw) ──────────────────────────
    public string UsuarioInclusao { get; set; } = string.Empty;
    public DateTime? DataInclusao { get; set; }
    public string UsuarioAlteracao { get; set; } = string.Empty;
    public DateTime? DataAlteracao { get; set; }

    public string Observacao { get; set; } = string.Empty;
}

/// <summary>
/// Item de um pedido de venda ou compra.
/// Baseado nas colunas de CadPed1.VW (Matrícula, Referência, Grade, Qtd, Vr Unitário, Vr Total).
/// </summary>
public class ItemPedido
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int Numero { get; set; }

    public int ProdutoId { get; set; }
    public string ProdutoCodigo { get; set; } = string.Empty;
    public string ProdutoNome { get; set; } = string.Empty;
    public string Referencia { get; set; } = string.Empty;
    /// <summary>Grade (cor/tamanho/variação).</summary>
    public string Grade { get; set; } = string.Empty;

    public decimal Quantidade { get; set; }
    public UnidadeMedida Unidade { get; set; }

    public decimal ValorUnitario { get; set; }
    public decimal PercentualDesconto { get; set; }
    public decimal ValorDesconto => Math.Round(ValorUnitario * Quantidade * PercentualDesconto / 100, 2);
    public decimal ValorTotal    => Math.Round(ValorUnitario * Quantidade - ValorDesconto, 2);

    // ── Tributos do item ──────────────────────────────────────────────
    public decimal AliquotaIcms  { get; set; }
    public decimal ValorIcms     { get; set; }
    public decimal AliquotaIpi   { get; set; }
    public decimal ValorIpi      { get; set; }
    public decimal AliquotaPis   { get; set; }
    public decimal ValorPis      { get; set; }
    public decimal AliquotaCofins { get; set; }
    public decimal ValorCofins   { get; set; }
    public decimal ValorIcmsSt   { get; set; }
    public decimal MvaPadrao     { get; set; }

    public string Cfop { get; set; } = string.Empty;
    public CstIcms CstIcms { get; set; }
}

/// <summary>
/// Parcela gerada diretamente no pedido (ForPar — DataFlex).
/// Difere de <see cref="Area27.Hub.Helpers.ParcelaItem"/> por ter chave do pedido.
/// </summary>
public class ParcelaPedido
{
    public int Numero { get; set; }
    public DateTime Vencimento { get; set; }
    public decimal Valor { get; set; }
    public TipoDocumentoFinanceiro TipoDocumento { get; set; } = TipoDocumentoFinanceiro.Boleto;
    public StatusFinanceiro Status { get; set; } = StatusFinanceiro.Aberto;
}
