namespace ConsoleWizard;

/// <summary>
/// The input from an action which is expecting a result of type TResult.
/// </summary>
/// <typeparam name="TInput">The type of the input entered from the action.</typeparam>
/// <typeparam name="TResult">The type of the result being returned.</typeparam>
public record WizardInput<TInput, TResult>
{
    internal WizardInput(TInput input)
    {
        Value = input;
    }

    /// <summary>
    /// The value of the input returned by the action.
    /// </summary>
    public TInput Value { get; private set; }

    /// <summary>
    /// Succeeds the input, returning the result back to the user.
    /// </summary>
    /// <param name="value">The value of the result being returned.</param>
    /// <returns>The successful result to return.</returns>
    public WizardResult<TResult> Succeed(TResult value)
    {
        return new WizardResult<TResult>(true, value);
    }

    /// <summary>
    /// Fails the input, returning the result back to the user.
    /// </summary>
    /// <returns>The unsuccessful result to return.</returns>
    public WizardResult<TResult> Fail()
    {
        return new WizardResult<TResult>(false, default);
    }
}
