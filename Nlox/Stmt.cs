using System.Collections.Generic;

namespace Nlox;

public abstract class Stmt
{
    public abstract T Accept<T>(IVisitor<T> visitor);

    public interface IVisitor<T>
    {
        public T Visit(Block stmt);
        public T Visit(Expression stmt);
        public T Visit(Print stmt);
        public T Visit(Var stmt);
    }
}

public class Block : Stmt
{
    public Block(List<Stmt> statements)
    {
        Statements = statements;
    }

    public List<Stmt> Statements { get; set; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}

public class Expression : Stmt
{
    public Expression(Expr expr)
    {
        Expr = expr;
    }

    public Expr Expr { get; set; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}

public class Print : Stmt
{
    public Print(Expr expr)
    {
        Expr = expr;
    }

    public Expr Expr { get; set; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}

public class Var : Stmt
{
    public Var(Token name, Expr? initializer)
    {
        Name = name;
        Initializer = initializer;
    }

    public Token Name { get; set; }
    public Expr? Initializer { get; set; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
