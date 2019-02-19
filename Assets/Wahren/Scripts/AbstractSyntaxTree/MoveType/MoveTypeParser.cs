namespace pcysl5edgo.Wahren.AST
{
    public static unsafe class MovetypeParser
    {
        public static TryInterpretReturnValue TryParseMovetypeStructMultiThread(this ref TextFile file, ref MovetypeParserTempData tempData, ref ASTValueTypePairList astValueTypePairList, Span name, Span parentName, Caret nextToLeftBrace, out Caret nextToRightBrace, out int treeIndex)
        {
            var tree = CreateNameTreeHelper.Create<MovetypeTree>(name, parentName);
            var _ = new InitialProc_USING_STRUCT(3, file, nextToLeftBrace, out nextToRightBrace, out var answer, out treeIndex);
            ref var column = ref nextToRightBrace.Column;
            for (ref var raw = ref nextToRightBrace.Line; raw < file.LineCount; raw++, column = 0)
            {
                for (int lineLength = file.LineLengths[raw]; column < lineLength; column++)
                {
                    switch ((file.Contents + file.LineStarts[raw])[column])
                    {
                        case '}':
                            answer = tree.CloseBrace(ref tree.Start, out tree.Length, ref astValueTypePairList, _.list, ref tempData.Values, tempData.Capacity, ref tempData.Length, Location.Movetype, SuccessSentence.MovetypeTreeInterpretSuccess, ref treeIndex, nextToLeftBrace, ref nextToRightBrace);
                            goto RETURN;
                        case 'n':
                            if (!(answer = NameDetect(ref file, ref tempData, ref nextToRightBrace, &_.list)))
                                goto RETURN;
                            nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                            break;
                        case 'h':
                            if (!(answer = HelpDetect(ref file, ref tempData, ref nextToRightBrace, &_.list)))
                                goto RETURN;
                            nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                            break;
                        case 'c':
                            if (!(answer = ConstiDetect(ref file, ref tempData, ref nextToRightBrace, &_.list)))
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

        private static TryInterpretReturnValue HelpDetect(ref TextFile file, ref MovetypeParserTempData tempData, ref Caret current, ASTValueTypePairList* list)
        {
            var expression = new MovetypeTree.HelpAssignExpression();
            var cs = stackalloc char[] { 'e', 'l', 'p' };
            if (!file.TryInitializeDetect(cs, 3, ref current, out var answer, out expression.ScenarioVariant))
                goto RETURN;
            answer = new TryInterpretReturnValue(file.ReadLine(current), SuccessSentence.AssignmentInterpretationSuccess, InterpreterStatus.Success);
            expression.Value = answer.Span;
            var ast = new ASTValueTypePair(MovetypeTree.help);
            if (ast.TryAddAST(tempData.Helps, expression, tempData.HelpCapacity, ref tempData.HelpLength))
            {
                ast.AddToTempJob(ref list->Values, ref list->Capacity, ref list->Length, out _);
            }
            else
            {
                answer = TryInterpretReturnValue.CreatePending(answer.Span, Location.Movetype, PendingReason.SectionListCapacityShortage, MovetypeTree.help + 1);
            }
        RETURN:
            return answer;
        }
        private static TryInterpretReturnValue NameDetect(ref TextFile file, ref MovetypeParserTempData tempData, ref Caret current, ASTValueTypePairList* list)
        {
            var expression = new MovetypeTree.NameAssignExpression();
            var cs = stackalloc char[] { 'a', 'm', 'e' };
            if (!file.TryInitializeDetect(cs, 3, ref current, out var answer, out expression.ScenarioVariant))
                goto RETURN;
            answer = new TryInterpretReturnValue(file.ReadLine(current), SuccessSentence.AssignmentInterpretationSuccess, InterpreterStatus.Success);
            expression.Value = answer.Span;
            var ast = new ASTValueTypePair(MovetypeTree.name);
            if (ast.TryAddAST(tempData.Names, expression, tempData.NameCapacity, ref tempData.NameLength))
            {
                ast.AddToTempJob(ref list->Values, ref list->Capacity, ref list->Length, out _);
            }
            else
            {
                answer = TryInterpretReturnValue.CreatePending(answer.Span, Location.Movetype, PendingReason.SectionListCapacityShortage, MovetypeTree.name + 1);
            }
        RETURN:
            return answer;
        }

        private static TryInterpretReturnValue ConstiDetect(ref TextFile file, ref MovetypeParserTempData tempData, ref Caret current, ASTValueTypePairList* list)
        {
            var expression = new MovetypeTree.ConstiAssignExpression();
            var cs = stackalloc char[] { 'o', 'n', 's', 't', 'i' };
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
            var ast = new ASTValueTypePair(MovetypeTree.consti);
            if (ast.TryAddAST(tempData.Constis, expression, tempData.ConstiCapacity, ref tempData.ConstiLength))
            {
                ast.AddToTempJob(ref list->Values, ref list->Capacity, ref list->Length, out _);
            }
            else
            {
                answer = TryInterpretReturnValue.CreatePending(answer.Span, Location.Movetype, PendingReason.SectionListCapacityShortage, MovetypeTree.consti + 1);
            }
        RETURN:
            return answer;
        }

        internal static TryInterpretReturnValue VerifyConsti(MovetypeTree.ConstiAssignExpression expression, in IdentifierNumberPairList list, Span span)
        {
            for (int i = expression.Start, end = expression.Start + expression.Length; i < end; i++)
            {
                ref IdentifierNumberPair val = ref list.Values[i];
                if (val.Span.Length == 0 || val.Number < 0 || val.Number > 10)
                    return new TryInterpretReturnValue(val.NumberSpan, ErrorSentence.OutOfRangeError, InterpreterStatus.Error);
            }
            return new TryInterpretReturnValue(span, default, default, InterpreterStatus.Success);
        }
    }
}