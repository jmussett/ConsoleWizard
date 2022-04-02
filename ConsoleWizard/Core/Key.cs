namespace ConsoleWizard;

public partial class ConsoleWizard
{
    /// <summary>
    /// Asynchronously waits for a key to be entered by the user.
    /// </summary>
    /// <param name="onDisplay">The message to display to the user on each input attempt.</param>
    /// <param name="onInput">The callback to be executed in response to the input. Asynchronously returns a boolean to indicate success status.</param>
    /// <returns>The successfully completed task.</returns>
    public static async Task WaitForKeyAsync(Action onDisplay, Func<ConsoleKeyInfo, Task<bool>> onInput)
    {
        await WaitForActionAsync(onDisplay, () =>
        {
            var key = Console.ReadKey();
            Console.WriteLine();
            return key;
        }, onInput);
    }

    /// <summary>
    /// Asynchronously waits for a key with a valid reply to be performed by the user.
    /// </summary>
    /// <typeparam name="T">The type of the output returned by a valid reply.</typeparam>
    /// <param name="onDisplay">The message to display to the user on each input attempt.</param>
    /// <param name="onInput">The callback to be executed in response to the action. Asyncronously returns a result with a success status and output.</param>
    /// <returns>The successfully completed task, with resulting output.</returns>
    public static async Task<T> WaitForKeyAsync<T>(Action onDisplay, Func<WizardInput<ConsoleKeyInfo, T>, Task<WizardResult<T>>> onInput)
    {
        return await WaitForActionAsync(onDisplay, () =>
        {
            var key = Console.ReadKey();
            Console.WriteLine();
            return key;
        }, onInput);
    }

    /// <summary>
    /// Waits for a key with a valid reply to be performed by the user.
    /// </summary>
    /// <param name="onDisplay">The message to display to the user on each input attempt.</param>
    /// <param name="onInput">The callback to be executed in response to the action. Asynchronously returns a boolean to indicate success status.</param>
    public static void WaitForKey(Action onDisplay, Func<ConsoleKeyInfo, bool> onInput)
    {
        WaitForKeyAsync(onDisplay, x => Task.FromResult(onInput(x)))
            .ConfigureAwait(false).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Waits for a key with a valid reply to be performed by the user.
    /// </summary>
    /// <typeparam name="T">The type of the output returned by a valid reply./typeparam>
    /// <param name="onDisplay">The message to display to the user on each input attempt.</param>
    /// <param name="onInput">The callback to be executed in response to the action. Returns a result with a success status and output.</param>
    /// <returns>The resulting output.</returns>
    public static T WaitForKey<T>(Action onDisplay, Func<WizardInput<ConsoleKeyInfo, T>, WizardResult<T>> onInput)
    {
        return WaitForKeyAsync<T>(onDisplay, x => Task.FromResult(onInput(x)))
            .ConfigureAwait(false).GetAwaiter().GetResult();
    }

}
