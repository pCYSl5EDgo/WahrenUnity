using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren
{
    public static unsafe class ReadUtility
    {
        public static Span ReadLine(this ref TextFile file, Caret current) => new Span(current, file.CurrentLineLength(current) - current.Column);
        public static TryInterpretReturnValue TryReadIdentifierNumberPairs(this ref TextFile file, ref IdentifierNumberPairList pairList, Caret current, out int start, out int length, long defaultValue = 0)
        {
            file.SkipWhiteSpace(ref current);
            var preservedFirstLocation = current;
            if (file.CurrentChar(current) == '@')
            {
                start = pairList.Length - 1;
                length = 0;
                return new TryInterpretReturnValue(new Span(current, 1), SuccessSentence.AssignmentInterpretationSuccess, InterpreterStatus.Success);
            }
            var tmpList = IdentifierNumberPairList.MallocTemp(4);
            int state = 0;
            ref var raw = ref current.Line;
            ref var column = ref current.Column;
            Span span = new Span { File = file.FilePathId };
            Span numberSpan = new Span { File = file.FilePathId };
            long number = defaultValue;
            bool isOnlyDigit = true;
            for (; raw < file.LineCount; raw++, column = 0)
            {
                for (; column < file.LineLengths[raw]; column++)
                {
                PARSE:
                    switch (state)
                    {
                        case 0: // Seek for the first char of the identifier.
                            switch (file.Lines[raw][column])
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
                                    state = 1;
                                    span = new Span(current, 1);
                                    isOnlyDigit = false;
                                    if (++tmpList.Length > tmpList.Capacity)
                                    {
                                        tmpList.Lengthen(Allocator.Temp);
                                    }
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
                                    state = 1;
                                    isOnlyDigit = true;
                                    span = new Span(current, 1);
                                    if (++tmpList.Length > tmpList.Capacity)
                                    {
                                        tmpList.Lengthen(Allocator.Temp);
                                    }
                                    break;
                                case ' ':
                                case '\t':
                                    break;
                                default:
                                    goto ERROR;
                            }
                            break;
                        case 1: // Seek for the rest chars of the identifier.
                            switch (file.Lines[raw][column])
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
                                    span.Length++;
                                    isOnlyDigit = false;
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
                                    tmpList.Values[tmpList.Length - 1].Span = span;
                                    state = 2;
                                    break;
                                case '*':
                                    tmpList.Values[tmpList.Length - 1].Span = span;
                                    state = 3;
                                    if (isOnlyDigit)
                                    {
                                        start = 0;
                                        length = 0;
                                        return new TryInterpretReturnValue(current, ErrorSentence.IdentifierCannotBeNumberError, InterpreterStatus.Error);
                                    }
                                    column++;
                                    file.SkipWhiteSpace(ref current);
                                    goto PARSE;
                                case ',':
                                    tmpList.Values[tmpList.Length - 1] = new IdentifierNumberPair(span, 0, numberSpan);
                                    state = 0;
                                    column++;
                                    file.SkipWhiteSpace(ref current);
                                    goto PARSE;
                                default:
                                    goto ERROR;
                            }
                            break;
                        case 2: // Seek for '*'.
                            switch (file.Lines[raw][column])
                            {
                                case ' ':
                                case '\t':
                                    break;
                                case '*':
                                    state = 3;
                                    column++;
                                    file.SkipWhiteSpace(ref current);
                                    goto PARSE;
                                case ',':
                                    tmpList.Values[tmpList.Length - 1].Number = 0;
                                    tmpList.Values[tmpList.Length - 1].NumberSpan = new Span { Start = current, Length = 0 };
                                    state = 0;
                                    column++;
                                    file.SkipWhiteSpace(ref current);
                                    goto PARSE;
                                default:
                                    goto ERROR;
                            }
                            break;
                        case 3: // Seek for the first digit or '-' of the number.
                            switch (file.Lines[raw][column])
                            {
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
                                    state = 4;
                                    number = file.Lines[raw][column] - '0';
                                    numberSpan.Start = current;
                                    numberSpan.Length = 1;
                                    break;
                                case '-':
                                    state = 5;
                                    numberSpan.Start = current;
                                    numberSpan.Length = 1;
                                    break;
                                case ' ':
                                case '\t':
                                    break;
                                default:
                                    goto ERROR;
                            }
                            break;
                        case 4: // Seek for the rest digit of the number.
                            switch (file.Lines[raw][column])
                            {
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
                                    number *= 10;
                                    number += file.Lines[raw][column] - '0';
                                    numberSpan.Length++;
                                    break;
                                case ' ':
                                case '\t':
                                    state = 6;
                                    tmpList.Values[tmpList.Length - 1].Number = number;
                                    tmpList.Values[tmpList.Length - 1].NumberSpan = numberSpan;
                                    break;
                                case ',':
                                    tmpList.Values[tmpList.Length - 1].Number = number;
                                    state = 0;
                                    column++;
                                    file.SkipWhiteSpace(ref current);
                                    tmpList.Values[tmpList.Length - 1].NumberSpan = numberSpan;
                                    goto PARSE;
                                default:
                                    goto ERROR;
                            }
                            break;
                        case 5: // Seek for the rest digit of the minus number.
                            switch (file.Lines[raw][column])
                            {
                                #region Digit
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
                                    number = -file.Lines[raw][column] + '0';
                                    state = 4;
                                    numberSpan.Length++;
                                    break;
                                default:
                                    goto ERROR;
                            }
                            break;
                        case 6: // Seek for ','
                            switch (file.Lines[raw][column])
                            {
                                case ',':
                                    state = 0;
                                    number = defaultValue;
                                    span = new Span { Start = current, Length = 0 };
                                    isOnlyDigit = true;
                                    column++;
                                    file.SkipWhiteSpace(ref current);
                                    goto PARSE;
                                case ' ':
                                case '\t':
                                    break;
                                default:
                                    goto ERROR;
                            }
                            break;
                    }
                }
                switch (state)
                {
                    case 0:
                    case 2:
                    case 3:
                        break;
                    case 1:
                        start = 0;
                        length = 0;
                        return new TryInterpretReturnValue(span, ErrorSentence.InvalidEndOfLineError, InterpreterStatus.Error);
                    case 5:
                        start = 0;
                        length = 0;
                        return new TryInterpretReturnValue(span, ErrorSentence.InvalidMinusNumberError, InterpreterStatus.Error);
                    case 4:
                        tmpList.Values[tmpList.Length - 1].Number = number;
                        tmpList.Values[tmpList.Length - 1].NumberSpan = numberSpan;
                        span = new Span(current, 0);
                        goto RETURN;
                    case 6:
                        span = new Span(current, 0);
                        goto RETURN;
                }
            }
        RETURN:
            length = tmpList.Length;
            if (pairList.TryAddMultiThread(tmpList.Values, tmpList.Length, out start))
            {
                return new TryInterpretReturnValue(span, SuccessSentence.AssignmentInterpretationSuccess, InterpreterStatus.Success);
            }
            return TryInterpretReturnValue.CreatePending(span, AST.Location.None, AST.PendingReason.IdentifierNumberPairListCapacityShortage, tmpList.Length);
        ERROR:
            start = 0;
            length = 0;
            return new TryInterpretReturnValue(current, ErrorSentence.NotExpectedCharacterError, InterpreterStatus.Error);
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
                            return new TryInterpretReturnValue(span, ErrorSentence.InvalidIdentifierError, InterpreterStatus.Error);
                        }
                        return new TryInterpretReturnValue(new Span(file.FilePathId, current.Line, i, 1), SuccessSentence.IdentifierInterpretSuccess, InterpreterStatus.Success);
                    default:
                        return new TryInterpretReturnValue(new Span(file.FilePathId, current.Line, i, 1), ErrorSentence.NotNumberError, InterpreterStatus.Error);
                }
            }
            if (onlyDigit)
                return new TryInterpretReturnValue(span, ErrorSentence.IdentifierCannotBeNumberError, InterpreterStatus.Error);
            return new TryInterpretReturnValue(span, SuccessSentence.IdentifierInterpretSuccess, InterpreterStatus.Success);
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
                        return new TryInterpretReturnValue(new Span(file.FilePathId, current.Line, i, 1), ErrorSentence.NotNumberError, InterpreterStatus.Error);
                }
            }
            return new TryInterpretReturnValue(span, SuccessSentence.NumberInterpretSuccess, InterpreterStatus.Success);
        }
    }
}