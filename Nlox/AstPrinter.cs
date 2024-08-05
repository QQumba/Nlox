using System.Text;

namespace Nlox;

public class AstPrinter : Expr.IVisitor<string>
{
    public string Print(Expr expr)
    {
        return expr.Accept(this);
    }
    
    public string Visit(Binary expr)
    {
        return Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
    }

    public string Visit(Grouping expr)
    {
        return Parenthesize("group", expr.Expr);
    }

    public string Visit(Literal expr)
    {
        return expr.Value is null ? "nil" : expr.Value.ToString()!;
    }

    public string Visit(Unary expr)
    {
        return Parenthesize(expr.Op.Lexeme, expr.Right);
    }

    public string Visit(Ternary expr)
    {
        return Parenthesize("?:", expr.Condition, expr.Left, expr.Right);
    }

    public string Visit(Variable expr)
    {
        throw new NotImplementedException();
    }

    private string Parenthesize(string tokenLexeme, params Expr[] expressions)
    {
        var sb = new StringBuilder();

        sb.Append('(').Append(tokenLexeme);
        foreach (var expr in expressions)
        {
            sb.Append(' ').Append(expr.Accept(this));
        }
        sb.Append(')');

        return sb.ToString();
    }
}