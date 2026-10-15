using System.ComponentModel;

namespace AiPlayground.Tools;

public static class CalculatorTools
{
    [Description("Adds two numbers and returns their sum. Use this tool when asked to add numbers.")]
    public static double Add(
        [Description("The first number to add.")] double first,
        [Description("The second number to add.")] double second)
    {
        return first + second;
    }
}
