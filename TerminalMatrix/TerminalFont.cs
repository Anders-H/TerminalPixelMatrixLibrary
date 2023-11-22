using System.CodeDom;

namespace TerminalMatrix;

public class TerminalFont : Dictionary<int, CharacterPixelMatrix>
{
    public TerminalFont()
    {
        var tcp = new TerminalCodePage();

        Add(tcp.Asc[' '], new CharacterPixelMatrix(
            @",,,,,,,," +
            @",,,,,,,," +
            @",,,,,,,," +
            @",,,,,,,," +
            @",,,,,,,," +
            @",,,,,,,," +
            @",,,,,,,," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['A'], new CharacterPixelMatrix(
            @",,,**,,," +
            @",,****,," +
            @",**,,**," +
            @",**,,**," +
            @",******," +
            @",**,,**," +
            @",**,,**," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['B'], new CharacterPixelMatrix(
            @",*****,," +
            @",**,,**," +
            @",**,,**," +
            @",*****,," +
            @",**,,**," +
            @",**,,**," +
            @",*****,," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['C'], new CharacterPixelMatrix(
            @",,****,," +
            @",**,,**," +
            @",**,,,,," +
            @",**,,,,," +
            @",**,,,,," +
            @",**,,**," +
            @",,****,," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['D'], new CharacterPixelMatrix(
            @",****,,," +
            @",**,**,," +
            @",**,,**," +
            @",**,,**," +
            @",**,,**," +
            @",**,**,," +
            @",****,,," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['E'], new CharacterPixelMatrix(
            @",******," +
            @",**,,,,," +
            @",**,,,,," +
            @",****,,," +
            @",**,,,,," +
            @",**,,,,," +
            @",******," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['-'], new CharacterPixelMatrix(
            @",,,,,,,," +
            @",,,,,,,," +
            @",,,,,,,," +
            @",******," +
            @",,,,,,,," +
            @",,,,,,,," +
            @",,,,,,,," +
            @",,,,,,,,"
        ));
    }
}