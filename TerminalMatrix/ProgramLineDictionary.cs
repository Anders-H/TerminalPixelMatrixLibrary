namespace TerminalMatrix;

public class ProgramLineDictionary : SortedDictionary<int, ProgramLine>
{
    public void InsertProgramLine(ProgramLine programLine)
    {
        Remove(programLine.LineNumber);
        Add(programLine.LineNumber, programLine);
    }

    public void RemoveProgramLine(int number)
    {
        Remove(number);
    }
}