namespace Nlox;

public class Token
{
    public Token(TokenType type, string lexeme, object? literal, int line)
    {
        Type = type;
        Lexeme = lexeme;
        Literal = literal;
        Line = line;
    }

    public TokenType Type { get; set; }
    public string Lexeme { get; set; }
    public object? Literal { get; set; }
    public int Line { get; set; }

    public override string ToString()
    {
        return $"{Type,10} {Lexeme,10} {Literal,10}";
    }
}

public enum TokenType
{
    Var,
    Equal,
    Semicolon,
    Colon,
    Eof,
    LeftParen,
    RightParen,
    LeftBrace,
    RightBrace,
    Comma,
    Dot,
    Minus,
    Plus,
    Star,
    BangEqual,
    Bang,
    EqualEqual,
    LessEqual,
    Less,
    Greater,
    GreaterEqual,
    Slash,
    String,
    Number,
    Identifier,
    And,
    Class,
    Else,
    False,
    For,
    Fun,
    If,
    Nil,
    Or,
    Print,
    Return,
    Super,
    This,
    True,
    While,
    Question
}