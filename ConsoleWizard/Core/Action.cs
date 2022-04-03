namespace System;

public partial class ConsoleWizard
{
    /// <summary>
    /// Asynchronously waits for a specified action with a valid reply to be performed by the user.
    /// </summary>
    /// <typeparam name="I">The type of the input entered by the user.</typeparam>
    /// <param name="onDisplay">The message to display to the user on each input attempt.</param>
    /// <param name="action">The action being performed by the user, returned an input of type T</param>
    /// <param name="onAction">The callback to be executed in response to the action. Asynchronously returns a boolean to indicate success status.</param>
    /// <returns>The successfully completed task.</returns>
    public static async Task WaitForActionAsync<I>(Action onDisplay, Func<I> action, Func<I, Task<bool>> onAction)
    {
        while (true)
        {
            onDisplay();

            var input = action();

            if (await onAction(input))
            {
                return;
            }
        }
    }

    /// <summary>
    /// Asynchronously waits for a specified action with a valid reply to be performed by the user.
    /// </summary>
    /// <typeparam name="T">The type of the output returned by a valid reply./typeparam>
    /// <typeparam name="I">The type of the input entered by the user.</typeparam>
    /// <param name="onDisplay">The message to display to the user on each input attempt.</param>
    /// <param name="action">The action being performed by the user, returned an input of type T</param>
    /// <param name="onAction">The callback to be executed in response to the action. Asyncronously returns a result with a success status and output.</param>
    /// <returns>The successfully completed task, with resulting output.</returns>
    public static async Task<T> WaitForActionAsync<T, I>(Action onDisplay, Func<I> action, Func<WizardInput<I, T>, Task<WizardResult<T>>> onAction)
    {
        while (true)
        {
            onDisplay();

            var input = action();

            var result = await onAction(new WizardInput<I, T>(input));

            if (result.Success)
            {
                return result.Value!;
            }
        }
    }

    /// <summary>
    /// Waits for a specified action with a valid reply to be performed by the user.
    /// </summary>
    /// <typeparam name="I">The type of the input entered by the user.</typeparam>
    /// <param name="onDisplay">The message to display to the user on each input attempt.</param>
    /// <param name="action">The action being performed by the user, returned an input of type T</param>
    /// <param name="onAction">The callback to be executed in response to the action. Asynchronously returns a boolean to indicate success status.</param>
    public static void WaitForAction<I>(Action onDisplay, Func<I> action, Func<I, bool> onAction)
    {
        WaitForActionAsync(onDisplay, action, x => Task.FromResult(onAction(x)))
            .ConfigureAwait(false).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Waits for a specified action with a valid reply to be performed by the user.
    /// </summary>
    /// <typeparam name="T">The type of the output returned by a valid reply./typeparam>
    /// <typeparam name="I">The type of the input entered by the user.</typeparam>
    /// <param name="onDisplay">The message to display to the user on each input attempt.</param>
    /// <param name="action">The action being performed by the user, returned an input of type T</param>
    /// <param name="onAction">The callback to be executed in response to the action. Returns a result with a success status and output.</param>
    /// <returns>The resulting output.</returns>

    public static T WaitForAction<T, I>(Action onDisplay, Func<I> action, Func<WizardInput<I, T>, WizardResult<T>> onAction)
    {
        return WaitForActionAsync<T, I>(onDisplay, action, x => Task.FromResult(onAction(x)))
            .ConfigureAwait(false).GetAwaiter().GetResult();
    }
}
