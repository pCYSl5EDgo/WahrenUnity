using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public static unsafe class RaceParser
    {
        public static TryInterpretReturnValue TryParseRaceStructMultiThread(this ref TextFile file, ref RaceParserTempData tempData, Span name, Span parentName, Caret nextToLeftBrace, out Caret nextToRightBrace, out RaceTree tree)
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
                            return new TryInterpretReturnValue(nextToRightBrace, SuccessSentence.RaceTreeIntrepretSuccess, 0, true);
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
                            answer = ConstiDetect(ref file, ref tempData, ref nextToRightBrace, ref list);
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
                            answer = new TryInterpretReturnValue(new Span { Start = nextToRightBrace, Length = 1 }, ErrorSentence.NotExpectedCharacterError, 0, false);
                            goto RETURN;
                    }
                }
            }
            answer = new TryInterpretReturnValue(nextToRightBrace, ErrorSentence.ExpectedCharNotFoundError, 2, false);
        RETURN:
            tree.Dispose();
            return answer;
        }

        private static TryInterpretReturnValue NameDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, ref ASTValueTypePairList list)
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
            var ast = new ASTValueTypePair(RaceTree.name);
            if (ast.TryAddAST(tempData.Names, expression, tempData.NameCapacity, ref tempData.NameLength))
            {
                if (list.TryAddMultiThread(ast))
                    goto RETURN;
                else
                    UnityEngine.Debug.Log("HOGE");
            }
            else UnityEngine.Debug.Log("FUGA");
            answer = new TryInterpretReturnValue(file.ReadLine(current), SuccessSentence.AssignmentInterpretationSuccess, 0, false);
        RETURN:
            return answer;
        }

        private static TryInterpretReturnValue MoveTypeDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, ref ASTValueTypePairList list)
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

        private static TryInterpretReturnValue ConstiDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, ref ASTValueTypePairList list)
        {
            char* cs = file.CurrentCharPointer(current);
            var answer = new TryInterpretReturnValue(new Span(current, 1), ErrorSentence.InvalidIdentifierError, 0, false);
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
            answer = file.TryReadIdentifierValuePairs(current, out expression.Length, out expression.Pairs);
            if (!answer.IsSuccess)
                goto RETURN;
            answer.DataIndex = SuccessSentence.AssignmentInterpretationSuccess;
            var ast = new ASTValueTypePair(RaceTree.consti);
            if (ast.TryAddAST(tempData.Constis, expression, tempData.ConstiCapacity, ref tempData.ConstiLength))
                list.TryAddMultiThread(ast);
            RETURN:
            return answer;
        }

        private static TryInterpretReturnValue BraveDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, ref ASTValueTypePairList list)
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
            var ast = new ASTValueTypePair(RaceTree.brave);
            if (ast.TryAddAST(tempData.Braves, expression, tempData.BraveCapacity, ref tempData.BraveLength))
                list.TryAddMultiThread(ast);
            RETURN:
            return answer;
        }

        private static TryInterpretReturnValue AlignDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, ref ASTValueTypePairList list)
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
            {
                for (int i = 0; i < ConstiLength; i++)
                {
                    Constis[i].Dispose();
                }
                UnsafeUtility.Free(Constis, Allocator.Persistent);
            }
            this = default;
        }
    }
}