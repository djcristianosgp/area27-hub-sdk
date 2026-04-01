namespace Area27.Hub.Entities;

public enum TipoImposto
{
    ICMS,
    ICMS_ST,
    IPI,
    PIS,
    COFINS,
    ISS,
    IBS,
    CBS
}

public class Imposto
{
    public TipoImposto Tipo { get; set; }
    public decimal Aliquota { get; set; }
    /// <summary>Base de cálculo utilizada.</summary>
    public decimal BaseCalculo { get; set; }
    /// <summary>Valor calculado do imposto.</summary>
    public decimal Valor { get; set; }
    public string Observacao { get; set; } = string.Empty;
}