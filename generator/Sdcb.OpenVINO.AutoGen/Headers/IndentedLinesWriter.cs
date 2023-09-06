using System.Text;

namespace Sdcb.OpenVINO.AutoGen.Headers;

public class IndentedLinesWriter
{
    public IndentedLinesWriter(string eachIndent = "    ")
    {
        _eachIdent = eachIndent;
    }

    private StringBuilder _currentLine = new StringBuilder();
    private List<string> _lines = new List<string>();
    private string _ident = "";
    private readonly string _eachIdent;

    public void WriteLine(string line)
    {
        _currentLine.Append(_ident).Append(line);
        _lines.Add(_currentLine.ToString());
        _currentLine.Clear();
    }

    public void WriteLine()
    {
        _lines.Add(_currentLine.ToString());
        _currentLine.Clear();
    }

    public void WriteLines(IEnumerable<string> lines)
    {
        foreach (string line in lines)
        {
            WriteLine(line);
        }
    }

    public void Write(string span)
    {
        _currentLine.Append(_ident).Append(span);
    }

    public IDisposable BeginIdent()
    {
        WriteLine("{");
        _ident += _eachIdent;
        return new IndentedGuard(this);
    }

    public IEnumerable<string> Lines
    {
        get
        {
            foreach (string line in _lines)
            {
                yield return line;
            }
            if (_currentLine.Length != 0)
            {
                yield return _currentLine.ToString();
            }
        }
    }

    public override string ToString() => string.Join(Environment.NewLine, Lines) + _currentLine.ToString();

    private sealed class IndentedGuard : IDisposable
    {
        private IndentedLinesWriter writer;

        public IndentedGuard(IndentedLinesWriter writer)
        {
            this.writer = writer;
        }

        public void Dispose()
        {
            writer._ident = writer._ident.Remove(writer._ident.Length - writer._eachIdent.Length);
            writer.WriteLine("}");
        }
    }
}
