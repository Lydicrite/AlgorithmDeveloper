using System;
using LogicalExpressions.Parsing;
using LogicalExpressions.Core.Nodes;

public class TypeCheck
{
    public static void Main()
    {
        var node = ExpressionParser.Parse("A & B | !C");
        PrintNode(node);
    }

    static void PrintNode(LogicNode node, int indent = 0)
    {
        string pad = new string(' ', indent * 2);
        Console.WriteLine($"{pad}{node.GetType().Name}");
        
        foreach (var prop in node.GetType().GetProperties())
        {
            if (typeof(LogicNode).IsAssignableFrom(prop.PropertyType))
            {
                Console.WriteLine($"{pad}- {prop.Name}:");
                PrintNode((LogicNode)prop.GetValue(node), indent + 1);
            }
            else if (prop.Name == "Name" || prop.Name == "Value")
            {
                 Console.WriteLine($"{pad}- {prop.Name}: {prop.GetValue(node)}");
            }
        }
    }
}
