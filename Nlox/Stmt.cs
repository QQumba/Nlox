namespace Nlox;

public abstract class Stmt
{
    public abstract T Accept<T>(IVisitor<T> visitor);

    public interface IVisitor<T>
    {
        public T Visit(Expression stmt);
        public T Visit(Print stmt);
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
