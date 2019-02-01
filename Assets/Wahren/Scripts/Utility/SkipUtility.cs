using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pcysl5edgo.Wahren
{
    public static unsafe class SkipUtility
    {
        public static Caret SkipWhiteSpace(this in TextFile file, Caret caret)
        {
            file.SkipWhiteSpace(ref caret);
            return caret;
        }
        public static void SkipWhiteSpace(this in TextFile file, ref Caret caret)
        {
            caret.File = file.FilePathId;
            ref int raw = ref caret.Line;
            ref int column = ref caret.Column;
            for (char* currentLine; raw < file.LineCount; raw++, column = 0)
            {
                currentLine = file.Lines[raw];
                for (int thisLineLength = file.LineLengths[raw]; column < thisLineLength; column++)
                {
                    switch (currentLine[column])
                    {
                        case '\t':
                        case ' ':
                            break;
                        default:
                            return;
                    }
                }
            }
        }
    }
}