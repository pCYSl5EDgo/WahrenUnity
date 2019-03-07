using System;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe static class VoiceParser
    {
        public static TryInterpretReturnValue TryParseVoiceStructMultiThread(this ref TextFile file, ref VoiceParserTempData tempData, ref ASTTypePageIndexPairListLinkedList astValueTypePairList, Span name, Span parentName, Caret nextToLeftBrace, out Caret nextToRightBrace, Unity.Collections.Allocator allocator)
        {
            var tree = CreateNameTreeHelper.Create<VoiceTree>(name, parentName);
            var _ = new InitialProc_USING_STRUCT(4, file, nextToLeftBrace, out nextToRightBrace, out var answer, allocator);
            try
            {
                ref var column = ref nextToRightBrace.Column;
                for (ref var raw = ref nextToRightBrace.Line; raw < file.LineCount; raw++, column = 0)
                {
                    for (int lineLength = file.LineLengths[raw]; column < lineLength; column++)
                    {
                        switch ((file.Contents + file.LineStarts[raw])[column])
                        {
                            case '}':
                                answer = tree.CloseBrace(out tree.Page, out tree.Start, out tree.Length, ref astValueTypePairList, _.list, ref tempData.Values, Location.Voice, SuccessSentence.Kind.VoiceTreeIntrepretSuccess, nextToLeftBrace, ref nextToRightBrace, allocator);
                                goto RETURN;
                            case 'p': // power
                                if (!(answer = PowerDetect(ref file, ref tempData, ref nextToRightBrace, ref _)))
                                    goto RETURN;
                                nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                                break;
                            case 's': // spot
                                if (!(answer = SpotDetect(ref file, ref tempData, ref nextToRightBrace, ref _)))
                                    goto RETURN;
                                nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                                break;
                            case 'v': // voice_type
                                if (!(answer = VoiceTypeDetect(ref file, ref tempData, ref nextToRightBrace, ref _)))
                                    goto RETURN;
                                nextToRightBrace = answer.Span.CaretNextToEndOfThisSpan;
                                break;
                            case 'd':
                                if (!(answer = DelskillDetect(ref file, ref tempData, ref nextToRightBrace, ref _)))
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
                return answer;
            }
            finally
            {
                _.Dispose();
            }
        }

        private static TryInterpretReturnValue DelskillDetect(ref TextFile file, ref VoiceParserTempData tempData, ref Caret current, ref InitialProc_USING_STRUCT proc)
        {
            var expression = new VoiceTree.DelskillAssignExpression();
            var cs = stackalloc ushort[] { 'e', 'l', 's', 'k', 'i', 'l', 'l' };
            if (!file.TryInitializeDetect(cs, 7, ref current, out var answer, out expression.ScenarioVariant))
                goto RETURN;
            if (tempData.Identifiers.List.First == null) throw new NullReferenceException();
            answer = file.TryReadIdentifiers(ref tempData.Identifiers, current, out expression.Page, out expression.Start, out expression.Length, proc.allocator);
            if (!answer)
                goto RETURN;
            current = answer.Span.CaretNextToEndOfThisSpan;
            file.SkipWhiteSpace(ref current);
            var ast = VoiceTree.Kind.delskill.CreateASTPair();
            tempData.Delskills.Add(ref expression, out ast.Page, out ast.Index, proc.allocator);
            proc.Add(ast);
        RETURN:
            return answer;
        }

        private static TryInterpretReturnValue VoiceTypeDetect(ref TextFile file, ref VoiceParserTempData tempData, ref Caret current, ref InitialProc_USING_STRUCT proc)
        {
            VoiceTree.VoiceTypeAssignExpression expression = default;
            var cs = stackalloc ushort[] { 'o', 'i', 'c', 'e', '_', 't', 'y', 'p', 'e' };
            if (!file.TryInitializeDetect(cs, 9, ref current, out var answer, out expression.ScenarioVariant))
                goto RETURN;
            answer = file.TryReadIdentifiers(ref tempData.Identifiers, current, out expression.Page, out expression.Start, out expression.Length, proc.allocator);
            if (!answer)
                goto RETURN;
            current = answer.Span.CaretNextToEndOfThisSpan;
            file.SkipWhiteSpace(ref current);
            var ast = VoiceTree.Kind.voice_type.CreateASTPair();
            tempData.VoiceTypes.Add(ref expression, out ast.Page, out ast.Index, proc.allocator);
            proc.Add(ast);
        RETURN:
            return answer;
        }

        private static TryInterpretReturnValue SpotDetect(ref TextFile file, ref VoiceParserTempData tempData, ref Caret current, ref InitialProc_USING_STRUCT proc)
        {
            VoiceTree.SpotAssignExpression expression = default;
            var cs = stackalloc ushort[] { 'p', 'o', 't' };
            if (!file.TryInitializeDetect(cs, 3, ref current, out var answer, out expression.ScenarioVariant))
                goto RETURN;
            answer = file.TryReadStringsEndWithSemicolon(ref tempData.Strings, ref tempData.StringMemories, current, out expression.Page, out expression.Start, out expression.Length, proc.allocator);
            if (!answer)
                goto RETURN;
            current = answer.Span.CaretNextToEndOfThisSpan;
            file.SkipWhiteSpace(ref current);
            var ast = VoiceTree.Kind.spot.CreateASTPair();
            tempData.Spots.Add(ref expression, out ast.Page, out ast.Index, proc.allocator);
            proc.Add(ast);
        RETURN:
            return answer;
        }

        private static TryInterpretReturnValue PowerDetect(ref TextFile file, ref VoiceParserTempData tempData, ref Caret current, ref InitialProc_USING_STRUCT proc)
        {
            VoiceTree.PowerAssignExpression expression = default;
            var cs = stackalloc ushort[] { 'o', 'w', 'e', 'r' };
            if (!file.TryInitializeDetect(cs, 4, ref current, out var answer, out expression.ScenarioVariant))
                goto RETURN;
            answer = file.TryReadStringsEndWithSemicolon(ref tempData.Strings, ref tempData.StringMemories, current, out expression.Page, out expression.Start, out expression.Length, proc.allocator);
            if (!answer)
                goto RETURN;
            current = answer.Span.CaretNextToEndOfThisSpan;
            file.SkipWhiteSpace(ref current);
            var ast = VoiceTree.Kind.power.CreateASTPair();
            tempData.Powers.Add(ref expression, out ast.Page, out ast.Index, proc.allocator);
            proc.Add(ast);
        RETURN:
            return answer;
        }
    }
}