using Area27.Hub.Helpers;

// Exemplo 1: Diferença entre duas datas
Console.WriteLine("=== EXEMPLO 1: Diferença entre Datas ===");
var data1 = new DateTime(2024, 1, 15);
var data2 = new DateTime(2026, 3, 27);

var diferencaAgrupada = DateHelper.CalculateDateDifference(data1, data2, DateHelper.DateResultFormat.Grouped);
Console.WriteLine($"Diferença (agrupada): {diferencaAgrupada}");

var diferencaSeparada = DateHelper.CalculateDateDifference(data1, data2, DateHelper.DateResultFormat.Separated);
Console.WriteLine($"Diferença (separada): {diferencaSeparada}");

// Exemplo 2: Calcular idade
Console.WriteLine("\n=== EXEMPLO 2: Calcular Idade ===");
var nascimento = new DateTime(1990, 6, 15);

var idadeFormatada = DateHelper.CalculateAge(nascimento);
Console.WriteLine($"Sua idade (formato agrupado): {idadeFormatada}");

var idadeSeparada = DateHelper.CalculateAge(nascimento, null, DateHelper.DateResultFormat.Separated);
Console.WriteLine($"Sua idade (formato separado): {idadeSeparada}");

var anosIdade = DateHelper.GetAgeInYears(nascimento);
Console.WriteLine($"Apenas em anos: {anosIdade}");

// Calcular idade em uma data específica
var idadeEm2020 = DateHelper.GetAgeInYears(nascimento, new DateTime(2020, 12, 31));
Console.WriteLine($"Idade em 31/12/2020: {idadeEm2020}");

// Exemplo 3: Adicionar/Remover períodos
Console.WriteLine("\n=== EXEMPLO 3: Adicionar/Remover Períodos ===");
var dataBase = new DateTime(2026, 1, 27);
Console.WriteLine($"Data base: {dataBase:dd/MM/yyyy}");

var mais10Dias = DateHelper.AddDays(dataBase, 10);
Console.WriteLine($"+ 10 dias: {mais10Dias:dd/MM/yyyy}");

var menos3Meses = DateHelper.AddMonths(dataBase, -3);
Console.WriteLine($"- 3 meses: {menos3Meses:dd/MM/yyyy}");

var mais1Ano = DateHelper.AddYears(dataBase, 1);
Console.WriteLine($"+ 1 ano: {mais1Ano:dd/MM/yyyy}");

var periodoCombinado = DateHelper.AddPeriod(dataBase, yearsToAdd: 1, monthsToAdd: 6, daysToAdd: 15);
Console.WriteLine($"+ 1 ano, 6 meses e 15 dias: {periodoCombinado:dd/MM/yyyy}");

// Exemplo 4: Dias entre datas
Console.WriteLine("\n=== EXEMPLO 4: Dias entre Datas ===");
var diasTotal = DateHelper.DaysBetween(data1, data2);
Console.WriteLine($"Dias totais entre {data1:dd/MM/yyyy} e {data2:dd/MM/yyyy}: {diasTotal}");

var diasUteis = DateHelper.BusinessDaysBetween(data1, data2);
Console.WriteLine($"Dias úteis (excluindo fins de semana): {diasUteis}");

// Exemplo 5: Primeiro e último dia do mês
Console.WriteLine("\n=== EXEMPLO 5: Primeiro e Último Dia do Mês ===");
var primeiro = DateHelper.GetFirstDayOfMonth(dataBase);
var ultimo = DateHelper.GetLastDayOfMonth(dataBase);
Console.WriteLine($"Primeiro dia do mês: {primeiro:dd/MM/yyyy}");
Console.WriteLine($"Último dia do mês: {ultimo:dd/MM/yyyy}");

// Exemplo 6: Verificar ano bissexto
Console.WriteLine("\n=== EXEMPLO 6: Ano Bissexto ===");
Console.WriteLine($"2024 é bissexto? {DateHelper.IsLeapYear(2024)}");
Console.WriteLine($"2025 é bissexto? {DateHelper.IsLeapYear(2025)}");
Console.WriteLine($"2000 é bissexto? {DateHelper.IsLeapYear(2000)}");

// Exemplo 7: Nomes em português
Console.WriteLine("\n=== EXEMPLO 7: Nomes em Português ===");
var nomeDia = DateHelper.GetDayNameInPortuguese(data2);
Console.WriteLine($"Dia da semana: {nomeDia}");

var nomeMes = DateHelper.GetMonthNameInPortuguese(data2.Month);
Console.WriteLine($"Mês: {nomeMes}");

// Exemplo 8: Formatação completa em português
Console.WriteLine("\n=== EXEMPLO 8: Formatação em Português ===");
var dataFormatada = DateHelper.FormatDateInPortuguese(data2);
Console.WriteLine($"Formatação completa: {dataFormatada}");

var hoje = DateTime.Now;
var hojeFormatado = DateHelper.FormatDateInPortuguese(hoje);
Console.WriteLine($"Hoje: {hojeFormatado}");
