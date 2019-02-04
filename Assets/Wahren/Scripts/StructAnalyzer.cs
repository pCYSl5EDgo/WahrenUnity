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

        public static bool IsStructKindWithName(int subDataIndex)
        {
            switch (subDataIndex)
            {
                // No Name
                case 3:
                case 9:
                case 11:
                case 14:
                case 19:
                    return false;
                // Name
                // case 0:
                // case 1:
                // case 2:
                // case 4:
                // case 5:
                // case 6:
                // case 7:
                // case 8:
                // case 10:
                // case 12:
                // case 13:
                // case 15:
                // case 16:
                // case 17:
                // case 18:
                default:
                    return true;
            }
        }

        public static TryInterpretReturnValue TryGetFirstStructLocation(this ref TextFile file, Caret start)
        {
            file.SkipWhiteSpace(ref start);
            var answer = new TryInterpretReturnValue(start, ErrorSentence.StructKindNotFoundError, InterpreterStatus.Error);
            switch (file.Lines[answer.Span.Line][answer.Span.Column])
            {
                case 'p': // power
                    answer.PowerDetect(answer.Span.Column, file.LineLengths[answer.Span.Line], file.Lines[answer.Span.Line] + answer.Span.Column);
                    goto RETURN;
                case 'u': // unit
                    answer.UnitDetect(answer.Span.Column, file.LineLengths[answer.Span.Line], file.Lines[answer.Span.Line] + answer.Span.Column);
                    goto RETURN;
                case 'r': // race
                    answer.RaceDetect(answer.Span.Column, file.LineLengths[answer.Span.Line], file.Lines[answer.Span.Line] + answer.Span.Column);
                    goto RETURN;
                case 'a': // attribute
                    answer.AttributeDetect(answer.Span.Column, file.LineLengths[answer.Span.Line], file.Lines[answer.Span.Line] + answer.Span.Column);
                    goto RETURN;
                case 'f': // field
                    answer.FieldDetect(answer.Span.Column, file.LineLengths[answer.Span.Line], file.Lines[answer.Span.Line] + answer.Span.Column);
                    goto RETURN;
                case 'o': // object
                    answer.ObjectDetect(answer.Span.Column, file.LineLengths[answer.Span.Line], file.Lines[answer.Span.Line] + answer.Span.Column);
                    goto RETURN;
                case 'm': // movetype
                    answer.MoveTypeDetect(answer.Span.Column, file.LineLengths[answer.Span.Line], file.Lines[answer.Span.Line] + answer.Span.Column);
                    goto RETURN;
                case 'e': // event
                    answer.EventDetect(answer.Span.Column, file.LineLengths[answer.Span.Line], file.Lines[answer.Span.Line] + answer.Span.Column);
                    goto RETURN;
                case 'd': // dungeon, detail
                    answer.DungeonOrDetailDetect(answer.Span.Column, file.LineLengths[answer.Span.Line], file.Lines[answer.Span.Line] + answer.Span.Column);
                    goto RETURN;
                case 'c': // class, context
                    answer.ClassOrContextDetect(answer.Span.Column, file.LineLengths[answer.Span.Line], file.Lines[answer.Span.Line] + answer.Span.Column);
                    goto RETURN;
                case 's': // scenario, skill, skillset, sound, story
                    answer.SDetect(answer.Span.Column, file.LineLengths[answer.Span.Line], file.Lines[answer.Span.Line] + answer.Span.Column);
                    goto RETURN;
                case 'w': // workspace
                    answer.WorkspaceDetect(answer.Span.Column, file.LineLengths[answer.Span.Line], file.Lines[answer.Span.Line] + answer.Span.Column);
                    goto RETURN;
                case 'v': // voice
                    answer.VoiceDetect(answer.Span.Column, file.LineLengths[answer.Span.Line], file.Lines[answer.Span.Line] + answer.Span.Column);
                    goto RETURN;
                default:
                    goto RETURN;
            }
        RETURN:
            return answer;
        }

        private static void WorkspaceDetect(this ref TryInterpretReturnValue answer, int column, int thisLineLength, char* ccp)
        {
            if (column + 8 < thisLineLength && *++ccp == 'o' && *++ccp == 'r' && *++ccp == 'k' && *++ccp == 's' && *++ccp == 'p' && *++ccp == 'a' && *++ccp == 'c' && *++ccp == 'e' && IsNextEndOfLineOrSpace(ccp, column + 8, thisLineLength))
                answer.Success(19, 5);
            else
                answer.Fail(23);
        }

        private static void VoiceDetect(this ref TryInterpretReturnValue answer, int column, int thisLineLength, char* ccp)
        {
            if (column + 4 < thisLineLength && *++ccp == 'o' && *++ccp == 'i' && *++ccp == 'c' && *++ccp == 'e' && IsNextEndOfLineOrSpace(ccp, column + 4, thisLineLength))
                answer.Success(18, 5);
            else
                answer.Fail(22);
        }

        private static void SDetect(this ref TryInterpretReturnValue answer, int column, int thisLineLength, char* ccp)
        {
            if (column + 3 >= thisLineLength)
                answer.Fail(20);
            else switch (*++ccp)
                {
                    case 'k':
                        if (column + 4 < thisLineLength && *++ccp == 'i' && *++ccp == 'l' && *++ccp == 'l')
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
                        if (column + 4 < thisLineLength && *++ccp == 'u' && *++ccp == 'n' && *++ccp == 'd' && IsNextEndOfLineOrSpaceOrLeftBrace(ccp, column + 4, thisLineLength))
                            answer.Success(14, 5);
                        else answer.Fail(17);
                        break;
                    case 't':
                        if (column + 4 < thisLineLength && *++ccp == 'o' && *++ccp == 'r' && *++ccp == 'y' && IsNextEndOfLineOrSpace(ccp, column + 4, thisLineLength))
                            answer.Success(15, 5);
                        else answer.Fail(18);
                        break;
                    case 'c':
                        if (column + 7 < thisLineLength && *++ccp == 'e' && *++ccp == 'n' && *++ccp == 'a' && *++ccp == 'r' && *++ccp == 'i' && *++ccp == 'o' && IsNextEndOfLineOrSpace(ccp, column + 7, thisLineLength))
                            answer.Success(16, 8);
                        else answer.Fail(19);
                        break;
                    case 'p':
                        if (*++ccp == 'o' && *++ccp == 't' && IsNextEndOfLineOrSpace(ccp, column + 3, thisLineLength))
                            answer.Success(17, 4);
                        else answer.Fail(21);
                        break;
                    default:
                        answer.Fail(20);
                        break;
                }
        }

        private static void ClassOrContextDetect(this ref TryInterpretReturnValue answer, int column, int thisLineLength, char* ccp)
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

        private static void DungeonOrDetailDetect(this ref TryInterpretReturnValue answer, int column, int thisLineLength, char* ccp)
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

        private static void EventDetect(this ref TryInterpretReturnValue answer, int column, int thisLineLength, char* ccp)
        {
            if (column + 4 < thisLineLength && *++ccp == 'v' && *++ccp == 'e' && *++ccp == 'n' && *++ccp == 't' && IsNextEndOfLineOrSpace(ccp, column + 4, thisLineLength))
                answer.Success(7, 5);
            else
                answer.Fail(7);
        }

        private static void MoveTypeDetect(this ref TryInterpretReturnValue answer, int column, int thisLineLength, char* ccp)
        {
            if (column + 7 < thisLineLength && *++ccp == 'o' && *++ccp == 'v' && *++ccp == 'e' && *++ccp == 't' && *++ccp == 'y' && *++ccp == 'p' && *++ccp == 'e' && IsNextEndOfLineOrSpace(ccp, column + 7, thisLineLength))
                answer.Success(6, 6);
            else
                answer.Fail(6);
        }

        private static void ObjectDetect(this ref TryInterpretReturnValue answer, int column, int thisLineLength, char* ccp)
        {
            if (column + 5 < thisLineLength && *++ccp == 'b' && *++ccp == 'j' && *++ccp == 'e' && *++ccp == 'c' && *++ccp == 't' && IsNextEndOfLineOrSpace(ccp, column + 5, thisLineLength))
                answer.Success(5, 6);
            else
                answer.Fail(5);
        }

        private static void FieldDetect(this ref TryInterpretReturnValue answer, int column, int thisLineLength, char* ccp)
        {
            if (column + 4 < thisLineLength && *++ccp == 'i' && *++ccp == 'e' && *++ccp == 'l' && *++ccp == 'd' && IsNextEndOfLineOrSpace(ccp, column + 4, thisLineLength))
                answer.Success(4, 5);
            else
                answer.Fail(4);
        }

        private static void AttributeDetect(this ref TryInterpretReturnValue answer, int column, int thisLineLength, char* ccp)
        {
            if (column + 8 < thisLineLength && *++ccp == 't' && *++ccp == 't' && *++ccp == 'r' && *++ccp == 'i' && *++ccp == 'b' && *++ccp == 'u' && *++ccp == 't' && *++ccp == 'e' && IsNextEndOfLineOrSpace(ccp, column + 8, thisLineLength))
                answer.Success(3, 9);
            else
                answer.Fail(3);
        }

        private static void RaceDetect(this ref TryInterpretReturnValue answer, int column, int thisLineLength, char* ccp)
        {
            if (column + 3 < thisLineLength && *++ccp == 'a' && *++ccp == 'c' && *++ccp == 'e' && IsNextEndOfLineOrSpace(ccp, column + 3, thisLineLength))
                answer.Success(2, 4);
            else
                answer.Fail(2);
        }

        private static void UnitDetect(this ref TryInterpretReturnValue answer, int column, int thisLineLength, char* ccp)
        {
            if (column + 3 < thisLineLength && *++ccp == 'n' && *++ccp == 'i' && *++ccp == 't' && IsNextEndOfLineOrSpace(ccp, column + 3, thisLineLength))
                answer.Success(1, 4);
            else
                answer.Fail(1);
        }

        private static void PowerDetect(this ref TryInterpretReturnValue answer, int column, int thisLineLength, char* ccp)
        {
            if (column + 4 < thisLineLength && *++ccp == 'o' && *++ccp == 'w' && *++ccp == 'e' && *++ccp == 'r' && IsNextEndOfLineOrSpace(ccp, column + 4, thisLineLength))
                answer.Success(0, 5);
            else
                answer.Fail(0);
        }

        private static void Fail(this ref TryInterpretReturnValue answer, byte errorSubData)
        {
            answer.Status = 0;
            answer.DataIndex = ErrorSentence.StructKindInterpretError;
            answer.SubDataIndex = errorSubData;
        }

        private static void Success(this ref TryInterpretReturnValue answer, byte successSubData, int length)
        {
            answer.Status = InterpreterStatus.Success;
            answer.DataIndex = SuccessSentence.StructKindInterpretSuccess;
            answer.SubDataIndex = successSubData;
            answer.Span.Length = length;
        }
    }
}