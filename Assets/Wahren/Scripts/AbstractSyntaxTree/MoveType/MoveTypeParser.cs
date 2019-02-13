namespace pcysl5edgo.Wahren.AST
{
    public static unsafe class MoveTypeParser
    {
        public static TryInterpretReturnValue TryParseMovetypeStructMultiThread(this ref TextFile file, ref MovetypeParserTempData tempData, ref ASTValueTypePairList astValueTypePairList, Span name, Span parentName, Caret nextToLeftBrace, out Caret nextToRightBrace, out int raceTreeIndex)
        {
            nextToRightBrace = nextToLeftBrace;
            file.SkipWhiteSpace(ref nextToRightBrace);
            ref var column = ref nextToRightBrace.Column;
            TryInterpretReturnValue answer;
            answer.Span.File = file.FilePathId;
            var tree = new MoveTypeTree
            {
                Name = name,
                ParentName = parentName,
            };
            var list = ASTValueTypePairList.MallocTemp(3);
            try
            {
                for (ref var raw = ref nextToRightBrace.Line; raw < file.LineCount; raw++, column = 0)
                {
                    for (int lineLength = file.LineLengths[raw]; column < lineLength; column++)
                    {
                        switch ((file.Contents + file.LineStarts[raw])[column])
                        {
                            case '}':
                                column++;
                                tree.Start = astValueTypePairList.TryAddBulkMultiThread(list, out tree.Length);
                                if (tree.Start == -1)
                                {
                                    raceTreeIndex = -1;
                                    return TryInterpretReturnValue.CreatePending(new Span(nextToLeftBrace, 0), Location.MoveType, PendingReason.ASTValueTypePairListCapacityShortage);
                                }
                                if (tree.TryAddToMultiThread(ref tempData.Values, tempData.Capacity, ref tempData.Length, out raceTreeIndex))
                                {
                                    return new TryInterpretReturnValue(nextToRightBrace, SuccessSentence.MoveTypeTreeInterpretSuccess, InterpreterStatus.Success);
                                }
                                else
                                {
                                    return TryInterpretReturnValue.CreatePending(new Span(nextToLeftBrace, 0), Location.MoveType, PendingReason.TreeListCapacityShortage);
                                }
                            case 'n':
                                if (!(answer = NameDetect(ref file, ref tempData, ref nextToRightBrace, ref list)))
                                {
                                    goto RETURN;
                                }
                                nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                                break;
                            case 'h':
                                if (!(answer = HelpDetect(ref file, ref tempData, ref nextToRightBrace, ref list)))
                                {
                                    goto RETURN;
                                }
                                nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                                break;
                            case 'c':
                                if (!(answer = ConstiDetect(ref file, ref tempData, ref nextToRightBrace, ref list)))
                                {
                                    goto RETURN;
                                }
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
            finally
            {
                ASTValueTypePairList.FreeTemp(ref list);
            }
        }

        private static TryInterpretReturnValue HelpDetect(ref TextFile file, ref MovetypeParserTempData tempData, ref Caret current, ref ASTValueTypePairList list)
        {
            var cs = file.CurrentCharPointer(current);
            var answer = new TryInterpretReturnValue(new Span(current, 1), ErrorSentence.InvalidIdentifierError, 0, InterpreterStatus.Error);
            int thisLineLength = file.CurrentLineLength(current);
            if (current.Column + 3 >= thisLineLength || *++cs != 'e' || *++cs != 'l' || *++cs != 'p')
                goto RETURN;
            current.Column += 4;
            var expression = new MoveTypeTree.HelpAssignExpression();
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
            var ast = new ASTValueTypePair(MoveTypeTree.help);
            if (ast.TryAddAST(tempData.Helps, expression, tempData.HelpCapacity, ref tempData.HelpLength))
            {
                ast.AddToTempJob(ref list.Values, ref list.Capacity, ref list.Length, out _);
            }
            else
            {
                answer = TryInterpretReturnValue.CreatePending(answer.Span, Location.MoveType, PendingReason.SectionListCapacityShortage, MoveTypeTree.help + 1);
            }
        RETURN:
            return answer;
        }
        private static TryInterpretReturnValue NameDetect(ref TextFile file, ref MovetypeParserTempData tempData, ref Caret current, ref ASTValueTypePairList list)
        {
            var cs = file.CurrentCharPointer(current);
            var answer = new TryInterpretReturnValue(new Span(current, 1), ErrorSentence.InvalidIdentifierError, 0, InterpreterStatus.Error);
            int thisLineLength = file.CurrentLineLength(current);
            if (current.Column + 3 >= thisLineLength || *++cs != 'a' || *++cs != 'm' || *++cs != 'e')
                goto RETURN;
            current.Column += 4;
            var expression = new MoveTypeTree.NameAssignExpression();
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
            var ast = new ASTValueTypePair(MoveTypeTree.name);
            if (ast.TryAddAST(tempData.Names, expression, tempData.NameCapacity, ref tempData.NameLength))
            {
                ast.AddToTempJob(ref list.Values, ref list.Capacity, ref list.Length, out _);
            }
            else
            {
                answer = TryInterpretReturnValue.CreatePending(answer.Span, Location.MoveType, PendingReason.SectionListCapacityShortage, MoveTypeTree.name + 1);
            }
        RETURN:
            return answer;
        }

        private static TryInterpretReturnValue ConstiDetect(ref TextFile file, ref MovetypeParserTempData tempData, ref Caret current, ref ASTValueTypePairList listTemp)
        {
            var cs = file.CurrentCharPointer(current);
            var answer = new TryInterpretReturnValue(new Span(current, 1), ErrorSentence.InvalidIdentifierError, InterpreterStatus.Error);
            int thisLineLength = file.CurrentLineLength(current);
            if (current.Column + 5 >= thisLineLength || *++cs != 'o' || *++cs != 'n' || *++cs != 's' || *++cs != 't' || *++cs != 'i')
                goto RETURN;
            current.Column += 6;
            var expression = new MoveTypeTree.ConstiAssignExpression();
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
            answer = file.TryReadIdentifierNumberPairs(Location.MoveType, ref tempData.IdentifierNumberPairs, current, out expression.Start, out expression.Length);
            if (!answer)
                goto RETURN;
            current = answer.Span.CaretNextToEndOfThisSpan;
            file.SkipWhiteSpace(ref current);
            answer = VerifyConsti(expression, tempData.IdentifierNumberPairs, answer.Span);
            if (answer.IsError)
                goto RETURN;
            var ast = new ASTValueTypePair(MoveTypeTree.consti);
            if (ast.TryAddAST(tempData.Constis, expression, tempData.ConstiCapacity, ref tempData.ConstiLength))
            {
                ast.AddToTempJob(ref listTemp.Values, ref listTemp.Capacity, ref listTemp.Length, out _);
            }
            else
            {
                answer = TryInterpretReturnValue.CreatePending(answer.Span, Location.MoveType, PendingReason.SectionListCapacityShortage, MoveTypeTree.consti + 1);
            }
        RETURN:
            return answer;
        }

        internal static TryInterpretReturnValue VerifyConsti(MoveTypeTree.ConstiAssignExpression expression, in IdentifierNumberPairList list, Span span)
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