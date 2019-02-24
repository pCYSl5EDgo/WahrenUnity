using System;

namespace pcysl5edgo.Wahren.AST
{
    public static unsafe class RaceParser
    {
        public static TryInterpretReturnValue TryParseRaceStructMultiThread(this ref TextFile file, ref RaceParserTempData tempData, ref ASTTypePageIndexPairList astValueTypePairList, Span name, Span parentName, Caret nextToLeftBrace, out Caret nextToRightBrace, Unity.Collections.Allocator allocator)
        {
            var tree = CreateNameTreeHelper.Create<RaceTree>(name, parentName);
            var _ = new InitialProc_USING_STRUCT(4, file, nextToLeftBrace, out nextToRightBrace, out TryInterpretReturnValue answer);
            ref var column = ref nextToRightBrace.Column;
            for (ref var raw = ref nextToRightBrace.Line; raw < file.LineCount; raw++, column = 0)
            {
                for (int lineLength = file.LineLengths[raw]; column < lineLength; column++)
                {
                    switch ((file.Contents + file.LineStarts[raw])[column])
                    {
                        case '}':
                            answer = tree.CloseBrace(ref tree.Start, out tree.Length, ref astValueTypePairList, _.list, ref tempData.Values, tempData.Capacity, ref tempData.Length, Location.Race, SuccessSentence.Kind.RaceTreeIntrepretSuccess, nextToLeftBrace, ref nextToRightBrace);
                            goto RETURN;
                        case 'a': // align
                            if (!(answer = AlignDetect(ref file, ref tempData, ref nextToRightBrace, ref _)))
                                goto RETURN;
                            nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                            break;
                        case 'b': // brave
                            if (!(answer = BraveDetect(ref file, ref tempData, ref nextToRightBrace, ref _)))
                                goto RETURN;
                            nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                            break;
                        case 'c': // consti
                            if (!(answer = ConstiDetect(ref file, ref tempData, ref nextToRightBrace, ref _)))
                                goto RETURN;
                            nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                            break;
                        case 'm': // movetype
                            if (!(answer = MoveTypeDetect(ref file, ref tempData, ref nextToRightBrace, ref _)))
                                goto RETURN;
                            nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                            break;
                        case 'n': // name
                            if (!(answer = NameDetect(ref file, ref tempData, ref nextToRightBrace, ref _)))
                                goto RETURN;
                            nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                            break;
                        case ' ':
                        case '\t':
                            break;
                        default:
                            answer = TryInterpretReturnValue.CreateNotExpectedCharacter(nextToRightBrace);
                            goto RETURN;
                    }
                }
            }
            answer = TryInterpretReturnValue.CreateRightBraceNotFound(nextToRightBrace);
        RETURN:
            _.Dispose();
            return answer;
        }

        private static TryInterpretReturnValue NameDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, ref InitialProc_USING_STRUCT proc)
        {
            var expression = new RaceTree.NameAssignExpression();
            var cs = stackalloc ushort[] { 'a', 'm', 'e' };
            if (!file.TryInitializeDetect(cs, 3, ref current, out var answer, out expression.ScenarioVariant))
                goto RETURN;
            answer = new TryInterpretReturnValue(file.ReadLine(current), SuccessSentence.Kind.AssignmentInterpretationSuccess);
            expression.Value = answer.Span;
            var ast = RaceTree.Kind.name.CreateASTPair();
            if (ast.TryAddAST(tempData.Names, expression, tempData.NameCapacity, ref tempData.NameLength))
            {
                proc.Add(ast);
            }
            else
            {
                answer = RaceTree.Kind.name.CreatePending(answer.Span);
            }
        RETURN:
            return answer;
        }

        private static TryInterpretReturnValue MoveTypeDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, ref InitialProc_USING_STRUCT proc)
        {
            var expression = new RaceTree.MovetypeAssignExpression();
            var cs = stackalloc ushort[] { 'o', 'v', 'e', 't', 'y', 'p', 'e' };
            if (!file.TryInitializeDetect(cs, 7, ref current, out var answer, out expression.ScenarioVariant))
                goto RETURN;
            answer = ReadUtility.TryReadIdentifierNotEmpty(file.Contents + file.LineStarts[current.Line], file.CurrentLineLength(current), current.File, current.Line, current.Column);
            if (!answer)
                goto RETURN;
            answer.DataIndex = (int)SuccessSentence.Kind.AssignmentInterpretationSuccess;
            expression.Value = answer.Span;
            var ast = RaceTree.Kind.movetype.CreateASTPair();
            if (ast.TryAddAST(tempData.Movetypes, expression, tempData.MovetypeCapacity, ref tempData.MovetypeLength))
            {
                proc.Add(ast);
            }
            else
            {
                answer = RaceTree.Kind.movetype.CreatePending(answer.Span);
            }
        RETURN:
            return answer;
        }

        private static TryInterpretReturnValue ConstiDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, ref InitialProc_USING_STRUCT proc)
        {
            var expression = new RaceTree.ConstiAssignExpression();
            var cs = stackalloc ushort[] { 'o', 'n', 's', 't', 'i' };
            if (!file.TryInitializeDetect(cs, 5, ref current, out var answer, out expression.ScenarioVariant))
                goto RETURN;
            answer = file.TryReadIdentifierNumberPairs(Location.Race, ref tempData.IdentifierNumberPairs, current, out expression.Start, out expression.Length);
            if (!answer)
                goto RETURN;
            current = answer.Span.CaretNextToEndOfThisSpan;
            file.SkipWhiteSpace(ref current);
            answer = VerifyConsti(expression, tempData.IdentifierNumberPairs, answer.Span);
            if (answer.IsError)
                goto RETURN;
            var ast = RaceTree.Kind.consti.CreateASTPair();
            if (ast.TryAddAST(tempData.Constis, expression, tempData.ConstiCapacity, ref tempData.ConstiLength))
            {
                proc.Add(ast);
            }
            else
            {
                answer = RaceTree.Kind.consti.CreatePending(answer.Span);
            }
        RETURN:
            return answer;
        }

        internal static TryInterpretReturnValue VerifyConsti(RaceTree.ConstiAssignExpression expression, in IdentifierNumberPairList list, Span span)
        {
            for (int i = expression.Start, end = expression.Start + expression.Length; i < end; i++)
            {
                ref IdentifierNumberPair val = ref list.This.Values[i];
                if (val.Span.Length == 0 || val.Number < 0 || val.Number > 10)
                    return new TryInterpretReturnValue(val.NumberSpan, ErrorSentence.Kind.OutOfRangeError);
            }
            return new TryInterpretReturnValue(span, default, default, InterpreterStatus.Success);
        }

        private static TryInterpretReturnValue BraveDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, ref InitialProc_USING_STRUCT proc)
        {
            var expression = new RaceTree.BraveAssignExpression();
            var cs = stackalloc ushort[] { 'r', 'a', 'v', 'e' };
            if (!file.TryInitializeDetect(cs, 4, ref current, out var answer, out expression.ScenarioVariant))
                goto RETURN;
            answer = file.TryReadNumber(current, out var value);
            VerifyNumber(ref answer, value);
            if (!answer)
                goto RETURN;
            expression.Value = (sbyte)value;
            var ast = RaceTree.Kind.brave.CreateASTPair();
            if (ast.TryAddAST(tempData.Braves, expression, tempData.BraveCapacity, ref tempData.BraveLength))
            {
                proc.Add(ast);
            }
            else
            {
                answer = RaceTree.Kind.brave.CreatePending(answer.Span);
            }
        RETURN:
            return answer;
        }

        private static TryInterpretReturnValue AlignDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, ref InitialProc_USING_STRUCT proc)
        {
            var expression = new RaceTree.AlignAssignExpression();
            var cs = stackalloc ushort[] { 'l', 'i', 'g', 'n' };
            if (!file.TryInitializeDetect(cs, 4, ref current, out var answer, out expression.ScenarioVariant))
                goto RETURN;
            answer = file.TryReadNumber(current, out var value);
            VerifyNumber(ref answer, value);
            if (!answer)
                goto RETURN;
            expression.Value = (sbyte)value;
            var ast = RaceTree.Kind.align.CreateASTPair();
            if (ast.TryAddAST(tempData.Aligns, expression, tempData.AlignCapacity, ref tempData.AlignLength))
            {
                proc.Add(ast);
            }
            else
            {
                answer = RaceTree.Kind.align.CreatePending(answer.Span);
            }
        RETURN:
            return answer;
        }

        private static void VerifyNumber(ref TryInterpretReturnValue answer, long value)
        {
            if (value < 0 || value > 100)
            {
                answer.Status = InterpreterStatus.Error;
                answer.DataIndex = (int)ErrorSentence.Kind.OutOfRangeError;
            }
        }
    }
}