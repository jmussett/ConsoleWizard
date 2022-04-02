namespace ConsoleWizard;

internal class SearchUtils : AdapterProvider
{
    private SearchUtils() { }

    public static async Task<SelectionResult<T>> SearchForSelectionAsync<T>(
        IEnumerable<T> initialOptions, Func<string, IEnumerable<T>, Task<IEnumerable<T>>> searchEvaluator,
        Func<T, bool> valueValidator, Func<T, bool> nextValidator,
        Func<T, int, string> optionFormatter, bool allowNavigation, int maxResults = 100, int initialSelection = 0)
    {
        Console.Clear();

        string search = string.Empty;

        var searchCursor = Console.CursorTop;

        var searchResults = initialOptions.Select((x, i) => new SearchResult<T>
        {
            Value = x,
            Lines = 1,
            Index = i
        });

        searchResults = RenderSearchOptions(searchCursor, searchResults, optionFormatter, Console.CursorLeft);

        SearchResult<T>? searchResult = null;

        int? currentIndex = null;

        if (initialOptions.Any())
        {
            currentIndex = initialSelection;
            Console.CursorTop += initialSelection + 1;
            Console.CursorVisible = false;

            searchResult = SelectSearchResult(searchCursor, searchResults, optionFormatter);
        }

        var key = Console.ReadKey();

        while (true)
        {
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    if (allowNavigation)
                    {
                        return new SelectionResult<T>
                        {
                            Value = searchResult == null ? default : searchResult.Value,
                            Index = currentIndex,
                            AllValues = initialOptions,
                            Type = SelectionResultType.Previous
                        };
                    }

                    key = Console.ReadKey();
                    continue;

                case ConsoleKey.RightArrow:
                    if (Console.CursorTop == searchCursor)
                    {
                        key = Console.ReadKey();
                        continue;
                    }

                    if (allowNavigation && searchResult != null && nextValidator(searchResult.Value))
                    {
                        return new SelectionResult<T>
                        {
                            Value = searchResult == null ? default : searchResult.Value,
                            Index = currentIndex,
                            AllValues = initialOptions,
                            Type = SelectionResultType.Next
                        };
                    }

                    key = Console.ReadKey();
                    continue;

                case ConsoleKey.Enter:
                    if (Console.CursorTop == searchCursor)
                    {
                        key = Console.ReadKey();
                        continue;
                    }

                    if (searchResult != null && valueValidator(searchResult.Value))
                    {
                        Console.CursorVisible = true;
                        Console.Clear();

                        return new SelectionResult<T>
                        {
                            Value = searchResult.Value,
                            Index = currentIndex,
                            AllValues = initialOptions,
                            Type = SelectionResultType.Selected
                        };
                    }

                    key = Console.ReadKey();
                    continue;

                case ConsoleKey.UpArrow:
                    UnselectSearchResult(searchResult, optionFormatter);

                    if (Console.CursorTop <= searchCursor + 1)
                    {
                        Console.CursorTop = searchCursor + 1;
                    }
                    else if (Console.CursorTop > searchCursor + searchResults.Sum(x => x.Lines))
                    {
                        Console.CursorTop = searchCursor + searchResults.Sum(x => x.Lines);
                    }
                    else
                    {
                        var previousSearchResult = searchResult!.Index == 0 ? null : searchResults.ElementAt(searchResult!.Index - 1);
                        Console.CursorTop -= previousSearchResult?.Lines ?? 0;
                    }

                    currentIndex = Console.CursorTop - searchCursor - 1;

                    searchResult = SelectSearchResult(searchCursor, searchResults, optionFormatter);

                    key = Console.ReadKey();
                    continue;

                case ConsoleKey.DownArrow:
                    UnselectSearchResult(searchResult, optionFormatter);

                    if (Console.CursorTop < searchCursor + 1)
                    {
                        Console.CursorTop = searchCursor + 1;
                    }
                    else if (Console.CursorTop >= searchCursor + searchResults.Sum(x => x.Lines))
                    {
                        Console.CursorTop = searchCursor + searchResults.Sum(x => x.Lines);
                    }
                    else
                    {
                        Console.CursorTop += searchResult?.Lines ?? 0;
                    }

                    currentIndex = Console.CursorTop - searchCursor - 1;

                    searchResult = SelectSearchResult(searchCursor, searchResults, optionFormatter);

                    key = Console.ReadKey();
                    continue;

                case ConsoleKey.Backspace:
                    search = search.Length == 0 ? search : search.Remove(search.Length - 1, 1);

                    Console.CursorTop = searchCursor;

                    Console.Write(" \b");

                    currentIndex = null;
                    searchResult = null;
                    break;

                default:
                    search += key.KeyChar;
                    currentIndex = null;
                    searchResult = null;
                    break;
            }

            var navigating = Console.CursorTop != searchCursor;

            Console.CursorTop = searchCursor;
            var currentCursorLeft = Console.CursorLeft;

            for (var i = searchCursor + 1; i <= searchCursor + maxResults; i++)
            {
                Console.SetCursorPosition(0, i);

                var value = new string(' ', Console.WindowWidth);

                Console.Write(value);
            }

            Console.SetCursorPosition(currentCursorLeft, searchCursor);

            var newResults = await searchEvaluator(search, initialOptions);

            searchResults = newResults.Select((x, i) => new SearchResult<T>
            {
                Value = x,
                Lines = 1,
                Index = i
            });

            searchResults = RenderSearchOptions(searchCursor, searchResults, optionFormatter, currentCursorLeft);

            if (navigating)
            {
                Console.Write("\b" + key.KeyChar);
            }

            key = Console.ReadKey();
        };
    }

    internal static SelectionResult<T> SearchForSelection<T>(
        IEnumerable<T> initialOptions, Func<string, IEnumerable<T>, IEnumerable<T>> searchEvaluator,
        Func<T, bool> valueValidator, Func<T, bool> nextValidator,
        Func<T, int, string> optionFormatter, bool allowNavigation, int maxResults = 100, int initialSelection = 0)
    {
        return SearchForSelectionAsync(
            initialOptions,
            (search, values) => Task.FromResult(searchEvaluator(search, values)),
            valueValidator,
            nextValidator,
            optionFormatter,
            allowNavigation,
            maxResults,
            initialSelection
        ).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    internal static IEnumerable<SearchResult<T>> RenderSearchOptions<T>(int searchCursor, IEnumerable<SearchResult<T>> searchResults, Func<T, int, string> optionFormatter, int currentCursorLeft)
    {
        var newSearchResults = new List<SearchResult<T>>();

        Console.SetCursorPosition(0, searchCursor + 1);

        for (var i = 0; i < searchResults.Count(); i++)
        {
            var result = searchResults.ElementAt(i);
            var currentTop = Console.CursorTop;

            var formattedValue = optionFormatter(result.Value, i);
            Console.WriteLine(formattedValue);

            result.Lines = Console.CursorTop - currentTop;

            newSearchResults.Add(result);
        }

        Console.SetCursorPosition(currentCursorLeft, searchCursor);

        return newSearchResults;
    }

    internal static void UnselectSearchResult<T>(SearchResult<T>? currentResult, Func<T, int, string> optionFormatter)
    {
        if (currentResult == null)
            return;

        var currentLeft = Console.CursorLeft;

        Console.CursorLeft = 0;

        var formattedValue = optionFormatter(currentResult.Value, currentResult.Index);
        Console.WriteLine(formattedValue);

        Console.CursorTop -= currentResult.Lines;

        Console.CursorLeft = currentLeft;
    }

    internal static SearchResult<T>? SelectSearchResult<T>(int searchCursor, IEnumerable<SearchResult<T>> searchResults, Func<T, int, string> optionFormatter)
    {
        if (!searchResults.Any())
            return null;

        var currentLeft = Console.CursorLeft;

        Console.CursorLeft = 0;

        var searchIndex = Console.CursorTop - searchCursor;

        var currentIndex = searchCursor + 1;

        SearchResult<T> searchResult = null!;

        for (var i = 0; i < searchResults.Count(); i++)
        {
            searchResult = searchResults.ElementAt(i);
            currentIndex += searchResult.Lines;

            if (currentIndex > searchIndex) break;
        }

        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;

        var formattedValue = optionFormatter(searchResult.Value, searchResult.Index);
        Console.Write(formattedValue);

        Console.CursorTop -= searchResult.Lines - 1;

        Console.ResetColor();
        Console.CursorLeft = currentLeft;

        return searchResult;
    }

    internal static SelectionResult<T?> SearchSingleValue<T>(T value, string? display = null)
    {
        return SearchForSelection(
            new List<T?> { value },
            (search, all) => all.Where(x => (display ?? value?.ToString() ?? string.Empty).ToLower().Contains(search.ToLower())),
            x => false,
            x => false,
            (x, i) => (display ?? value?.ToString() ?? string.Empty),
            true,
            1
        );
    }
}
