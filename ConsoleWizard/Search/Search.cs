namespace System;

public partial class ConsoleWizard
{
    /// <summary>
    /// Waits for the user to navigate and asynchronously search through a list of options.
    /// </summary>
    /// <typeparam name="T">The type of the options being search against.</typeparam>
    /// <param name="initialOptions">The initial options to display to the user.</param>
    /// <param name="searchEvaluator">The asynchronous evaluator used to search through the list, returning the matching options.</param>
    /// <param name="optionFormatter">The formatter used render the text to the console. Takes in the option and index, returning the formatted string.</param>
    /// <param name="maxResults">The maximum results that can be displayed in the consoleat once.</param>
    /// <returns>The option selected by the user.</returns>
    public static async Task<T> SearchAsync<T>(IEnumerable<T> initialOptions, Func<string, IEnumerable<T>, Task<IEnumerable<T>>> searchEvaluator, Func<T, int, string> optionFormatter, int maxResults = 100) where T : class
    {
        var result = await SearchUtils.SearchForSelectionAsync(
            initialOptions,
            searchEvaluator,
            x => true,
            x => true,
            optionFormatter,
            false,
            maxResults
        );

        return result.Value!;
    }

    /// <summary>
    /// Waits for the user to navigate and search through a list of options.
    /// </summary>
    /// <typeparam name="T">The type of the options being search against.</typeparam>
    /// <param name="initialOptions">The initial options to display to the user.</param>
    /// <param name="searchEvaluator">The evaluator used to search through the list, returning the matching options.</param>
    /// <param name="optionFormatter">The formatter used render the text to the console. Takes in the option and index, returning the formatted string.</param>
    /// <param name="maxResults">The maximum results that can be displayed in the consoleat once.</param>
    /// <returns>The option selected by the user.</returns>
    public static T Search<T>(IEnumerable<T> initialOptions, Func<string, IEnumerable<T>, IEnumerable<T>> searchEvaluator, Func<T, int, string> optionFormatter, int maxResults = 100) where T : class
    {
        return SearchAsync(
            initialOptions,
            (search, values) => Task.FromResult(searchEvaluator(search, values)),
            optionFormatter,
            maxResults
        ).ConfigureAwait(false).GetAwaiter().GetResult();
    }
}