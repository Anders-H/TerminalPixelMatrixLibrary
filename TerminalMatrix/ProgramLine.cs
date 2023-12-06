namespace TerminalMatrix;

public class ProgramLine
{
    public string RawString { get; }
    public int RowNumber { get; }
    public string Code { get; }

    public ProgramLine(string rawString, int rowNumber, string code)
    {
        RawString = rawString;
        RowNumber = rowNumber;
        Code = code;
    }
}