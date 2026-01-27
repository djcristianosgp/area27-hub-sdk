using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Area27.Hub.Helpers;

/// <summary>
/// Provides utilities for CNPJ consultation using the public CNPJ API.
/// </summary>
public static class CnpjHelper
{
    private static readonly HttpClient HttpClient = new();
    private const string BaseUrl = "https://publica.cnpj.ws/cnpj/";

    /// <summary>
    /// Queries CNPJ data from the public API (publica.cnpj.ws).
    /// </summary>
    /// <param name="cnpj">CNPJ number with or without formatting.</param>
    /// <returns>CNPJ data object with company information.</returns>
    /// <exception cref="ArgumentNullException">Thrown when cnpj is null or empty.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="JsonException">Thrown when the response cannot be parsed.</exception>
    public static async Task<CnpjData?> ConsultarCnpjAsync(string? cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            throw new ArgumentNullException(nameof(cnpj), "CNPJ cannot be null or empty.");

        var cnpjLimpo = cnpj.Replace(".", "").Replace("/", "").Replace("-", "").Trim();

        if (cnpjLimpo.Length != 14)
            throw new ArgumentException("CNPJ must contain 14 digits.", nameof(cnpj));

        try
        {
            var response = await HttpClient.GetAsync($"{BaseUrl}{cnpjLimpo}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            return JsonSerializer.Deserialize<CnpjData>(json, options);
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException($"Failed to query CNPJ {cnpjLimpo} from API.", ex);
        }
        catch (JsonException ex)
        {
            throw new JsonException("Failed to parse CNPJ API response.", ex);
        }
    }

    /// <summary>
    /// Queries CNPJ data synchronously (wrapper for async method).
    /// </summary>
    /// <param name="cnpj">CNPJ number with or without formatting.</param>
    /// <returns>CNPJ data object with company information.</returns>
    /// <exception cref="ArgumentNullException">Thrown when cnpj is null or empty.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="JsonException">Thrown when the response cannot be parsed.</exception>
    public static CnpjData? ConsultarCnpj(string? cnpj)
    {
        return ConsultarCnpjAsync(cnpj).GetAwaiter().GetResult();
    }
}

/// <summary>
/// Represents CNPJ data returned from the public API.
/// </summary>
public class CnpjData
{
    /// <summary>Root CNPJ number (first 8 digits).</summary>
    [JsonPropertyName("cnpj_raiz")]
    public string? CnpjRaiz { get; set; }

    /// <summary>Company legal name.</summary>
    [JsonPropertyName("razao_social")]
    public string? RazaoSocial { get; set; }

    /// <summary>Share capital value.</summary>
    [JsonPropertyName("capital_social")]
    public string? CapitalSocial { get; set; }

    /// <summary>Federative responsible entity.</summary>
    [JsonPropertyName("responsavel_federativo")]
    public string? ResponsavelFederativo { get; set; }

    /// <summary>Last update timestamp.</summary>
    [JsonPropertyName("atualizado_em")]
    public DateTime? AtualizadoEm { get; set; }

    /// <summary>Company size classification.</summary>
    [JsonPropertyName("porte")]
    public Porte? Porte { get; set; }

    /// <summary>Legal nature of the company.</summary>
    [JsonPropertyName("natureza_juridica")]
    public NaturezaJuridica? NaturezaJuridica { get; set; }

    /// <summary>Qualification of the responsible person.</summary>
    [JsonPropertyName("qualificacao_do_responsavel")]
    public Qualificacao? QualificacaoDoResponsavel { get; set; }

    /// <summary>List of company partners/shareholders.</summary>
    [JsonPropertyName("socios")]
    public Socio[]? Socios { get; set; }

    /// <summary>Simples Nacional information.</summary>
    [JsonPropertyName("simples")]
    public Simples? Simples { get; set; }

    /// <summary>Main establishment data.</summary>
    [JsonPropertyName("estabelecimento")]
    public Estabelecimento? Estabelecimento { get; set; }
}

/// <summary>Company size classification.</summary>
public class Porte
{
    /// <summary>Size code.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Size description.</summary>
    [JsonPropertyName("descricao")]
    public string? Descricao { get; set; }
}

/// <summary>Legal nature information.</summary>
public class NaturezaJuridica
{
    /// <summary>Legal nature code.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Legal nature description.</summary>
    [JsonPropertyName("descricao")]
    public string? Descricao { get; set; }
}

/// <summary>Partner/shareholder qualification.</summary>
public class Qualificacao
{
    /// <summary>Qualification code.</summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>Qualification description.</summary>
    [JsonPropertyName("descricao")]
    public string? Descricao { get; set; }
}

/// <summary>Partner/shareholder information.</summary>
public class Socio
{
    /// <summary>Partner CPF or CNPJ (partially masked).</summary>
    [JsonPropertyName("cpf_cnpj_socio")]
    public string? CpfCnpjSocio { get; set; }

    /// <summary>Partner name.</summary>
    [JsonPropertyName("nome")]
    public string? Nome { get; set; }

    /// <summary>Partner type (Pessoa Física or Pessoa Jurídica).</summary>
    [JsonPropertyName("tipo")]
    public string? Tipo { get; set; }

    /// <summary>Date when the partner joined.</summary>
    [JsonPropertyName("data_entrada")]
    public string? DataEntrada { get; set; }

    /// <summary>Legal representative CPF (partially masked).</summary>
    [JsonPropertyName("cpf_representante_legal")]
    public string? CpfRepresentanteLegal { get; set; }

    /// <summary>Legal representative name.</summary>
    [JsonPropertyName("nome_representante")]
    public string? NomeRepresentante { get; set; }

    /// <summary>Age range of the partner.</summary>
    [JsonPropertyName("faixa_etaria")]
    public string? FaixaEtaria { get; set; }

    /// <summary>Last update timestamp.</summary>
    [JsonPropertyName("atualizado_em")]
    public DateTime? AtualizadoEm { get; set; }

    /// <summary>Country ID.</summary>
    [JsonPropertyName("pais_id")]
    public string? PaisId { get; set; }

    /// <summary>Partner qualification.</summary>
    [JsonPropertyName("qualificacao_socio")]
    public Qualificacao? QualificacaoSocio { get; set; }

    /// <summary>Representative qualification.</summary>
    [JsonPropertyName("qualificacao_representante")]
    public Qualificacao? QualificacaoRepresentante { get; set; }

    /// <summary>Country information.</summary>
    [JsonPropertyName("pais")]
    public Pais? Pais { get; set; }
}

/// <summary>Country information.</summary>
public class Pais
{
    /// <summary>Country ID.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>ISO 3166-1 alpha-2 code.</summary>
    [JsonPropertyName("iso2")]
    public string? Iso2 { get; set; }

    /// <summary>ISO 3166-1 alpha-3 code.</summary>
    [JsonPropertyName("iso3")]
    public string? Iso3 { get; set; }

    /// <summary>Country name.</summary>
    [JsonPropertyName("nome")]
    public string? Nome { get; set; }

    /// <summary>COMEX ID.</summary>
    [JsonPropertyName("comex_id")]
    public string? ComexId { get; set; }
}

/// <summary>Simples Nacional tax regime information.</summary>
public class Simples
{
    /// <summary>MEI (Microempreendedor Individual) status.</summary>
    [JsonPropertyName("mei")]
    public string? Mei { get; set; }

    /// <summary>Simples Nacional status.</summary>
    [JsonPropertyName("simples")]
    public string? SimplesNacional { get; set; }

    /// <summary>MEI enrollment date.</summary>
    [JsonPropertyName("data_opcao_mei")]
    public string? DataOpcaoMei { get; set; }

    /// <summary>MEI exclusion date.</summary>
    [JsonPropertyName("data_exclusao_mei")]
    public string? DataExclusaoMei { get; set; }

    /// <summary>Simples Nacional enrollment date.</summary>
    [JsonPropertyName("data_opcao_simples")]
    public string? DataOpcaoSimples { get; set; }

    /// <summary>Simples Nacional exclusion date.</summary>
    [JsonPropertyName("data_exclusao_simples")]
    public string? DataExclusaoSimples { get; set; }

    /// <summary>Last update timestamp.</summary>
    [JsonPropertyName("atualizado_em")]
    public DateTime? AtualizadoEm { get; set; }
}

/// <summary>Establishment (branch) information.</summary>
public class Estabelecimento
{
    /// <summary>Complete CNPJ (14 digits).</summary>
    [JsonPropertyName("cnpj")]
    public string? Cnpj { get; set; }

    /// <summary>Secondary business activities.</summary>
    [JsonPropertyName("atividades_secundarias")]
    public Atividade[]? AtividadesSecundarias { get; set; }

    /// <summary>Root CNPJ number.</summary>
    [JsonPropertyName("cnpj_raiz")]
    public string? CnpjRaiz { get; set; }

    /// <summary>Order number (branch number).</summary>
    [JsonPropertyName("cnpj_ordem")]
    public string? CnpjOrdem { get; set; }

    /// <summary>Check digit.</summary>
    [JsonPropertyName("cnpj_digito_verificador")]
    public string? CnpjDigitoVerificador { get; set; }

    /// <summary>Type (Matriz or Filial).</summary>
    [JsonPropertyName("tipo")]
    public string? Tipo { get; set; }

    /// <summary>Trade name (fantasy name).</summary>
    [JsonPropertyName("nome_fantasia")]
    public string? NomeFantasia { get; set; }

    /// <summary>Registration status.</summary>
    [JsonPropertyName("situacao_cadastral")]
    public string? SituacaoCadastral { get; set; }

    /// <summary>Registration status date.</summary>
    [JsonPropertyName("data_situacao_cadastral")]
    public string? DataSituacaoCadastral { get; set; }

    /// <summary>Business start date.</summary>
    [JsonPropertyName("data_inicio_atividade")]
    public string? DataInicioAtividade { get; set; }

    /// <summary>Foreign city name (if applicable).</summary>
    [JsonPropertyName("nome_cidade_exterior")]
    public string? NomeCidadeExterior { get; set; }

    /// <summary>Street type (e.g., Avenida, Rua).</summary>
    [JsonPropertyName("tipo_logradouro")]
    public string? TipoLogradouro { get; set; }

    /// <summary>Street name.</summary>
    [JsonPropertyName("logradouro")]
    public string? Logradouro { get; set; }

    /// <summary>Street number.</summary>
    [JsonPropertyName("numero")]
    public string? Numero { get; set; }

    /// <summary>Address complement.</summary>
    [JsonPropertyName("complemento")]
    public string? Complemento { get; set; }

    /// <summary>Neighborhood.</summary>
    [JsonPropertyName("bairro")]
    public string? Bairro { get; set; }

    /// <summary>ZIP code.</summary>
    [JsonPropertyName("cep")]
    public string? Cep { get; set; }

    /// <summary>Primary phone area code.</summary>
    [JsonPropertyName("ddd1")]
    public string? Ddd1 { get; set; }

    /// <summary>Primary phone number.</summary>
    [JsonPropertyName("telefone1")]
    public string? Telefone1 { get; set; }

    /// <summary>Secondary phone area code.</summary>
    [JsonPropertyName("ddd2")]
    public string? Ddd2 { get; set; }

    /// <summary>Secondary phone number.</summary>
    [JsonPropertyName("telefone2")]
    public string? Telefone2 { get; set; }

    /// <summary>Fax area code.</summary>
    [JsonPropertyName("ddd_fax")]
    public string? DddFax { get; set; }

    /// <summary>Fax number.</summary>
    [JsonPropertyName("fax")]
    public string? Fax { get; set; }

    /// <summary>Email address.</summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>Special situation description.</summary>
    [JsonPropertyName("situacao_especial")]
    public string? SituacaoEspecial { get; set; }

    /// <summary>Special situation date.</summary>
    [JsonPropertyName("data_situacao_especial")]
    public string? DataSituacaoEspecial { get; set; }

    /// <summary>Last update timestamp.</summary>
    [JsonPropertyName("atualizado_em")]
    public DateTime? AtualizadoEm { get; set; }

    /// <summary>Primary business activity.</summary>
    [JsonPropertyName("atividade_principal")]
    public Atividade? AtividadePrincipal { get; set; }

    /// <summary>Country information.</summary>
    [JsonPropertyName("pais")]
    public Pais? Pais { get; set; }

    /// <summary>State information.</summary>
    [JsonPropertyName("estado")]
    public Estado? Estado { get; set; }

    /// <summary>City information.</summary>
    [JsonPropertyName("cidade")]
    public Cidade? Cidade { get; set; }

    /// <summary>Registration status reason.</summary>
    [JsonPropertyName("motivo_situacao_cadastral")]
    public string? MotivoSituacaoCadastral { get; set; }

    /// <summary>State registrations.</summary>
    [JsonPropertyName("inscricoes_estaduais")]
    public object[]? InscricoesEstaduais { get; set; }
}

/// <summary>Business activity (CNAE) information.</summary>
public class Atividade
{
    /// <summary>Activity code.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>CNAE section.</summary>
    [JsonPropertyName("secao")]
    public string? Secao { get; set; }

    /// <summary>CNAE division.</summary>
    [JsonPropertyName("divisao")]
    public string? Divisao { get; set; }

    /// <summary>CNAE group.</summary>
    [JsonPropertyName("grupo")]
    public string? Grupo { get; set; }

    /// <summary>CNAE class.</summary>
    [JsonPropertyName("classe")]
    public string? Classe { get; set; }

    /// <summary>CNAE subclass.</summary>
    [JsonPropertyName("subclasse")]
    public string? Subclasse { get; set; }

    /// <summary>Activity description.</summary>
    [JsonPropertyName("descricao")]
    public string? Descricao { get; set; }
}

/// <summary>State information.</summary>
public class Estado
{
    /// <summary>State ID.</summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>State name.</summary>
    [JsonPropertyName("nome")]
    public string? Nome { get; set; }

    /// <summary>State abbreviation.</summary>
    [JsonPropertyName("sigla")]
    public string? Sigla { get; set; }

    /// <summary>IBGE state code.</summary>
    [JsonPropertyName("ibge_id")]
    public int IbgeId { get; set; }
}

/// <summary>City information.</summary>
public class Cidade
{
    /// <summary>City ID.</summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>City name.</summary>
    [JsonPropertyName("nome")]
    public string? Nome { get; set; }

    /// <summary>IBGE city code.</summary>
    [JsonPropertyName("ibge_id")]
    public int IbgeId { get; set; }

    /// <summary>SIAFI code.</summary>
    [JsonPropertyName("siafi_id")]
    public string? SiafiId { get; set; }
}
