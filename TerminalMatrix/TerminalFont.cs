namespace TerminalMatrix;

public class TerminalFont : List<CharacterPixelMatrix>
{
    public TerminalFont()
    {
        // 0: Space
        Add(new CharacterPixelMatrix(
            @",,,,,,,," +
            @",,,,,,,," +
            @",,,,,,,," +
            @",,,,,,,," +
            @",,,,,,,," +
            @",,,,,,,," +
            @",,,,,,,," +
            @",,,,,,,,"
        ));
        // 1: A
        Add(new CharacterPixelMatrix(
            @",,,**,,," +
            @",,****,," +
            @",**,,**," +
            @",**,,**," +
            @",******," +
            @",**,,**," +
            @",**,,**," +
            @",,,,,,,,"
        ));
        // 2: B
        Add(new CharacterPixelMatrix(
            @",*****,," +
            @",**,,**," +
            @",**,,**," +
            @",*****,," +
            @",**,,**," +
            @",**,,**," +
            @",*****,," +
            @",,,,,,,,"
        ));
        // 3: C
        Add(new CharacterPixelMatrix(
            @",,****,," +
            @",**,,**," +
            @",**,,,,," +
            @",**,,,,," +
            @",**,,,,," +
            @",**,,**," +
            @",,****,," +
            @",,,,,,,,"
        ));
    }
}