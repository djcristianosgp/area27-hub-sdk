# Area27.Hub

Biblioteca utilitária .NET 8 pronta para publicação no NuGet, oferecendo validações comuns, extensões de string, processamento de imagens, análise de texto, geração de CPF e muito mais.

## Instalação

Use o .NET CLI para adicionar o pacote:

```bash
dotnet add package Area27.Hub
```

## Uso

### Calculadora
```csharp
using Area27.Hub;

var calc = new Calculadora();
var soma = calc.Somar(2, 3);          // 5
var diferenca = calc.Subtrair(10, 4); // 6
var produto = calc.Multiplicar(6, 7); // 42
var quociente = calc.Dividir(8, 2);   // 4
```

### Extensões de String
```csharp
using Area27.Hub.Extensions;

var vazia = "  ".IsNullOrEmptySafe(); // true
var somenteDigitos = "ABC123-90".OnlyNumbers(); // "12390"
```

### Validações
```csharp
using Area27.Hub.Helpers;

var cpfValido = ValidationHelper.IsCpfValido("390.533.447-05");
var emailValido = ValidationHelper.IsEmailValido("contato@area27.dev");

// Gerar CPF válido
var cpfFormatado = ValidationHelper.GenerateCpf(formatted: true);     // "123.456.789-09"
var cpfSimples = ValidationHelper.GenerateCpf(formatted: false);      // "12345678909"
```

### Processamento de Imagens
```csharp
using Area27.Hub.Helpers;

// Converter e comprimir imagem
var base64Processada = ImageHelper.ProcessImage(
    imageInput: "caminho/para/imagem.jpg", // ou base64
    maxSizeMb: 2,
    outputFormat: ImageHelper.ImageFormat.Jpg
);

// Apenas converter formato
var convertida = ImageHelper.ConvertImageFormat("base64_aqui", ImageHelper.ImageFormat.Png);

// Obter tamanho da imagem
var tamanho = ImageHelper.GetImageSizeMb("base64_aqui"); // em MB
```

### Contagem de Palavras
```csharp
using Area27.Hub.Helpers;

var texto = "Hello world hello area27 world";

// Contar total de palavras
var totalPalavras = WordCountHelper.CountWords(texto); // 5

// Contar palavras específicas
var contagem = WordCountHelper.CountSpecificWords(
    texto, 
    new[] { "hello", "world", "area27" }
);
// Resultado: { "hello": 2, "world": 2, "area27": 1 }

// Contar todas as palavras únicas
var todasPalavras = WordCountHelper.CountAllWords(texto);
// Resultado: { "hello": 2, "world": 2, "area27": 1 }

// Obter estatísticas do texto
var stats = WordCountHelper.GetStatistics(texto);
Console.WriteLine($"Total de palavras: {stats.TotalWords}");
Console.WriteLine($"Palavras únicas: {stats.UniqueWords}");
Console.WriteLine($"Comprimento médio: {stats.AverageWordLength}");
Console.WriteLine($"Palavra mais longa: {stats.LongestWord}");
Console.WriteLine($"Palavra mais curta: {stats.ShortestWord}");
```

### Gerador de SQL
```csharp
using Area27.Hub.Helpers;

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public decimal Preco { get; set; }
    public DateTime DataCriacao { get; set; }
    public bool Ativo { get; set; }
}

// Gerar CREATE TABLE para PostgreSQL
var sqlPostgres = SqlTableGenerator.GenerateCreateTable<Produto>(
    SqlTableGenerator.DatabaseType.PostgreSql,
    tableName: "produtos"
);

// Suporta: PostgreSQL, MySQL, SQL Server, SQLite
var sqlMysql = SqlTableGenerator.GenerateCreateTable<Produto>(
    SqlTableGenerator.DatabaseType.MySql
);

var sqlServer = SqlTableGenerator.GenerateCreateTable<Produto>(
    SqlTableGenerator.DatabaseType.SqlServer
);

var sqlSqlite = SqlTableGenerator.GenerateCreateTable<Produto>(
    SqlTableGenerator.DatabaseType.SQLite
);

// Gerar múltiplas tabelas
var sqlMultiplas = SqlTableGenerator.GenerateCreateTables(
    new[] { typeof(Produto), typeof(Pedido) },
    SqlTableGenerator.DatabaseType.PostgreSql
);
```

## Recursos

- ✅ **Validações**: CPF, Email
- ✅ **Geração**: CPF válido (com/sem pontuação)
- ✅ **Extensões de String**: Tratamento seguro, apenas números
- ✅ **Processamento de Imagens**: Compressão inteligente, conversão de formatos
- ✅ **Análise de Texto**: Contagem de palavras, estatísticas
- ✅ **Gerador SQL**: CREATE TABLE para múltiplos bancos de dados
- ✅ **Recursos SaaS**: API Keys, Tokens, Hashing, Licenças, Slugs

## Recursos Adicionais para SaaS

```csharp
using Area27.Hub.Helpers;

// Gerar API Key segura
var apiKey = SaasHelper.GenerateApiKey();
var token = SaasHelper.GenerateToken(length: 64);

// Validar força de senha
var resultado = SaasHelper.ValidatePasswordStrength("MyP@ssw0rd123");
if (resultado.IsValid)
{
    Console.WriteLine($"Força: {resultado.Strength}"); // Strong
}

// Hash seguro de senha
var senhaHash = SaasHelper.HashPassword("usuario_password");
var senhaValida = SaasHelper.VerifyPassword("usuario_password", senhaHash);

// Gerar e validar licenças
var licenca = SaasHelper.GenerateLicenseKey("cliente123", DateTime.Now.AddYears(1));
var licencaValida = SaasHelper.ValidateLicenseKey(licenca, "cliente123");

// Gerar slugs para URLs
var slug = SaasHelper.GenerateSlug("Meu Produto Incrível");
// Resultado: "meu-produto-incrivel"

// Truncar texto
var resumo = SaasHelper.Truncate("Um texto muito longo e chato", 20);
// Resultado: "Um texto muito long..."

// Gerar código de plano
var codigoPlano = SaasHelper.GeneratePlanCode("Premium Plus");
// Resultado: "premium-plus-9876"
```

## Build e empacotamento

O projeto está configurado com `GeneratePackageOnBuild=true`. Ao executar um build em Release, o `.nupkg` é gerado automaticamente em `bin/Release`:

```bash
dotnet build -c Release
```

## Licença

MIT. Consulte o campo `PackageLicenseExpression` no `.csproj`.
