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
                            case 'a': // attribute
                                if (!AttributeDetect(ref file, ref tempData, ref nextToRightBrace, out answer, list))
                                    return answer;
                                nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                                break;
                            case 'b': // brave
                                if (!BraveDetect(ref file, ref tempData, ref nextToRightBrace, out answer, list))
                                    return answer;
                                nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                                break;
                            case 'c': // consti
                                if (!ConstibuteDetect(ref file, ref tempData, ref nextToRightBrace, out answer, list))
                                    return answer;
                                nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                                break;
                            case 'm': // movetype
                                if (!MoveTypeDetect(ref file, ref tempData, ref nextToRightBrace, out answer, list))
                                    return answer;
                                nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                                break;
                            case 'n': // name
                                if (!NameDetect(ref file, ref tempData, ref nextToRightBrace, out answer, list))
                                    return answer;
                                nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
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

        private static bool NameDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, out TryInterpretReturnValue answer, NativeList<ASTValueTypePair> list)
        {
            char* cs = file.CurrentCharPointer(current);
            answer = new TryInterpretReturnValue(new Span(current, 1), ErrorSentence.InvalidIdentifierError, 0, false);
            int thisLineLength = file.CurrentLineLength(current);
            if (current.Column + 3 >= thisLineLength)
            {
                return false;
            }
            if (*++cs != 'a')
            {
                answer.Span.Length = 2;
                return false;
            }
            if (*++cs != 'm')
            {
                answer.Span.Length = 3;
                return false;
            }
            if (*++cs != 'e')
            {
                answer.Span.Length = 4;
                return false;
            }
            current.Column += 4;
            RaceTree.NameAssignExpression expression = new RaceTree.NameAssignExpression();
            if (current.Column < thisLineLength && *++cs == '@')
            {
                if (file.TryGetScenarioVariantName(current, out var trial))
                {
                    expression.ScenarioVariant = trial.Span;
                    current = trial.Span.CaretNextToEndOfThisSpan;
                }
                else
                {
                    answer = trial;
                    return false;
                }
            }
            file.SkipWhiteSpace(ref current);
            if (file.CurrentChar(current) != '=')
            {
                answer.DataIndex = ErrorSentence.ExpectedCharNotFoundError;
                return false;
            }
            current.Column++;
            file.SkipWhiteSpace(ref current);
            UnityEngine.Debug.Log("AT " + file.CurrentChar(current) + current.ToString());
            answer = new TryInterpretReturnValue(new Span(current, file.CurrentLineLength(current) - current.Column), SuccessSentence.AssignmentInterpretationSuccess, 0, true);
            expression.Value = answer.Span;
            list.Add(new ASTValueTypePair(tempData.NameList.Length, RaceTree.name));
            tempData.NameList.Add(expression);
            return true;
        }

        private static bool MoveTypeDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, out TryInterpretReturnValue answer, NativeList<ASTValueTypePair> list)
        {
            throw new NotImplementedException();
        }

        private static bool ConstibuteDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, out TryInterpretReturnValue answer, NativeList<ASTValueTypePair> list)
        {
            throw new NotImplementedException();
        }

        private static bool BraveDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, out TryInterpretReturnValue answer, NativeList<ASTValueTypePair> list)
        {
            throw new NotImplementedException();
        }

        private static bool AttributeDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, out TryInterpretReturnValue answer, NativeList<ASTValueTypePair> list)
        {
            throw new NotImplementedException();
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