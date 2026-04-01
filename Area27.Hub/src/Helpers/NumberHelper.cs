using System;
using System.Globalization;

namespace Area27.Hub.Helpers;

/// <summary>
/// Métodos utilitários para manipulação de números.
/// </summary>
public static class NumberHelper
{
    /// <summary>
    /// Arredonda um número decimal para a quantidade de casas decimais especificada.
    /// </summary>
    public static decimal Arredondar(decimal valor, int casasDecimais)
    {
        return Math.Round(valor, casasDecimais);
    }

    /// <summary>
    /// Trunca um número decimal para a quantidade de casas decimais especificada.
    /// </summary>
    public static decimal Truncar(decimal valor, int casasDecimais)
    {
        var fator = (decimal)Math.Pow(10, casasDecimais);
        return Math.Truncate(valor * fator) / fator;
    }

    /// <summary>
    /// Converte um valor para moeda brasileira formatada.
    /// </summary>
    public static string ParaMoeda(decimal valor)
    {
        return valor.ToString("C", new CultureInfo("pt-BR"));
    }
}