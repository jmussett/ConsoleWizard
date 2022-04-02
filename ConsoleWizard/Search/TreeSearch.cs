namespace ConsoleWizard;

public partial class ConsoleWizard
{
    /// <summary>
    /// Allows the user to navigate and asynchronously search through a tree structure.
    /// </summary>
    /// <typeparam name="T">The type of the options being search against.</typeparam>
    /// <param name="initialOptions">The initial options to display to the user.</param>
    /// <param name="nextSelector">Selects the next node to navigate to in the tree.</param>
    /// <param name="previousSelector">Navigates to the previous node in the tree.</param>
    /// <param name="searchEvaluator">The asynchronous evaluator used to search through the list, returning the matching options.</param>
    /// <param name="valueValidator">Validates whether the selected node can be navigated to.</param>
    /// <param name="optionFormatter">The formatter used render the text to the console. Takes in the option and index, returning the formatted string.</param>
    /// <param name="maxResults">The maximum results that can be displayed in the consoleat once.</param>
    /// <returns>The option selected by the user.</returns>
    public static async Task<T?> SearchFromTreeAsync<T>(
        IEnumerable<T> initialOptions, Func<T, IEnumerable<T>, IEnumerable<T>> nextSelector, Func<T?, IEnumerable<T>, IEnumerable<T>> previousSelector,
        Func<string, IEnumerable<T>, Task<IEnumerable<T>>> searchEvaluator, Func<T, bool> valueValidator, Func<T, int, string> optionFormatter, int maxResults = 100)
    {
        var childSelection = await SearchUtils.SearchForSelectionAsync(initialOptions, searchEvaluator, valueValidator, x => true, optionFormatter, true, maxResults);

        var parentSelections = new Stack<SelectionResult<T>>();

        while (true)
        {
            switch (childSelection.Type)
            {
                case SelectionResultType.Previous:
                    if (!parentSelections.TryPop(out var parentSelection))
                        return default;

                    var previousList = previousSelector(parentSelection.Value, childSelection.AllValues);
                    childSelection = await SearchUtils.SearchForSelectionAsync(previousList, searchEvaluator, valueValidator, x => true, optionFormatter, true, maxResults, parentSelection?.Index ?? 0);
                    break;
                case SelectionResultType.Next:
                    parentSelections.Push(childSelection);
                    var nextList = nextSelector(childSelection.Value!, childSelection.AllValues);
                    childSelection = await SearchUtils.SearchForSelectionAsync(nextList, searchEvaluator, valueValidator, x => true, optionFormatter, true, maxResults);
                    break;
                case SelectionResultType.Selected:
                    return childSelection.Value!;
            }
        }
    }

    /// <summary>
    /// Allows the user to navigate and search through a tree structure.
    /// </summary>
    /// <typeparam name="T">The type of the options being search against.</typeparam>
    /// <param name="initialOptions">The initial options to display to the user.</param>
    /// <param name="nextSelector">Selects the next node to navigate to in the tree.</param>
    /// <param name="previousSelector">Navigates to the previous node in the tree.</param>
    /// <param name="searchEvaluator">The evaluator used to search through the list, returning the matching options.</param>
    /// <param name="valueValidator">Validates whether the selected node can be navigated to.</param>
    /// <param name="optionFormatter">The formatter used render the text to the console. Takes in the option and index, returning the formatted string.</param>
    /// <param name="maxResults">The maximum results that can be displayed in the consoleat once.</param>
    /// <returns>The option selected by the user.</returns>
    public static T? SearchFromTree<T>(
        IEnumerable<T> initialOptions, Func<T, IEnumerable<T>, IEnumerable<T>> nextSelector, Func<T?, IEnumerable<T>, IEnumerable<T>> previousSelector,
        Func<string, IEnumerable<T>, IEnumerable<T>> searchEvaluator, Func<T, bool> valueValidator, Func<T, int, string> optionFormatter, int maxResults = 100)
    {
        return SearchFromTreeAsync(
            initialOptions,
            nextSelector,
            previousSelector,
            (search, values) => Task.FromResult(searchEvaluator(search, values)),
            valueValidator,
            optionFormatter,
            maxResults
        ).ConfigureAwait(false).GetAwaiter().GetResult();
    }
}