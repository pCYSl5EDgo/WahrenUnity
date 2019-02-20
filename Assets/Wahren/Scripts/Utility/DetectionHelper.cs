using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public static unsafe class DetectionHelper
    {
        public static bool TryInitializeDetect(this ref TextFile file, ushort* input, int inputLength, ref Caret current, out TryInterpretReturnValue answer, out Span scenarioVariant)
        {
            var cs = file.CurrentCharPointer(current) + 1;
            answer = new TryInterpretReturnValue(new Span(current, 1), ErrorSentence.InvalidIdentifierError, 0, InterpreterStatus.Error);
            var thisLineLength = file.CurrentLineLength(current);
            scenarioVariant = default;
            if (current.Column + inputLength >= thisLineLength || UnsafeUtility.MemCmp(input, cs, sizeof(ushort) * inputLength) != 0)
                return false;
            current.Column += inputLength + 1;
            if (current.Column < thisLineLength && cs[inputLength] == '@')
            {
                if (!file.TryGetScenarioVariantName(current, out answer))
                    return false;
                scenarioVariant = answer.Span;
                current = answer.Span.CaretNextToEndOfThisSpan;
            }
            file.SkipWhiteSpace(ref current);
            if (file.CurrentChar(current) != '=')
            {
                answer.DataIndex = ErrorSentence.ExpectedCharNotFoundError;
                answer.Status = InterpreterStatus.Error;
                return false;
            }
            current.Column++;
            file.SkipWhiteSpace(ref current);
            return true;
        }
    }
}