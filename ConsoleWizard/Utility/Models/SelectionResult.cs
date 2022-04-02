namespace ConsoleWizard;

internal class SelectionResult<T>
{
    public T? Value { get; set; }
    public int? Index { get; set; }
    public IEnumerable<T> AllValues { get; set; }
    public SelectionResultType Type { get; set; }
}
