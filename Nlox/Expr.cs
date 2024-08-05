namespace Nlox;

public abstract class Expr
{
    public abstract T Accept<T>(IVisitor<T> visitor);
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
    public Grouping(Expr expression)
    {
        Expression = expression;
    }

    public Expr Expression { get; set; }

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

public interface IVisitor<T>
{
    public T Visit(Binary expr);
    public T Visit(Grouping expr);
    public T Visit(Literal expr);
    public T Visit(Unary expr);
    public T Visit(Ternary expr);
}
