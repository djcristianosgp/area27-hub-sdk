using System;

namespace Area27.Hub;

/// <summary>
/// Provides basic arithmetic operations with guard rails for invalid input.
/// </summary>
public class Calculadora
{
    /// <summary>
    /// Adds two integers.
    /// </summary>
    /// <param name="a">First addend.</param>
    /// <param name="b">Second addend.</param>
    /// <returns>The sum of <paramref name="a"/> and <paramref name="b"/>.</returns>
    public int Somar(int a, int b) => a + b;

    /// <summary>
    /// Subtracts the second integer from the first.
    /// </summary>
    /// <param name="a">Minuend.</param>
    /// <param name="b">Subtrahend.</param>
    /// <returns>The difference of <paramref name="a"/> and <paramref name="b"/>.</returns>
    public int Subtrair(int a, int b) => a - b;

    /// <summary>
    /// Multiplies two integers.
    /// </summary>
    /// <param name="a">First factor.</param>
    /// <param name="b">Second factor.</param>
    /// <returns>The product of <paramref name="a"/> and <paramref name="b"/>.</returns>
    public int Multiplicar(int a, int b) => a * b;

    /// <summary>
    /// Divides the first integer by the second.
    /// </summary>
    /// <param name="a">Dividend.</param>
    /// <param name="b">Divisor.</param>
    /// <returns>The quotient of <paramref name="a"/> divided by <paramref name="b"/>.</returns>
    /// <exception cref="DivideByZeroException">Thrown when <paramref name="b"/> is zero.</exception>
    public int Dividir(int a, int b)
    {
        if (b == 0)
        {
            throw new DivideByZeroException("Cannot divide by zero.");
        }

        return a / b;
    }
}
