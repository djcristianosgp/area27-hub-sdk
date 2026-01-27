# Area27.Hub - AI Coding Agent Instructions

## Project Overview
Area27.Hub is a .NET 8 NuGet utility library providing reusable helpers for validation, string handling, image processing, text analysis, SQL generation, XML/JSON conversion, and PDF generation. The library follows a static helper pattern with no state management.

## Architecture Patterns

### Organization Structure
- **Root Classes**: `Calculadora.cs` - simple arithmetic operations (rare pattern, likely for testing)
- **Extensions**: `src/Extensions/` - Extension methods for built-in types (e.g., `StringExtensions`)
- **Helpers**: `src/Helpers/` - Static utility classes (e.g., `ValidationHelper`, `ImageHelper`, `PdfHelper`)

### Code Conventions
1. **All Helpers are static classes** - No instantiation, pure utility functions
2. **XML Documentation is mandatory** - Every public method must have `<summary>`, `<param>`, `<returns>`, and `<exception>` tags
3. **Namespace pattern**: `Area27.Hub` (root), `Area27.Hub.Extensions`, `Area27.Hub.Helpers`
4. **Nullable annotations enabled** - Use `string?` for nullable parameters, validate with `string.IsNullOrWhiteSpace()`

### Error Handling Pattern
```csharp
// Standard validation pattern used throughout
if (string.IsNullOrWhiteSpace(parameter))
    throw new ArgumentNullException(nameof(parameter), "Cannot be null or empty.");
    
// Domain-specific exceptions
throw new FileNotFoundException($"File not found: {path}");
throw new XmlException("Failed to parse XML content.", ex);
```

### Dependency Guidelines
- **SixLabors.ImageSharp** (3.1.12): All image operations
- **QuestPDF** (2024.12.3): PDF generation - **must call `QuestPDF.Settings.License = LicenseType.Community;`** in static constructors
- Avoid external dependencies unless absolutely necessary for utility function

## Key Components

### ValidationHelper (`src/Helpers/ValidationHelper.cs`)
Brazilian-specific validations (CPF, CNPJ) plus email validation. CPF generation uses weighted checksums.

### CnpjHelper (`src/Helpers/CnpjHelper.cs`)
- Queries CNPJ data from public API (publica.cnpj.ws)
- Auto-removes formatting (dots, slashes, hyphens) before query
- Returns comprehensive company data including partners, activities, address
- Both async (`ConsultarCnpjAsync`) and sync (`ConsultarCnpj`) methods available
- Uses `HttpClient` singleton for efficiency

### ImageHelper (`src/Helpers/ImageHelper.cs`)
- Accepts base64 strings OR file paths (auto-detect)
- Uses `ImageFormat` enum (Png, Jpg, Bmp)
- Compression loop: reduce quality by 5% until target size met

### XmlJsonHelper (`src/Helpers/XmlJsonHelper.cs`)
- Attributes prefixed with `@` in JSON (e.g., `"@id": "123"`)
- Text content uses `#text` key in mixed-content scenarios
- Default root element name is "root" for JSON→XML

### PdfHelper (`src/Helpers/PdfHelper.cs`)
- **Critical**: Must initialize `QuestPDF.Settings.License = LicenseType.Community;` in static constructor
- Two layout modes: plain text (`JsonToPdf`) and structured table (`JsonToPdfTable`)
- Uses Courier font for code/JSON display, default font for tables

### SqlTableGenerator (`src/Helpers/SqlTableGenerator.cs`)
- Generates CREATE TABLE from C# POCOs using reflection
- Supports 4 database types: PostgreSql, MySql, SqlServer, SQLite
- Maps .NET types to DB types (e.g., `string` → `VARCHAR(255)`, `DateTime` → varies by DB)

### SaasHelper (`src/Helpers/SaasHelper.cs`)
- Uses `RandomNumberGenerator.Create()` for cryptographically secure tokens
- Password hashing with PBKDF2 (10,000 iterations minimum)
- Slug generation: lowercase, replace spaces with hyphens, remove special chars

## Development Workflow

### Building
```bash
dotnet build -c Release                    # Builds and generates NuGet package
dotnet pack -c Release                     # Explicit package creation
```

### Package Management
- **Version**: Update in `Area27.Hub.csproj` → `<Version>` tag
- **Auto-pack**: `<GeneratePackageOnBuild>true</GeneratePackageOnBuild>` is enabled
- **Output**: `bin/Release/net8.0/Area27.Hub.{version}.nupkg`

### Testing Approach
No test project currently exists. When adding features:
1. Ensure comprehensive XML docs
2. Add usage examples to `README.md`
3. Test manually or add unit test project (xUnit recommended)

## Adding New Features

### For new Helper class:
1. Create in `src/Helpers/{Name}Helper.cs`
2. Make class static with `public static class {Name}Helper`
3. Add XML documentation to all public members
4. Update `Area27.Hub.csproj`:
   - Increment `<Version>`
   - Add keywords to `<PackageTags>`
   - Update `<Description>`
5. Add usage example to both `README.md` files (root and Area27.Hub/)

### For new dependencies:
1. Add `<PackageReference>` in `Area27.Hub.csproj`
2. Prefer stable, well-maintained packages
3. Document any special initialization (like QuestPDF license)

## Common Patterns to Follow

### String Extensions Pattern
```csharp
public static string OnlyNumbers(this string? value)
{
    if (string.IsNullOrEmpty(value)) return string.Empty;
    var builder = new StringBuilder(value.Length);
    // ... efficient char-by-char processing
    return builder.ToString();
}
```

### Dictionary Result Pattern
```csharp
// Used in WordCountHelper, returns case-insensitive dictionary
var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
```

### Enum for Options
```csharp
// Always document enum members with XML comments
public enum ImageFormat
{
    /// <summary>PNG format</summary>
    Png,
    /// <summary>JPEG format</summary>
    Jpg
}
```

## Anti-Patterns to Avoid
- ❌ Instance classes (use static)
- ❌ Mutable state in helpers
- ❌ Missing XML documentation
- ❌ Generic exception messages (be specific: include parameter names, file paths)
- ❌ Ignoring nullability annotations
