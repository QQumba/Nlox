// See https://aka.ms/new-console-template for more information

using System.Text;

if (args.Length != 1)
{
    Console.Error.WriteLine("Provide output dir");
    Environment.Exit(64);
}

var outputDir = args[0];
List<string> types =
[
    "Binary   : Expr left, Token op, Expr right",
    "Grouping : Expr expression",
    "Literal  : object? value",
    "Unary    : Token op, Expr right",
    "Ternary  : Expr condition, Expr left, Expr right",
];
AstBuilder.DefineAst(outputDir, "Expr", types);

public static class AstBuilder
{
    public static void DefineAst(string outputDir, string baseName, List<string> types)
    {
        var path = Path.Combine(outputDir, baseName + ".cs");

        using var sw = new StreamWriter(path);
        sw.WriteLine("namespace Nlox;");
        sw.WriteLine();
        sw.WriteLine($"public abstract class {baseName}");
        sw.WriteLine("{");
        sw.WriteLine("public abstract T Accept<T>(IVisitor<T> visitor);".LeftPad());
        sw.WriteLine("}");
        
        foreach (var type in types) 
        {
            sw.WriteLine();
            var className = type.Split(':')[0].Trim();
            var fields = type.Split(':')[1].Trim(); 
            DefineType(sw, baseName, className, fields);
        }

        DefineVisitor(sw, baseName, types);
    }

    private static void DefineVisitor(StreamWriter sw, string baseName, List<string> types)
    {
        sw.WriteLine();
        sw.WriteLine("public interface IVisitor<T>");
        sw.WriteLine("{");

        foreach (var type in types)
        {
            var typeName = type.Split(':')[0].Trim();
            sw.WriteLine($"public T Visit({typeName} {baseName.ToLower()});".LeftPad());
        }
        
        sw.WriteLine("}");
    }

    private static void DefineType(StreamWriter sw, string baseName, string className, string fieldsString)
    {
        var fields = fieldsString.Split(',');
        
        sw.WriteLine($"public class {className} : {baseName}");
        sw.WriteLine("{");
        sw.WriteLine($"public {className}({fieldsString})".LeftPad());
        sw.WriteLine("{".LeftPad());
        
        foreach (var field in fields)
        {
            var parts = field.Trim().Split(' ');
            
            var name = parts[1];

            var sb = new StringBuilder()
                .Append(char.ToUpper(parts[1][0]))
                .Append(parts[1][1..]);
            var pascalCaseName = sb.ToString();
            
            sw.WriteLine($"{pascalCaseName} = {name};".LeftPad(2));    
        }
        
        sw.WriteLine("}".LeftPad());
        sw.WriteLine();
        
        foreach (var field in fields)
        {
            var parts = field.Trim().Split(' ');
            
            var type = parts[0];

            var sb = new StringBuilder()
                .Append(char.ToUpper(parts[1][0]))
                .Append(parts[1][1..]);
            var pascalCaseName = sb.ToString();
            
            sw.WriteLine($"public {type} {pascalCaseName} {{ get; set; }}".LeftPad());
        }
        
        sw.WriteLine();
        sw.WriteLine("public override T Accept<T>(IVisitor<T> visitor)".LeftPad());
        sw.WriteLine("{".LeftPad());
        sw.WriteLine("return visitor.Visit(this);".LeftPad(2));
        sw.WriteLine("}".LeftPad());
        
        sw.WriteLine("}");
    }
}

public static class StringExtensions
{
    public static string LeftPad(this string value, int pad = 1)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < pad * 4; i++)
        {
            sb.Append(' ');
        }

        sb.Append(value);

        return sb.ToString();
    }
}

