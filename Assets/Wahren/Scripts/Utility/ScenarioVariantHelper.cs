namespace pcysl5edgo.Wahren
{
    public static unsafe class ScenarioVariantHelper
    {
        public static bool TryGetScenarioVariantName(this ref TextFile file, Caret start, out TryInterpretReturnValue value)
        {
            char* cptr = file.Lines[start.Line];
            start.Column++;
            value = new TryInterpretReturnValue(start, SuccessSentence.ScenarioVariantInterpretSuccess, InterpreterStatus.Success);
            ref var length = ref value.Span.Length;
            bool onlyDigit = true;
            for (int i = length + value.Span.Column; i < file.LineLengths[value.Span.Line]; i++)
            {
                switch (cptr[i])
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
                        length++;
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
                        length++;
                        break;
                    case '=':
                    case ' ':
                    case '\t':
                        goto RETURN;
                    default:
                        length++;
                        value.DataIndex = ErrorSentence.InvalidIdentifierError;
                        value.Status = 0;
                        return false;
                }
            }
        RETURN:
            if (onlyDigit && length != 0)
            {
                value.DataIndex = ErrorSentence.IdentifierCannotBeNumberError;
                value.Status = 0;
                return false;
            }
            return true;
        }
    }
}