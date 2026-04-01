namespace Area27.Hub.Entities;

public enum UnidadeMedida
{
    Unidade,
    Quilograma,
    Litro,
    Metro,
    Caixa,
    Par,
    Duzia,
    Mililitro,
    Grama
}

// ── Fiscal ──────────────────────────────────────────────────────

/// <summary>Código de Situação Tributária do ICMS (CST — empresa tributada pelo Lucro Real/Presumido).</summary>
public enum CstIcms
{
    /// <summary>00 — Tributada integralmente.</summary>
    TributadaIntegralmente = 0,
    /// <summary>10 — Tributada e com cobrança do ICMS por substituição tributária.</summary>
    TributadaComST = 10,
    /// <summary>20 — Com redução de base de cálculo.</summary>
    ReducaoBase = 20,
    /// <summary>30 — Isenta ou não tributada e com cobrança do ICMS por ST.</summary>
    IsentaComST = 30,
    /// <summary>40 — Isenta.</summary>
    Isenta = 40,
    /// <summary>41 — Não tributada.</summary>
    NaoTributada = 41,
    /// <summary>50 — Suspensão.</summary>
    Suspensao = 50,
    /// <summary>51 — Diferimento.</summary>
    Diferimento = 51,
    /// <summary>60 — ICMS cobrado anteriormente por ST.</summary>
    CobradorAnteriormenteST = 60,
    /// <summary>70 — Com redução de base de cálculo e cobrança do ICMS por ST.</summary>
    ReducaoBaseComST = 70,
    /// <summary>90 — Outras.</summary>
    Outras = 90
}

/// <summary>CSOSN — Código de Situação da Operação no Simples Nacional.</summary>
public enum Csosn
{
    /// <summary>101 — Tributada pelo Simples Nacional com permissão de crédito.</summary>
    TributadaComCredito = 101,
    /// <summary>102 — Tributada pelo Simples Nacional sem permissão de crédito.</summary>
    TributadaSemCredito = 102,
    /// <summary>103 — Isenção do ICMS no Simples Nacional para faixa de receita bruta.</summary>
    IsentaFaixaReceita = 103,
    /// <summary>201 — Tributada pelo Simples Nacional com permissão de crédito e com cobrança do ICMS por ST.</summary>
    TributadaComCreditoST = 201,
    /// <summary>202 — Tributada pelo Simples Nacional sem permissão de crédito e com cobrança do ICMS por ST.</summary>
    TributadaSemCreditoST = 202,
    /// <summary>203 — Isenção do ICMS no Simples Nacional para faixa de receita bruta e com cobrança do ICMS por ST.</summary>
    IsentaComST = 203,
    /// <summary>300 — Imune.</summary>
    Imune = 300,
    /// <summary>400 — Não tributada pelo Simples Nacional.</summary>
    NaoTributada = 400,
    /// <summary>500 — ICMS cobrado anteriormente por ST ou por antecipação.</summary>
    CobradorAnteriormente = 500,
    /// <summary>900 — Outras.</summary>
    Outras = 900
}

/// <summary>CST de PIS e COFINS.</summary>
public enum CstPisCofins
{
    /// <summary>01 — Operação tributável à alíquota básica.</summary>
    TributavelAliquota = 1,
    /// <summary>02 — Operação tributável à alíquota diferenciada.</summary>
    TributavelDiferenciada = 2,
    /// <summary>03 — Operação tributável à alíquota por unidade de medida de produto.</summary>
    TributavelUnidade = 3,
    /// <summary>04 — Operação tributável monofásica — revenda à alíquota zero.</summary>
    Monofasica = 4,
    /// <summary>05 — Operação tributável por substituição tributária.</summary>
    SubstituicaoTributaria = 5,
    /// <summary>06 — Operação tributável a alíquota zero.</summary>
    AliquotaZero = 6,
    /// <summary>07 — Operação isenta da contribuição.</summary>
    Isenta = 7,
    /// <summary>08 — Operação sem incidência da contribuição.</summary>
    SemIncidencia = 8,
    /// <summary>09 — Operação com suspensão da contribuição.</summary>
    Suspensao = 9,
    /// <summary>49 — Outras operações de saída.</summary>
    OutrasSaida = 49,
    /// <summary>50 — Operação com direito a crédito — vinculada exclusivamente a receita tributada.</summary>
    CreditoReceitaTributada = 50,
    /// <summary>70 — Operação de aquisição sem direito a crédito.</summary>
    SemDireito = 70,
    /// <summary>98 — Outras operações de entrada.</summary>
    OutrasEntrada = 98,
    /// <summary>99 — Outras operações.</summary>
    Outras = 99
}

/// <summary>Tipo de operação fiscal na NF.</summary>
public enum TipoOperacao
{
    Entrada  = 0,
    Saida    = 1
}

/// <summary>Modalidade de frete.</summary>
public enum ModalidadeFrete
{
    /// <summary>0 — CIF (por conta do emitente).</summary>
    Cif = 0,
    /// <summary>1 — FOB (por conta do destinatário).</summary>
    Fob = 1,
    /// <summary>2 — Por conta de terceiros.</summary>
    Terceiros = 2,
    /// <summary>9 — Sem frete.</summary>
    SemFrete = 9
}

// ── Financeiro ──────────────────────────────────────────────────

/// <summary>Status de um documento financeiro.</summary>
public enum StatusFinanceiro
{
    Aberto,
    BaixadoParcial,
    Baixado,
    Cancelado,
    Renegociado,
    Prorrogado
}

/// <summary>Tipo de documento financeiro.</summary>
public enum TipoDocumentoFinanceiro
{
    Duplicata,
    Cheque,
    NotaPromissoria,
    Boleto,
    CartaoCredito,
    CartaoDebito,
    Pix,
    Dinheiro,
    Outro
}

/// <summary>Status de um cheque.</summary>
public enum StatusCheque
{
    Recebido,
    Compensado,
    Devolvido,
    Sustado,
    Cancelado
}

// ── Pedido ──────────────────────────────────────────────────────

/// <summary>Status de um pedido de venda ou compra.</summary>
public enum StatusPedido
{
    Digitado,
    Aprovado,
    EmSeparacao,
    Faturado,
    Cancelado,
    Suspenso
}

/// <summary>Tipo de pedido.</summary>
public enum TipoPedido
{
    Venda,
    Compra,
    Transferencia,
    Devolucao
}

// ── MDF-e ────────────────────────────────────────────────────────

/// <summary>Status de um MDF-e (Manifesto de Documentos Fiscais Eletrônicos).</summary>
public enum StatusMdfe
{
    Digitado,
    Gerado,
    Transmitido,
    Autorizado,
    Cancelado,
    Encerrado,
    Rejeitado
}
