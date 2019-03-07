using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public struct AssignExpressionStruct_Range
    {
        public Span ScenarioVariant;
        public int Start, Length;
    }
    public static unsafe class ReadUtility
    {
        public static Span ReadLine(this ref TextFile file, Caret current) => new Span(current, file.CurrentLineLength(current) - current.Column);

        public static TryInterpretReturnValue TryReadIdentifierNumberPairs(this ref TextFile file, ref IdentifierNumberPairListLinkedList pairList, Caret current, out IdentifierNumberPairList* page, out int start, out int length, Allocator allocator, long defaultValue = 0)
        {
            file.SkipWhiteSpace(ref current);
            if (file.CurrentChar(current) == '@')
            {
                page = null;
                start = 0;
                length = 0;
                return new TryInterpretReturnValue(new Span(current, 1), SuccessSentence.Kind.AssignmentInterpretationSuccess);
            }
            var tempList = new IdentifierNumberPairList(4, Allocator.Temp);
            int state = 0;
            ref var raw = ref current.Line;
            ref var column = ref current.Column;
            Span span = new Span { File = file.FilePathId };
            Span numberSpan = new Span { File = file.FilePathId };
            long number = defaultValue;
            bool isOnlyDigit = true;
            TryInterpretReturnValue answer;
            for (; raw < file.LineCount; raw++, column = 0)
            {
                for (; column < file.LineLengths[raw]; column++)
                {
                PARSE:
                    var c = (file.Contents + file.LineStarts[raw])[column];
                    switch (state)
                    {
                        case 0: // Seek for the first char of the identifier.
                            switch (c)
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
                                    if (++tempList.This.Length > tempList.This.Capacity)
                                    {
                                        tempList.Lengthen(Allocator.Temp);
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
                                    if (++tempList.This.Length > tempList.This.Capacity)
                                    {
                                        tempList.Lengthen(Allocator.Temp);
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
                            switch (c)
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
                                    tempList.This.Values[tempList.This.Length - 1].Span = span;
                                    state = 2;
                                    break;
                                case '*':
                                    tempList.This.Values[tempList.This.Length - 1].Span = span;
                                    state = 3;
                                    if (isOnlyDigit)
                                    {
                                        page = null;
                                        start = 0;
                                        length = 0;
                                        answer = new TryInterpretReturnValue(current, ErrorSentence.Kind.IdentifierCannotBeNumberError);
                                        goto RETURN;
                                    }
                                    column++;
                                    file.SkipWhiteSpace(ref current);
                                    goto PARSE;
                                case ',':
                                    tempList.This.Values[tempList.This.Length - 1] = new IdentifierNumberPair(span, 0, numberSpan);
                                    state = 0;
                                    column++;
                                    file.SkipWhiteSpace(ref current);
                                    goto PARSE;
                                default:
                                    goto ERROR;
                            }
                            break;
                        case 2: // Seek for '*'.
                            switch (c)
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
                                    tempList.This.Values[tempList.This.Length - 1].Number = 0;
                                    tempList.This.Values[tempList.This.Length - 1].NumberSpan = new Span { Start = current, Length = 0 };
                                    state = 0;
                                    column++;
                                    file.SkipWhiteSpace(ref current);
                                    goto PARSE;
                                default:
                                    goto ERROR;
                            }
                            break;
                        case 3: // Seek for the first digit or '-' of the number.
                            switch (c)
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
                                    number = c - '0';
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
                            switch (c)
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
                                    number += c - '0';
                                    numberSpan.Length++;
                                    break;
                                case ' ':
                                case '\t':
                                    state = 6;
                                    tempList.This.Values[tempList.This.Length - 1].Number = number;
                                    tempList.This.Values[tempList.This.Length - 1].NumberSpan = numberSpan;
                                    break;
                                case ',':
                                    tempList.This.Values[tempList.This.Length - 1].Number = number;
                                    state = 0;
                                    column++;
                                    file.SkipWhiteSpace(ref current);
                                    tempList.This.Values[tempList.This.Length - 1].NumberSpan = numberSpan;
                                    goto PARSE;
                                default:
                                    goto ERROR;
                            }
                            break;
                        case 5: // Seek for the rest digit of the minus number.
                            switch (c)
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
                                    number = -c + '0';
                                    state = 4;
                                    numberSpan.Length++;
                                    break;
                                default:
                                    goto ERROR;
                            }
                            break;
                        case 6: // Seek for ','
                            switch (c)
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
                        page = null;
                        start = 0;
                        length = 0;
                        answer = new TryInterpretReturnValue(span, ErrorSentence.Kind.InvalidEndOfLineError);
                        goto RETURN;
                    case 5:
                        page = null;
                        start = 0;
                        length = 0;
                        answer = new TryInterpretReturnValue(span, ErrorSentence.Kind.InvalidMinusNumberError);
                        goto RETURN;
                    case 4:
                        tempList.This.Values[tempList.This.Length - 1].Number = number;
                        tempList.This.Values[tempList.This.Length - 1].NumberSpan = numberSpan;
                        span = new Span(current, 0);
                        goto SUCCESS;
                    case 6:
                        span = new Span(current, 0);
                        goto SUCCESS;
                }
            }
        SUCCESS:
            length = tempList.This.Length;
            pairList.AddRange(tempList.This.Values, tempList.This.Length, out page, out start, allocator);
            answer = new TryInterpretReturnValue(span, SuccessSentence.Kind.AssignmentInterpretationSuccess);
            goto RETURN;
        ERROR:
            page = null;
            start = 0;
            length = 0;
            answer = TryInterpretReturnValue.CreateNotExpectedCharacter(current);
        RETURN:
            tempList.Dispose(Allocator.Temp);
            return answer;
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
                        return new TryInterpretReturnValue(new Span(filePathId, line, i, 0), SuccessSentence.Kind.IdentifierInterpretSuccess);
                    default:
                        span.Length++;
                        return new TryInterpretReturnValue(span, ErrorSentence.Kind.InvalidIdentifierError);
                }
            }
            if (onlyDigit)
                return new TryInterpretReturnValue(span, ErrorSentence.Kind.IdentifierCannotBeNumberError);
            return new TryInterpretReturnValue(span, SuccessSentence.Kind.IdentifierInterpretSuccess);
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
                                state = 1;
                                value = *cptr - '0';
                                span.Length++;
                                span.Column = i;
                                break;
                            case '-':
                                state = 2;
                                span.Length++;
                                span.Column = i;
                                break;
                            default:
                                return new TryInterpretReturnValue(new Span(file.FilePathId, current.Line, i, 1), ErrorSentence.Kind.NotNumberError);
                        }
                        break;
                    case 1:
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
                                span.Length++;
                                value *= 10;
                                value += *cptr - '0';
                                break;
                            default:
                                return new TryInterpretReturnValue(new Span(file.FilePathId, current.Line, i, 1), ErrorSentence.Kind.NotNumberError);
                        }
                        break;
                    case 2:
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
                                span.Length++;
                                value *= 10;
                                value -= *cptr - '0';
                                break;
                            default:
                                return new TryInterpretReturnValue(new Span(file.FilePathId, current.Line, i, 1), ErrorSentence.Kind.NotNumberError);
                        }
                        break;
                }
            }
            return new TryInterpretReturnValue(span, SuccessSentence.Kind.NumberInterpretSuccess);
        }

        public static TryInterpretReturnValue TryReadIdentifiers(this ref TextFile file, ref IdentifierListLinkedList list, Caret current, out IdentifierList* page, out int start, out int length, Allocator allocator)
        {
            file.SkipWhiteSpace(ref current);
            if (file.CurrentChar(current) == '@')
            {
                page = null;
                start = 0;
                length = 0;
                return new TryInterpretReturnValue(new Span(current, 1), SuccessSentence.Kind.AssignmentInterpretationSuccess);
            }
            var tempList = new IdentifierList(16, Allocator.Temp);
            int state = 0;
            ref var raw = ref current.Line;
            ref var column = ref current.Column;
            bool isOnlyDigit = true;
            TryInterpretReturnValue answer = default;
            for (; raw < file.LineCount; raw++, column = 0)
            {
                for (; column < file.LineLengths[raw]; column++)
                {
                PARSE:
                    var c = (file.Contents + file.LineStarts[raw])[column];
                    switch (state)
                    {
                        case 0: // Seek for the first char of the identifier.
                            switch (c)
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
                                    isOnlyDigit = false;
                                    if (++tempList.This.Length > tempList.This.Capacity)
                                    {
                                        tempList.Lengthen(Allocator.Temp);
                                    }
                                    tempList.This.Values[tempList.This.Length - 1].Start = current;
                                    tempList.This.Values[tempList.This.Length - 1].Length = 1;
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
                                    if (++tempList.This.Length > tempList.This.Capacity)
                                    {
                                        tempList.Lengthen(Allocator.Temp);
                                    }
                                    tempList.This.Values[tempList.This.Length - 1].Start = current;
                                    tempList.This.Values[tempList.This.Length - 1].Length = 1;
                                    break;
                                case ' ':
                                case '\t':
                                    break;
                                case ',':
                                default:
                                    goto ERROR;
                            }
                            break;
                        case 1: // Seek for the rest chars of the identifier.
                            switch (c)
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
                                    tempList.This.Values[tempList.This.Length - 1].Length++;
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
                                    tempList.This.Values[tempList.This.Length - 1].Length++;
                                    break;
                                case ' ':
                                case '\t':
                                    state = 2;
                                    if (isOnlyDigit)
                                        goto ERROR;
                                    break;
                                case ',':
                                    if (isOnlyDigit)
                                        goto ERROR;
                                    state = 0;
                                    column++;
                                    file.SkipWhiteSpace(ref current);
                                    goto PARSE;
                                default:
                                    goto ERROR;
                            }
                            break;
                        case 2: // Seek for the comma
                            switch (c)
                            {
                                case ' ':
                                case '\t':
                                    break;
                                case ',':
                                    state = 0;
                                    column++;
                                    file.SkipWhiteSpace(ref current);
                                    goto PARSE;
                                default:
                                    goto ERROR;
                            }
                            break;
                    }
                }
                if (state != 0)
                    goto SUCCESS;
            }
        SUCCESS:
            length = tempList.This.Length;
            list.AddRange(tempList.This.Values, tempList.This.Length, out page, out start, allocator);
            answer = new TryInterpretReturnValue(tempList.This.Values[tempList.This.Length - 1], SuccessSentence.Kind.AssignmentInterpretationSuccess);
            goto RETURN;
        ERROR:
            page = null;
            start = 0;
            length = 0;
            answer = TryInterpretReturnValue.CreateNotExpectedCharacter(current);
        RETURN:
            tempList.Dispose(Allocator.Temp);
            return answer;
        }


        public static TryInterpretReturnValue TryReadStringsEndWithSemicolon(this ref TextFile file, ref NativeStringListLinkedList strings, ref NativeStringMemorySoyrceLinkedList memories, Caret current, out NativeStringList* page, out int start, out int length, Allocator allocator)
        {
            var node = new ListLinkedListNode(8, sizeof(NativeString), Allocator.Temp);
            bool isComplete;
            length = 0;
            var nativeStringLength = 0;

        READ_LOOP:
            if (node.IsFull)
                ListUtility.Lengthen(ref node.Values, ref node.Capacity, sizeof(NativeString), Allocator.Temp);
            ref var lastString = ref node.GetRef<NativeString>(length);
            if (!ReadNativeString(ref file, ref current, out lastString, out isComplete))
                goto ERROR;
            if (!lastString.IsEmpty)
            {
                nativeStringLength += lastString.Length;
                node.Length = ++length;
            }
            if (isComplete) goto COMPLETE;
#if UNITY_EDITOR
            if (lastString.IsEmpty) throw new System.InvalidOperationException();
#endif
            goto READ_LOOP;

        ERROR:
            page = null;
            start = 0;
            // Error吐く以前のはキレイキレイしてあげなくては洩れる
            for (int i = 0; i < node.Length; i++)
            {
                var values = node.GetRef<NativeString>(i).Values;
                if (values != null)
                    UnsafeUtility.Free(values, Allocator.Temp);
            }
            node.Dispose(Allocator.Temp);
            return new TryInterpretReturnValue(current, ErrorSentence.Kind.SentencesEndWithSemicolonInterpretError);
        COMPLETE:
            if (length != 0)
            {
                // 断片化したメモリ領域に散らばった文字列集合を１つの文字列にまとめる
                var tempMemory = (ushort*)UnsafeUtility.Malloc(sizeof(ushort) * nativeStringLength, 2, Allocator.Temp);
                ref var ptri = ref node.GetRef<NativeString>(0);
                for (int i = 0, accum = 0; i < length; i++)
                {
                    ptri = ref node.GetRef<NativeString>(i);
                    if (ptri.IsAtmark) continue;
                    // １つの文字列にコピペしたら元のは解放する
                    UnsafeUtility.MemCpy(tempMemory + accum, ptri.Values, sizeof(ushort) * ptri.Length);
                    UnsafeUtility.Free(ptri.Values, Allocator.Temp);
                    ptri.Values = tempMemory + accum;
                    accum += ptri.Length;
                }
                strings.AddRange(node.GetPointer<NativeString>(0), length, out page, out start, allocator);
                memories.AddRange(tempMemory, nativeStringLength, allocator);
                UnsafeUtility.Free(tempMemory, Allocator.Temp);
            }
            else
            {
                page = null;
                start = 0;
            }
            node.Dispose(Allocator.Temp);
            return new TryInterpretReturnValue(current, SuccessSentence.Kind.SentencesEndWithSemicolonInterpretSuccess);
        }

        private static bool ReadNativeString(ref TextFile file, ref Caret current, out NativeString answer, out bool isComplete)
        {
            answer = new NativeString { File = file.FilePathId };
            var tempCapacity = 256;
            var tempList = (ushort*)UnsafeUtility.Malloc(sizeof(ushort) * tempCapacity, 2, Allocator.Temp);
            file.SkipWhiteSpace(ref current);
            var c = file.CurrentChar(current);
            switch (c)
            {
                case '@':
                    answer.ColumnStartInclusive = current.Column;
                    answer.ColumnEndExclusive = current.Column + 1;
                    answer.LineStartInclusive = answer.LineEndInclusive = current.Line;
                    current.Column++;
                    file.SkipWhiteSpace(ref current);
                    switch (c)
                    {
                        case ';':
                            UnsafeUtility.Free(tempList, Allocator.Temp);
                            goto COMPLETE;
                        case ',':
                            goto CONTINUE;
                        default:
                            goto ERROR;
                    }
                case ',':
                    goto ERROR;
                case ';':
                    UnsafeUtility.Free(tempList, Allocator.Temp);
                    goto COMPLETE;
                default:
                    answer.Length = 1;
                    answer.ColumnStartInclusive = current.Column;
                    answer.LineStartInclusive = current.Line;
                    *tempList = c;
                    current.Column++;
                    break;
            }
            for (; current.Line < file.LineCount; current.Column = 0, current.Line++)
            {
                file.SkipWhiteSpace(ref current);
                for (; current.Column < file.LineLengths[current.Line]; current.Column++)
                {
                    c = file.CurrentChar(current);
                    switch (c)
                    {
                        case '@':
                            goto ERROR;
                        case ';':
                            answer.LineEndInclusive = current.Line;
                            answer.ColumnEndExclusive = current.Column;
                            answer.Values = tempList;
                            goto COMPLETE;
                        case ',':
                            answer.LineEndInclusive = current.Line;
                            answer.ColumnEndExclusive = current.Column;
                            answer.Values = tempList;
                            goto CONTINUE;
                        default:
                            if (answer.Length == tempCapacity)
                            {
                                ListUtility.Lengthen(ref tempList, ref tempCapacity, Allocator.Temp);
                            }
                            tempList[answer.Length++] = c;
                            break;
                    }
                }
            }
        CONTINUE:
            current.Column++;
            isComplete = false;
            return true;
        COMPLETE:
            current.Column++;
            isComplete = true;
            return true;
        ERROR:
            UnsafeUtility.Free(tempList, Allocator.Temp);
            answer = default;
            isComplete = true;
            return false;
        }
    }
}