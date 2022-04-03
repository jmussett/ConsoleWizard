# Console Wizard

Console Wizard is a library for creating console-based wizards and application prototypes.

It provides the following types of console utility functions:

- Retrying actions in response to invalid user input.
- Automatic input validation for numeric data types.
- Searching flat and tree-based data structures.
- Searching for files and directories in a file system.
- Inspecting the properties and values of object instances.

## Basic Usage

For basic input validation, you can use switch statements to validate a list of opions:

``` C#
ConsoleWizard.WaitForDigitAsync(
    () =>
    {
        Console.WriteLine("Please select an option:");
        Console.WriteLine("1. Option 1");
        Console.WriteLine("2. Option 2");
        Console.WriteLine("3. Option 3");
    },
    result =>
    {
        switch (result)
        {
            case 1:
                // Do stuff
                return true;
            case 2:
                // Do stuff
                return true;
            case 3:
                // Do stuff
                return true;
            default:
                Console.WriteLine("Input invalid.");
                return false;
        }
    }
);
```
