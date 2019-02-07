using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pcysl5edgo.Wahren
{
    public static unsafe class VerifyUtility
    {
        public static TryInterpretReturnValue IsCurrentCharEquals(this Caret caret, ushort cLeft, ushort cRight)
        {

            if (cLeft == cRight)
            {
                return new TryInterpretReturnValue(new Span(caret, 1), SuccessSentence.LeftBraceConfirmationSuccess, InterpreterStatus.Success);
            }
            else
            {
                return new TryInterpretReturnValue(new Span(caret, 1), ErrorSentence.ExpectedCharNotFoundError, 1, InterpreterStatus.Error);
            }
        }
    }
}
