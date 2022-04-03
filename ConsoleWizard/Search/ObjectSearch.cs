using System.Collections;

namespace System;

public partial class ConsoleWizard
{
    /// <summary>
    /// Waits for the user to navigate and search through an instance of an object. This includes nested properties and their values. 
    /// </summary>
    /// <typeparam name="T">The type of the object to naigate through.</typeparam>
    /// <param name="value">The value of the object we want to navigate throgh</param>
    public static void InspectObject<T>(T value)
    {
        var childSelection = SearchObject(value, 0);

        if (childSelection == null) return;

        var parentSelections = new Stack<SelectionResult<object?>>();

        parentSelections.Push(new SelectionResult<object?>
        {
            Value = value,
            Index = 0,
            AllValues = new List<object?>(),
            Type = SelectionResultType.Next
        });

        while (true)
        {
            if (childSelection == null)
                return;

            switch (childSelection.Type)
            {
                case SelectionResultType.Previous:

                    if (!parentSelections.TryPop(out var parentSelection))
                        return;

                    if (!parentSelections.TryPeek(out var grandParentSelection))
                        return;

                    childSelection = SearchObject(grandParentSelection.Value, parentSelection.Index);

                    break;

                case SelectionResultType.Next:

                    parentSelections.Push(childSelection);

                    childSelection = SearchObject(childSelection.Value, 0);

                    break;

                case SelectionResultType.Selected:

                    break;
            }
        }
    }

    private static SelectionResult<object?>? SearchObject(object? value, int? initialSelection)
    {
        if (value == null)
            return SearchUtils.SearchSingleValue<object?>(null, "Null");

        var type = value.GetType();

        if (type == typeof(string))
            return SearchUtils.SearchSingleValue<object?>(value);

        if (typeof(IEnumerable).IsAssignableFrom(type))
        {
            var list = ((IEnumerable)value).Cast<object?>();

            if (!list.Any())
                return SearchUtils.SearchSingleValue<object?>(null, "Empty");

            return SearchUtils.SearchForSelection(
                list,
                (search, all) => all.Where(x => (x?.ToString() ?? string.Empty).ToLower().Contains(search.ToLower())),
                x => false,
                x => true,
                (x, i) => $"{i}. {x?.ToString() ?? string.Empty}",
                true,
                100,
                initialSelection ?? 0
             );
        }

        if (type.IsValueType)
            return SearchUtils.SearchSingleValue<object?>(value);

        var initialOptions = type
            .GetProperties()
            .ToDictionary(x => x, x => x.GetValue(value));

        var childSelection = SearchUtils.SearchForSelection(
            initialOptions,
            (search, all) => all.Where(x => x.Key.Name.ToLower().Contains(search.ToLower())),
            x => false,
            x => true,
            (x, i) => $"{x.Key.Name} - {x.Value}",
            true,
            100,
            initialSelection ?? 0
         );

        Console.Clear();

        return new SelectionResult<object?>
        {
            Index = childSelection.Index,
            AllValues = childSelection.AllValues.Select(x => x.Value),
            Type = childSelection.Type,
            Value = childSelection.Value.Value
        };
    }
}