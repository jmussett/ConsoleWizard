namespace System;

public partial class ConsoleWizard
{
    /// <summary>
    /// Asynchronously waits for an integer to be entered by the user.
    /// </summary>
    /// <param name="onDisplay">The message to display to the user on each input attempt.</param>
    /// <param name="onInput">The callback to be executed in response to the input. Asynchronously returns a boolean to indicate success status.</param>
    /// <returns>The successfully completed task.</returns>
    public static async Task WaitForIntegerAsync(Action onDisplay, Func<int, Task<bool>> onInput)
    {
        await WaitForInputAsync(onDisplay, async input =>
        {
            if (!int.TryParse(input, out var number))
            {
                Console.WriteLine("Invalid input, please try again.");
                return false;
            }

            return await onInput(number);
        });
    }

    /// <summary>
    /// Asynchronously waits for an integer with a valid reply to be performed by the user.
    /// </summary>
    /// <typeparam name="T">The type of the output returned by a valid reply.</typeparam>
    /// <param name="onDisplay">The message to display to the user on each input attempt.</param>
    /// <param name="onInput">The callback to be executed in response to the action. Asyncronously returns a result with a success status and output.</param>
    /// <returns>The successfully completed task, with resulting output.</returns>
    public static async Task<T> WaitForIntegerAsync<T>(Action onDisplay, Func<WizardInput<int, T>, Task<WizardResult<T>>> onInput)
    {
        return await WaitForInputAsync<T>(onDisplay, async input =>
        {
            if (!int.TryParse(input.Value, out var number))
            {
                Console.WriteLine("Invalid input, please try again.");
                return input.Fail();
            }

            return await onInput(new WizardInput<int, T>(number));
        });
    }

    /// <summary>
    /// Waits for an integer with a valid reply to be performed by the user.
    /// </summary>
    /// <param name="onDisplay">The message to display to the user on each input attempt.</param>
    /// <param name="onInput">The callback to be executed in response to the action. Asynchronously returns a boolean to indicate success status.</param>
    public static void WaitForInteger(Action onDisplay, Func<int, bool> onInput)
    {
        WaitForIntegerAsync(onDisplay, x => Task.FromResult(onInput(x)))
            .ConfigureAwait(false).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Waits for an integer with a valid reply to be performed by the user.
    /// </summary>
    /// <typeparam name="T">The type of the output returned by a valid reply./typeparam>
    /// <param name="onDisplay">The message to display to the user on each input attempt.</param>
    /// <param name="onInput">The callback to be executed in response to the action. Returns a result with a success status and output.</param>
    /// <returns>The resulting output.</returns>
    public static T WaitForInteger<T>(Action onDisplay, Func<WizardInput<int, T>, WizardResult<T>> onInput)
    {
        return WaitForIntegerAsync<T>(onDisplay, x => Task.FromResult(onInput(x)))
            .ConfigureAwait(false).GetAwaiter().GetResult();
    }
}
