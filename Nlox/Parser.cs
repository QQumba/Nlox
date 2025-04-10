﻿using Nlox.Exceptions;

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

        while (!IsAtEnd())
        {
            statements.Add(Declaration());
        }

        return statements;
    }

    private Stmt? Declaration()
    {
        try
        {
            if (Match(TokenType.Var))
            {
                return VarDeclaration();
            }

            return Statement();
        }
        catch (ParserException)
        {
            Synchronize();
            return null;
        }
    }

    private Stmt VarDeclaration()
    {
        var name = Consume(TokenType.Identifier, "Expect variable name.");
        Expr? initializer = null;
        
        if (Match(TokenType.Equal))
        {
            initializer = Expression();
        }
        
        Consume(TokenType.Semicolon, "Expect ';' after variable declaration.");
        return new Var(name, initializer);
    }

    private Stmt Statement()
    {
        if (Match(TokenType.If))
        {
            return IfStatement();
        }
        
        if (Match(TokenType.Print))
        {
            return PrintStatement();
        }

        if (Match(TokenType.LeftBrace))
        {
            return new Block(Block());
        }

        return ExpressionStatement();
    }

    private Stmt IfStatement()
    {
        Consume(TokenType.LeftParen, "Expected '(' after 'if'.");
        var condition = Expression();
        Consume(TokenType.RightParen, "Expected ')' after condition.");

        var thenBranch = Statement();
        Stmt? elseBranch = null;
        
        if (Match(TokenType.Else))
        {
            elseBranch = Statement();
        }
        
        return new If(condition, thenBranch, elseBranch);
    }

    private List<Stmt> Block()
    {
        var statements = new List<Stmt>();

        while (!Check(TokenType.RightBrace) && !IsAtEnd())
        {
            statements.Add(Declaration());
        }

        Consume(TokenType.RightBrace, "Expected '}' after block.");
        return statements;
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
        var expr = Assignment();
        
        while (Match(TokenType.Question))
        {
            var left = Expression();
            Consume(TokenType.Colon, "Expected ':' after expression.");
            var right = Expression();

            expr = new Ternary(expr, left, right);
        }

        return expr;
    }

    private Expr Assignment()
    {
        var left = Or();

        if (Match(TokenType.Equal))
        {
            var equals = Previous();
            var right = Assignment();

            if (left is Variable variable)
            {
                var name = variable.Name;
                return new Assign(name, right);
            }

            Error(equals, "Invalid assign target.");
        }
        
        return left;
    }

    private Expr Or()
    {
        var expr = And();

        while (Match(TokenType.Or))
        {
            var op = Previous();
            var right = And();
            expr = new Logical(expr, op, right);
        }

        return expr;
    }
    
    private Expr And()
    {
        var expr = Equality();

        while (Match(TokenType.And))
        {
            var op = Previous();
            var right = Equality();
            expr = new Logical(expr, op, right);
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

        if (Match(TokenType.Identifier))
        {
            return new Variable(Previous());
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
        if (Check(type))
        {
            return Advance();
        }

        throw Error(Peek(), error);
    }

    private bool Check(TokenType type)
    {
        return !IsAtEnd() && Peek().Type == type;
    }

    private bool Match(params TokenType[] types)
    {
        if (types.Any(Check))
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