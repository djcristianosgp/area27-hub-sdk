# Area27.Hub

Biblioteca utilitária .NET 8 pronta para publicação no NuGet, oferecendo validações comuns, extensões de string e uma calculadora básica.

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
```

## Build e empacotamento

O projeto está configurado com `GeneratePackageOnBuild=true`. Ao executar um build em Release, o `.nupkg` é gerado automaticamente em `bin/Release`:

```bash
dotnet build -c Release
```

## Licença

MIT. Consulte o campo `PackageLicenseExpression` no `.csproj`.
