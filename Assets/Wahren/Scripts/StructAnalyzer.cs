using System;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe static class StructAnalyzer
    {
        internal static bool IsNextEndOfLineOrSpaceOrLeftBrace(ushort* lastChar, int column, int length)
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
        internal static bool IsNextEndOfLineOrSpace(ushort* lastChar, int column, int length)
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

        public static bool IsStructKindWithName(Location location)
        {
            switch (location)
            {
                // No Name
                case Location.Attribute:
                case Location.Detail:
                case Location.Context:
                case Location.Sound:
                case Location.WorkSpace:
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

        public static TryInterpretReturnValue TryGetFirstStructLocation(ushort* ccp, int lineLength, Caret start, int column)
        {
            switch (*ccp)
            {
                case 'p': // power
                    return start.PowerDetect(column, lineLength, ccp);
                case 'u': // unit
                    return start.UnitDetect(column, lineLength, ccp);
                case 'r': // race
                    return start.RaceDetect(column, lineLength, ccp);
                case 'a': // attribute
                    return start.AttributeDetect(column, lineLength, ccp);
                case 'f': // field
                    return start.FieldDetect(column, lineLength, ccp);
                case 'o': // object
                    return start.ObjectDetect(column, lineLength, ccp);
                case 'm': // movetype
                    return start.MoveTypeDetect(column, lineLength, ccp);
                case 'e': // event
                    return start.EventDetect(column, lineLength, ccp);
                case 'd': // dungeon, detail
                    return start.DungeonOrDetailDetect(column, lineLength, ccp);
                case 'c': // class, context
                    return start.ClassOrContextDetect(column, lineLength, ccp);
                case 's': // scenario, skill, skillset, sound, story
                    return start.SDetect(column, lineLength, ccp);
                case 'w': // workspace
                    return start.WorkspaceDetect(column, lineLength, ccp);
                case 'v': // voice
                    return start.VoiceDetect(column, lineLength, ccp);
                default:
                    return new TryInterpretReturnValue(start, ErrorSentence.StructKindNotFoundError, InterpreterStatus.Error);
            }
        }

        private static TryInterpretReturnValue WorkspaceDetect(this Caret start, int column, int thisLineLength, ushort* ccp)
        {
            const int _length = 9;
            const int _len1 = _length - 1;
            if (column + _len1 < thisLineLength && *++ccp == 'o' && *++ccp == 'r' && *++ccp == 'k' && *++ccp == 's' && *++ccp == 'p' && *++ccp == 'a' && *++ccp == 'c' && *++ccp == 'e' && IsNextEndOfLineOrSpace(ccp, column + _len1, thisLineLength))
            {
                return TryInterpretReturnValue.CreateSuccessDetectStructType(start, _length, Location.WorkSpace);
            }
            else
            {
                return start.Fail(23);
            }
        }

        private static TryInterpretReturnValue VoiceDetect(this Caret start, int column, int thisLineLength, ushort* ccp)
        {
            const int _length = 5;
            const int _len1 = _length - 1;
            if (column + _len1 < thisLineLength && *++ccp == 'o' && *++ccp == 'i' && *++ccp == 'c' && *++ccp == 'e' && IsNextEndOfLineOrSpace(ccp, column + _len1, thisLineLength))
            {
                return TryInterpretReturnValue.CreateSuccessDetectStructType(start, _length, Location.Voice);
            }
            else
            {
                return start.Fail(22);
            }
        }

        private static TryInterpretReturnValue SDetect(this Caret start, int column, int thisLineLength, ushort* ccp)
        {
            if (column + 3 >= thisLineLength)
                return start.Fail(20);
            else switch (*++ccp)
                {
                    case 'k':
                        if (column + 4 < thisLineLength && *++ccp == 'i' && *++ccp == 'l' && *++ccp == 'l')
                        {
                            if (IsNextEndOfLineOrSpace(ccp, column + 4, thisLineLength))
                            {
                                return TryInterpretReturnValue.CreateSuccessDetectStructType(start, 5, Location.Skill);
                            }
                            else if (*++ccp == 's' && column + 7 < thisLineLength && *++ccp == 'e' && *++ccp == 't' && IsNextEndOfLineOrSpace(ccp, column + 7, thisLineLength))
                            {
                                return TryInterpretReturnValue.CreateSuccessDetectStructType(start, 8, Location.SkillSet);
                            }
                            else
                            {
                                return start.Fail(15);
                            }
                        }
                        else
                        {
                            return start.Fail(16);
                        }
                    case 'o':
                        if (column + 4 < thisLineLength && *++ccp == 'u' && *++ccp == 'n' && *++ccp == 'd' && IsNextEndOfLineOrSpaceOrLeftBrace(ccp, column + 4, thisLineLength))
                        {
                            return TryInterpretReturnValue.CreateSuccessDetectStructType(start, 5, Location.Sound);
                        }
                        else
                        {
                            return start.Fail(17);
                        }
                    case 't':
                        if (column + 4 < thisLineLength && *++ccp == 'o' && *++ccp == 'r' && *++ccp == 'y' && IsNextEndOfLineOrSpace(ccp, column + 4, thisLineLength))
                        {
                            return TryInterpretReturnValue.CreateSuccessDetectStructType(start, 5, Location.Story);
                        }
                        else
                        {
                            return start.Fail(18);
                        }
                    case 'c':
                        if (column + 7 < thisLineLength && *++ccp == 'e' && *++ccp == 'n' && *++ccp == 'a' && *++ccp == 'r' && *++ccp == 'i' && *++ccp == 'o' && IsNextEndOfLineOrSpace(ccp, column + 7, thisLineLength))
                        {
                            return TryInterpretReturnValue.CreateSuccessDetectStructType(start, 8, Location.Scenario);
                        }
                        else
                        {
                            return start.Fail(19);
                        }
                    case 'p':
                        if (*++ccp == 'o' && *++ccp == 't' && IsNextEndOfLineOrSpace(ccp, column + 3, thisLineLength))
                        {
                            return TryInterpretReturnValue.CreateSuccessDetectStructType(start, 4, Location.Spot);
                        }
                        else
                        {
                            return start.Fail(21);
                        }
                    default:
                        return start.Fail(20);
                }
        }

        private static TryInterpretReturnValue ClassOrContextDetect(this Caret start, int column, int thisLineLength, ushort* ccp)
        {
            if (column + 4 >= thisLineLength)
                return start.Fail(13);
            else switch (*++ccp)
                {
                    case 'o':
                        if (column + 6 < thisLineLength && *++ccp == 'n' && *++ccp == 't' && *++ccp == 'e' && *++ccp == 'x' && *++ccp == 't' && IsNextEndOfLineOrSpaceOrLeftBrace(ccp, column + 6, thisLineLength))
                        {
                            return TryInterpretReturnValue.CreateSuccessDetectStructType(start, 7, Location.Context);
                        }
                        else
                        {
                            return start.Fail(12);
                        }
                    case 'l':
                        if (*++ccp == 'a' && *++ccp == 's' && *++ccp == 's' && IsNextEndOfLineOrSpace(ccp, column + 4, thisLineLength))
                        {
                            return TryInterpretReturnValue.CreateSuccessDetectStructType(start, 5, Location.Class);
                        }
                        else
                        {
                            return start.Fail(11);
                        }
                    default:
                        return start.Fail(13);
                }
        }

        private static TryInterpretReturnValue DungeonOrDetailDetect(this Caret start, int column, int thisLineLength, ushort* ccp)
        {
            if (column + 5 >= thisLineLength)
                return start.Fail(10);
            else switch (*++ccp)
                {
                    case 'u':
                        if (column + 6 < thisLineLength && *++ccp == 'n' && *++ccp == 'g' && *++ccp == 'e' && *++ccp == 'o' && *++ccp == 'n' && IsNextEndOfLineOrSpace(ccp, column + 6, thisLineLength))
                        {
                            return TryInterpretReturnValue.CreateSuccessDetectStructType(start, 7, Location.Dungeon);
                        }
                        else
                        {
                            return start.Fail(8);
                        }
                    case 'e':
                        if (*++ccp == 't' && *++ccp == 'a' && *++ccp == 'i' && *++ccp == 'l' && IsNextEndOfLineOrSpace(ccp, column + 5, thisLineLength))
                        {
                            return TryInterpretReturnValue.CreateSuccessDetectStructType(start, 6, Location.Detail);
                        }
                        else
                        {
                            return start.Fail(9);
                        }
                    default:
                        return start.Fail(10);
                }
        }

        private static TryInterpretReturnValue EventDetect(this Caret start, int column, int thisLineLength, ushort* ccp)
        {
            const int _length = 5;
            const int _len1 = _length - 1;
            if (column + _len1 < thisLineLength && *++ccp == 'v' && *++ccp == 'e' && *++ccp == 'n' && *++ccp == 't' && IsNextEndOfLineOrSpace(ccp, column + _len1, thisLineLength))
            {
                return TryInterpretReturnValue.CreateSuccessDetectStructType(start, _length, Location.Event);
            }
            else
            {
                return start.Fail(7);
            }
        }

        private static TryInterpretReturnValue MoveTypeDetect(this Caret start, int column, int thisLineLength, ushort* ccp)
        {
            const int _length = 8;
            const int _len1 = _length - 1;
            if (column + _len1 < thisLineLength && *++ccp == 'o' && *++ccp == 'v' && *++ccp == 'e' && *++ccp == 't' && *++ccp == 'y' && *++ccp == 'p' && *++ccp == 'e' && IsNextEndOfLineOrSpace(ccp, column + _len1, thisLineLength))
            {
                return TryInterpretReturnValue.CreateSuccessDetectStructType(start, _length, Location.Movetype);
            }
            else
            {
                return start.Fail(6);
            }
        }

        private static TryInterpretReturnValue ObjectDetect(this Caret start, int column, int thisLineLength, ushort* ccp)
        {
            const int _length = 6;
            const int _len1 = _length - 1;
            if (column + _len1 < thisLineLength && *++ccp == 'b' && *++ccp == 'j' && *++ccp == 'e' && *++ccp == 'c' && *++ccp == 't' && IsNextEndOfLineOrSpace(ccp, column + _len1, thisLineLength))
            {
                return TryInterpretReturnValue.CreateSuccessDetectStructType(start, _length, Location.Object);
            }
            else
            {
                return start.Fail(5);
            }
        }

        private static TryInterpretReturnValue FieldDetect(this Caret start, int column, int thisLineLength, ushort* ccp)
        {
            const int _length = 5;
            const int _len1 = _length - 1;
            if (column + _len1 < thisLineLength && *++ccp == 'i' && *++ccp == 'e' && *++ccp == 'l' && *++ccp == 'd' && IsNextEndOfLineOrSpace(ccp, column + _len1, thisLineLength))
            {
                return TryInterpretReturnValue.CreateSuccessDetectStructType(start, _length, Location.Field);
            }
            else
            {
                return start.Fail(4);
            }
        }

        private static TryInterpretReturnValue AttributeDetect(this Caret start, int column, int thisLineLength, ushort* ccp)
        {
            const int _length = 9;
            if (column + _length - 1 < thisLineLength && *++ccp == 't' && *++ccp == 't' && *++ccp == 'r' && *++ccp == 'i' && *++ccp == 'b' && *++ccp == 'u' && *++ccp == 't' && *++ccp == 'e' && IsNextEndOfLineOrSpace(ccp, column + _length - 1, thisLineLength))
            {
                return TryInterpretReturnValue.CreateSuccessDetectStructType(start, _length, Location.Attribute);
            }
            else
            {
                return start.Fail(3);
            }
        }

        private static TryInterpretReturnValue RaceDetect(this Caret start, int column, int thisLineLength, ushort* ccp)
        {
            const int _length = 4;
            if (column + _length - 1 < thisLineLength && *++ccp == 'a' && *++ccp == 'c' && *++ccp == 'e' && IsNextEndOfLineOrSpace(ccp, column + _length - 1, thisLineLength))
            {
                return TryInterpretReturnValue.CreateSuccessDetectStructType(start, _length, Location.Race);
            }
            else
            {
                return start.Fail(2);
            }
        }

        private static TryInterpretReturnValue UnitDetect(this Caret start, int column, int thisLineLength, ushort* ccp)
        {
            const int _length = 4;
            if (column + _length - 1 < thisLineLength && *++ccp == 'n' && *++ccp == 'i' && *++ccp == 't' && IsNextEndOfLineOrSpace(ccp, column + _length - 1, thisLineLength))
            {
                return TryInterpretReturnValue.CreateSuccessDetectStructType(start, _length, Location.Unit);
            }
            else
            {
                return start.Fail(1);
            }
        }

        private static TryInterpretReturnValue PowerDetect(this Caret start, int column, int thisLineLength, ushort* ccp)
        {
            const int _length = 5;
            if (column + _length - 1 < thisLineLength && *++ccp == 'o' && *++ccp == 'w' && *++ccp == 'e' && *++ccp == 'r' && IsNextEndOfLineOrSpace(ccp, column + _length - 1, thisLineLength))
            {
                return TryInterpretReturnValue.CreateSuccessDetectStructType(start, _length, Location.Power);
            }
            else
            {
                return start.Fail(0);
            }
        }

        private static TryInterpretReturnValue Fail(this Caret start, byte errorSubData) => new TryInterpretReturnValue(start, ErrorSentence.StructKindInterpretError, errorSubData, InterpreterStatus.Error);

        private static TryInterpretReturnValue Success(this Caret start, byte successSubData, int length) => new TryInterpretReturnValue(new Span(start, length), SuccessSentence.StructKindInterpretSuccess, successSubData, InterpreterStatus.Success);
    }
}