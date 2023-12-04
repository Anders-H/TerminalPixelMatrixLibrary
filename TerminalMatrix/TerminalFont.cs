﻿namespace TerminalMatrix;

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
        Add(tcp.Asc[','], new CharacterPixelMatrix(
            @",,,,,,,," +
            @",,,,,,,," +
            @",,,,,,,," +
            @",,,,,,,," +
            @",,,,,,,," +
            @",,,**,,," +
            @",,,**,,," +
            @",,**,,,,"
        ));
        Add(tcp.Asc['.'], new CharacterPixelMatrix(
            @",,,,,,,," +
            @",,,,,,,," +
            @",,,,,,,," +
            @",,,,,,,," +
            @",,,,,,,," +
            @",,,**,,," +
            @",,,**,,," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['0'], new CharacterPixelMatrix(
            @",,****,," +
            @",**,,**," +
            @",**,***," +
            @",******," +
            @",***,**," +
            @",**,,**," +
            @",,****,," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['1'], new CharacterPixelMatrix(
            @",,,**,,," +
            @",,***,,," +
            @",****,,," +
            @",,,**,,," +
            @",,,**,,," +
            @",,,**,,," +
            @",******," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['2'], new CharacterPixelMatrix(
            @",,****,," +
            @",**,,**," +
            @",,,, **," +
            @",,, **,," +
            @",, **,,," +
            @", **,,,," +
            @",******," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['3'], new CharacterPixelMatrix(
            @",,****,," +
            @",**,,**," +
            @",,,,,**," +
            @",,,,**,," +
            @",,,,,**," +
            @",**,,**," +
            @",,****,," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['4'], new CharacterPixelMatrix(
            @",,,,***," +
            @",,,****," +
            @",,**,**," +
            @",**,,**," +
            @",******," +
            @",,,,,**," +
            @",,,,,**," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['5'], new CharacterPixelMatrix(
            @",******," +
            @",**,,,,," +
            @",**,,,,," +
            @",*****,," +
            @",,,,,**," +
            @",**,,**," +
            @",,****,," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['6'], new CharacterPixelMatrix(
            @",,****,," +
            @",**,,**," +
            @",**,,,,," +
            @",*****,," +
            @",**,,**," +
            @",**,,**," +
            @",,****,," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['7'], new CharacterPixelMatrix(
            @",******," +
            @",,,,,**," +
            @",,,,,**," +
            @",,,,**,," +
            @",,,**,,," +
            @",,,**,,," +
            @",,,**,,," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['8'], new CharacterPixelMatrix(
            @",,****,," +
            @",**,,**," +
            @",**,,**," +
            @",,****,," +
            @",**,,**," +
            @",**,,**," +
            @",,****,," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['9'], new CharacterPixelMatrix(
            @",,****,," +
            @",**,,**," +
            @",**,,**," +
            @",,*****," +
            @",,,,,**," +
            @",,,,,**," +
            @",,****,," +
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
        Add(tcp.Asc['a'], new CharacterPixelMatrix(
            @",,,,,,,," +
            @",,,,,,,," +
            @",,****,," +
            @",,,,,**," +
            @",,*****," +
            @",**,,**," +
            @",,*****," +
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
        Add(tcp.Asc['b'], new CharacterPixelMatrix(
            @",**,,,,," +
            @",**,,,,," +
            @",*****,," +
            @",**,,**," +
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
        Add(tcp.Asc['c'], new CharacterPixelMatrix(
            @",,,,,,,," +
            @",,,,,,,," +
            @",,****,," +
            @",**,,**," +
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
        Add(tcp.Asc['F'], new CharacterPixelMatrix(
            @",******," +
            @",**,,,,," +
            @",**,,,,," +
            @",****,,," +
            @",**,,,,," +
            @",**,,,,," +
            @",**,,,,," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['G'], new CharacterPixelMatrix(
            @",,****,," +
            @",**,,**," +
            @",**,,,,," +
            @",**,***," +
            @",**,,**," +
            @",**,,**," +
            @",,****,," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['H'], new CharacterPixelMatrix(
            @",**,,**," +
            @",**,,**," +
            @",**,,**," +
            @",******," +
            @",**,,**," +
            @",**,,**," +
            @",**,,**," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['I'], new CharacterPixelMatrix(
            @",,****,," +
            @",,,**,,," +
            @",,,**,,," +
            @",,,**,,," +
            @",,,**,,," +
            @",,,**,,," +
            @",,****,," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['J'], new CharacterPixelMatrix(
            @",,,****," +
            @",,,,,**," +
            @",,,,,**," +
            @",,,,,**," +
            @",,,,,**," +
            @",**,,**," +
            @",,****,," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['K'], new CharacterPixelMatrix(
            @",**,,**," +
            @",**,,**," +
            @",**,**,," +
            @",****,,," +
            @",**,**,," +
            @",**,,**," +
            @",**,,**," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['L'], new CharacterPixelMatrix(
            @",**,,,,," +
            @",**,,,,," +
            @",**,,,,," +
            @",**,,,,," +
            @",**,,,,," +
            @",**,,,,," +
            @",******," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['M'], new CharacterPixelMatrix(
            @",**,,,**" +
            @",***,***" +
            @",*******" +
            @",**,*,**" +
            @",**,,,**" +
            @",**,,,**" +
            @",**,,,**" +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['N'], new CharacterPixelMatrix(
            @",**,,**," +
            @",***,**," +
            @",******," +
            @",**,***," +
            @",**,,**," +
            @",**,,**," +
            @",**,,**," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['O'], new CharacterPixelMatrix(
            @",,****,," +
            @",**,,**," +
            @",**,,**," +
            @",**,,**," +
            @",**,,**," +
            @",**,,**," +
            @",,****,," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['P'], new CharacterPixelMatrix(
            @",*****,," +
            @",**,,**," +
            @",**,,**," +
            @",*****,," +
            @",**,,,,," +
            @",**,,,,," +
            @",**,,,,," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['Q'], new CharacterPixelMatrix(
            @",,****,," +
            @",**,,**," +
            @",**,,**," +
            @",**,,**," +
            @",**,,**," +
            @",**,,**," +
            @",,****,," +
            @",,,,,**,"
        ));
        Add(tcp.Asc['R'], new CharacterPixelMatrix(
            @",*****,," +
            @",**,,**," +
            @",**,,**," +
            @",*****,," +
            @",**,,**," +
            @",**,,**," +
            @",**,,**," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['S'], new CharacterPixelMatrix(
            @",,****,," +
            @",**,,**," +
            @",**,,,,," +
            @",,****,," +
            @",,,,,**," +
            @",**,,**," +
            @",,****,," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['T'], new CharacterPixelMatrix(
            @",******," +
            @",,,**,,," +
            @",,,**,,," +
            @",,,**,,," +
            @",,,**,,," +
            @",,,**,,," +
            @",,,**,,," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['U'], new CharacterPixelMatrix(
            @",**,,**," +
            @",**,,**," +
            @",**,,**," +
            @",**,,**," +
            @",**,,**," +
            @",**,,**," +
            @",,****,," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['V'], new CharacterPixelMatrix(
            @",**,,**," +
            @",**,,**," +
            @",**,,**," +
            @",**,,**," +
            @",**,,**," +
            @",,****,," +
            @",,,**,,," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['W'], new CharacterPixelMatrix(
            @",**,,,**" +
            @",**,,,**" +
            @",**,,,**" +
            @",**,*,**" +
            @",*******" +
            @",***,***" +
            @",**,,,**" +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['X'], new CharacterPixelMatrix(
            @",**,,**," +
            @",**,,**," +
            @",,****,," +
            @",,,**,,," +
            @",,****,," +
            @",**,,**," +
            @",**,,**," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['Y'], new CharacterPixelMatrix(
            @",**,,**," +
            @",**,,**," +
            @",**,,**," +
            @",,****,," +
            @",,,**,,," +
            @",,,**,,," +
            @",,,**,,," +
            @",,,,,,,,"
        ));
        Add(tcp.Asc['Z'], new CharacterPixelMatrix(
            @",******," +
            @",,,,,**," +
            @",,,,**,," +
            @",,'**,,," +
            @",,**,,,," +
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