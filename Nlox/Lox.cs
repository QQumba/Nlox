using Nlox.Exceptions;

namespace Nlox;

public class Lox
{
    public List<LoxError> Errors { get; } = [];
    public bool HadError { get; set; }
    public static bool HadRuntimeError { get; set; }

    public void Error(int line, string message)
    {
        Report(line, "", message);
    }

    public void Error(Token token, string message)
    {
        if (token.Type == TokenType.Eof)
        {
            Report(token.Line, "", message);
            return;
        }
        
        Report(token.Line, $" at '{token.Lexeme}'", message);
    }

    public void Error(Expr expr, string message)
    {
        Report(0, "", message);
    }

    private void Report(int line, string where, string message)
    {
        HadError = true;
        Errors.Add(new LoxError(line, where, message));
    }
    
    public void RuntimeError(RuntimeException exception) {
        Console.WriteLine($"{exception.Message} at\n[line {exception.Token.Line}]");
        HadRuntimeError = true;
    }
}

public class LoxError
{
    public LoxError(int line, string where, string message)
    {
        Line = line;
        Where = where;
        Message = message;
    }

    public int Line { get; set; }
    public string Where { get; set; }
    public string Message { get; set; }

    public override string ToString()
    {
        return $"[line {Line}] Error{Where}: {Message}";
    }
}