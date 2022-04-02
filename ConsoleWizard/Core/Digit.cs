namespace ConsoleWizard;

public partial class ConsoleWizard
{
    /// <summary>
    /// Asynchronously waits for a digit to be entered by the user.
    /// </summary>
    /// <param name="onDisplay">The message to display to the user on each input attempt.</param>
    /// <param name="onInput">The callback to be executed in response to the input. Asynchronously returns a boolean to indicate success status.</param>
    /// <returns>The successfully completed task.</returns>
    public static async Task WaitForDigitAsync(Action onDisplay, Func<byte, Task<bool>> onInput)
    {
        await WaitForKeyAsync(onDisplay, async input =>
        {
            if (!byte.TryParse(input.KeyChar.ToString(), out var number))
            {
                Console.WriteLine("Invalid input, please try again.");
                return false;
            }

            return await onInput(number);
        });
    }

    /// <summary>
    /// Asynchronously waits for a digit with a valid reply to be performed by the user.
    /// </summary>
    /// <typeparam name="T">The type of the output returned by a valid reply.</typeparam>
    /// <param name="onDisplay">The message to display to the user on each input attempt.</param>
    /// <param name="onInput">The callback to be executed in response to the action. Asyncronously returns a result with a success status and output.</param>
    /// <returns>The successfully completed task, with resulting output.</returns>
    public static async Task<T> WaitForDigitAsync<T>(Action onDisplay, Func<WizardInput<ushort, T>, Task<WizardResult<T>>> onInput)
    {
        return await WaitForKeyAsync<T>(onDisplay, async input =>
        {
            if (!ushort.TryParse(input.Value.KeyChar.ToString(), out var number))
            {
                Console.WriteLine("Invalid input, please try again.");
                return input.Fail();
            }

            return await onInput(new WizardInput<ushort, T>(number));
        });
    }

    /// <summary>
    /// Waits for a digit with a valid reply to be performed by the user.
    /// </summary>
    /// <param name="onDisplay">The message to display to the user on each input attempt.</param>
    /// <param name="onInput">The callback to be executed in response to the action. Asynchronously returns a boolean to indicate success status.</param>
    public static void WaitForDigit(Action onDisplay, Func<ushort, bool> onInput)
    {
        WaitForDigitAsync(onDisplay, x => Task.FromResult(onInput(x)))
            .ConfigureAwait(false).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Waits for a digit with a valid reply to be performed by the user.
    /// </summary>
    /// <typeparam name="T">The type of the output returned by a valid reply./typeparam>
    /// <param name="onDisplay">The message to display to the user on each input attempt.</param>
    /// <param name="onInput">The callback to be executed in response to the action. Returns a result with a success status and output.</param>
    /// <returns>The resulting output.</returns>
    public static T WaitForDigit<T>(Action onDisplay, Func<WizardInput<ushort, T>, WizardResult<T>> onInput)
    {
        return WaitForDigitAsync<T>(onDisplay, x => Task.FromResult(onInput(x)))
            .ConfigureAwait(false).GetAwaiter().GetResult();
    }

}
