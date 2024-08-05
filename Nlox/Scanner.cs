using System.Globalization;

namespace Nlox;

public class Scanner
{
    private readonly Lox _lox;
    private readonly TextSource _source;
    
    private readonly List<Token> _tokens = new();

    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        { "and", TokenType.And },
        { "class", TokenType.Class },
        { "else", TokenType.Else },
        { "false", TokenType.False },
        { "for", TokenType.For },
        { "fun", TokenType.Fun },
        { "if", TokenType.If },
        { "nil", TokenType.Nil },
        { "or", TokenType.Or },
        { "print", TokenType.Print },
        { "return", TokenType.Return },
        { "super", TokenType.Super },
        { "this", TokenType.This },
        { "true", TokenType.True },
        { "var", TokenType.Var },
        { "while", TokenType.While },
    };

    private int _line = 1; 
    
    public Scanner(Lox lox, TextSource source)
    {
        _source = source;
        _lox = lox;
    }

    public List<Token> Scan()
    {
        while (!_source.IsAtEnd())
        {
            ScanToken();
        }

        _tokens.Add(new Token(TokenType.Eof, "", null, _line));
        return _tokens;
    }

    private void ScanToken()
    {
        _source.NextToken();
        var c = _source.AdvanceChar();
        switch (c)
        {
            case '(':
                AddToken(TokenType.LeftParen);
                break;
            case ')':
                AddToken(TokenType.RightParen);
                break;
            case '{':
                AddToken(TokenType.LeftBrace);
                break;
            case '}':
                AddToken(TokenType.RightBrace);
                break;
            case ',':
                AddToken(TokenType.Comma);
                break;
            case '.':
                AddToken(TokenType.Dot);
                break;
            case '-':
                AddToken(TokenType.Minus);
                break;
            case '+':
                AddToken(TokenType.Plus);
                break;
            case ';':
                AddToken(TokenType.Semicolon);
                break;
            case ':':
                AddToken(TokenType.Colon);
                break;
            case '*':
                AddToken(TokenType.Star);
                break;
            case '?':
                AddToken(TokenType.Question);
                break;
            case '!':
                AddToken(_source.TryAdvance('=') ? TokenType.BangEqual : TokenType.Bang);
                break;
            case '=':
                AddToken(_source.TryAdvance('=') ? TokenType.EqualEqual : TokenType.Equal);
                break;
            case '<':
                AddToken(_source.TryAdvance('=') ? TokenType.LessEqual : TokenType.Less);
                break;
            case '>':
                AddToken(_source.TryAdvance('=') ? TokenType.GreaterEqual : TokenType.Greater);
                break;
            case '/':
                if (_source.TryAdvance('/'))
                {
                    while (_source.PeekChar() != '\n' && !_source.IsAtEnd())
                    {
                        _source.AdvanceChar();
                    }
                }
                else
                {
                    AddToken(TokenType.Slash);
                }
                break;
            case '\n':
                _line++;
                break;
            case '"':
                ScanStringLiteral();
                break;
            default:
                if (char.IsDigit(c))
                {
                    ScanNumberLiteral();
                }
                else if(IsValidIdentifierStartChar(c))
                {
                    ScanIdentifierOrKeyword();
                }
                break;
        }
    }

    private void ScanIdentifierOrKeyword()
    {
        while (IsValidIdentifierChar(_source.PeekChar()))
        {
            _source.AdvanceChar();
        }

        var type = Keywords.GetValueOrDefault(_source.CurrentLexeme, TokenType.Identifier);
        AddToken(type);
    }

    private void ScanNumberLiteral()
    {
        while (char.IsDigit(_source.PeekChar()))
        {
            _source.AdvanceChar();
        }

        if (_source.PeekChar() == '.' && char.IsDigit(_source.PeekChar(1)))
        {
            _source.AdvanceChar();
            
            while (char.IsDigit(_source.PeekChar()))
            {
                _source.AdvanceChar();
            }       
        }

        var value = double.Parse(_source.CurrentLexeme, CultureInfo.InvariantCulture);
        AddToken(TokenType.Number, value);
    }

    private void ScanStringLiteral()
    {
        while (_source.PeekChar() != '"' && !_source.IsAtEnd())
        {
            if (_source.PeekChar() == '\n')
            {
                _line++;
            }

            _source.AdvanceChar();
        }

        if (_source.IsAtEnd())
        {
            _lox.Error(_line, "Expected \"");
            return;
        }

        // get closing "
        _source.AdvanceChar();

        AddToken(TokenType.String, _source.CurrentStringLiteral);
    }

    private void AddToken(TokenType type, object? literal = null)
    {
        var text = _source.CurrentLexeme;
        _tokens.Add(new Token(type, text, literal, _line));
    }

    private bool IsValidIdentifierStartChar(char c)
    {
        return char.IsLetter(c) || c == '_';
    }
    
    private bool IsValidIdentifierChar(char c)
    {
        return char.IsLetterOrDigit(c) || c == '_';
    }
}