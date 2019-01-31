using System;

namespace pcysl5edgo.Wahren
{
    public unsafe static class StructAnalyzer
    {
        internal static bool IsNextEndOfLineOrSpaceOrLeftBrace(char* lastChar, int column, int length)
        {
            if (column + 1 == length) return true;
            switch (*++lastChar)
            {
                case ' ':
                case '\t':
                case '{':
                    return true;
                default:
                    return false;
            }
        }
        internal static bool IsNextEndOfLineOrSpace(char* lastChar, int column, int length)
        {
            if (column == length - 1) return true;
            switch (*++lastChar)
            {
                case ' ':
                case '\t':
                    return true;
                default:
                    return false;
            }
        }
        public static TryInterpretReturnValue TryGetFirstStructLocationUnsafe(this ref TextFile file, Span spanIgnoreLength)
        {
            if (spanIgnoreLength.Line < 0 || spanIgnoreLength.Column < 0 || spanIgnoreLength.Line >= file.LineCount || (spanIgnoreLength.Column > file.LineLengths[spanIgnoreLength.Line]))
                throw new ArgumentOutOfRangeException($"spanIgnoreLength : {spanIgnoreLength}\nLineCount : {file.LineCount}");
            spanIgnoreLength.File = file.FilePathId;
            var tmpLines = file.Lines;
            var answer = new TryInterpretReturnValue(ref spanIgnoreLength, 0, 0, true);
            answer.DataIndex = ErrorSentence.StructKindNotFoundError;
            ref var foundSpan = ref answer.Span;
            ref int raw = ref foundSpan.Line;
            ref int column = ref foundSpan.Column;
            ref int length = ref foundSpan.Length;
            char* currentLine;
            for (; raw < file.LineCount; raw++, column = 0)
            {
                currentLine = file.Lines[raw];
                for (int thisLineLength = file.LineLengths[raw]; column < thisLineLength; column++)
                {
                    char* ccp = currentLine + column; // current char pointer
                    switch (currentLine[column])
                    {
                        case 'p': // power
                            PowerDetect(ref answer, column, thisLineLength, ref ccp);
                            goto RETURN;
                        case 'u': // unit
                            UnitDetect(ref answer, column, thisLineLength, ref ccp);
                            goto RETURN;
                        case 'r': // race
                            RaceDetect(ref answer, column, thisLineLength, ref ccp);
                            goto RETURN;
                        case 'a': // attribute
                            AttributeDetect(ref answer, column, thisLineLength, ref ccp);
                            goto RETURN;
                        case 'f': // field
                            FieldDetect(ref answer, column, thisLineLength, ref ccp);
                            goto RETURN;
                        case 'o': // object
                            ObjectDetect(ref answer, column, thisLineLength, ref ccp);
                            goto RETURN;
                        case 'm': // movetype
                            MoveTypeDetect(ref answer, column, thisLineLength, ref ccp);
                            goto RETURN;
                        case 'e': // event
                            EventDetect(ref answer, column, thisLineLength, ref ccp);
                            goto RETURN;
                        case 'd': // dungeon, detail
                            DungeonOrDetailDetect(ref answer, column, thisLineLength, ref ccp);
                            goto RETURN;
                        case 'c': // class, context
                            ClassOrContextDetect(ref answer, column, thisLineLength, ref ccp);
                            goto RETURN;
                        case 's': // scenario, skill, skillset, sound, story
                            SDetect(ref answer, column, thisLineLength, ref ccp);
                            goto RETURN;
                        case ' ':
                        case '\t':
                            continue;
                        default:
                            goto RETURN;
                    }
                }
            }
        RETURN:
            return answer;
        }

        private static void SDetect(ref TryInterpretReturnValue answer, int column, int thisLineLength, ref char* ccp)
        {
            if (column + 4 >= thisLineLength)
                answer.Fail(20);
            else switch (*++ccp)
                {
                    case 'k':
                        if (*++ccp == 'i' && *++ccp == 'l' && *++ccp == 'l')
                        {
                            if (IsNextEndOfLineOrSpace(ccp, column + 4, thisLineLength))
                                answer.Success(12, 5);
                            else if (*++ccp == 's' && column + 7 < thisLineLength && *++ccp == 'e' && *++ccp == 't' && IsNextEndOfLineOrSpace(ccp, column + 7, thisLineLength))
                                answer.Success(13, 8);
                            else
                                answer.Fail(15);
                        }
                        else answer.Fail(16);
                        break;
                    case 'o':
                        if (*++ccp == 'u' && *++ccp == 'n' && *++ccp == 'd' && IsNextEndOfLineOrSpaceOrLeftBrace(ccp, column + 4, thisLineLength))
                            answer.Success(14, 5);
                        else answer.Fail(17);
                        break;
                    case 't':
                        if (*++ccp == 'o' && *++ccp == 'r' && *++ccp == 'y' && IsNextEndOfLineOrSpace(ccp, column + 4, thisLineLength))
                            answer.Success(15, 5);
                        else answer.Fail(18);
                        break;
                    case 'c':
                        if (column + 7 < thisLineLength && *++ccp == 'e' && *++ccp == 'n' && *++ccp == 'a' && *++ccp == 'r' && *++ccp == 'i' && *++ccp == 'o' && IsNextEndOfLineOrSpace(ccp, column + 7, thisLineLength))
                            answer.Success(16, 8);
                        else answer.Fail(19);
                        break;
                    default:
                        answer.Fail(20);
                        break;
                }
        }

        private static void ClassOrContextDetect(ref TryInterpretReturnValue answer, int column, int thisLineLength, ref char* ccp)
        {
            if (column + 4 >= thisLineLength)
                answer.Fail(13);
            else switch (*++ccp)
                {
                    case 'o':
                        if (column + 6 < thisLineLength && *++ccp == 'n' && *++ccp == 't' && *++ccp == 'e' && *++ccp == 'x' && *++ccp == 't' && IsNextEndOfLineOrSpaceOrLeftBrace(ccp, column + 6, thisLineLength))
                            answer.Success(11, 7);
                        else answer.Fail(12);
                        break;
                    case 'l':
                        if (*++ccp == 'a' && *++ccp == 's' && *++ccp == 's' && IsNextEndOfLineOrSpace(ccp, column + 4, thisLineLength))
                            answer.Success(10, 5);
                        else answer.Fail(11);
                        break;
                    default:
                        answer.Fail(13);
                        break;
                }
        }

        private static void DungeonOrDetailDetect(ref TryInterpretReturnValue answer, int column, int thisLineLength, ref char* ccp)
        {
            if (column + 5 >= thisLineLength)
                answer.Fail(10);
            else switch (*++ccp)
                {
                    case 'u':
                        if (column + 6 < thisLineLength && *++ccp == 'n' && *++ccp == 'g' && *++ccp == 'e' && *++ccp == 'o' && *++ccp == 'n' && IsNextEndOfLineOrSpace(ccp, column + 6, thisLineLength))
                            answer.Success(8, 7);
                        else
                            answer.Fail(8);
                        break;
                    case 'e':
                        if (*++ccp == 't' && *++ccp == 'a' && *++ccp == 'i' && *++ccp == 'l' && IsNextEndOfLineOrSpace(ccp, column + 5, thisLineLength))
                            answer.Success(9, 6);
                        else
                            answer.Fail(9);
                        break;
                    default:
                        answer.Fail(10);
                        break;
                }
        }

        private static void EventDetect(ref TryInterpretReturnValue answer, int column, int thisLineLength, ref char* ccp)
        {
            if (column + 4 < thisLineLength && *++ccp == 'v' && *++ccp == 'e' && *++ccp == 'n' && *++ccp == 't' && IsNextEndOfLineOrSpace(ccp, column + 3, thisLineLength))
                answer.Success(7, 5);
            else
                answer.Fail(7);
        }

        private static void MoveTypeDetect(ref TryInterpretReturnValue answer, int column, int thisLineLength, ref char* ccp)
        {
            if (column + 5 < thisLineLength && *++ccp == 'o' && *++ccp == 'v' && *++ccp == 'e' && *++ccp == 't' && *++ccp == 'y' && *++ccp == 'p' && *++ccp == 'e' && IsNextEndOfLineOrSpace(ccp, column + 7, thisLineLength))
                answer.Success(6, 6);
            else
                answer.Fail(6);
        }

        private static void ObjectDetect(ref TryInterpretReturnValue answer, int column, int thisLineLength, ref char* ccp)
        {
            if (column + 5 < thisLineLength && *++ccp == 'b' && *++ccp == 'j' && *++ccp == 'e' && *++ccp == 'c' && *++ccp == 't' && IsNextEndOfLineOrSpace(ccp, column + 5, thisLineLength))
                answer.Success(5, 6);
            else
                answer.Fail(5);
        }

        private static void FieldDetect(ref TryInterpretReturnValue answer, int column, int thisLineLength, ref char* ccp)
        {
            if (column + 4 < thisLineLength && *++ccp == 'i' && *++ccp == 'e' && *++ccp == 'l' && *++ccp == 'd' && IsNextEndOfLineOrSpace(ccp, column + 4, thisLineLength))
                answer.Success(4, 5);
            else
                answer.Fail(4);
        }

        private static void AttributeDetect(ref TryInterpretReturnValue answer, int column, int thisLineLength, ref char* ccp)
        {
            if (column + 8 < thisLineLength && *++ccp == 't' && *++ccp == 't' && *++ccp == 'r' && *++ccp == 'i' && *++ccp == 'b' && *++ccp == 'u' && *++ccp == 't' && *++ccp == 'e' && IsNextEndOfLineOrSpace(ccp, column + 8, thisLineLength))
                answer.Success(3, 9);
            else
                answer.Fail(3);
        }

        private static void RaceDetect(ref TryInterpretReturnValue answer, int column, int thisLineLength, ref char* ccp)
        {
            if (column + 3 < thisLineLength && *++ccp == 'a' && *++ccp == 'c' && *++ccp == 'e' && IsNextEndOfLineOrSpace(ccp, column + 3, thisLineLength))
                answer.Success(2, 4);
            else
                answer.Fail(2);
        }

        private static void UnitDetect(ref TryInterpretReturnValue answer, int column, int thisLineLength, ref char* ccp)
        {
            if (column + 3 < thisLineLength && *++ccp == 'n' && *++ccp == 'i' && *++ccp == 't' && IsNextEndOfLineOrSpace(ccp, column + 3, thisLineLength))
                answer.Success(1, 4);
            else
                answer.Fail(1);
        }

        private static void PowerDetect(ref TryInterpretReturnValue answer, int column, int thisLineLength, ref char* ccp)
        {
            if (column + 4 < thisLineLength && *++ccp == 'o' && *++ccp == 'w' && *++ccp == 'e' && *++ccp == 'r' && IsNextEndOfLineOrSpace(ccp, column + 4, thisLineLength))
                answer.Success(0, 5);
            else
                answer.Fail(0);
        }

        private static void Fail(this ref TryInterpretReturnValue answer, byte errorSubData)
        {
            answer.isSuccess = 0;
            answer.DataIndex = ErrorSentence.StructKindInterpretError;
            answer.SubDataIndex = errorSubData;
        }

        private static void Success(this ref TryInterpretReturnValue answer, byte successSubData, int length)
        {
            answer.isSuccess = 1;
            answer.DataIndex = SuccessSentence.StructKindInterpretSuccess;
            answer.SubDataIndex = successSubData;
            answer.Span.Length = length;
        }
    }
}