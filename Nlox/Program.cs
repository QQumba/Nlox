namespace Nlox;

public static class Program
{
    public static void Main(string[] args)
    {
        var expression = "";
        while (expression != "exit")
        {
            Console.Write(">> ");
            expression = Console.ReadLine();

            if (string.IsNullOrEmpty(expression))
            {
                continue;
            }
            Parse(new TextSource(expression));
        }

        return;
        
        var fileContent = File.ReadAllText("C:/git/Nlox/sources/test.lox");
        var textSource = new TextSource(fileContent);
        
        Parse(textSource);
    }

    private static void Parse(TextSource textSource)
    {
        var lox = new Lox();
        
        var scanner = new Scanner(lox, textSource);
        var tokens = scanner.Scan();

        var parser = new Parser(lox, tokens);
        var ast = parser.Parse();
        
        if (lox.HadError)
        {
            foreach (var error in lox.Errors)
            {
                Console.WriteLine(error);
            }

            return;
        }

        Console.WriteLine(new AstPrinter().Print(ast!));
    }

    private static void Tokenize(TextSource textSource)
    {
        var lox = new Lox();
        
        var scanner = new Scanner(lox, textSource);
        var tokens = scanner.Scan();

        if (lox.HadError)
        {
            foreach (var error in lox.Errors)
            {
                Console.WriteLine(error);
            }
            return;
        }

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }

    private static void PrintTree()
    {
        var expression = new Binary(
            new Unary(
                new Token(TokenType.Minus, "-", null, 1),
                new Literal(123)),
            new Token(TokenType.Star, "*", null, 1),
            new Grouping(
                new Literal(45.67)));

        Console.WriteLine(new AstPrinter().Print(expression));
    }
}