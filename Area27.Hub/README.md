# Area27.Hub

Biblioteca utilitária .NET 8 com validações, extensões de string, processamento de imagens, análise de texto, geração de CPF, consulta de CNPJ, geração de SQL e utilitários SaaS.

## Instalação

```bash
dotnet add package Area27.Hub
```

## Principais recursos
- Validação de CPF e email
- Geração de CPF (com/sem pontuação)
- Consulta de dados de CNPJ via API pública
- Processamento de imagens (compressão e conversão)
- Contagem e estatísticas de palavras
- Manipulação de datas (diferenças, idade, cálculos, nomes em português)
- Gerador de CREATE TABLE para PostgreSQL, MySQL, SQL Server e SQLite
- Conversão entre XML e JSON
- Geração de PDF a partir de JSON
- Utilitários SaaS: API Keys, tokens, hashing de senha, licenças, slugs

## Exemplos rápidos

### CPF
```csharp
using Area27.Hub.Helpers;

var cpfValido = ValidationHelper.IsCpfValido("390.533.447-05");
var cpfGerado = ValidationHelper.GenerateCpf(formatted: true);
```

### CNPJ
```csharp
using Area27.Hub.Helpers;

// Consultar dados de CNPJ (async)
var dados = await CnpjHelper.ConsultarCnpjAsync("40.357.190/0001-13");
Console.WriteLine($"Razão Social: {dados?.RazaoSocial}");
Console.WriteLine($"Capital Social: {dados?.CapitalSocial}");
Console.WriteLine($"Situação: {dados?.Estabelecimento?.SituacaoCadastral}");

// Versão síncrona
var dadosSync = CnpjHelper.ConsultarCnpj("40357190000113");
```

### Imagens
```csharp
using Area27.Hub.Helpers;

var base64Processada = ImageHelper.ProcessImage(
    imageInput: "caminho/para/imagem.jpg", // ou base64
    maxSizeMb: 2,
    outputFormat: ImageHelper.ImageFormat.Jpg
);
```

### Texto
```csharp
using Area27.Hub.Helpers;

var total = WordCountHelper.CountWords("hello world hello");
var stats = WordCountHelper.GetStatistics("hello world hello");
```

### Datas
```csharp
using Area27.Hub.Helpers;

// Diferença entre datas
var data1 = new DateTime(2024, 1, 15);
var data2 = new DateTime(2026, 3, 27);
var diferenca = DateHelper.CalculateDateDifference(data1, data2); // "12 dias, 2 meses, 2 anos"

// Calcular idade
var nascimento = new DateTime(1990, 6, 15);
var idade = DateHelper.CalculateAge(nascimento); // Com data de hoje

// Adicionar período
var novaData = DateHelper.AddPeriod(data1, yearsToAdd: 2, monthsToAdd: 3, daysToAdd: 15);

// Dias úteis
var diasUteis = DateHelper.BusinessDaysBetween(data1, data2);

// Formatação em português
var dataFormatada = DateHelper.FormatDateInPortuguese(data2); // "27 de março de 2026, segunda-feira"
```

### SQL
```csharp
using Area27.Hub.Helpers;

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public decimal Preco { get; set; }
}

var sql = SqlTableGenerator.GenerateCreateTable<Produto>(
    SqlTableGenerator.DatabaseType.PostgreSql,
    tableName: "produtos"
);
```

### SaaS
```csharp
using Area27.Hub.Helpers;

var apiKey = SaasHelper.GenerateApiKey();
var senhaHash = SaasHelper.HashPassword("MyP@ssw0rd");
var slug = SaasHelper.GenerateSlug("Meu Produto Incrivel");
```

### Tributação Brasileira
```csharp
using Area27.Hub.Helpers;

// ICMS
var baseCalculo = 1000m;
var icms = TributacaoHelper.CalcularIcms(baseCalculo, aliquota: 12);              // 120,00
var baseReduzida = TributacaoHelper.CalcularBaseIcmsReduzida(baseCalculo, 33.33m); // 666,70

// ICMS-ST com MVA
var baseST = TributacaoHelper.CalcularBaseST(
    valorProduto: 1000m, ipi: 50m, frete: 80m, outrasDespesas: 0m, mvaPadrao: 35m);
var icmsST = TributacaoHelper.CalcularIcmsST(baseST, aliquotaDestino: 18m, icmsProprio: 120m);

// IPI / PIS / COFINS
var ipi     = TributacaoHelper.CalcularIpi(1000m, aliquota: 5m);        // 50,00
var pis     = TributacaoHelper.CalcularPis(1000m, aliquota: 0.65m);     // 6,50
var cofins  = TributacaoHelper.CalcularCofins(1000m, aliquota: 3m);     // 30,00

// Reforma Tributária (IBS/CBS)
var ibs = TributacaoHelper.CalcularIbs(1000m, aliquota: 17.7m);
var cbs = TributacaoHelper.CalcularCbs(1000m, aliquota: 9.3m);

// Back-calculation: preço bruto dado preço líquido desejado
var precoBruto = TributacaoHelper.CalcularPrecoBrutoComIcms(precoLiquido: 880m, aliquota: 12m);
```

### Custo, Markup e Lucro
```csharp
using Area27.Hub.Helpers;

var custo      = 100m;
var precoVenda = 150m;

var markup = CustoHelper.CalcularMarkup(precoVenda, custo);      // 50,00 (%)
var lucro  = CustoHelper.CalcularLucro(precoVenda, custo);       // 33,33 (%)

// Calcular preço a partir do markup desejado
var preco = CustoHelper.CalcularPrecoVendaMarkup(custo, markupPercent: 50m); // 150,00

// Composição de custo (frete CIF + IPI + créditos recuperáveis)
var custoTotal = CustoHelper.CalcularCustoTotal(
    valorCompra: 1000m, desconto: 50m, ipiValor: 50m,
    freteValor: 80m, icmsCredito: 120m, pisCredito: 6.5m, cofinsCredito: 30m);

// Análise de lucro por item de venda
var analise = CustoHelper.AnalisarLucro(custoZero: 100m, valorVenda: 150m, percentualDesconto: 5m);
Console.WriteLine($"Lucro líquido: {analise.PercentualLucro}% | Markup: {analise.PercentualMarkup}%");
Console.WriteLine(analise.CustoZeroInvalido ? "Atenção: CustoZero não informado!" : string.Empty);
```

### Financeiro — Parcelas e Juros
```csharp
using Area27.Hub.Helpers;

// Gerar parcelas (absorve diferença de arredondamento na última)
var parcelas = FinanceiroHelper.GerarParcelas(
    valorTotal: 1000m,
    quantidadeParcelas: 3,
    primeiroVencimento: new DateTime(2026, 5, 10),
    intervaloDias: 30);

// Parcelas mensais (mesma data do mês)
var mensais = FinanceiroHelper.GerarParcelasMensais(1000m, 3, new DateTime(2026, 5, 10));

// Encargos de atraso
var encargos = FinanceiroHelper.CalcularEncargosAtraso(
    valorParcela: 500m, diasAtraso: 15,
    taxaJurosMensalPercent: 1m, percentualMulta: 2m);

// Desconto / Acréscimo
var liquido   = FinanceiroHelper.AplicarDesconto(1000m, 5m);    // 950,00
var corrigido = FinanceiroHelper.AplicarAcrescimo(1000m, 2.5m); // 1025,00

// Prorrogar vencimentos
FinanceiroHelper.ProrrogarVencimentos(parcelas, dias: 10);
```

### Documentos Bancários
```csharp
using Area27.Hub.Helpers;

// Validar cheque
var valido = DocumentoBancarioHelper.ValidarCheque("1234", "00012345", "000125");

// Calcular líquido de borderô
var liquido = DocumentoBancarioHelper.CalcularLiquidoBordero(50000m, taxaDesconto: 1.5m, tarifas: 15m);

// Extrair dados de código de barras de boleto
var vencimento = DocumentoBancarioHelper.ExtrairVencimentoBoleto("00190500954014481606906809350314337370000000100");
var valor      = DocumentoBancarioHelper.ExtrairValorBoleto("00190500954014481606906809350314337370000000100");

// Próximo dia útil bancário
var proxUtil = DocumentoBancarioHelper.ProximoDiaUtil(new DateTime(2026, 4, 3)); // sexta → segunda
```

### Validação de CNPJ (dígito verificador)
```csharp
using Area27.Hub.Helpers;

var cnpjValido = ValidationHelper.IsCnpjValido("11.222.333/0001-81"); // true
var cnpjInvalido = ValidationHelper.IsCnpjValido("11.111.111/1111-11"); // false
```

## Recursos

- ✅ **Validações**: CPF, CNPJ (dígito verificador), Email
- ✅ **Geração**: CPF válido (com/sem pontuação)
- ✅ **Extensões de String**: Tratamento seguro, apenas números, remoção de acentos, máscara
- ✅ **Processamento de Imagens**: Compressão inteligente, conversão de formatos
- ✅ **Análise de Texto**: Contagem de palavras, estatísticas
- ✅ **Manipulação de Datas**: Diferenças, idade, cálculos, formatação em português
- ✅ **Gerador SQL**: CREATE TABLE para PostgreSQL, MySQL, SQL Server, SQLite
- ✅ **Recursos SaaS**: API Keys, Tokens, Hashing, Licenças, Slugs
- ✅ **Tributação Brasileira**: ICMS, IPI, PIS/COFINS, ICMS-ST/MVA, IBS/CBS (Reforma Tributária)
- ✅ **Custo e Markup**: Markup, lucro, composição de custo, análise de lucro por item
- ✅ **Financeiro**: Geração de parcelas, juros simples/compostos, multa, prorrogação
- ✅ **Documentos Bancários**: Cheque, borderô, boleto (código de barras), dia útil
- ✅ **Entidades de Domínio**: Cliente, Fornecedor, Produto (com tributação), Pedido, ItemPedido, Cheque, Borderô, Parcela
- ✅ **Enums Fiscais**: CstIcms, Csosn, CstPisCofins, TipoOperacao, ModalidadeFrete, StatusMdfe

## Build
```bash
dotnet build -c Release
```

MIT
