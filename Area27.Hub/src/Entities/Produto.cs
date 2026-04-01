namespace Area27.Hub.Entities;

public enum TipoProduto
{
    Mercadoria,
    Servico,
    MateriaPrima,
    Embalagem,
    Combustivel
}

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string CodigoBarras { get; set; } = string.Empty;
    public string Referencia { get; set; } = string.Empty;
    /// <summary>NCM — Nomenclatura Comum do Mercosul (8 dígitos).</summary>
    public string Ncm { get; set; } = string.Empty;
    /// <summary>CEST — Código Especificador da Substituição Tributária.</summary>
    public string Cest { get; set; } = string.Empty;
    public TipoProduto Tipo { get; set; }
    public UnidadeMedida Unidade { get; set; }
    /// <summary>Preço de venda ao consumidor final.</summary>
    public decimal PrecoVenda { get; set; }
    /// <summary>Custo de compra (CustoZero).</summary>
    public decimal CustoZero { get; set; }
    /// <summary>Estoque atual.</summary>
    public decimal EstoqueAtual { get; set; }
    /// <summary>Estoque mínimo para alerta de ressuprimento.</summary>
    public decimal EstoqueMinimo { get; set; }
    public int FornecedorId { get; set; }
    public bool Ativo { get; set; } = true;
    // ── Tributação ────────────────────────────────────────────────
    public CstIcms CstIcms { get; set; }
    public decimal AliquotaIcms { get; set; }
    public decimal AliquotaIpi  { get; set; }
    public CstPisCofins CstPis    { get; set; }
    public CstPisCofins CstCofins { get; set; }
    public decimal AliquotaPis    { get; set; }
    public decimal AliquotaCofins { get; set; }
    /// <summary>MVA Padrão para cálculo do ICMS-ST.</summary>
    public decimal MvaPadrao { get; set; }
}