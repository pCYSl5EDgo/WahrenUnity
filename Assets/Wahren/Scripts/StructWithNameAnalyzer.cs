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
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                    case 'g':
                    case 'h':
                    case 'i':
                    case 'j':
                    case 'k':
                    case 'l':
                    case 'm':
                    case 'n':
                    case 'o':
                    case 'p':
                    case 'q':
                    case 'r':
                    case 's':
                    case 't':
                    case 'u':
                    case 'v':
                    case 'w':
                    case 'x':
                    case 'y':
                    case 'z':
                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                    case 'G':
                    case 'H':
                    case 'I':
                    case 'J':
                    case 'K':
                    case 'L':
                    case 'M':
                    case 'N':
                    case 'O':
                    case 'P':
                    case 'Q':
                    case 'R':
                    case 'S':
                    case 'T':
                    case 'U':
                    case 'V':
                    case 'W':
                    case 'X':
                    case 'Y':
                    case 'Z':
                    #endregion
                    case '_':
                        onlyDigit = false;
                        ++length;
                        break;
                    #region Digit
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        #endregion
                        ++length;
                        break;
                    case ':':
                    case ' ':
                    case '\t':
                    case '{':
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
                case ':':
                    column++;
                    file.SkipWhiteSpace(ref start);
                    return GetParentStructNameInternal(ref file, start);
                case '{':
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
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                    case 'g':
                    case 'h':
                    case 'i':
                    case 'j':
                    case 'k':
                    case 'l':
                    case 'm':
                    case 'n':
                    case 'o':
                    case 'p':
                    case 'q':
                    case 'r':
                    case 's':
                    case 't':
                    case 'u':
                    case 'v':
                    case 'w':
                    case 'x':
                    case 'y':
                    case 'z':
                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                    case 'G':
                    case 'H':
                    case 'I':
                    case 'J':
                    case 'K':
                    case 'L':
                    case 'M':
                    case 'N':
                    case 'O':
                    case 'P':
                    case 'Q':
                    case 'R':
                    case 'S':
                    case 'T':
                    case 'U':
                    case 'V':
                    case 'W':
                    case 'X':
                    case 'Y':
                    case 'Z':
                    #endregion
                    case '_':
                        onlyDigit = false;
                        span.Length++;
                        break;
                    #region Digit
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        #endregion
                        span.Length++;
                        break;
                    case ' ':
                    case '\t':
                    case '{':
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