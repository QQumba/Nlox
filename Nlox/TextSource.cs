namespace Nlox;

public class TextSource
{
    private const char InvalidCharacter = char.MaxValue;
    
    private readonly string _source;
    private int _start = 0;
    private int _current = 0;
    
    public TextSource(string source)
    {
        _source = source;
    }

    public string CurrentLexeme => _source.Substring(_start, _current - _start);
    public string CurrentStringLiteral => _source.Substring(_start + 1, _current - _start - 2);

    public bool IsAtEnd()
    {
        return _current >= _source.Length;
    }

    public void NextToken()
    {
        _start = _current;
    }
    
    public char PeekChar(int offset = 0)
    {
        if (_current + offset >= _source.Length)
        {
            return InvalidCharacter;
        }

        return _source[_current + offset];
    }
    
    public char AdvanceChar()
    {
        if (IsAtEnd())
        {
            return InvalidCharacter;
        }

        var c = _source[_current]; 
        _current++;
        
        return c;
    }

    public bool TryAdvance(char c)
    {
        if (IsAtEnd())
        {
            return false;
        }

        if (_source[_current] != c)
        {
            return false;
        }

        _current++;
        return true;
    }
}