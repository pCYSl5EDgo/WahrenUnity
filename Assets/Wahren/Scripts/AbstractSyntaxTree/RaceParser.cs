using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public static unsafe class RaceParser
    {
        public static TryInterpretReturnValue TryParseRaceStructMultiThread(this ref TextFile file, ref RaceParserTempData tempData, ref IdentifierNumberPairList identifierNumberPairList, ref ASTValueTypePairList astValueTypePairList, Span name, Span parentName, Caret nextToLeftBrace, out Caret nextToRightBrace, out int raceTreeIndex)
        {
            nextToRightBrace = file.SkipWhiteSpace(nextToLeftBrace);
            ref var column = ref nextToRightBrace.Column;
            TryInterpretReturnValue answer;
            answer.Span.File = file.FilePathId;
            var tree = new RaceTree
            {
                Name = name,
                ParentName = parentName,
            };
            var list = ASTValueTypePairList.MallocTemp(4);
            for (ref var raw = ref nextToRightBrace.Line; raw < file.LineCount; raw++, column = 0)
            {
                for (int lineLength = file.LineLengths[raw]; column < lineLength; column++)
                {
                    switch (file.Lines[raw][column])
                    {
                        case '}':
                            nextToRightBrace.Column++;
                            (tree.Start, tree.Length) = astValueTypePairList.TryAddBulkMultiThread(list);
                            if (tree.Start == -1)
                            {
                                raceTreeIndex = -1;
                                return TryInterpretReturnValue.CreatePending(new Span(nextToLeftBrace, 0), Location.Race, PendingReason.ASTValueTypePairListCapacityShortage);
                            }
                            if (tree.TryAddToMultiThread(ref tempData.Values, tempData.Capacity, ref tempData.Length, out raceTreeIndex))
                            {
                                return new TryInterpretReturnValue(nextToRightBrace, SuccessSentence.RaceTreeIntrepretSuccess, InterpreterStatus.Success);
                            }
                            else
                            {
                                return TryInterpretReturnValue.CreatePending(new Span(nextToLeftBrace, 0), Location.Race, PendingReason.TreeListCapacityShortage);
                            }
                        case 'a': // align
                            answer = AlignDetect(ref file, ref tempData, ref nextToRightBrace, ref list);
                            if (!answer.IsSuccess)
                                goto RETURN;
                            nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                            break;
                        case 'b': // brave
                            answer = BraveDetect(ref file, ref tempData, ref nextToRightBrace, ref list);
                            if (!answer.IsSuccess)
                                goto RETURN;
                            nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                            break;
                        case 'c': // consti
                            answer = ConstiDetect(ref file, ref tempData, ref nextToRightBrace, ref list, ref identifierNumberPairList);
                            if (!answer.IsSuccess)
                                goto RETURN;
                            nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                            break;
                        case 'm': // movetype
                            answer = MoveTypeDetect(ref file, ref tempData, ref nextToRightBrace, ref list);
                            if (!answer.IsSuccess)
                                goto RETURN;
                            nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                            break;
                        case 'n': // name
                            answer = NameDetect(ref file, ref tempData, ref nextToRightBrace, ref list);
                            if (!answer.IsSuccess)
                                goto RETURN;
                            nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                            break;
                        case ' ':
                        case '\t':
                            break;
                        default:
                            answer = new TryInterpretReturnValue(new Span(nextToRightBrace, 1), ErrorSentence.NotExpectedCharacterError, InterpreterStatus.Error);
                            goto RETURN;
                    }
                }
            }
            answer = new TryInterpretReturnValue(nextToRightBrace, ErrorSentence.ExpectedCharNotFoundError, 2, InterpreterStatus.Error);
        RETURN:
            raceTreeIndex = -1;
            return answer;
        }

        private static TryInterpretReturnValue NameDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, ref ASTValueTypePairList list)
        {
            char* cs = file.CurrentCharPointer(current);
            var answer = new TryInterpretReturnValue(new Span(current, 1), ErrorSentence.InvalidIdentifierError, 0, InterpreterStatus.Error);
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
                answer.Status = InterpreterStatus.Error;
                goto RETURN;
            }
            current.Column++;
            file.SkipWhiteSpace(ref current);
            answer = new TryInterpretReturnValue(file.ReadLine(current), SuccessSentence.AssignmentInterpretationSuccess, InterpreterStatus.Success);
            expression.Value = answer.Span;
            var ast = new ASTValueTypePair(RaceTree.name);
            if (ast.TryAddAST(tempData.Names, expression, tempData.NameCapacity, ref tempData.NameLength))
            {
                ast.AddToTempJob(ref list.Values, ref list.Capacity, ref list.Length, out _);
            }
            else
            {
                answer = TryInterpretReturnValue.CreatePending(answer.Span, Location.Race, PendingReason.SectionListCapacityShortage);
            }
        RETURN:
            return answer;
        }

        private static TryInterpretReturnValue MoveTypeDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, ref ASTValueTypePairList listTemp)
        {
            char* cs = file.CurrentCharPointer(current);
            var answer = new TryInterpretReturnValue(new Span(current, 1), ErrorSentence.InvalidIdentifierError, InterpreterStatus.Error);
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
            answer = file.TryReadIdentifierNotEmpty(current);
            if (!answer.IsSuccess)
                goto RETURN;
            answer.DataIndex = SuccessSentence.AssignmentInterpretationSuccess;
            expression.Value = answer.Span;
            var ast = new ASTValueTypePair(RaceTree.movetype);
            if (ast.TryAddAST(tempData.MoveTypes, expression, tempData.MoveTypeCapacity, ref tempData.MoveTypeLength))
            {
                ast.AddToTempJob(ref listTemp.Values, ref listTemp.Capacity, ref listTemp.Length, out _);
            }
            else
            {
                answer = TryInterpretReturnValue.CreatePending(answer.Span, Location.Race, PendingReason.SectionListCapacityShortage);
            }
        RETURN:
            return answer;
        }

        private static TryInterpretReturnValue ConstiDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, ref ASTValueTypePairList listTemp, ref IdentifierNumberPairList pairList)
        {
            char* cs = file.CurrentCharPointer(current);
            var answer = new TryInterpretReturnValue(new Span(current, 1), ErrorSentence.InvalidIdentifierError, InterpreterStatus.Error);
            int thisLineLength = file.CurrentLineLength(current);
            if (current.Column + 5 >= thisLineLength || *++cs != 'o' || *++cs != 'n' || *++cs != 's' || *++cs != 't' || *++cs != 'i')
                goto RETURN;
            current.Column += 6;
            var expression = new RaceTree.ConstiAssignExpression();
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
            answer = file.TryReadIdentifierNumberPairs(ref pairList, current, out expression.Start, out expression.Length);
            if (!answer.IsSuccess)
                goto RETURN;
#if UNITY_EDITOR
            answer = VerifyConsti(expression, pairList, answer.Span);
            if (answer.IsError)
                goto RETURN;
#else
            if (!VerifyConsti(expression, pairList))
                return new TryInterpretReturnValue(current, ErrorSentence.OutOfRangeError, InterpreterStatus.Error);
#endif
            var ast = new ASTValueTypePair(RaceTree.consti);
            if (ast.TryAddAST(tempData.Constis, expression, tempData.ConstiCapacity, ref tempData.ConstiLength))
            {
                ast.AddToTempJob(ref listTemp.Values, ref listTemp.Capacity, ref listTemp.Length, out _);
            }
            else
            {
                answer = TryInterpretReturnValue.CreatePending(answer.Span, Location.Race, PendingReason.SectionListCapacityShortage);
            }
        RETURN:
            return answer;
        }

        internal static
#if UNITY_EDITOR
        TryInterpretReturnValue
#else
        bool
#endif
        VerifyConsti(RaceTree.ConstiAssignExpression expression, in IdentifierNumberPairList list, Span span)
        {
            for (int i = expression.Start, end = expression.Start + expression.Length; i < end; i++)
            {
                ref IdentifierNumberPair val = ref list.Values[i];
                if (val.Span.Length == 0 || val.Number < 0 || val.Number > 10)
                    return new TryInterpretReturnValue(val.NumberSpan, ErrorSentence.OutOfRangeError, InterpreterStatus.Error);
            }
            return new TryInterpretReturnValue(span, default, default, InterpreterStatus.Success);
        }

        private static TryInterpretReturnValue BraveDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, ref ASTValueTypePairList listTemp)
        {
            char* cs = file.CurrentCharPointer(current);
            var answer = new TryInterpretReturnValue(new Span(current, 1), ErrorSentence.InvalidIdentifierError, InterpreterStatus.Error);
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
            var ast = new ASTValueTypePair(RaceTree.brave);
            if (ast.TryAddAST(tempData.Braves, expression, tempData.BraveCapacity, ref tempData.BraveLength))
            {
                ast.AddToTempJob(ref listTemp.Values, ref listTemp.Capacity, ref listTemp.Length, out _);
            }
            else
            {

            }
        RETURN:
            return answer;
        }

        private static TryInterpretReturnValue AlignDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, ref ASTValueTypePairList listTemp)
        {
            char* cs = file.CurrentCharPointer(current);
            var answer = new TryInterpretReturnValue(new Span(current, 1), ErrorSentence.InvalidIdentifierError, InterpreterStatus.Error);
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
            var ast = new ASTValueTypePair(RaceTree.align);
            if (ast.TryAddAST(tempData.Aligns, expression, tempData.AlignCapacity, ref tempData.AlignLength))
            {
                ast.AddToTempJob(ref listTemp.Values, ref listTemp.Capacity, ref listTemp.Length, out _);
            }
            else
            {

            }
        RETURN:
            return answer;
        }
    }

    public unsafe struct RaceParserTempData : IDisposable
    {
        public RaceTree* Values;
        public int Length;
        public int Capacity;
        public RaceTree.NameAssignExpression* Names;
        public int NameLength;
        public int NameCapacity;
        public readonly RaceTree.AlignAssignExpression* Aligns;
        public int AlignLength;
        public int AlignCapacity;
        public RaceTree.BraveAssignExpression* Braves;
        public int BraveLength;
        public int BraveCapacity;
        public RaceTree.ConstiAssignExpression* Constis;
        public int ConstiLength;
        public int ConstiCapacity;
        public RaceTree.MoveTypeAssignExpression* MoveTypes;
        public int MoveTypeLength;
        public int MoveTypeCapacity;

        public RaceParserTempData(int capacity)
        {
            Length = NameLength = AlignLength = BraveLength = ConstiLength = MoveTypeLength = 0;
            Capacity = NameCapacity = AlignCapacity = BraveCapacity = ConstiCapacity = MoveTypeCapacity = capacity;
            if (capacity != 0)
            {
                Values = (RaceTree*)UnsafeUtility.Malloc(sizeof(RaceTree) * capacity, 4, Allocator.Persistent);
                Names = (RaceTree.NameAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.NameAssignExpression) * capacity, 4, Allocator.Persistent);
                Aligns = (RaceTree.AlignAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.AlignAssignExpression) * capacity, 4, Allocator.Persistent);
                Braves = (RaceTree.BraveAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.BraveAssignExpression) * capacity, 4, Allocator.Persistent);
                Constis = (RaceTree.ConstiAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.ConstiAssignExpression) * capacity, 4, Allocator.Persistent);
                MoveTypes = (RaceTree.MoveTypeAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.MoveTypeAssignExpression) * capacity, 4, Allocator.Persistent);
            }
            else
            {
                Values = null;
                Names = null;
                Aligns = null;
                Braves = null;
                Constis = null;
                MoveTypes = null;
            }
        }

        public void Dispose()
        {
            if (NameCapacity != 0)
                UnsafeUtility.Free(Names, Allocator.Persistent);
            if (AlignCapacity != 0)
                UnsafeUtility.Free(Aligns, Allocator.Persistent);
            if (BraveCapacity != 0)
                UnsafeUtility.Free(Braves, Allocator.Persistent);
            if (MoveTypeCapacity != 0)
                UnsafeUtility.Free(MoveTypes, Allocator.Persistent);
            if (ConstiCapacity != 0)
                UnsafeUtility.Free(Constis, Allocator.Persistent);
            if (Capacity != 0)
                UnsafeUtility.Free(Values, Allocator.Persistent);
            this = default;
        }
    }
}