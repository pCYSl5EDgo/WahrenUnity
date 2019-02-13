namespace pcysl5edgo.Wahren.AST
{
    public static unsafe class RaceParser
    {
        public static TryInterpretReturnValue TryParseRaceStructMultiThread(this ref TextFile file, ref RaceParserTempData tempData, ref ASTValueTypePairList astValueTypePairList, Span name, Span parentName, Caret nextToLeftBrace, out Caret nextToRightBrace, out int raceTreeIndex)
        {
            nextToRightBrace = nextToLeftBrace;
            file.SkipWhiteSpace(ref nextToRightBrace);
            ref var column = ref nextToRightBrace.Column;
            TryInterpretReturnValue answer;
            answer.Span.File = file.FilePathId;
            var tree = new RaceTree
            {
                Name = name,
                ParentName = parentName,
            };
            var list = ASTValueTypePairList.MallocTemp(4);
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
                                if (!(answer = AlignDetect(ref file, ref tempData, ref nextToRightBrace, ref list)))
                                {
                                    goto RETURN;
                                }
                                nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                                break;
                            case 'b': // brave
                                if (!(answer = BraveDetect(ref file, ref tempData, ref nextToRightBrace, ref list)))
                                {
                                    goto RETURN;
                                }
                                nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                                break;
                            case 'c': // consti
                                if (!(answer = ConstiDetect(ref file, ref tempData, ref nextToRightBrace, ref list)))
                                {
                                    goto RETURN;
                                }
                                nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                                break;
                            case 'm': // movetype
                                if (!(answer = MoveTypeDetect(ref file, ref tempData, ref nextToRightBrace, ref list)))
                                {
                                    goto RETURN;
                                }
                                nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                                break;
                            case 'n': // name
                                if (!(answer = NameDetect(ref file, ref tempData, ref nextToRightBrace, ref list)))
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

        private static TryInterpretReturnValue NameDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, ref ASTValueTypePairList list)
        {
            var cs = file.CurrentCharPointer(current);
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
                answer = TryInterpretReturnValue.CreatePending(answer.Span, Location.Race, PendingReason.SectionListCapacityShortage, RaceTree.name + 1);
            }
        RETURN:
            return answer;
        }

        private static TryInterpretReturnValue MoveTypeDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, ref ASTValueTypePairList listTemp)
        {
            var cs = file.CurrentCharPointer(current);
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
            answer = ReadUtility.TryReadIdentifierNotEmpty(file.Contents + file.LineStarts[current.Line], file.CurrentLineLength(current), current.File, current.Line, current.Column);
            if (!answer)
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
                answer = TryInterpretReturnValue.CreatePending(answer.Span, Location.Race, PendingReason.SectionListCapacityShortage, RaceTree.movetype + 1);
            }
        RETURN:
            return answer;
        }

        private static TryInterpretReturnValue ConstiDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, ref ASTValueTypePairList listTemp)
        {
            var cs = file.CurrentCharPointer(current);
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
            answer = file.TryReadIdentifierNumberPairs(ref tempData.IdentifierNumberPairs, current, out expression.Start, out expression.Length);
            if (!answer)
                goto RETURN;
            current = answer.Span.CaretNextToEndOfThisSpan;
            file.SkipWhiteSpace(ref current);
            answer = VerifyConsti(expression, tempData.IdentifierNumberPairs, answer.Span);
            if (answer.IsError)
                goto RETURN;
            var ast = new ASTValueTypePair(RaceTree.consti);
            if (ast.TryAddAST(tempData.Constis, expression, tempData.ConstiCapacity, ref tempData.ConstiLength))
            {
                ast.AddToTempJob(ref listTemp.Values, ref listTemp.Capacity, ref listTemp.Length, out _);
            }
            else
            {
                answer = TryInterpretReturnValue.CreatePending(answer.Span, Location.Race, PendingReason.SectionListCapacityShortage, RaceTree.consti + 1);
            }
        RETURN:
            return answer;
        }

        internal static TryInterpretReturnValue VerifyConsti(RaceTree.ConstiAssignExpression expression, in IdentifierNumberPairList list, Span span)
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
            var cs = file.CurrentCharPointer(current);
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
            if (!answer)
                goto RETURN;
            expression.Value = (sbyte)value;
            var ast = new ASTValueTypePair(RaceTree.brave);
            if (ast.TryAddAST(tempData.Braves, expression, tempData.BraveCapacity, ref tempData.BraveLength))
            {
                ast.AddToTempJob(ref listTemp.Values, ref listTemp.Capacity, ref listTemp.Length, out _);
            }
            else
            {
                answer = TryInterpretReturnValue.CreatePending(answer.Span, Location.Race, PendingReason.SectionListCapacityShortage, RaceTree.brave + 1);
            }
        RETURN:
            return answer;
        }

        private static TryInterpretReturnValue AlignDetect(ref TextFile file, ref RaceParserTempData tempData, ref Caret current, ref ASTValueTypePairList listTemp)
        {
            var cs = file.CurrentCharPointer(current);
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
            if (!answer)
                goto RETURN;
            expression.Value = (sbyte)value;
            var ast = new ASTValueTypePair(RaceTree.align);
            if (ast.TryAddAST(tempData.Aligns, expression, tempData.AlignCapacity, ref tempData.AlignLength))
            {
                ast.AddToTempJob(ref listTemp.Values, ref listTemp.Capacity, ref listTemp.Length, out _);
            }
            else
            {
                answer = TryInterpretReturnValue.CreatePending(answer.Span, Location.Race, PendingReason.SectionListCapacityShortage, RaceTree.align + 1);
            }
        RETURN:
            return answer;
        }
    }
}