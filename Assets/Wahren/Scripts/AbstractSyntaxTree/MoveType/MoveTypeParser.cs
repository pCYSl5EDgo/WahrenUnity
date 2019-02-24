namespace pcysl5edgo.Wahren.AST
{
    public static unsafe class MovetypeParser
    {
        public static TryInterpretReturnValue TryParseMovetypeStructMultiThread(this ref TextFile file, ref MovetypeParserTempData tempData, ref ASTTypePageIndexPairList astValueTypePairList, Span name, Span parentName, Caret nextToLeftBrace, out Caret nextToRightBrace, Unity.Collections.Allocator allocator)
        {
            var tree = CreateNameTreeHelper.Create<MovetypeTree>(name, parentName);
            var _ = new InitialProc_USING_STRUCT(3, file, nextToLeftBrace, out nextToRightBrace, out var answer, allocator);
            ref var column = ref nextToRightBrace.Column;
            for (ref var raw = ref nextToRightBrace.Line; raw < file.LineCount; raw++, column = 0)
            {
                for (int lineLength = file.LineLengths[raw]; column < lineLength; column++)
                {
                    switch ((file.Contents + file.LineStarts[raw])[column])
                    {
                        case '}':
                            answer = tree.CloseBrace(ref tree.Start, out tree.Length, ref astValueTypePairList, _.list, ref tempData.Values, tempData.Capacity, ref tempData.Length, Location.Movetype, SuccessSentence.Kind.MovetypeTreeInterpretSuccess, nextToLeftBrace, ref nextToRightBrace);
                            goto RETURN;
                        case 'n':
                            if (!(answer = NameDetect(ref file, ref tempData, ref nextToRightBrace, ref _)))
                                goto RETURN;
                            nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                            break;
                        case 'h':
                            if (!(answer = HelpDetect(ref file, ref tempData, ref nextToRightBrace, ref _)))
                                goto RETURN;
                            nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                            break;
                        case 'c':
                            if (!(answer = ConstiDetect(ref file, ref tempData, ref nextToRightBrace, ref _)))
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

        private static TryInterpretReturnValue HelpDetect(ref TextFile file, ref MovetypeParserTempData tempData, ref Caret current, ref InitialProc_USING_STRUCT proc)
        {
            var expression = new MovetypeTree.HelpAssignExpression();
            var cs = stackalloc ushort[] { 'e', 'l', 'p' };
            if (!file.TryInitializeDetect(cs, 3, ref current, out var answer, out expression.ScenarioVariant))
                goto RETURN;
            answer = new TryInterpretReturnValue(file.ReadLine(current), SuccessSentence.Kind.AssignmentInterpretationSuccess);
            expression.Value = answer.Span;
            var ast = MovetypeTree.Kind.help.CreateASTPair();
            if (ast.TryAddAST(tempData.Helps, expression, tempData.HelpCapacity, ref tempData.HelpLength))
            {
                proc.Add(ast);
            }
            else
            {
                answer = MovetypeTree.Kind.help.CreatePending(answer.Span);
            }
        RETURN:
            return answer;
        }
        private static TryInterpretReturnValue NameDetect(ref TextFile file, ref MovetypeParserTempData tempData, ref Caret current, ref InitialProc_USING_STRUCT proc)
        {
            var expression = new MovetypeTree.NameAssignExpression();
            var cs = stackalloc ushort[] { 'a', 'm', 'e' };
            if (!file.TryInitializeDetect(cs, 3, ref current, out var answer, out expression.ScenarioVariant))
                goto RETURN;
            answer = new TryInterpretReturnValue(file.ReadLine(current), SuccessSentence.Kind.AssignmentInterpretationSuccess);
            expression.Value = answer.Span;
            var ast = MovetypeTree.Kind.name.CreateASTPair();
            if (ast.TryAddAST(tempData.Names, expression, tempData.NameCapacity, ref tempData.NameLength))
            {
                proc.Add(ast);
            }
            else
            {
                answer = MovetypeTree.Kind.name.CreatePending(answer.Span);
            }
        RETURN:
            return answer;
        }

        private static TryInterpretReturnValue ConstiDetect(ref TextFile file, ref MovetypeParserTempData tempData, ref Caret current, ref InitialProc_USING_STRUCT proc)
        {
            var expression = new MovetypeTree.ConstiAssignExpression();
            var cs = stackalloc ushort[] { 'o', 'n', 's', 't', 'i' };
            if (!file.TryInitializeDetect(cs, 5, ref current, out var answer, out expression.ScenarioVariant))
                goto RETURN;
            answer = file.TryReadIdentifierNumberPairs(Location.Movetype, ref tempData.IdentifierNumberPairs, current, out expression.Start, out expression.Length);
            if (!answer)
                goto RETURN;
            current = answer.Span.CaretNextToEndOfThisSpan;
            file.SkipWhiteSpace(ref current);
            answer = VerifyConsti(expression, tempData.IdentifierNumberPairs, answer.Span);
            if (answer.IsError)
                goto RETURN;
            var ast = MovetypeTree.Kind.consti.CreateASTPair();
            if (ast.TryAddAST(tempData.Constis, expression, tempData.ConstiCapacity, ref tempData.ConstiLength))
            {
                proc.Add(ast);
            }
            else
            {
                answer = MovetypeTree.Kind.consti.CreatePending(answer.Span);
            }
        RETURN:
            return answer;
        }

        internal static TryInterpretReturnValue VerifyConsti(MovetypeTree.ConstiAssignExpression expression, in IdentifierNumberPairList list, Span span)
        {
            for (int i = expression.Start, end = expression.Start + expression.Length; i < end; i++)
            {
                ref IdentifierNumberPair val = ref list.This.Values[i];
                if (val.Span.Length == 0 || val.Number < 0 || val.Number > 10)
                    return new TryInterpretReturnValue(val.NumberSpan, ErrorSentence.Kind.OutOfRangeError);
            }
            return new TryInterpretReturnValue(span, default, default, InterpreterStatus.Success);
        }
    }
}