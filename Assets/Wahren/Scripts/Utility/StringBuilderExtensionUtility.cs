using System;
using System.Text;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public static unsafe class StringBuilderExtensionUtility
    {
        private static StringBuilder AppendPrimitive(this StringBuilder buffer, in TextFile file, Span span)
        => buffer.Append((char*)file.Contents + file.LineStarts[span.Line] + span.Column, span.Length);
        private static StringBuilder AppendPrimitive(this StringBuilder buffer, in ScriptAnalyzeDataManager script, Span span)
        => buffer.AppendPrimitive(script[span.File], span);
        private static StringBuilder AppendPrimitive(this StringBuilder buffer, TextFile* files, Span span)
        => buffer.AppendPrimitive(files[span.File], span);

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
            buffer.AppendPrimitive(files, list.Values[0].Span).Append('*').Append(list.Values[0].Number);
            for (int i = start + 1, end = start + length; i < end; i++)
                buffer.Append(", ").AppendPrimitive(files, list.Values[i].Span).Append('*').Append(list.Values[i].Number);
            return buffer;
        }

        private static StringBuilder AppendHeader(this StringBuilder buffer, TextFile* files, string structKind, Span structName, Span structParentName)
        {
            buffer.Append(structKind).Append(' ').AppendPrimitive(files, structName);
            if (structParentName.Length == 0)
            {
                return buffer.Append("\n{");
            }
            else
            {
                return buffer.Append(" : ").AppendPrimitive(files, structParentName).Append("\n{");
            }
        }

        private static StringBuilder Append(this StringBuilder buffer, TextFile* files, in RaceTree.AlignAssignExpression expression)
        => buffer.AppendExtension(files, "align", expression.ScenarioVariant, expression.Value);

        private static StringBuilder Append(this StringBuilder buffer, TextFile* files, in RaceTree.BraveAssignExpression expression)
        => buffer.AppendExtension(files, "brave", expression.ScenarioVariant, expression.Value);

        private static StringBuilder Append(this StringBuilder buffer, TextFile* files, in RaceTree.MoveTypeAssignExpression expression)
        => buffer.AppendExtension(files, "movetype", expression.ScenarioVariant, expression.Value);

        private static StringBuilder Append(this StringBuilder buffer, TextFile* files, in RaceTree.NameAssignExpression expression)
        => buffer.AppendExtension(files, "name", expression.ScenarioVariant, expression.Value);

        private static StringBuilder Append(this StringBuilder buffer, TextFile* files, in RaceTree.ConstiAssignExpression expression, in IdentifierNumberPairList list)
        => buffer.AppendExtension(files, "consti", expression.ScenarioVariant, list, expression.Start, expression.Length);

        public static StringBuilder AppendEx(this StringBuilder buffer, in RaceTree tree, TextFile* files, in RaceParserTempData tempData, in ASTValueTypePairList astValueTypePairList)
        {
            buffer.AppendHeader(files, "race", tree.Name, tree.ParentName);
            for (int i = tree.Start, end = tree.Start + tree.Length; i < end; i++)
            {
                var pair = astValueTypePairList.Values[i];
                buffer.Append("\n  ");
                switch (pair.Type)
                {
                    case RaceTree.name:
                        buffer.Append(files, tempData.Names[pair.Value]);
                        break;
                    case RaceTree.align:
                        buffer.Append(files, tempData.Aligns[pair.Value]);
                        break;
                    case RaceTree.brave:
                        buffer.Append(files, tempData.Braves[pair.Value]);
                        break;
                    case RaceTree.consti:
                        buffer.Append(files, tempData.Constis[pair.Value], tempData.IdentifierNumberPairs);
                        break;
                    case RaceTree.movetype:
                        buffer.Append(files, tempData.MoveTypes[pair.Value]);
                        break;
                }
            }
            return buffer.Append("\n}");
        }

        private static StringBuilder Append(this StringBuilder buffer, TextFile* files, MoveTypeTree.NameAssignExpression expression)
        => buffer.AppendExtension(files, "name", expression.ScenarioVariant, expression.Value);

        private static StringBuilder Append(this StringBuilder buffer, TextFile* files, MoveTypeTree.HelpAssignExpression expression)
        => buffer.AppendExtension(files, "help", expression.ScenarioVariant, expression.Value);

        private static StringBuilder Append(this StringBuilder buffer, TextFile* files, MoveTypeTree.ConstiAssignExpression expression, in IdentifierNumberPairList list)
        => buffer.AppendExtension(files, "consti", expression.ScenarioVariant, list, expression.Start, expression.Length);

        public static StringBuilder AppendEx(this StringBuilder buffer, in MoveTypeTree tree, TextFile* files, in MovetypeParserTempData tempData, in ASTValueTypePairList astValueTypePairList)
        {
            buffer.AppendHeader(files, "movetype", tree.Name, tree.ParentName);
            for (int i = tree.Start, end = tree.Start + tree.Length; i < end; i++)
            {
                var pair = astValueTypePairList.Values[i];
                buffer.Append("\n  ");
                switch (pair.Type)
                {
                    case MoveTypeTree.name:
                        buffer.Append(files, tempData.Names[pair.Value]);
                        break;
                    case MoveTypeTree.consti:
                        buffer.Append(files, tempData.Constis[pair.Value], tempData.IdentifierNumberPairs);
                        break;
                    case MoveTypeTree.help:
                        buffer.Append(files, tempData.Helps[pair.Value]);
                        break;
                }
            }
            return buffer.Append("\n}");
        }

        public static StringBuilder Append(this StringBuilder buffer, in TryInterpretReturnValue value, ScriptAnalyzeDataManager script)
        {
            if (value.Status == InterpreterStatus.Error)
            {
                buffer.Append("Error - ").AppendLine(ErrorSentence.Contents[value.DataIndex]);
                var subs = ErrorSentence.SubContents[value.DataIndex];
                if (subs != null && subs.Length != 0)
                    buffer.AppendLine(subs[value.SubDataIndex]);
            }
            else if (value.Status == InterpreterStatus.Pending)
            {
                buffer.AppendLine("Pending");
            }
            buffer.Append("at (").Append(script.FullPaths[value.Span.File]).Append(':').Append(value.Span.Line + 1).Append('(').Append(value.Span.Column + 1);
            if (value.Span.Length > 1)
                buffer.Append('-').Append(value.Span.Column + 1 + value.Span.Length);
            return buffer.Append(")\n").AppendPrimitive(script, value.Span);
        }
    }
}