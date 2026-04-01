using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Area27.Hub.Helpers;

/// <summary>
/// Métodos utilitários para manipulação de strings.
/// </summary>
public static class StringHelper
{
    /// <summary>
    /// Remove acentuação de uma string.
    /// </summary>
    public static string RemoverAcentos(string texto)
    {
        if (string.IsNullOrEmpty(texto)) return texto;
        var normalized = texto.Normalize(NormalizationForm.FormD);
        var regex = new Regex("[^\u0000-\u007F]+", RegexOptions.None);
        return regex.Replace(normalized, string.Empty);
    }

    /// <summary>
    /// Retorna apenas os dígitos de uma string.
    /// </summary>
    public static string SomenteDigitos(string texto)
    {
        if (string.IsNullOrEmpty(texto)) return texto;
        return Regex.Replace(texto, "[^0-9]", "");
    }

    /// <summary>
    /// Aplica uma máscara a uma string (ex: CPF, CNPJ).
    /// </summary>
    public static string AplicarMascara(string valor, string mascara)
    {
        if (string.IsNullOrEmpty(valor) || string.IsNullOrEmpty(mascara)) return valor;
        int i = 0;
        var resultado = string.Empty;
        foreach (var m in mascara)
        {
            if (m == '#')
            {
                if (i < valor.Length)
                    resultado += valor[i++];
            }
            else
            {
                resultado += m;
            }
        }
        return resultado;
    }
}