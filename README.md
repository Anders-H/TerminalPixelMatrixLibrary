# TerminalPixelMatrixLibrary
An 8-bit video interface emulator. Hope to make it good enough to replace the GUI code in our Altair BASIC emulator, [A-BASIC-Language](https://github.com/tomas-hakansson/A-BASIC-Language).
Place the control on a Windows Forms-window and get events for what the user is typing, or call the `Input` function to prompt the user.
[Nuget](https://www.nuget.org/packages/TerminalMatrix):

```
Install-Package TerminalMatrix
```

## Limit text area

When the text area is unlimited, the text terminal displays 25 lines of text.
This amount can be limited using the `SetTextRenderLimit` method.
`0` means that text is allowed everywhere, a larger value leaves empty space on the
upper part of the screen, and the largest value (`23`) only allows two
lines of text on the bottom of the screen.

## Bitmap images

To produce a bitmap image, create a picture (preferably a 16 or 32 color GIF file) using
the palette described in the
[ah-c64-palette.act](https://github.com/Anders-H/TerminalPixelMatrixLibrary/blob/main/ah-c64-palette.act) Photoshop file
(or the [extended 32 color file](https://github.com/Anders-H/TerminalPixelMatrixLibrary/blob/main/ah-c64-palette-extended.act)).
The color palette is also described in the source code, [here](https://github.com/Anders-H/TerminalPixelMatrixLibrary/blob/main/TerminalMatrix/TerminalColor/Palette.cs).
Remember that pixels usually are more high then wide, and that the interface emulator only can display 640 * 200 pixels at one time. The default resolution is 320 * 200 rectangular pixels.*

To display an image, load it as a byte array using the `LoadPictureFromGif` function and draw it using the `SetPixels` function.
Call `UpdateBitmap` to force the pixel buffer to be displayed.

```
var gif = terminalMatrixControl1.LoadPictureFromGif(@"..\..\..\..\testgif.gif");
terminalMatrixControl1.SetPixels(0, 0, gif);
terminalMatrixControl1.UpdateBitmap();
```

The image is a screenshot of the AdventureGameExample projet, a text adventure game dummy.

![Text adventure dummy](https://raw.githubusercontent.com/Anders-H/TerminalPixelMatrixLibrary/main/screenshot_adventure_game_dummy.jpg)

## Limitations

For a clean exit, you must call the Quit method in the FormClosed event handler.

```
private void Form1_FormClosed(object sender, FormClosedEventArgs e)
{
    terminalMatrixControl1.Quit();
}
```