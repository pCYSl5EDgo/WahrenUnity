using System;

namespace pcysl5edgo.Wahren
{
    public unsafe static class StructAnalyzer
    {
        internal static bool IsNextEndOfLineOrSpaceOrLeftBrace(char* lastChar, int column, int length, bool isDebug)
        {
            if (column + 1 == length) return true;
            switch (*++lastChar)
            {
                case ' ':
                case '\t':
                case '{':
                    return true;
                case '/':
                    if (column + 2 == length) return false;
                    switch (*++lastChar)
                    {
                        case '/':
                            return true;
                        case '+':
                            if (isDebug)
                                return IsNextEndOfLineOrSpaceOrLeftBrace(lastChar, column + 2, length, isDebug);
                            return true;
                        default:
                            return false;
                    }
                default:
                    return false;
            }
        }
        internal static bool IsNextEndOfLineOrSpace(char* lastChar, int column, int length, bool isDebug)
        {
            if (column == length - 1) return true;
            switch (*++lastChar)
            {
                case ' ':
                case '\t':
                    return true;
                case '/':
                    if (column + 2 >= length) return false;
                    switch (*++lastChar)
                    {
                        case '/':
                            return true;
                        case '+':
                            if (isDebug)
                                return IsNextEndOfLineOrSpace(lastChar, column + 2, length, isDebug);
                            return true;
                        default:
                            return false;
                    }
                default:
                    return false;
            }
        }
        public static TryInterpretReturnValue TryGetFirstStructLocationUnsafe(this ref TextFile file, bool isDebug, Span spanIgnoreLength)
        {
            if (spanIgnoreLength.Line < 0 || spanIgnoreLength.Column < 0 || spanIgnoreLength.Line >= file.LineCount || (spanIgnoreLength.Column > file.LineLengths[spanIgnoreLength.Line]))
                throw new ArgumentOutOfRangeException($"spanIgnoreLength : {spanIgnoreLength}\nLineCount : {file.LineCount}");
            var tmpLines = file.Lines;
            var answer = new TryInterpretReturnValue(ref spanIgnoreLength, 0, 0, true);
            ref var foundSpan = ref answer.Span;
            ref int raw = ref foundSpan.Line;
            ref int column = ref foundSpan.Column;
            ref int length = ref foundSpan.Length;
            int state = 0;
            char* currentLine;
            for (; raw < file.LineCount; raw++, column = state = 0)
            {
                currentLine = file.Lines[raw];
                for (int thisLineLength = file.LineLengths[raw]; column < thisLineLength; column++)
                {
                    char* ccp = currentLine + column; // current char pointer
                    switch (state)
                    {
                        case 0:
                            switch (currentLine[column])
                            {
                                case '/':
                                    state = 1;
                                    break;
                                case 'p': // power
                                    if (column + 4 < thisLineLength && *++ccp == 'o' && *++ccp == 'w' && *++ccp == 'e' && *++ccp == 'r' && IsNextEndOfLineOrSpace(ccp, column + 4, thisLineLength, isDebug))
                                        length = 5;
                                    else
                                        answer.Fail(0);
                                    return answer;
                                case 'u': // unit
                                    if (column + 3 < thisLineLength && *++ccp == 'n' && *++ccp == 'i' && *++ccp == 't' && IsNextEndOfLineOrSpace(ccp, column + 3, thisLineLength, isDebug))
                                        length = 4;
                                    else
                                        answer.Fail(1);
                                    return answer;
                                case 'r': // race
                                    if (column + 3 < thisLineLength && *++ccp == 'a' && *++ccp == 'c' && *++ccp == 'e' && IsNextEndOfLineOrSpace(ccp, column + 3, thisLineLength, isDebug))
                                        foundSpan.Length = 4;
                                    else
                                        answer.Fail(2);
                                    return answer;
                                case 'a': // attribute
                                    if (column + 8 < thisLineLength && *++ccp == 't' && *++ccp == 't' && *++ccp == 'r' && *++ccp == 'i' && *++ccp == 'b' && *++ccp == 'u' && *++ccp == 't' && *++ccp == 'e' && IsNextEndOfLineOrSpace(ccp, column + 8, thisLineLength, isDebug))
                                        foundSpan.Length = 9;
                                    else
                                        answer.Fail(3);
                                    return answer;
                                case 'f': // field
                                    if (column + 4 < thisLineLength && *++ccp == 'i' && *++ccp == 'e' && *++ccp == 'l' && *++ccp == 'd' && IsNextEndOfLineOrSpace(ccp, column + 4, thisLineLength, isDebug))
                                        foundSpan.Length = 5;
                                    else
                                        answer.Fail(4);
                                    return answer;
                                case 'o': // object
                                    if (column + 5 < thisLineLength && *++ccp == 'b' && *++ccp == 'j' && *++ccp == 'e' && *++ccp == 'c' && *++ccp == 't' && IsNextEndOfLineOrSpace(ccp, column + 5, thisLineLength, isDebug))
                                        foundSpan.Length = 6;
                                    else
                                        answer.Fail(5);
                                    return answer;
                                case 'm': // movetype
                                    if (column + 5 < thisLineLength && *++ccp == 'o' && *++ccp == 'v' && *++ccp == 'e' && *++ccp == 't' && *++ccp == 'y' && *++ccp == 'p' && *++ccp == 'e' && IsNextEndOfLineOrSpace(ccp, column + 7, thisLineLength, isDebug))
                                        foundSpan.Length = 8;
                                    else
                                        answer.Fail(6);
                                    return answer;
                                case 'e': // event
                                    if (column + 4 < thisLineLength && *++ccp == 'v' && *++ccp == 'e' && *++ccp == 'n' && *++ccp == 't' && IsNextEndOfLineOrSpace(ccp, column + 3, thisLineLength, isDebug))
                                        foundSpan.Length = 5;
                                    else
                                        answer.Fail(7);
                                    return answer;
                                case 'd': // dungeon, detail
                                    if (column + 5 >= thisLineLength)
                                    {
                                        answer.Fail(10);
                                        return answer;
                                    }
                                    switch (*++ccp)
                                    {
                                        case 'u':
                                            if (column + 6 < thisLineLength && *++ccp == 'n' && *++ccp == 'g' && *++ccp == 'e' && *++ccp == 'o' && *++ccp == 'n' && IsNextEndOfLineOrSpace(ccp, column + 6, thisLineLength, isDebug))
                                                foundSpan.Length = 7;
                                            else
                                                answer.Fail(8);
                                            return answer;
                                        case 'e':
                                            if (*++ccp == 't' && *++ccp == 'a' && *++ccp == 'i' && *++ccp == 'l' && IsNextEndOfLineOrSpace(ccp, column + 5, thisLineLength, isDebug))
                                                foundSpan.Length = 6;
                                            else
                                                answer.Fail(9);
                                            return answer;
                                        default:
                                            answer.Fail(10);
                                            return answer;
                                    }
                                case 'c': // class, context
                                    if (column + 4 >= thisLineLength)
                                    {
                                        answer.Fail(13);
                                        return answer;
                                    }
                                    switch (*++ccp)
                                    {
                                        case 'o':
                                            if (column + 6 < thisLineLength && *++ccp == 'n' && *++ccp == 't' && *++ccp == 'e' && *++ccp == 'x' && *++ccp == 't' && IsNextEndOfLineOrSpaceOrLeftBrace(ccp, column + 6, thisLineLength, isDebug))
                                                foundSpan.Length = 7;
                                            else
                                                answer.Fail(12);
                                            break;
                                        case 'l':
                                            if (*++ccp == 'a' && *++ccp == 's' && *++ccp == 's' && IsNextEndOfLineOrSpace(ccp, column + 4, thisLineLength, isDebug))
                                                foundSpan.Length = 5;
                                            else answer.Fail(11);
                                            break;
                                        default:
                                            answer.Fail(13);
                                            break;
                                    }
                                    return answer;
                                case 's': // scenario, skill, skillset, sound, story
                                    if (column + 4 >= thisLineLength)
                                    {
                                        answer.Fail(20);
                                        return answer;
                                    }
                                    switch (*++ccp)
                                    {
                                        case 'k':
                                            if (*++ccp == 'i' && *++ccp == 'l' && *++ccp == 'l')
                                            {
                                                if (IsNextEndOfLineOrSpace(ccp, column + 4, thisLineLength, isDebug))
                                                    length = 5;
                                                else if (*++ccp == 's' && column + 7 < thisLineLength && *++ccp == 'e' && *++ccp == 't' && IsNextEndOfLineOrSpace(ccp, column + 7, thisLineLength, isDebug))
                                                    length = 8;
                                                else
                                                    answer.Fail(15);
                                            }
                                            else answer.Fail(16);
                                            break;
                                        case 'o':
                                            if (*++ccp == 'u' && *++ccp == 'n' && *++ccp == 'd' && IsNextEndOfLineOrSpaceOrLeftBrace(ccp, column + 4, thisLineLength, isDebug))
                                                length = 5;
                                            else answer.Fail(17);
                                            break;
                                        case 't':
                                            if (*++ccp == 'o' && *++ccp == 'r' && *++ccp == 'y' && IsNextEndOfLineOrSpace(ccp, column + 4, thisLineLength, isDebug))
                                                length = 5;
                                            else answer.Fail(18);
                                            break;
                                        case 'c':
                                            if (column + 7 < thisLineLength && *++ccp == 'e' && *++ccp == 'n' && *++ccp == 'a' && *++ccp == 'r' && *++ccp == 'i' && *++ccp == 'o' && IsNextEndOfLineOrSpace(ccp, column + 7, thisLineLength, isDebug))
                                                length = 8;
                                            else answer.Fail(19);
                                            break;
                                        default:
                                            answer.Fail(20);
                                            break;
                                    }
                                    return answer;
                            }
                            break;
                        case 1:
                            switch (file.Lines[raw][column])
                            {
                                case '/':
                                    state = 2;
                                    break;
                                case '+':
                                    if (isDebug)
                                        state = 0;
                                    else
                                        state = 2;
                                    break;
                            }
                            break;
                        case 2: // COMMENT
                            break;
                    }
                }
            }
            answer.isSuccess = 0;
            answer.DataIndex = ErrorSentence.StructKindNotFoundError;
            return answer;
        }
        public static TryInterpretReturnValue TryGetFirstStructLocationUnsafe(this ref ScriptLoadReturnValue script, bool isDebug, Span spanIgnoreLength)
        {
            if (!script.IsCreated) throw new ArgumentException();
            var file = script.Files[spanIgnoreLength.File];
            return file.TryGetFirstStructLocationUnsafe(isDebug, spanIgnoreLength);
        }

        private static void Fail(this ref TryInterpretReturnValue answer, byte errorSubData)
        {
            answer.isSuccess = 0;
            answer.DataIndex = ErrorSentence.StructKindInterpretError;
            answer.SubDataIndex = errorSubData;
        }
    }
}