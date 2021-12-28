using System.Diagnostics.CodeAnalysis;

namespace RabbitRelink;

/// <summary>
/// Frequently used function
/// </summary>
public static class Fn
{

    /// <summary>
    /// Identical object transformation
    /// </summary>
    /// <typeparam name="T">object type</typeparam>
    /// <returns>identical transformation function</returns>
    public static T Id<T>(T value) => value;

    /// <summary>
    /// No operation action with 2 arguments
    /// </summary>
    /// <param name="a">ignored parameter</param>
    /// <param name="b">ignored parameter</param>
    /// <typeparam name="T1">Type of 1 argument</typeparam>
    /// <typeparam name="T2">Type of 2 argument</typeparam>
    public static void NoOp2<T1, T2>(T1 a, T2 b) { }


    /// <summary>
    /// Throw ArgumentOutOfRangeException
    /// </summary>
    /// <param name="value">unknown case</param>
    /// <typeparam name="T">type of unknown case</typeparam>
    /// <typeparam name="TOut">return type</typeparam>
    /// <exception cref="ArgumentOutOfRangeException">always throw</exception>
    [DoesNotReturn]
    public static TOut ThrowUnknownCase<T, TOut>(T value) where T : notnull
        => throw new ArgumentOutOfRangeException(nameof(value), $"{value} of {value.GetType().Name} not recognized");
}
