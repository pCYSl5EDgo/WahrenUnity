using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public static unsafe class RaceParser
    {
        public static TryInterpretReturnValue TryParseRaceStruct(this ref TextFile file, ref RaceParserTempData tempData, Span name, Span parentName, Caret nextToLeftBrace, out Caret nextToRightBrace, out RaceTree tree)
        {
            using (var list = new NativeList<ASTValueTypePair>(4, Allocator.Persistent))
            {
                nextToRightBrace = file.SkipWhiteSpace(nextToLeftBrace);
                ref var column = ref nextToRightBrace.Column;
                TryInterpretReturnValue answer;
                answer.Span.File = file.FilePathId;
                tree = new RaceTree
                {
                    Name = name,
                    ParentName = parentName,
                };
                for (ref var raw = ref nextToRightBrace.Line; raw < file.LineCount; raw++, column = 0)
                {
                    char* cptr = file.Lines[raw];
                    for (int lineLength = file.LineLengths[raw]; column < lineLength; column++)
                    {
                        switch (cptr[column])
                        {
                            case '}':
                                nextToRightBrace.Column++;
                                tree.CopyFrom(list);
                                return new TryInterpretReturnValue(nextToRightBrace, SuccessSentence.RaceTreeIntrepretSuccess, 0, true);
                            case 'a': // align
                                answer = AlignDetect(ref file, ref tempData, ref nextToRightBrace, list);
                                if (!answer.IsSuccess)
                                    return answer;
                                nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                                break;
                            case 'b': // brave
                                answer = BraveDetect(ref file, ref tempData, ref nextToRightBrace, list);
                                if (!answer.IsSuccess)
                                    return answer;
                                nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                                break;
                            case 'c': // consti
                                answer = ConstibuteDetect(ref file, ref tempData, ref nextToRightBrace, list);
                                if (!answer.IsSuccess)
                                    return answer;
                                nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                                break;
                            case 'm': // movetype
                                answer = MoveTypeDetect(ref file, ref tempData, ref nextToRightBrace, list);
                                if (!answer.IsSuccess)
                                    return answer;
                                nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                                break;
                            case 'n': // name
                                answer = NameDetect(ref file, ref tempData, ref nextToRightBrace, list);
                                if (!answer.IsSuccess)
                                    return answer;
                                nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                                break;
                            case ' ':
                            case '\t':
                                break;
                            default:
                                tree = default;
                                return new TryInterpretReturnValue(new Span { Start = nextToRightBrace, Length = 1 }, ErrorSentence.NotExpectedCharacterError, 0, false);
                        }
                    }
                }
            }
            return new TryInterpretReturnValue(nextToRightBrace, ErrorSentence.ExpectedCharNotFoundError, 2, false);
        }

        private static TryInterpretReturnValue NameDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, NativeList<ASTValueTypePair> list)
        {
            char* cs = file.CurrentCharPointer(current);
            var answer = new TryInterpretReturnValue(new Span(current, 1), ErrorSentence.InvalidIdentifierError, 0, false);
            int thisLineLength = file.CurrentLineLength(current);
            if (current.Column + 3 >= thisLineLength || *++cs != 'a' || *++cs != 'm' || *++cs != 'e')
                goto RETURN;
            current.Column += 4;
            RaceTree.NameAssignExpression expression = new RaceTree.NameAssignExpression();
            if (current.Column < thisLineLength && *++cs == '@')
            {
                if (!file.TryGetScenarioVariantName(current, out answer))
                    goto RETURN;
                expression.ScenarioVariant = answer.Span;
                current = answer.Span.CaretNextToEndOfThisSpan;
            }
            file.SkipWhiteSpace(ref current);
            if (file.CurrentChar(current) != '=')
            {
                answer.DataIndex = ErrorSentence.ExpectedCharNotFoundError;
                goto RETURN;
            }
            current.Column++;
            file.SkipWhiteSpace(ref current);
            answer = new TryInterpretReturnValue(file.ReadLine(current), SuccessSentence.AssignmentInterpretationSuccess, 0, true);
            expression.Value = answer.Span;
            list.Add(new ASTValueTypePair(tempData.NameList.Length, RaceTree.name));
            tempData.NameList.Add(expression);
        RETURN:
            return answer;
        }

        private static TryInterpretReturnValue MoveTypeDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, NativeList<ASTValueTypePair> list)
        {
            char* cs = file.CurrentCharPointer(current);
            var answer = new TryInterpretReturnValue(new Span(current, 1), ErrorSentence.InvalidIdentifierError, 0, false);
            int thisLineLength = file.CurrentLineLength(current);
            if (current.Column + 7 >= thisLineLength || *++cs != 'o' || *++cs != 'v' || *++cs != 'e' || *++cs != 't' || *++cs != 'y' || *++cs != 'p' || *++cs != 'e')
                goto RETURN;
            current.Column += 8;
            var expression = new RaceTree.MoveTypeAssignExpression();
            if (current.Column < thisLineLength && *++cs == '@')
            {
                if (!file.TryGetScenarioVariantName(current, out answer))
                    goto RETURN;
                expression.ScenarioVariant = answer.Span;
                current = answer.Span.CaretNextToEndOfThisSpan;
            }
            file.SkipWhiteSpace(ref current);
            if (file.CurrentChar(current) != '=')
            {
                answer.DataIndex = ErrorSentence.ExpectedCharNotFoundError;
                goto RETURN;
            }
            current.Column++;
            file.SkipWhiteSpace(ref current);
            answer = file.TryReadIdentifier(current);
            if (!answer.IsSuccess)
                goto RETURN;
            answer.DataIndex = SuccessSentence.AssignmentInterpretationSuccess;
            expression.Value = answer.Span;
            list.Add(new ASTValueTypePair(tempData.MoveTypeList.Length, RaceTree.movetype));
            tempData.MoveTypeList.Add(expression);
        RETURN:
            return answer;
        }

        private static TryInterpretReturnValue ConstibuteDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, NativeList<ASTValueTypePair> list)
        {
            throw new NotImplementedException();
        }

        private static TryInterpretReturnValue BraveDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, NativeList<ASTValueTypePair> list)
        {
            char* cs = file.CurrentCharPointer(current);
            var answer = new TryInterpretReturnValue(new Span(current, 1), ErrorSentence.InvalidIdentifierError, 0, false);
            int thisLineLength = file.CurrentLineLength(current);
            if (current.Column + 4 >= thisLineLength || *++cs != 'r' || *++cs != 'a' || *++cs != 'v' || *++cs != 'e')
                goto RETURN;
            current.Column += 5;
            var expression = new RaceTree.BraveAssignExpression();
            if (current.Column < thisLineLength && *++cs == '@')
            {
                if (!file.TryGetScenarioVariantName(current, out answer))
                    goto RETURN;
                expression.ScenarioVariant = answer.Span;
                current = answer.Span.CaretNextToEndOfThisSpan;
            }
            file.SkipWhiteSpace(ref current);
            if (file.CurrentChar(current) != '=')
            {
                answer.DataIndex = ErrorSentence.ExpectedCharNotFoundError;
                goto RETURN;
            }
            current.Column++;
            file.SkipWhiteSpace(ref current);
            answer = file.TryReadNumber(current, out var value);
            if (!answer.IsSuccess)
                goto RETURN;
            expression.Value = (sbyte)value;
            list.Add(new ASTValueTypePair(tempData.BraveList.Length, RaceTree.brave));
            tempData.BraveList.Add(expression);
        RETURN:
            return answer;
        }

        private static TryInterpretReturnValue AlignDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, NativeList<ASTValueTypePair> list)
        {
            char* cs = file.CurrentCharPointer(current);
            var answer = new TryInterpretReturnValue(new Span(current, 1), ErrorSentence.InvalidIdentifierError, 0, false);
            int thisLineLength = file.CurrentLineLength(current);
            if (current.Column + 4 >= thisLineLength || *++cs != 'l' || *++cs != 'i' || *++cs != 'g' || *++cs != 'n')
                goto RETURN;
            current.Column += 5;
            var expression = new RaceTree.AlignAssignExpression();
            if (current.Column < thisLineLength && *++cs == '@')
            {
                if (!file.TryGetScenarioVariantName(current, out answer))
                    goto RETURN;
                expression.ScenarioVariant = answer.Span;
                current = answer.Span.CaretNextToEndOfThisSpan;
            }
            file.SkipWhiteSpace(ref current);
            if (file.CurrentChar(current) != '=')
            {
                answer.DataIndex = ErrorSentence.ExpectedCharNotFoundError;
                goto RETURN;
            }
            current.Column++;
            file.SkipWhiteSpace(ref current);
            answer = file.TryReadNumber(current, out var value);
            if (!answer.IsSuccess)
                goto RETURN;
            expression.Value = (sbyte)value;
            list.Add(new ASTValueTypePair(tempData.AlignList.Length, RaceTree.align));
            tempData.AlignList.Add(expression);
        RETURN:
            return answer;
        }
    }

    public unsafe readonly struct RaceParserTempData : IDisposable
    {
        public readonly NativeList<RaceTree.NameAssignExpression> NameList;
        public readonly NativeList<RaceTree.AlignAssignExpression> AlignList;
        public readonly NativeList<RaceTree.BraveAssignExpression> BraveList;
        public readonly NativeList<RaceTree.ConstiAssignExpression> ConstiList;
        public readonly NativeList<RaceTree.MoveTypeAssignExpression> MoveTypeList;

        public RaceParserTempData(int capacity)
        {
            this.NameList = new NativeList<RaceTree.NameAssignExpression>(16, Allocator.Persistent);
            this.AlignList = new NativeList<RaceTree.AlignAssignExpression>(16, Allocator.Persistent);
            this.BraveList = new NativeList<RaceTree.BraveAssignExpression>(16, Allocator.Persistent);
            this.ConstiList = new NativeList<RaceTree.ConstiAssignExpression>(16, Allocator.Persistent);
            this.MoveTypeList = new NativeList<RaceTree.MoveTypeAssignExpression>(16, Allocator.Persistent);
        }

        public void Dispose()
        {
            NameList.Dispose();
            AlignList.Dispose();
            BraveList.Dispose();
            MoveTypeList.Dispose();
            for (int i = 0; i < ConstiList.Length; i++)
            {
                ConstiList[i].Dispose();
            }
            ConstiList.Dispose();
        }
    }
}