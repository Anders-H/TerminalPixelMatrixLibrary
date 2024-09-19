# TerminalPixelMatrixLibrary
An 8-bit video interface emulator. Hope to make it good enough to replace the GUI code in our Altair BASIC emulator, [A-BASIC-Language](https://github.com/tomas-hakansson/A-BASIC-Language).
Place the control on a Windows Forms-window and get events for what the user is typing, or call the `Input` function to prompt the user.
[Nuget](https://www.nuget.org/packages/TerminalMatrix):

```
Install-Package TerminalMatrix
```

## Basic functionality

For adding text to the terminal matrix, use the `WriteLine` method. To prompt the user, use the `Input` method.

This library is used by [A-BASIC-Language](https://github.com/tomas-hakansson/A-BASIC-Language) and it
is using [PixelmapLibrary](https://github.com/Anders-H/PixelmapLibrary) for fast bitmap graphics.

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

### Clean exit

For a clean exit, you must call the `Quit` method in the `FormClosed` event handler.

```
private void Form1_FormClosed(object sender, FormClosedEventArgs e)
{
    terminalMatrixControl1.Quit();
}
```

### Layer limitations

Background layer can be activated using the `UseBackground24Bit` property.
This will add a background to the console. It will always be on (in 24 bit) or off.
The background pixels are stored in the `Background24Bit` array. `Background24Bit` is a two dimensional `int` array, but only three bits are used (R, G and B).

The foreground (accessed using `GetPixel` and `SetPixel`) is always treated as 24 bit.
However, the `ControlOverlayPainter` delegate will support transparency if the `Use32BitForeground` property is set.
Default is false, meaning that opacity is ignored.

In short, the control can have one or two bitmap layers, the top layer can be exposed as 32 bit for better overlaying.