using System.Collections.Generic;

namespace Nlox;

public abstract class Expr
{
    public abstract T Accept<T>(IVisitor<T> visitor);

    public interface IVisitor<T>
    {
        public T Visit(Assign expr);
        public T Visit(Binary expr);
        public T Visit(Grouping expr);
        public T Visit(Literal expr);
        public T Visit(Unary expr);
        public T Visit(Ternary expr);
        public T Visit(Variable expr);
    }
}

public class Assign : Expr
{
    public Assign(Token name, Expr value)
    {
        Name = name;
        Value = value;
    }

    public Token Name { get; set; }
    public Expr Value { get; set; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}

public class Binary : Expr
{
    public Binary(Expr left, Token op, Expr right)
    {
        Left = left;
        Op = op;
        Right = right;
    }

    public Expr Left { get; set; }
    public Token Op { get; set; }
    public Expr Right { get; set; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}

public class Grouping : Expr
{
    public Grouping(Expr expr)
    {
        Expr = expr;
    }

    public Expr Expr { get; set; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}

public class Literal : Expr
{
    public Literal(object? value)
    {
        Value = value;
    }

    public object? Value { get; set; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}

public class Unary : Expr
{
    public Unary(Token op, Expr right)
    {
        Op = op;
        Right = right;
    }

    public Token Op { get; set; }
    public Expr Right { get; set; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}

public class Ternary : Expr
{
    public Ternary(Expr condition, Expr left, Expr right)
    {
        Condition = condition;
        Left = left;
        Right = right;
    }

    public Expr Condition { get; set; }
    public Expr Left { get; set; }
    public Expr Right { get; set; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}

public class Variable : Expr
{
    public Variable(Token name)
    {
        Name = name;
    }

    public Token Name { get; set; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
