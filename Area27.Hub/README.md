# Area27.Hub

Biblioteca utilitária .NET 8 com validações, extensões de string, processamento de imagens, análise de texto, geração de CPF, geração de SQL e utilitários SaaS.

## Instalação

```bash
dotnet add package Area27.Hub
```

## Principais recursos
- Validação de CPF e email
- Geração de CPF (com/sem pontuação)
- Processamento de imagens (compressão e conversão)
- Contagem e estatísticas de palavras
- Gerador de CREATE TABLE para PostgreSQL, MySQL, SQL Server e SQLite
- Utilitários SaaS: API Keys, tokens, hashing de senha, licenças, slugs

## Exemplos rápidos

### CPF
```csharp
using Area27.Hub.Helpers;

var cpfValido = ValidationHelper.IsCpfValido("390.533.447-05");
var cpfGerado = ValidationHelper.GenerateCpf(formatted: true);
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
