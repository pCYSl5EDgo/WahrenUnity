using System;

namespace pcysl5edgo.Wahren
{
    public static unsafe class StructWithNameAnalyzer
    {
        public static TryInterpretReturnValue TryGetStructName(this ref TextFile file, Caret start)
        {
            file.SkipWhiteSpace(ref start);
            var answer = new TryInterpretReturnValue(start, ErrorSentence.StructNameNotFoundError, InterpreterStatus.Error);
            ref var span = ref answer.Span;
            ref int length = ref span.Length;
            length = 0;
            var currentLine = file.Lines[span.Line];
            bool onlyDigit = true;
            for (int thisLineLength = file.LineLengths[span.Line], column = span.Column; column < thisLineLength; column++)
            {
                switch (currentLine[column])
                {
                    #region Alphabet
                    case (ushort)'a':
                    case (ushort)'b':
                    case (ushort)'c':
                    case (ushort)'d':
                    case (ushort)'e':
                    case (ushort)'f':
                    case (ushort)'g':
                    case (ushort)'h':
                    case (ushort)'i':
                    case (ushort)'j':
                    case (ushort)'k':
                    case (ushort)'l':
                    case (ushort)'m':
                    case (ushort)'n':
                    case (ushort)'o':
                    case (ushort)'p':
                    case (ushort)'q':
                    case (ushort)'r':
                    case (ushort)'s':
                    case (ushort)'t':
                    case (ushort)'u':
                    case (ushort)'v':
                    case (ushort)'w':
                    case (ushort)'x':
                    case (ushort)'y':
                    case (ushort)'z':
                    case (ushort)'A':
                    case (ushort)'B':
                    case (ushort)'C':
                    case (ushort)'D':
                    case (ushort)'E':
                    case (ushort)'F':
                    case (ushort)'G':
                    case (ushort)'H':
                    case (ushort)'I':
                    case (ushort)'J':
                    case (ushort)'K':
                    case (ushort)'L':
                    case (ushort)'M':
                    case (ushort)'N':
                    case (ushort)'O':
                    case (ushort)'P':
                    case (ushort)'Q':
                    case (ushort)'R':
                    case (ushort)'S':
                    case (ushort)'T':
                    case (ushort)'U':
                    case (ushort)'V':
                    case (ushort)'W':
                    case (ushort)'X':
                    case (ushort)'Y':
                    case (ushort)'Z':
                    #endregion
                    case (ushort)'_':
                        onlyDigit = false;
                        ++length;
                        break;
                    #region Digit
                    case (ushort)'0':
                    case (ushort)'1':
                    case (ushort)'2':
                    case (ushort)'3':
                    case (ushort)'4':
                    case (ushort)'5':
                    case (ushort)'6':
                    case (ushort)'7':
                    case (ushort)'8':
                    case (ushort)'9':
                        #endregion
                        ++length;
                        break;
                    case (ushort)':':
                    case (ushort)' ':
                    case (ushort)'\t':
                    case (ushort)'{':
                        goto RETURN;
                    default:
                        answer.DataIndex = ErrorSentence.InvalidIdentifierError;
                        return answer;
                }
            }
        RETURN:
            if (length == 0)
            {
                answer.DataIndex = ErrorSentence.StructNameNotFoundError;
                answer.Status = 0;
            }
            else if (onlyDigit)
            {
                answer.DataIndex = ErrorSentence.IdentifierCannotBeNumberError;
                answer.Status = 0;
            }
            else
            {
                answer.DataIndex = SuccessSentence.StructNameInterpretSuccess;
                answer.Status = InterpreterStatus.Success;
            }
            return answer;
        }

        public static TryInterpretReturnValue TryGetParentStructName(this ref TextFile file, Caret start)
        {
            file.SkipWhiteSpace(ref start);
            ref var column = ref start.Column;
            switch (file.Lines[start.Line][column])
            {
                case (ushort)':':
                    column++;
                    file.SkipWhiteSpace(ref start);
                    return GetParentStructNameInternal(ref file, start);
                case (ushort)'{':
                    return new TryInterpretReturnValue(start, 0, InterpreterStatus.Success); ;
                default:
                    return new TryInterpretReturnValue(start, ErrorSentence.NotExpectedCharacterError, InterpreterStatus.Error);
            }
        }

        private static TryInterpretReturnValue GetParentStructNameInternal(ref TextFile file, Caret caret)
        {
            var currentLine = file.Lines[caret.Line];
            bool onlyDigit = true;
            Span span = new Span
            {
                Start = caret,
                Length = 0,
            };
            for (int thisLineLength = file.LineLengths[span.Line], column = span.Column; column < thisLineLength; column++)
            {
                switch (currentLine[column])
                {
                    #region Alphabet
                    case (ushort)'a':
                    case (ushort)'b':
                    case (ushort)'c':
                    case (ushort)'d':
                    case (ushort)'e':
                    case (ushort)'f':
                    case (ushort)'g':
                    case (ushort)'h':
                    case (ushort)'i':
                    case (ushort)'j':
                    case (ushort)'k':
                    case (ushort)'l':
                    case (ushort)'m':
                    case (ushort)'n':
                    case (ushort)'o':
                    case (ushort)'p':
                    case (ushort)'q':
                    case (ushort)'r':
                    case (ushort)'s':
                    case (ushort)'t':
                    case (ushort)'u':
                    case (ushort)'v':
                    case (ushort)'w':
                    case (ushort)'x':
                    case (ushort)'y':
                    case (ushort)'z':
                    case (ushort)'A':
                    case (ushort)'B':
                    case (ushort)'C':
                    case (ushort)'D':
                    case (ushort)'E':
                    case (ushort)'F':
                    case (ushort)'G':
                    case (ushort)'H':
                    case (ushort)'I':
                    case (ushort)'J':
                    case (ushort)'K':
                    case (ushort)'L':
                    case (ushort)'M':
                    case (ushort)'N':
                    case (ushort)'O':
                    case (ushort)'P':
                    case (ushort)'Q':
                    case (ushort)'R':
                    case (ushort)'S':
                    case (ushort)'T':
                    case (ushort)'U':
                    case (ushort)'V':
                    case (ushort)'W':
                    case (ushort)'X':
                    case (ushort)'Y':
                    case (ushort)'Z':
                    #endregion
                    case (ushort)'_':
                        onlyDigit = false;
                        span.Length++;
                        break;
                    #region Digit
                    case (ushort)'0':
                    case (ushort)'1':
                    case (ushort)'2':
                    case (ushort)'3':
                    case (ushort)'4':
                    case (ushort)'5':
                    case (ushort)'6':
                    case (ushort)'7':
                    case (ushort)'8':
                    case (ushort)'9':
                        #endregion
                        span.Length++;
                        break;
                    case (ushort)' ':
                    case (ushort)'\t':
                    case (ushort)'{':
                        goto RETURN;
                    default:
                        return new TryInterpretReturnValue(span, ErrorSentence.InvalidIdentifierError, InterpreterStatus.Error);
                }
            }
        RETURN:
            if (span.Length == 0)
            {
                return new TryInterpretReturnValue(span, ErrorSentence.ParentStructNameNotFoundError, InterpreterStatus.Error);
            }
            if (onlyDigit)
            {
                return new TryInterpretReturnValue(span, ErrorSentence.IdentifierCannotBeNumberError, InterpreterStatus.Error);
            }
            return new TryInterpretReturnValue(span, SuccessSentence.ParentStructNameInterpretSuccess, InterpreterStatus.Success);
        }
    }
}