namespace QResurgence.SST.Utilities
{
    /// <summary>
    ///     Implements the container type which does not contain a value of <typeparamref name="T" />
    /// </summary>
    /// <typeparam name="T">The type not contained</typeparam>
    public class Nothing<T> : Maybe<T>
    {
    }
}