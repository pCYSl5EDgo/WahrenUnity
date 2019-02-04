using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public static unsafe class RaceParser
    {
        public static TryInterpretReturnValue TryParseRaceStructMultiThread(this ref TextFile file, ref RaceParserTempData tempData, ref IdentifierNumberPairList pairList, Span name, Span parentName, Caret nextToLeftBrace, out Caret nextToRightBrace, out RaceTree tree)
        {
            nextToRightBrace = file.SkipWhiteSpace(nextToLeftBrace);
            ref var column = ref nextToRightBrace.Column;
            TryInterpretReturnValue answer;
            answer.Span.File = file.FilePathId;
            tree = new RaceTree
            {
                Name = name,
                ParentName = parentName,
                List = new ASTValueTypePairList(8),
            };
            ref var list = ref tree.List;
            for (ref var raw = ref nextToRightBrace.Line; raw < file.LineCount; raw++, column = 0)
            {
                for (int lineLength = file.LineLengths[raw]; column < lineLength; column++)
                {
                    switch (file.Lines[raw][column])
                    {
                        case '}':
                            nextToRightBrace.Column++;
                            return new TryInterpretReturnValue(nextToRightBrace, SuccessSentence.RaceTreeIntrepretSuccess, InterpreterStatus.Success);
                        case 'a': // align
                            UnityEngine.Debug.Log("a" + nextToRightBrace.ToString() + " " + file.CurrentChar(nextToRightBrace));
                            answer = AlignDetect(ref file, ref tempData, ref nextToRightBrace, ref list);
                            if (!answer.IsSuccess)
                                goto RETURN;
                            nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                            UnityEngine.Debug.Log("a" + nextToRightBrace.ToString() + " " + file.CurrentChar(nextToRightBrace));
                            break;
                        case 'b': // brave
                            UnityEngine.Debug.Log("b" + nextToRightBrace.ToString() + " " + file.CurrentChar(nextToRightBrace));
                            answer = BraveDetect(ref file, ref tempData, ref nextToRightBrace, ref list);
                            if (!answer.IsSuccess)
                                goto RETURN;
                            nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                            UnityEngine.Debug.Log("b" + nextToRightBrace.ToString() + " " + file.CurrentChar(nextToRightBrace));
                            break;
                        case 'c': // consti
                            UnityEngine.Debug.Log("c" + nextToRightBrace.ToString() + " " + file.CurrentChar(nextToRightBrace));
                            answer = ConstiDetect(ref file, ref tempData, ref nextToRightBrace, ref list, ref pairList);
                            if (!answer.IsSuccess)
                                goto RETURN;
                            UnityEngine.Debug.Log("ck" + answer.Span.ToString());
                            nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                            UnityEngine.Debug.Log("c" + nextToRightBrace.ToString() + " " + file.CurrentChar(nextToRightBrace));
                            break;
                        case 'm': // movetype
                            UnityEngine.Debug.Log("m" + nextToRightBrace.ToString() + " " + file.CurrentChar(nextToRightBrace));
                            answer = MoveTypeDetect(ref file, ref tempData, ref nextToRightBrace, ref list);
                            if (!answer.IsSuccess)
                                goto RETURN;
                            nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                            UnityEngine.Debug.Log("m" + nextToRightBrace.ToString() + " " + file.CurrentChar(nextToRightBrace));
                            break;
                        case 'n': // name
                            UnityEngine.Debug.Log("n" + nextToRightBrace.ToString() + " " + file.CurrentChar(nextToRightBrace));
                            answer = NameDetect(ref file, ref tempData, ref nextToRightBrace, ref list);
                            if (!answer.IsSuccess)
                                goto RETURN;
                            nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                            UnityEngine.Debug.Log("n" + nextToRightBrace.ToString() + " " + file.CurrentChar(nextToRightBrace));
                            break;
                        case ' ':
                        case '\t':
                            break;
                        default:
                            UnityEngine.Debug.Log("z" + nextToRightBrace.ToString() + " " + file.CurrentChar(nextToRightBrace));
                            answer = new TryInterpretReturnValue(new Span(nextToRightBrace, 1), ErrorSentence.NotExpectedCharacterError, InterpreterStatus.Error);
                            goto RETURN;
                    }
                }
            }
            answer = new TryInterpretReturnValue(nextToRightBrace, ErrorSentence.ExpectedCharNotFoundError, 2, InterpreterStatus.Error);
        RETURN:
            tree.Dispose();
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
                goto RETURN;
            }
            current.Column++;
            file.SkipWhiteSpace(ref current);
            answer = new TryInterpretReturnValue(file.ReadLine(current), SuccessSentence.AssignmentInterpretationSuccess, InterpreterStatus.Success);
            expression.Value = answer.Span;
            var ast = new ASTValueTypePair(RaceTree.name);
            if (ast.TryAddAST(tempData.Names, expression, tempData.NameCapacity, ref tempData.NameLength))
            {
                if (list.TryAddMultiThread(ast))
                {
                    goto RETURN;
                }
                else
                {
                }
            }
            else
            {
            }
            answer = new TryInterpretReturnValue(file.ReadLine(current), SuccessSentence.AssignmentInterpretationSuccess, InterpreterStatus.Error);
        RETURN:
            return answer;
        }

        private static TryInterpretReturnValue MoveTypeDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, ref ASTValueTypePairList list)
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
                list.TryAddMultiThread(ast);
            RETURN:
            return answer;
        }

        private static TryInterpretReturnValue ConstiDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, ref ASTValueTypePairList list, ref IdentifierNumberPairList pairList)
        {
            UnityEngine.Debug.Log("0:" + current.ToString() + " " + file.CurrentChar(current));
            char* cs = file.CurrentCharPointer(current);
            var answer = new TryInterpretReturnValue(new Span(current, 1), ErrorSentence.InvalidIdentifierError, InterpreterStatus.Error);
            int thisLineLength = file.CurrentLineLength(current);
            if (current.Column + 5 >= thisLineLength || *++cs != 'o' || *++cs != 'n' || *++cs != 's' || *++cs != 't' || *++cs != 'i')
                goto RETURN;
            current.Column += 6;
            UnityEngine.Debug.Log("0:" + current.ToString() + " " + file.CurrentChar(current));
            var expression = new RaceTree.ConstiAssignExpression();
            if (current.Column < thisLineLength && *++cs == '@')
            {
                if (!file.TryGetScenarioVariantName(current, out answer))
                    goto RETURN;
                expression.ScenarioVariant = answer.Span;
                current = answer.Span.CaretNextToEndOfThisSpan;
            }
            file.SkipWhiteSpace(ref current);
            UnityEngine.Debug.Log("0:" + current.ToString() + " " + file.CurrentChar(current));
            if (file.CurrentChar(current) != '=')
            {
                answer.DataIndex = ErrorSentence.ExpectedCharNotFoundError;
                goto RETURN;
            }
            current.Column++;
            file.SkipWhiteSpace(ref current);
            UnityEngine.Debug.Log("0:" + current.ToString() + " " + file.CurrentChar(current));
            answer = file.TryReadIdentifierNumberPairs(ref pairList, current, out expression.Start, out expression.Length);
            UnityEngine.Debug.Log("ckckck" + answer.Span.ToString());
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
                if (!list.TryAddMultiThread(ast))
                {
                    answer.Status = InterpreterStatus.Pending;
                }
            }
            else
            {
                answer.Status = InterpreterStatus.Pending;
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
        VerifyConsti(RaceTree.ConstiAssignExpression expression, in IdentifierNumberPairList list
#if UNITY_EDITOR
        , Span span
#endif
        )
        {
            for (int i = expression.Start, end = expression.Start + expression.Length; i < end; i++)
            {
                ref IdentifierNumberPair val = ref list.Values[i];
                if (val.Span.Length == 0 || val.Number < 0 || val.Number > 10)
#if UNITY_EDITOR
                    return new TryInterpretReturnValue(val.NumberSpan, ErrorSentence.OutOfRangeError, InterpreterStatus.Error);
#else
                    return false;
#endif
            }
#if UNITY_EDITOR
            return new TryInterpretReturnValue(span, default, default, InterpreterStatus.Success);
#else
            return true;
#endif
        }

        private static TryInterpretReturnValue BraveDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, ref ASTValueTypePairList list)
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
                list.TryAddMultiThread(ast);
            RETURN:
            return answer;
        }

        private static TryInterpretReturnValue AlignDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, ref ASTValueTypePairList list)
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
                list.TryAddMultiThread(ast);
            RETURN:
            return answer;
        }
    }

    public unsafe struct RaceParserTempData : IDisposable
    {
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
            this = default;
            NameCapacity = AlignCapacity = BraveCapacity = ConstiCapacity = MoveTypeCapacity = capacity;
            if (capacity != 0)
            {
                this.Names = (RaceTree.NameAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.NameAssignExpression) * capacity, 4, Allocator.Persistent);
                this.Aligns = (RaceTree.AlignAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.AlignAssignExpression) * capacity, 4, Allocator.Persistent);
                this.Braves = (RaceTree.BraveAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.BraveAssignExpression) * capacity, 4, Allocator.Persistent);
                this.Constis = (RaceTree.ConstiAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.ConstiAssignExpression) * capacity, 4, Allocator.Persistent);
                this.MoveTypes = (RaceTree.MoveTypeAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.MoveTypeAssignExpression) * capacity, 4, Allocator.Persistent);
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
            this = default;
        }
    }
}