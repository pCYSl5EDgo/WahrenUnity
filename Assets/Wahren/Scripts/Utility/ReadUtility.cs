﻿using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren
{
    public static unsafe class ReadUtility
    {
        public static Span ReadLine(this ref TextFile file, Caret current) => new Span(current, file.CurrentLineLength(current) - current.Column);
        public static TryInterpretReturnValue TryReadIdentifierNumberPairs(this ref TextFile file, ref IdentifierNumberPairList pairList, Caret current, out int start, out int length)
        {
            length = 0;
            file.SkipWhiteSpace(ref current);
            var preservedFirstLocation = current;
            if (file.CurrentChar(current) == '@')
            {
                start = pairList.Length - 1;
                return new TryInterpretReturnValue(new Span(current, 1), SuccessSentence.AssignmentInterpretationSuccess, InterpreterStatus.Success);
            }
            int capacity = 4;
            IdentifierNumberPair* tmpList = stackalloc IdentifierNumberPair[capacity];
            int state = 0;
            ref var raw = ref current.Line;
            ref var column = ref current.Column;
            Span span = new Span { File = file.FilePathId };
#if UNITY_EDITOR
            Span numberSpan = new Span { File = file.FilePathId };
#endif
            long number = 0;
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
                                    if (++length > capacity)
                                    {
                                        capacity *= 2;
                                        var _ = stackalloc IdentifierNumberPair[capacity];
                                        UnsafeUtility.MemCpy(_, tmpList, sizeof(IdentifierNumberPair) * capacity / 2);
                                        tmpList = _;
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
                                    if (++length > capacity)
                                    {
                                        capacity *= 2;
                                        var _ = stackalloc IdentifierNumberPair[capacity];
                                        UnsafeUtility.MemCpy(_, tmpList, sizeof(IdentifierNumberPair) * capacity / 2);
                                        tmpList = _;
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
                                    tmpList[length - 1].Span = span;
                                    state = 2;
                                    break;
                                case '*':
                                    tmpList[length - 1].Span = span;
                                    state = 3;
                                    if (isOnlyDigit)
                                    {
                                        start = 0;
                                        return new TryInterpretReturnValue(current, ErrorSentence.IdentifierCannotBeNumberError, InterpreterStatus.Error);
                                    }
                                    column++;
                                    file.SkipWhiteSpace(ref current);
                                    goto PARSE;
                                case ',':
#if UNITY_EDITOR
                                    tmpList[length - 1] = new IdentifierNumberPair(span, 0, numberSpan);
#else
                                    tmpList[length - 1] = new IdentifierNumberPair(span, 0);
#endif
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
                                    tmpList[length - 1].Number = 0;
#if UNITY_EDITOR
                                    tmpList[length - 1].NumberSpan = new Span { Start = current, Length = 0 };
#endif
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
#if UNITY_EDITOR
                                    numberSpan.Start = current;
                                    numberSpan.Length = 1;
#endif
                                    break;
                                case '-':
                                    state = 5;
#if UNITY_EDITOR
                                    numberSpan.Start = current;
                                    numberSpan.Length = 1;
#endif
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
#if UNITY_EDITOR
                                    numberSpan.Length++;
#endif
                                    break;
                                case ' ':
                                case '\t':
                                    state = 6;
                                    tmpList[length - 1].Number = number;
#if UNITY_EDITOR
                                    tmpList[length - 1].NumberSpan = numberSpan;
#endif
                                    break;
                                case ',':
                                    tmpList[length - 1].Number = number;
                                    state = 0;
                                    column++;
                                    file.SkipWhiteSpace(ref current);
#if UNITY_EDITOR
                                    tmpList[length - 1].NumberSpan = numberSpan;
#endif
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
#if UNITY_EDITOR
                                    numberSpan.Length++;
#endif
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
                                    number = 0;
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
                        return new TryInterpretReturnValue(span, ErrorSentence.InvalidEndOfLineError, InterpreterStatus.Error);
                    case 5:
                        start = 0;
                        return new TryInterpretReturnValue(span, ErrorSentence.InvalidMinusNumberError, InterpreterStatus.Error);
                    case 4:
                        tmpList[length - 1].Number = number;
#if UNITY_EDITOR
                        tmpList[length - 1].NumberSpan = numberSpan;
#endif
                        span = new Span(current, 0);
                        goto RETURN;
                    case 6:
                        span = new Span(current, 0);
                        goto RETURN;
                }
            }
        RETURN:
            if (pairList.TryAddMultiThread(tmpList, length, out start))
            {
                return new TryInterpretReturnValue(span, SuccessSentence.AssignmentInterpretationSuccess, InterpreterStatus.Success);
            }
            return new TryInterpretReturnValue(preservedFirstLocation, 0, 0, InterpreterStatus.Pending);
        ERROR:
            start = 0;
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