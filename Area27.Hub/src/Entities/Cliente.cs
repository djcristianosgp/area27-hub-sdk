namespace Area27.Hub.Entities;

public enum TipoCliente
{
    PessoaFisica,
    PessoaJuridica
}

public enum StatusCliente
{
    Ativo,
    Inativo,
    Bloqueado
}

public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    /// <summary>CPF (PF) ou CNPJ (PJ) sem formatação.</summary>
    public string Documento { get; set; } = string.Empty;
    public TipoCliente Tipo { get; set; }
    public StatusCliente Status { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Celular { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string UF { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    /// <summary>Inscrição Estadual (PJ).</summary>
    public string InscricaoEstadual { get; set; } = string.Empty;
    /// <summary>Limite de crédito aprovado.</summary>
    public decimal LimiteCredito { get; set; }
    /// <summary>Tabela de preço vinculada ao cliente.</summary>
    public int TabelaPrecoId { get; set; }
    /// <summary>Vendedor responsável pelo cliente.</summary>
    public int VendedorId { get; set; }
    public DateTime? DataNascimento { get; set; }
    public DateTime DataCadastro { get; set; } = DateTime.Today;
    public string Observacao { get; set; } = string.Empty;
}

/// <summary>Representa um fornecedor de produtos ou serviços.</summary>
public class Fornecedor
{
    public int Id { get; set; }
    /// <summary>Razão social.</summary>
    public string RazaoSocial { get; set; } = string.Empty;
    /// <summary>Nome fantasia.</summary>
    public string NomeFantasia { get; set; } = string.Empty;
    /// <summary>CNPJ sem formatação (14 dígitos).</summary>
    public string Cnpj { get; set; } = string.Empty;
    public string InscricaoEstadual { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string UF { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    /// <summary>Banco preferencial para pagamentos.</summary>
    public string BancoCodigo { get; set; } = string.Empty;
    public string BancoAgencia { get; set; } = string.Empty;
    public string BancoConta { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public string Observacao { get; set; } = string.Empty;
}