using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pcysl5edgo.Wahren
{
    public static unsafe class SkipWhiteSpaceUtility
    {
        public static Span SkipWhiteSpace(this ref TextFile file, Span span)
        {
            span.File = file.FilePathId;
            ref int raw = ref span.Line;
            ref int column = ref span.Column;
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
                            return span;
                    }
                }
            }
            return span;
        }
    }
}