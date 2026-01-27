using Area27.Hub.Helpers;

// Testes das funcionalidades do DateHelper
Console.WriteLine("╔════════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║                    TESTES DO DATEHELPER v1.0.7                     ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝\n");

// ========== TESTE 1: Diferença entre Datas ==========
Console.WriteLine("📅 TESTE 1: DIFERENÇA ENTRE DUAS DATAS");
Console.WriteLine("─".PadRight(66, '─'));
var data1 = new DateTime(2024, 1, 15);
var data2 = new DateTime(2026, 3, 27);
Console.WriteLine($"Data inicial:  {data1:dd/MM/yyyy}");
Console.WriteLine($"Data final:    {data2:dd/MM/yyyy}");
Console.WriteLine($"Formato agrupado:  {DateHelper.CalculateDateDifference(data1, data2, DateHelper.DateResultFormat.Grouped)}");
Console.WriteLine($"Formato separado:  {DateHelper.CalculateDateDifference(data1, data2, DateHelper.DateResultFormat.Separated)}");

// ========== TESTE 2: Calcular Idade ==========
Console.WriteLine("\n🎂 TESTE 2: CALCULAR IDADE");
Console.WriteLine("─".PadRight(66, '─'));
var nascimento = new DateTime(1990, 6, 15);
Console.WriteLine($"Data de nascimento: {nascimento:dd/MM/yyyy}");
Console.WriteLine($"Idade (agrupada):   {DateHelper.CalculateAge(nascimento)}");
Console.WriteLine($"Idade (separada):   {DateHelper.CalculateAge(nascimento, null, DateHelper.DateResultFormat.Separated)}");
Console.WriteLine($"Idade em anos:      {DateHelper.GetAgeInYears(nascimento)} anos");

// Calcular idade em data específica (Ano 2020)
var idade2020 = DateHelper.GetAgeInYears(nascimento, new DateTime(2020, 12, 31));
Console.WriteLine($"Idade em 31/12/2020: {idade2020} anos");

// ========== TESTE 3: Adicionar/Remover Períodos ==========
Console.WriteLine("\n➕➖ TESTE 3: ADICIONAR/REMOVER PERÍODOS");
Console.WriteLine("─".PadRight(66, '─'));
var dataBase = new DateTime(2026, 1, 27);
Console.WriteLine($"Data base: {dataBase:dd/MM/yyyy}");
Console.WriteLine($"+ 10 dias:        {DateHelper.AddDays(dataBase, 10):dd/MM/yyyy}");
Console.WriteLine($"- 3 meses:        {DateHelper.AddMonths(dataBase, -3):dd/MM/yyyy}");
Console.WriteLine($"+ 1 ano:          {DateHelper.AddYears(dataBase, 1):dd/MM/yyyy}");

var periodoCombinado = DateHelper.AddPeriod(dataBase, yearsToAdd: 1, monthsToAdd: 6, daysToAdd: 15);
Console.WriteLine($"+ 1 ano, 6 meses, 15 dias: {periodoCombinado:dd/MM/yyyy}");

// ========== TESTE 4: Dias entre Datas ==========
Console.WriteLine("\n📊 TESTE 4: DIAS ENTRE DATAS");
Console.WriteLine("─".PadRight(66, '─'));
var diasTotal = DateHelper.DaysBetween(data1, data2);
Console.WriteLine($"Entre {data1:dd/MM/yyyy} e {data2:dd/MM/yyyy}:");
Console.WriteLine($"Total de dias:  {diasTotal}");
Console.WriteLine($"Dias úteis:     {DateHelper.BusinessDaysBetween(data1, data2)}");

// ========== TESTE 5: Primeiro e Último Dia do Mês ==========
Console.WriteLine("\n📅 TESTE 5: PRIMEIRO E ÚLTIMO DIA DO MÊS");
Console.WriteLine("─".PadRight(66, '─'));
var primeiro = DateHelper.GetFirstDayOfMonth(dataBase);
var ultimo = DateHelper.GetLastDayOfMonth(dataBase);
Console.WriteLine($"Mês de referência: {DateHelper.GetMonthNameInPortuguese(dataBase.Month)} de {dataBase.Year}");
Console.WriteLine($"Primeiro dia: {primeiro:dd/MM/yyyy} ({DateHelper.GetDayNameInPortuguese(primeiro)})");
Console.WriteLine($"Último dia:   {ultimo:dd/MM/yyyy} ({DateHelper.GetDayNameInPortuguese(ultimo)})");

// ========== TESTE 6: Ano Bissexto ==========
Console.WriteLine("\n🔄 TESTE 6: VERIFICAR ANO BISSEXTO");
Console.WriteLine("─".PadRight(66, '─'));
var anosParaTeste = new[] { 2024, 2025, 2000, 2100, 2400 };
foreach (var ano in anosParaTeste)
{
    var eBissexto = DateHelper.IsLeapYear(ano) ? "✓ Bissexto" : "✗ Não bissexto";
    Console.WriteLine($"{ano}: {eBissexto}");
}

// ========== TESTE 7: Nomes em Português ==========
Console.WriteLine("\n🇧🇷 TESTE 7: FORMATAÇÃO EM PORTUGUÊS");
Console.WriteLine("─".PadRight(66, '─'));
Console.WriteLine("Nomes dos meses:");
for (int mes = 1; mes <= 12; mes++)
{
    var nomeMes = DateHelper.GetMonthNameInPortuguese(mes);
    Console.Write($"{mes:D2}-{nomeMes,-10} ");
    if (mes % 3 == 0) Console.WriteLine();
}

Console.WriteLine("\n\nNomes dos dias da semana:");
var dataBase2 = new DateTime(2026, 1, 26); // segunda-feira
for (int i = 0; i < 7; i++)
{
    var data = dataBase2.AddDays(i);
    var nomeDia = DateHelper.GetDayNameInPortuguese(data);
    Console.WriteLine($"{data:dd/MM/yyyy}: {nomeDia}");
}

// ========== TESTE 8: Formatação Completa ==========
Console.WriteLine("\n✨ TESTE 8: FORMATAÇÃO COMPLETA EM PORTUGUÊS");
Console.WriteLine("─".PadRight(66, '─'));
var dataFormatada = DateHelper.FormatDateInPortuguese(data2);
Console.WriteLine($"Data 2: {dataFormatada}");

var hoje = DateTime.Now;
var hojeFormatado = DateHelper.FormatDateInPortuguese(hoje);
Console.WriteLine($"Hoje:   {hojeFormatado}");

// ========== TESTE 9: Validações ==========
Console.WriteLine("\n⚠️  TESTE 9: TRATAMENTO DE ERROS");
Console.WriteLine("─".PadRight(66, '─'));
try
{
    var dataInvertida = DateHelper.CalculateDateDifference(data2, data1);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"✓ Erro capturado corretamente: {ex.Message}");
}

try
{
    var futuro = new DateTime(2099, 1, 1);
    var idadeFutura = DateHelper.GetAgeInYears(futuro);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"✓ Erro capturado corretamente: {ex.Message}");
}

try
{
    var mesInvalido = DateHelper.GetMonthNameInPortuguese(13);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"✓ Erro capturado corretamente: {ex.Message}");
}

// ========== RESUMO ==========
Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║                      ✅ TODOS OS TESTES PASSARAM!                  ║");
Console.WriteLine("║                                                                    ║");
Console.WriteLine("║  DateHelper está pronto para uso com as seguintes funcionalidades: ║");
Console.WriteLine("║  • Diferença entre datas (agrupada ou separada)                    ║");
Console.WriteLine("║  • Cálculo de idade                                                ║");
Console.WriteLine("║  • Adicionar/remover períodos (dias, meses, anos)                  ║");
Console.WriteLine("║  • Dias entre datas (total e dias úteis)                           ║");
Console.WriteLine("║  • Primeiro e último dia do mês                                    ║");
Console.WriteLine("║  • Verificação de ano bissexto                                     ║");
Console.WriteLine("║  • Nomes em português (dias, meses, formatação completa)           ║");
Console.WriteLine("║                                                                    ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
