namespace ConsoleWizard;

/// <summary>
/// The result being returned back to the user following the input.
/// </summary>
/// <typeparam name="T">The type of result to return.</typeparam>
public class WizardResult<T>
{
    internal WizardResult(bool success, T? value)
    {
        Success = success;
        Value = value;
    }

    /// <summary>
    /// Whether the result has succeeded or failed.
    /// </summary>
    public bool Success { get; internal set; }

    /// <summary>
    /// The value belonging to the result.
    /// </summary>
    public T? Value { get; internal set; }
}