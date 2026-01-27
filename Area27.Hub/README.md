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

## Build
```bash
dotnet build -c Release
```

MIT
