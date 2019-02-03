namespace pcysl5edgo.Wahren
{
    public static unsafe class ReadUtility
    {
        public static Span ReadLine(this ref TextFile file, Caret current) => new Span(current, file.CurrentLineLength(current) - current.Column);
        public static TryInterpretReturnValue TryReadIdentifierValuePairs(this ref TextFile file, Caret current, out int length, out IdentifierNumberPair* pairs)
        {
            var cptr = (ushort*)file.CurrentCharPointer(current);
            length = 0;
            int capacity = 2;
            IdentifierNumberPair* tmpPairs = stackalloc IdentifierNumberPair[capacity];
            
            throw new System.NotImplementedException();
        }
        public static TryInterpretReturnValue TryReadIdentifierNotEmpty(this ref TextFile file, Caret current)
        {
            var cptr = (ushort*)file.CurrentCharPointer(current);
            Span span = new Span(current, 0);
            bool onlyDigit = true;
            for (int i = current.Column, thisLineLength = file.CurrentLineLength(current); i < thisLineLength; i++, cptr++)
            {
                switch (*cptr)
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
                    case '@':
                        if (span.Length++ != 0 || i != thisLineLength - 1)
                        {
                            span.Length = thisLineLength - current.Column;
                            return new TryInterpretReturnValue(span, ErrorSentence.InvalidIdentifierError, 0, false);
                        }
                        return new TryInterpretReturnValue(new Span(file.FilePathId, current.Line, i, 1), SuccessSentence.IdentifierInterpretSuccess, 0, true);
                    default:
                        return new TryInterpretReturnValue(new Span(file.FilePathId, current.Line, i, 1), ErrorSentence.NotNumberError, 0, false);
                }
            }
            if (onlyDigit)
                return new TryInterpretReturnValue(span, ErrorSentence.IdentifierCannotBeNumberError, 0, false);
            return new TryInterpretReturnValue(span, SuccessSentence.IdentifierInterpretSuccess, 0, true);
        }
        public static TryInterpretReturnValue TryReadNumber(this ref TextFile file, Caret current, out long value)
        {
            value = 0;
            var cptr = (ushort*)file.CurrentCharPointer(current);
            Span span = new Span(current, 0);
            for (int i = current.Column, thisLineLength = file.CurrentLineLength(current); i < thisLineLength; i++, cptr++)
            {
                switch (*cptr)
                {
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
                        value *= 10;
                        value += cptr[0] - '0';
                        span.Length++;
                        break;
                    default:
                        return new TryInterpretReturnValue(new Span(file.FilePathId, current.Line, i, 1), ErrorSentence.NotNumberError, 0, false);
                }
            }
            return new TryInterpretReturnValue(span, SuccessSentence.NumberInterpretSuccess, 0, true);
        }
    }
}