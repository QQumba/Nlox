using Nlox.Exceptions;

namespace Nlox;

public class Environment
{
    private readonly Environment? _enclosing;
    private readonly Dictionary<string, object?> _variables = new();

    public Environment()
    {
        _enclosing = null;
    }
    
    public Environment(Environment enclosing)
    {
        _enclosing = enclosing;
    }

    public void Define(string name, object? value)
    {
        _variables[name] = value;
    }

    public bool IsDefined(string name)
    {
        return _variables.ContainsKey(name);
    }
    
    public object? Read(Token name)
    {
        if (_variables.TryGetValue(name.Lexeme, out var value))
        {
            return value;
        }
        
        if (_enclosing is not null)
        {
            return _enclosing.Read(name);
        }

        throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
    }

    public void Assign(Token name, object? value)
    {
        if (_variables.ContainsKey(name.Lexeme))
        {
            _variables[name.Lexeme] = value;
            return;
        }

        if (_enclosing is not null)
        {
            _enclosing.Assign(name, value);
            return;
        }

        throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
    }
}