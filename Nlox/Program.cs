namespace Nlox;

public static class Program
{
    public static void Main(string[] args)
    {
        StartRepl();
    }

    private static void StartRepl()
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
            Interpret(new TextSource(expression));
        }
    }

    private static void ProcessFile()
    {
        var fileContent = File.ReadAllText("C:/git/Nlox/sources/test.lox");
        var textSource = new TextSource(fileContent);
        
        Interpret(textSource);
    }

    private static void Interpret(TextSource textSource)
    {
        var lox = new Lox();
        
        var scanner = new Scanner(lox, textSource);
        var tokens = scanner.Scan();

        var parser = new Parser(lox, tokens);
        var statements = parser.Parse();
        
        if (lox.HadError)
        {
            foreach (var error in lox.Errors)
            {
                Console.WriteLine(error);
            }

            return;
        }
        
        var interpreter = new Interpreter(lox);
        interpreter.Interpret(statements);
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