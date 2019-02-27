namespace pcysl5edgo.Wahren.AST
{
    public static unsafe class MovetypeParser
    {
        public static TryInterpretReturnValue TryParseMovetypeStructMultiThread(this ref TextFile file, ref MovetypeParserTempData tempData, ref ASTTypePageIndexPairListLinkedList astValueTypePairList, Span name, Span parentName, Caret nextToLeftBrace, out Caret nextToRightBrace, Unity.Collections.Allocator allocator)
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
                            answer = tree.CloseBrace<MovetypeTree>(out tree.Page, out tree.Start, out tree.Length, ref astValueTypePairList, _.list, ref tempData.Values, Location.Movetype, SuccessSentence.Kind.MovetypeTreeInterpretSuccess, nextToLeftBrace, ref nextToRightBrace, allocator);
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
            tempData.Helps.Add(ref expression, out ast.Page, out ast.Index, proc.allocator);
            proc.Add(ast);
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
            tempData.Names.Add(ref expression, out ast.Page, out ast.Index, proc.allocator);
            proc.Add(ast);
        RETURN:
            return answer;
        }

        private static TryInterpretReturnValue ConstiDetect(ref TextFile file, ref MovetypeParserTempData tempData, ref Caret current, ref InitialProc_USING_STRUCT proc)
        {
            var expression = new MovetypeTree.ConstiAssignExpression();
            var cs = stackalloc ushort[] { 'o', 'n', 's', 't', 'i' };
            if (!file.TryInitializeDetect(cs, 5, ref current, out var answer, out expression.ScenarioVariant))
                goto RETURN;
            answer = file.TryReadIdentifierNumberPairs(Location.Movetype, ref tempData.IdentifierNumberPairs2, current,  out expression.Page, out expression.Start, out expression.Length, proc.allocator);
            if (!answer)
                goto RETURN;
            current = answer.Span.CaretNextToEndOfThisSpan;
            file.SkipWhiteSpace(ref current);
            answer = VerifyConsti(expression, ref tempData.IdentifierNumberPairs2, answer.Span);
            if (answer.IsError)
                goto RETURN;
            var ast = MovetypeTree.Kind.consti.CreateASTPair();
            tempData.Constis.Add(ref expression, out ast.Page, out ast.Index, proc.allocator);
            proc.Add(ast);
        RETURN:
            return answer;
        }

        internal static TryInterpretReturnValue VerifyConsti(MovetypeTree.ConstiAssignExpression expression, ref IdentifierNumberPairListLinkedList list, Span span)
        {
            for (int i = expression.Start, end = expression.Start + expression.Length; i < end; i++)
            {
                ref var val = ref expression.Page->GetRef(i);
                if (val.Span.Length == 0 || val.Number < 0 || val.Number > 10)
                    return new TryInterpretReturnValue(val.NumberSpan, ErrorSentence.Kind.OutOfRangeError);
            }
            return new TryInterpretReturnValue(span, default, default, InterpreterStatus.Success);
        }
    }
}