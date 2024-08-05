using Nlox.Exceptions;

namespace Nlox;

public class Parser
{
    private readonly Lox _lox;
    private readonly List<Token> _tokens;

    private int _current = 0;
    
    public Parser(Lox lox, List<Token> tokens)
    {
        _lox = lox;
        _tokens = tokens;
    }

    public List<Stmt> Parse()
    {
        var statements = new List<Stmt>();

        try
        {
            while (!IsAtEnd())
            {
                statements.Add(Declaration());
            }

            return statements;
        }
        catch (ParserException)
        {
            return new List<Stmt>();
        }
    }

    private Stmt Declaration()
    {
        if (Match(TokenType.Var))
        {
            return VarDeclaration();
        }

        return Statement();
    }

    private Stmt VarDeclaration()
    {
        Consume(TokenType.Identifier, "Missing identifier.");
        while (Match(TokenType.Equal))
        {
            var expr = Expression();
        }
        
        Consume(TokenType.Semicolon, "Expect ';' after statement.");
        return null;
    }

    private Stmt Statement()
    {
        if (Match(TokenType.Print))
        {
            return PrintStatement();
        }

        return ExpressionStatement();
    }

    private Stmt PrintStatement()
    {
        var expr = Expression();
        Consume(TokenType.Semicolon, "Expect ';' after expression.");
        return new Print(expr);
    }

    private Stmt ExpressionStatement()
    {
        var expr = Expression();
        Consume(TokenType.Semicolon, "Expect ';' after expression.");
        return new Expression(expr);
    }

    private Expr Expression()
    {
        return Ternary();
    }

    private Expr Ternary()
    {
        var expr = Equality();

        while (Match(TokenType.Question))
        {
            var left = Expression();
            Consume(TokenType.Colon, "Expected ':' after expression.");
            var right = Expression();

            expr = new Ternary(expr, left, right);
        }

        return expr;
    }

    private Expr Equality()
    {
        var expr = Comparison();

        while (Match(TokenType.BangEqual, TokenType.EqualEqual))
        {
            var op = Previous();
            var right = Comparison();
            expr = new Binary(expr, op, right);
        }
        
        return expr;
    }

    private Expr Comparison()
    {
        var expr = Term();

        while (Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
        {
            var op = Previous();
            var right = Term();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Term()
    {
        var expr = Factor();

        while (Match(TokenType.Minus, TokenType.Plus))
        {
            var op = Previous();
            var right = Factor();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Factor()
    {
        var expr = Unary();

        while (Match(TokenType.Slash, TokenType.Star))
        {
            var op = Previous();
            var right = Unary();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Unary()
    {
        if (Match(TokenType.Bang, TokenType.Minus))
        {
            var op = Previous();
            var right = Unary();
            return new Unary(op, right);
        }

        return Primary();
    }

    private Expr Primary()
    {
        if (Match(TokenType.False))
        {
            return new Literal(false);
        }

        if (Match(TokenType.True))
        {
            return new Literal(true);
        }

        if (Match(TokenType.Nil))
        {
            return new Literal(null);
        }

        if (Match(TokenType.Number, TokenType.String))
        {
            return new Literal(Previous().Literal);
        }

        if (Match(TokenType.LeftParen))
        {
            var expr = Expression();
            while (Match(TokenType.Comma))
            {
                var op = Previous();
                var right = Expression();
                expr = new Binary(expr, op, right);
            }
            
            Consume(TokenType.RightParen, "Expect ')' after expression.");
            return expr;
        }

        throw Error(Peek(), "Expect expression.");
    }

    private void Synchronize() {
        Advance();

        while (!IsAtEnd()) {
            if (Previous().Type == TokenType.Semicolon) return;

            switch (Peek().Type) {
                case TokenType.Class:
                case TokenType.Fun:
                case TokenType.Var:
                case TokenType.For:
                case TokenType.If:
                case TokenType.While:
                case TokenType.Print:
                case TokenType.Return:
                    return;
            }

            Advance();
        }
    }
    
    private Token Consume(TokenType type, string error)
    {
        if (!IsAtEnd() && Peek().Type == type)
        {
            return Advance();
        }

        throw Error(Peek(), error);
    }

    private bool Match(params TokenType[] types)
    {
        if (types.Any(type => !IsAtEnd() && Peek().Type == type))
        {
            Advance();
            return true;
        }

        return false;
    }

    private bool IsAtEnd()
    {
        return Peek().Type == TokenType.Eof;
    }

    private Token Peek()
    {
        return _tokens[_current];
    }

    private Token Previous()
    {
        return _tokens[_current - 1];
    }

    private Token Advance()
    {
        if (!IsAtEnd())
        {
            _current++;
        }

        return Previous();
    }

    private ParserException Error(Token token, string error)
    {
        _lox.Error(token, error);
        throw new ParserException();
    }
}