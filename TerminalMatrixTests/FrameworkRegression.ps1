$ErrorActionPreference = 'Stop'
$bin = Join-Path $PSScriptRoot '..\TerminalMatrixNetFramework\bin\Release'
Add-Type -Path (Join-Path $bin 'PixelmapLibraryNetFramework.dll')
Add-Type -Path (Join-Path $bin 'TerminalMatrixNetFramework.dll')
$source = @"
using System;
using System.Reflection;
using TerminalMatrixNetFramework;
using TerminalMatrixNetFramework.TerminalColor;
public static class RegressionChecks
{
    static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        Console.WriteLine("PASS: " + message);
    }
    public static void Run()
    {
        using (var wide = new TerminalMatrixControl())
        using (var small = new TerminalMatrixControl())
        {
            wide.BorderWidth = 4;
            wide.BorderHeight = 2;
            wide.SetResolution(Resolution.Pixels640x200Characters80x25);
            wide.SetTextRenderLimit(5);
            small.SetResolution(Resolution.LogPixels640x80Characters80x10);
            small.SetTextRenderLimit(2);
            Check(wide.TextRenderLimit == 5 && small.TextRenderLimit == 2, "Independent render limits");
            small.SetResolution(Resolution.Pixels160x200Characters20x25);
            wide.SetStartPosition(79, 24);
            Check(wide.CursorPosition.AtEnd(), "Cursor retains its own dimensions");
            wide.UpdateBitmap();
            small.UpdateBitmap();
            Check(wide.CharacterWidth == 80 && small.CharacterWidth == 20, "Independent resolutions render");
            bool rejected = false;
            small.SetResolution(Resolution.LogPixels640x80Characters80x10);
            try { small.SetTextRenderLimit(9); }
            catch (ArgumentOutOfRangeException) { rejected = true; }
            Check(rejected, "Render limit validated against actual height");
            var pixels = new byte[2,2];
            for (int y=0; y<2; y++) for (int x=0; x<2; x++) pixels[x,y]=(byte)ColorName.White;
            wide.UpdateBitmap(null, map => {
                wide.SetPixels(500, 10, pixels);
                Check(map.GetPixel(504,12).R == 255 && map.GetPixel(504,13).R == 255,
                      "SetPixels preserves border offset and supports wide resolution");
                Check(map.GetPixel(500,13).R == 0, "SetPixels does not shift subsequent rows");
            });
            bool threw = false;
            try { wide.UpdateBitmap(map => { throw new InvalidOperationException("expected"); }); }
            catch (InvalidOperationException) { threw = true; }
            Check(threw, "Callback exception propagated");
            wide.UpdateBitmap();
            Check(true, "Bitmap unlocked after callback exception");
            var bitmapField = typeof(TerminalMatrixControl).GetField("_fastBitmap", BindingFlags.Instance | BindingFlags.NonPublic);
            wide.Dispose();
            Check(wide.QuitFlag && bitmapField.GetValue(wide) == null, "Dispose releases rendering resources");
        }
    }
}
"@
Add-Type -TypeDefinition $source -ReferencedAssemblies @((Join-Path $bin 'TerminalMatrixNetFramework.dll'), (Join-Path $bin 'PixelmapLibraryNetFramework.dll'), 'System.Windows.Forms', 'System.Drawing')
[RegressionChecks]::Run()