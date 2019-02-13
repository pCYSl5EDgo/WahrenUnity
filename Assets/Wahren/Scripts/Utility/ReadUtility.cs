using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public static unsafe class ReadUtility
    {
        public static Span ReadLine(this ref TextFile file, Caret current) => new Span(current, file.CurrentLineLength(current) - current.Column);
        public static TryInterpretReturnValue TryReadIdentifierNumberPairs(this ref TextFile file, Location location, ref IdentifierNumberPairList pairList, Caret current, out int start, out int length, long defaultValue = 0)
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
            try
            {
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
                                switch ((file.Contents + file.LineStarts[raw])[column])
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
                                        state = 1;
                                        span = new Span(current, 1);
                                        isOnlyDigit = false;
                                        if (++tmpList.Length > tmpList.Capacity)
                                        {
                                            tmpList.Lengthen(Allocator.Temp);
                                        }
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
                                        state = 1;
                                        isOnlyDigit = true;
                                        span = new Span(current, 1);
                                        if (++tmpList.Length > tmpList.Capacity)
                                        {
                                            tmpList.Lengthen(Allocator.Temp);
                                        }
                                        break;
                                    case (ushort)' ':
                                    case (ushort)'\t':
                                        break;
                                    default:
                                        goto ERROR;
                                }
                                break;
                            case 1: // Seek for the rest chars of the identifier.
                                switch ((file.Contents + file.LineStarts[raw])[column])
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
                                        span.Length++;
                                        isOnlyDigit = false;
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
                                        tmpList.Values[tmpList.Length - 1].Span = span;
                                        state = 2;
                                        break;
                                    case (ushort)'*':
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
                                    case (ushort)',':
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
                                switch ((file.Contents + file.LineStarts[raw])[column])
                                {
                                    case (ushort)' ':
                                    case (ushort)'\t':
                                        break;
                                    case (ushort)'*':
                                        state = 3;
                                        column++;
                                        file.SkipWhiteSpace(ref current);
                                        goto PARSE;
                                    case (ushort)',':
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
                                switch ((file.Contents + file.LineStarts[raw])[column])
                                {
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
                                        state = 4;
                                        number = (file.Contents + file.LineStarts[raw])[column] - '0';
                                        numberSpan.Start = current;
                                        numberSpan.Length = 1;
                                        break;
                                    case (ushort)'-':
                                        state = 5;
                                        numberSpan.Start = current;
                                        numberSpan.Length = 1;
                                        break;
                                    case (ushort)' ':
                                    case (ushort)'\t':
                                        break;
                                    default:
                                        goto ERROR;
                                }
                                break;
                            case 4: // Seek for the rest digit of the number.
                                switch ((file.Contents + file.LineStarts[raw])[column])
                                {
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
                                        number *= 10;
                                        number += (file.Contents + file.LineStarts[raw])[column] - '0';
                                        numberSpan.Length++;
                                        break;
                                    case (ushort)' ':
                                    case (ushort)'\t':
                                        state = 6;
                                        tmpList.Values[tmpList.Length - 1].Number = number;
                                        tmpList.Values[tmpList.Length - 1].NumberSpan = numberSpan;
                                        break;
                                    case (ushort)',':
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
                                switch ((file.Contents + file.LineStarts[raw])[column])
                                {
                                    #region Digit
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
                                        number = -(file.Contents + file.LineStarts[raw])[column] + '0';
                                        state = 4;
                                        numberSpan.Length++;
                                        break;
                                    default:
                                        goto ERROR;
                                }
                                break;
                            case 6: // Seek for ','
                                switch ((file.Contents + file.LineStarts[raw])[column])
                                {
                                    case (ushort)',':
                                        state = 0;
                                        number = defaultValue;
                                        span = new Span { Start = current, Length = 0 };
                                        isOnlyDigit = true;
                                        column++;
                                        file.SkipWhiteSpace(ref current);
                                        goto PARSE;
                                    case (ushort)' ':
                                    case (ushort)'\t':
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
                return TryInterpretReturnValue.CreatePending(span, location, PendingReason.IdentifierNumberPairListCapacityShortage, tmpList.Length);
            ERROR:
                start = 0;
                length = 0;
                return new TryInterpretReturnValue(current, ErrorSentence.NotExpectedCharacterError, InterpreterStatus.Error);
            }
            finally
            {
                IdentifierNumberPairList.FreeTemp(ref tmpList);
            }
        }
        public static TryInterpretReturnValue TryReadIdentifierNotEmpty(ushort* lineCharPointer, int thisLineLength, int filePathId, int line, int column)
        {
            Span span = new Span(filePathId, line, column, 0);
            bool onlyDigit = true;
            for (int i = column; i < thisLineLength; i++)
            {
                switch (lineCharPointer[i])
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
                    case (ushort)'@':
                        return new TryInterpretReturnValue(new Span(filePathId, line, i, 1), SuccessSentence.IdentifierInterpretSuccess, InterpreterStatus.Success);
                    default:
                        return new TryInterpretReturnValue(new Span(filePathId, line, i, 1), ErrorSentence.NotNumberError, InterpreterStatus.Error);
                }
            }
            if (onlyDigit)
                return new TryInterpretReturnValue(span, ErrorSentence.IdentifierCannotBeNumberError, InterpreterStatus.Error);
            return new TryInterpretReturnValue(span, SuccessSentence.IdentifierInterpretSuccess, InterpreterStatus.Success);
        }
        public static TryInterpretReturnValue TryReadNumber(this ref TextFile file, Caret current, out long value)
        {
            value = 0;
            var cptr = file.CurrentCharPointer(current);
            Span span = new Span(current, 0);
            int state = 0;
            for (int i = current.Column, thisLineLength = file.CurrentLineLength(current); i < thisLineLength; i++, cptr++)
            {
                switch (state)
                {
                    case 0:
                        switch (*cptr)
                        {
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
                                state = 1;
                                value = *cptr - '0';
                                span.Length++;
                                span.Column = i;
                                break;
                            case (ushort)'-':
                                state = 2;
                                span.Length++;
                                span.Column = i;
                                break;
                            default:
                                return new TryInterpretReturnValue(new Span(file.FilePathId, current.Line, i, 1), ErrorSentence.NotNumberError, InterpreterStatus.Error);
                        }
                        break;
                    case 1:
                        switch (*cptr)
                        {
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
                                span.Length++;
                                value *= 10;
                                value += *cptr - '0';
                                break;
                            default:
                                return new TryInterpretReturnValue(new Span(file.FilePathId, current.Line, i, 1), ErrorSentence.NotNumberError, InterpreterStatus.Error);
                        }
                        break;
                    case 2:
                        switch (*cptr)
                        {
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
                                span.Length++;
                                value *= 10;
                                value -= *cptr - '0';
                                break;
                            default:
                                return new TryInterpretReturnValue(new Span(file.FilePathId, current.Line, i, 1), ErrorSentence.NotNumberError, InterpreterStatus.Error);
                        }
                        break;
                }
            }
            return new TryInterpretReturnValue(span, SuccessSentence.NumberInterpretSuccess, InterpreterStatus.Success);
        }
    }
}