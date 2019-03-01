using System;
using System.Text;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public static unsafe class StringBuilderExtensionUtility
    {
        internal static StringBuilder AppendPrimitive(this StringBuilder buffer, in TextFile file, Span span)
        {
            if (span.Length == 0)
            {
                return buffer.Append('@');
            }
            return buffer.Append((char*)file.CurrentCharPointer(span.Start), span.Length);
        }
        internal static StringBuilder AppendPrimitive(this StringBuilder buffer, in ScriptAnalyzeDataManager script, Span span)
        => buffer.AppendPrimitive(script[span.File], span);
        internal static StringBuilder AppendPrimitive(this StringBuilder buffer, TextFile* files, Span span) => buffer.AppendPrimitive(files[span.File], span);

        private static StringBuilder AppendExtension(this StringBuilder buffer, TextFile* files, string sectionName, Span scenarioVariant, Span value)
        {
            buffer.Append(sectionName);
            if (scenarioVariant.Length != 0)
            {
                buffer.Append('@').AppendPrimitive(files, scenarioVariant);
            }
            buffer.Append(" = ");
            if (value.Length == 0)
            {
                buffer.Append('@');
            }
            else
            {
                buffer.AppendPrimitive(files, value);
            }
            return buffer;
        }

        private static StringBuilder AppendExtension(this StringBuilder buffer, TextFile* files, string sectionName, Span scenarioVariant, long value)
        {
            buffer.Append(sectionName);
            if (scenarioVariant.Length != 0)
            {
                buffer.Append('@').AppendPrimitive(files, scenarioVariant);
            }
            return buffer.Append(" = ").Append(value);
        }

        private static StringBuilder AppendExtension(this StringBuilder buffer, TextFile* files, string sectionName, Span scenarioVariant, in IdentifierNumberPairList list, int start, int length)
        {
            buffer.Append(sectionName);
            if (scenarioVariant.Length != 0)
                buffer.Append('@').AppendPrimitive(files, scenarioVariant);
            buffer.Append(" = ");
            if (length == 0)
                return buffer.Append('@');
            buffer.AppendPrimitive(files, list.This.Values[0].Span).Append('*').Append(list.This.Values[0].Number);
            for (int i = start + 1, end = start + length; i < end; i++)
                buffer.Append(", ").AppendPrimitive(files, list.This.Values[i].Span).Append('*').Append(list.This.Values[i].Number);
            return buffer;
        }

        private static StringBuilder AppendExtension(this StringBuilder buffer, TextFile* files, string sectionName, Span scenarioVariant, IdentifierNumberPairList* page, int start, int length)
        {
            buffer.Append(sectionName);
            if (scenarioVariant.Length != 0)
                buffer.Append('@').AppendPrimitive(files, scenarioVariant);
            buffer.Append(" = ");
            if (length == 0)
                return buffer.Append('@');
            var listPtr = page->GetPointer(start);
            buffer.AppendPrimitive(files, listPtr[0].Span).Append('*').Append(listPtr[0].Number);
            for (int i = 1; i < length; i++)
                buffer.Append(", ").AppendPrimitive(files, listPtr[i].Span).Append('*').Append(listPtr[i].Number);
            return buffer;
        }

        private static StringBuilder Append(this StringBuilder buffer, TextFile* files, in RaceTree.AlignAssignExpression expression)
        => buffer.AppendExtension(files, "align", expression.ScenarioVariant, expression.Value);

        private static StringBuilder Append(this StringBuilder buffer, TextFile* files, in RaceTree.BraveAssignExpression expression)
        => buffer.AppendExtension(files, "brave", expression.ScenarioVariant, expression.Value);

        private static StringBuilder Append(this StringBuilder buffer, TextFile* files, in RaceTree.MovetypeAssignExpression expression)
        => buffer.AppendExtension(files, "movetype", expression.ScenarioVariant, expression.Value);

        private static StringBuilder Append(this StringBuilder buffer, TextFile* files, in RaceTree.NameAssignExpression expression)
        => buffer.AppendExtension(files, "name", expression.ScenarioVariant, expression.Value);

        private static StringBuilder Append(this StringBuilder buffer, TextFile* files, in RaceTree.ConstiAssignExpression expression)
        => buffer.AppendExtension(files, "consti", expression.ScenarioVariant, expression.Page, expression.Start, expression.Length);

        public static StringBuilder AppendEx(this StringBuilder buffer, in RaceTree tree, TextFile* files, in RaceParserTempData tempData)
        {
            using (new ClosingStruct(buffer, files, "race", tree.Name, tree.ParentName))
            {
                for (int i = tree.Start, end = tree.Start + tree.Length; i < end; i++)
                {
                    ref var pair = ref tree.Page->GetRef(i);
                    buffer.Append("\n  ");
                    switch ((RaceTree.Kind)pair.Type)
                    {
                        case RaceTree.Kind.name:
                            buffer.Append(files, pair.GetRef<RaceTree.NameAssignExpression>());
                            break;
                        case RaceTree.Kind.align:
                            buffer.Append(files, pair.GetRef<RaceTree.AlignAssignExpression>());
                            break;
                        case RaceTree.Kind.brave:
                            buffer.Append(files, pair.GetRef<RaceTree.BraveAssignExpression>());
                            break;
                        case RaceTree.Kind.consti:
                            buffer.Append(files, pair.GetRef<RaceTree.ConstiAssignExpression>());
                            break;
                        case RaceTree.Kind.movetype:
                            buffer.Append(files, pair.GetRef<RaceTree.MovetypeAssignExpression>());
                            break;
                    }
                }
            }
            return buffer;
        }

        private static StringBuilder Append(this StringBuilder buffer, TextFile* files, MovetypeTree.NameAssignExpression expression)
        => buffer.AppendExtension(files, "name", expression.ScenarioVariant, expression.Value);

        private static StringBuilder Append(this StringBuilder buffer, TextFile* files, MovetypeTree.HelpAssignExpression expression)
        => buffer.AppendExtension(files, "help", expression.ScenarioVariant, expression.Value);

        private static StringBuilder Append(this StringBuilder buffer, TextFile* files, MovetypeTree.ConstiAssignExpression expression)
        => buffer.AppendExtension(files, "consti", expression.ScenarioVariant, (IdentifierNumberPairList*)expression.Page, expression.Start, expression.Length);

        public static StringBuilder AppendEx(this StringBuilder buffer, in MovetypeTree tree, TextFile* files, in MovetypeParserTempData tempData)
        {
            using (new ClosingStruct(buffer, files, "movetype", tree.Name, tree.ParentName))
            {
                for (int i = tree.Start, end = tree.Start + tree.Length; i < end; i++)
                {
                    var pair = tree.Page->GetRef(i);
                    buffer.Append("\n  ");
                    switch ((MovetypeTree.Kind)pair.Type)
                    {
                        case MovetypeTree.Kind.name:
                            buffer.Append(files, pair.GetRef<MovetypeTree.NameAssignExpression>());
                            break;
                        case MovetypeTree.Kind.consti:
                            buffer.Append(files, pair.GetRef<MovetypeTree.ConstiAssignExpression>());
                            break;
                        case MovetypeTree.Kind.help:
                            buffer.Append(files, pair.GetRef<MovetypeTree.HelpAssignExpression>());
                            break;
                    }
                }
            }
            return buffer;
        }

        public static StringBuilder Append(this StringBuilder buffer, in TryInterpretReturnValue value, ScriptAnalyzeDataManager script)
        {
            if (value.Status == InterpreterStatus.Error)
            {
                buffer.Append("Error - ").AppendLine(ErrorSentence.Contents[(ErrorSentence.Kind)value.DataIndex]);
            }
            buffer.Append("at (").Append(script.FullPaths[value.Span.File]).Append(':').Append(value.Span.Line + 1).Append('(').Append(value.Span.Column + 1);
            if (value.Span.Length > 1)
                buffer.Append('-').Append(value.Span.Column + 1 + value.Span.Length);
            return buffer.Append(")\n").AppendPrimitive(script, value.Span);
        }
        struct ClosingStruct : IDisposable
        {
            public unsafe ClosingStruct(StringBuilder buffer, TextFile* files, string structKind, Span structName, Span structParentName)
            {
                this.buffer = buffer.Append(structKind).Append(' ').AppendPrimitive(files, structName);
                if (structParentName.Length == 0)
                {
                    buffer.Append("\n{");
                }
                else
                {
                    buffer.Append(" : ").AppendPrimitive(files, structParentName).Append("\n{");
                }
            }
            StringBuilder buffer;

            public void Dispose()
            {
                buffer.Append("\n}");
            }
        }
    }

    internal static class FromStringHelper
    {
        public static unsafe ushort* ToNativeArray(this string text, Allocator allocator)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException();
            var answer = (ushort*)UnsafeUtility.Malloc(sizeof(ushort) * text.Length, 2, allocator);
            fixed (char* cptr = text)
            {
                UnsafeUtility.MemCpy(answer, cptr, text.Length * 2);
            }
            return answer;
        }
    }
}