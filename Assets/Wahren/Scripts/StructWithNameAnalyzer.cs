using System;

namespace pcysl5edgo.Wahren
{
    public static unsafe class StructWithNameAnalyzer
    {
        public static TryInterpretReturnValue TryGetStructName(this ref TextFile file, Span spanIgnoreFile)
        {
            spanIgnoreFile = file.SkipWhiteSpace(spanIgnoreFile);
            var answer = new TryInterpretReturnValue(ref spanIgnoreFile, ErrorSentence.StructNameNotFoundError, 0, false);
            ref var span = ref answer.Span;
            ref int length = ref span.Length;
            length = 0;
            char* currentLine = file.Lines[span.Line];
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
                answer.isSuccess = 0;
            }
            else if (onlyDigit)
            {
                answer.DataIndex = ErrorSentence.IdentifierCannotBeNumberError;
                answer.isSuccess = 0;
            }
            else
            {
                answer.DataIndex = SuccessSentence.StructNameInterpretSuccess;
                answer.isSuccess = 1;
            }
            return answer;
        }

        public static bool TryGetParentStructName(this ref TextFile file, Span spanIgnoreFile, out TryInterpretReturnValue value)
        {
            spanIgnoreFile = file.SkipWhiteSpace(spanIgnoreFile);
            switch (file.Lines[spanIgnoreFile.Line][spanIgnoreFile.Column])
            {
                case ':':
                    spanIgnoreFile.Column++;
                    spanIgnoreFile = file.SkipWhiteSpace(spanIgnoreFile);
                    return GetParentStructNameInternal(ref file, spanIgnoreFile, out value);
                case '{':
                    value = new TryInterpretReturnValue(ref spanIgnoreFile, 0, 0, true);
                    return false;
                default:
                    value = new TryInterpretReturnValue(ref spanIgnoreFile, ErrorSentence.NotExpectedIdentifierAfterStructNameError, 0, false);
                    return false;
            }
        }

        private static bool GetParentStructNameInternal(ref TextFile file, Span span, out TryInterpretReturnValue value)
        {
            char* currentLine = file.Lines[span.Line];
            bool onlyDigit = true;
            span.Length = 0;
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
                        value = new TryInterpretReturnValue(ref span, ErrorSentence.InvalidIdentifierError, 0, false);
                        return false;
                }
            }
        RETURN:
            if (span.Length == 0)
            {
                value = new TryInterpretReturnValue(ref span, ErrorSentence.ParentStructNameNotFoundError, 0, false);
                return false;
            }
            if (onlyDigit)
            {
                value = new TryInterpretReturnValue(ref span, ErrorSentence.IdentifierCannotBeNumberError, 0, false);
                return false;
            }
            value = new TryInterpretReturnValue(ref span, SuccessSentence.ParentStructNameInterpretSuccess, 0, true);
            return true;
        }
    }
}