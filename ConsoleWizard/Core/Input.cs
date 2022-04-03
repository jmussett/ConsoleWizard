namespace System;

public partial class ConsoleWizard
{
    /// <summary>
    /// Asynchronously waits for an input to be entered by the user.
    /// </summary>
    /// <param name="onDisplay">The message to display to the user on each input attempt.</param>
    /// <param name="onInput">The callback to be executed in response to the input. Asynchronously returns a boolean to indicate success status.</param>
    /// <returns>The successfully completed task.</returns>
    public static async Task WaitForInputAsync(Action onDisplay, Func<string, Task<bool>> onInput)
    {
        await WaitForActionAsync(onDisplay, () => Console.ReadLine() ?? string.Empty, onInput);
    }

    /// <summary>
    /// Asynchronously waits for an input with a valid reply to be performed by the user.
    /// </summary>
    /// <typeparam name="T">The type of the output returned by a valid reply.</typeparam>
    /// <param name="onDisplay">The message to display to the user on each input attempt.</param>
    /// <param name="onInput">The callback to be executed in response to the action. Asyncronously returns a result with a success status and output.</param>
    /// <returns>The successfully completed task, with resulting output.</returns>
    public static async Task<T> WaitForInputAsync<T>(Action onDisplay, Func<WizardInput<string, T>, Task<WizardResult<T>>> onInput)
    {
        return await WaitForActionAsync(onDisplay, () => Console.ReadLine() ?? string.Empty, onInput);
    }

    /// <summary>
    /// Waits for an input with a valid reply to be performed by the user.
    /// </summary>
    /// <param name="onDisplay">The message to display to the user on each input attempt.</param>
    /// <param name="onInput">The callback to be executed in response to the action. Asynchronously returns a boolean to indicate success status.</param>
    public static void WaitForInput(Action onDisplay, Func<string, bool> onInput)
    {
        WaitForInputAsync(onDisplay, x => Task.FromResult(onInput(x)))
            .ConfigureAwait(false).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Waits for an input with a valid reply to be performed by the user.
    /// </summary>
    /// <typeparam name="T">The type of the output returned by a valid reply./typeparam>
    /// <param name="onDisplay">The message to display to the user on each input attempt.</param>
    /// <param name="onInput">The callback to be executed in response to the action. Returns a result with a success status and output.</param>
    /// <returns>The resulting output.</returns>
    public static T WaitForInput<T>(Action onDisplay, Func<WizardInput<string, T>, WizardResult<T>> onInput)
    {
        return WaitForInputAsync<T>(onDisplay, x => Task.FromResult(onInput(x)))
            .ConfigureAwait(false).GetAwaiter().GetResult();
    }
}