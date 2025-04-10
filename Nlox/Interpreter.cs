﻿using Nlox.Exceptions;

namespace Nlox;

public class Interpreter : Expr.IVisitor<object?>, Stmt.IVisitor<object?>
{
    private readonly Lox _lox;
    
    private Environment _env = new();
    
    public Interpreter(Lox lox)
    {
        _lox = lox;
    }

    public bool ReplMode { get; set; }
    
    public void Interpret(List<Stmt> statements)
    {
        try
        {
            if (ReplMode && statements.Last() is Expression expr)
            {
                var value = Evaluate(expr.Expr);
                Console.WriteLine(Stringify(value));
                return;
            }

            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        catch (RuntimeException e)
        {
            _lox.RuntimeError(e);
        }
    }

    private string Stringify(object? obj) {
        if (obj == null)
        {
            return "nil";
        }

        if (obj is double) {
            var text = obj.ToString()!;
            if (text.EndsWith(".0")) {
                text = text.Substring(0, text.Length - 2);
            }
            return text;
        }

        return obj.ToString()!;
    }

    public object? Visit(Assign expr)
    {
        var value = Evaluate(expr.Value);
        _env.Assign(expr.Name, value);
        return value;
    }

    public object? Visit(Binary expr)
    {
        var left = Evaluate(expr.Left);
        var right = Evaluate(expr.Right);

        switch (expr.Op.Type)
        {
            case TokenType.Plus:
                return left switch
                {
                    string sLeft when right is string sRight => sLeft + sRight,
                    double dLeft when right is double dRight => dLeft + dRight,
                    _ => throw new RuntimeException(expr.Op, "Both operands must be strings or numbers.")
                };
            case TokenType.Minus:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left! - (double)right!;
            case TokenType.Star:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left! * (double)right!;
            case TokenType.Slash:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left! / (double)right!;
            case TokenType.Less:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left! < (double)right!;
            case TokenType.LessEqual:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left! <= (double)right!;
            case TokenType.Greater:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left! > (double)right!;
            case TokenType.GreaterEqual:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left! >= (double)right!;
            case TokenType.EqualEqual:
                return IsEqual(left, right);
            case TokenType.BangEqual:
                return !IsEqual(left, right);
            case TokenType.Comma:
                return right;
        }

        return null;
    }

    public object? Visit(Grouping expr)
    {
        return Evaluate(expr);
    }

    public object? Visit(Literal expr)
    {
        return expr.Value;
    }

    public object? Visit(Logical expr)
    {
        var left = Evaluate(expr.Left);

        // todo: replace left expression value with true/false
        return expr.Op.Type switch
        {
            TokenType.Or when IsTruthy(left) => left,
            TokenType.And when !IsTruthy(left) => left,
            _ => Evaluate(expr.Right)
        };
    }

    public object? Visit(Unary expr)
    {
        var value = Evaluate(expr.Right);

        switch (expr.Op.Type)
        {
            case TokenType.Minus:
                CheckNumberOperands(expr.Op, value);
                return -(double)value!;
            case TokenType.Bang:
                return !IsTruthy(value);
        }

        return null;
    }

    public object? Visit(Ternary expr)
    {
        var condition = Evaluate(expr.Condition);
        var left = Evaluate(expr.Left);
        var right = Evaluate(expr.Right);

        return IsTruthy(condition) ? left : right;
    }

    public object? Visit(Variable expr)
    {
        return _env.Read(expr.Name);
    }

    public object? Visit(If stmt)
    {
        if (IsTruthy(Evaluate(stmt.Condition)))
        {
            Execute(stmt.ThenBranch);
        }
        else if (stmt.ElseBranch is not null)
        {
            Execute(stmt.ElseBranch);
        }
        
        return null;
    }

    public object? Visit(Block stmt)
    {
        ExecuteBlock(stmt.Statements, new Environment(_env));
        return null;
    }

    private void ExecuteBlock(List<Stmt> statements, Environment env)
    {
        var previous = _env;
        try
        {
            _env = env;

            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        finally
        {
            _env = previous;
        }
    }

    public object? Visit(Expression stmt)
    {
        Evaluate(stmt.Expr);
        return null;
    }

    public object? Visit(Print stmt)
    {
        var value = Evaluate(stmt.Expr);
        Console.WriteLine(Stringify(value));
        return null;
    }

    public object? Visit(Var stmt)
    {
        if (!ReplMode && _env.IsDefined(stmt.Name.Lexeme))
        {
            throw new RuntimeException(stmt.Name, $"Variable '{stmt.Name.Lexeme}' already defined.");
        }
        
        object? value = null;
        if (stmt.Initializer is not null)
        {
            value = Evaluate(stmt.Initializer);
        }
        
        _env.Define(stmt.Name.Lexeme, value);
        return null;
    }

    private object? Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }

    private void Execute(Stmt statement)
    {
        statement.Accept(this);
    }

    private bool IsTruthy(object? value)
    {
        return value switch
        {
            null => false,
            bool b => b,
            _ => true
        };
    }

    private bool IsEqual(object? a, object? b)
    {
        return a switch
        {
            null when b == null => true,
            null => false,
            _ => a.Equals(b)
        };
    }

    private void CheckNumberOperands(Token op, params object?[] values)
    {
        if (values.All(x => x is double))
        {
            return;
        }

        throw new RuntimeException(op, "Operands must be numbers.");
    }
}