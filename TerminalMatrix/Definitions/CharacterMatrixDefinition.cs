﻿namespace TerminalMatrix.Definitions;

public static class CharacterMatrixDefinition
{
    public const int Width = 80;
    public const int Height = 25;

    public static int[,] Create() =>
        new int[Width, Height];
}