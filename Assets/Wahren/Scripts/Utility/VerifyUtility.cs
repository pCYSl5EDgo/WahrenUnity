using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pcysl5edgo.Wahren
{
    public static unsafe class VerifyUtility
    {
        public static bool IsNextValidCharacter(this ref TextFile file, Span span, char expectedCharacter)
        {
            span.Column++;
            for (; span.Line < file.LineCount; span.Line++)
            {
                for (; span.Column < file.LineLengths[span.Line]; span.Column++)
                {
                    switch (file.Lines[span.Line][span.Column])
                    {
                        case ' ':
                        case '\t':
                            continue;
                        default:
                            return expectedCharacter == file.Lines[span.Line][span.Column];
                    }
                }
            }
            return false;
        }
    }
}
