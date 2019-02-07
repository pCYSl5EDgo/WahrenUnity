using System;
using System.Text;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren
{
    public static unsafe class StringBuilderExtensionUtility
    {
        public static StringBuilder Append(this StringBuilder buffer, in TextFile file, Span span)
        => buffer.Append((char*)file.Contents + file.LineStarts[span.Line] + span.Column, span.Length);
        public static StringBuilder Append(this StringBuilder buffer, in AST.ScriptAnalyzeDataManager script, Span span)
        => buffer.Append(script[span.File], span);
        public static StringBuilder Append(this StringBuilder buffer, TextFile* files, Span span)
        => buffer.Append(files[span.File], span);

        public static StringBuilder Append(this StringBuilder buffer, TextFile* files, in AST.RaceTree.AlignAssignExpression expression)
        {
            buffer.Append("align");
            if (expression.ScenarioVariant.Length != 0)
                buffer.Append('@').Append(files, expression.ScenarioVariant);
            return buffer.Append(" = ").Append(expression.Value);
        }

        public static StringBuilder Append(this StringBuilder buffer, TextFile* files, in AST.RaceTree.BraveAssignExpression expression)
        {
            buffer.Append("brave");
            if (expression.ScenarioVariant.Length != 0)
                buffer.Append('@').Append(files, expression.ScenarioVariant);
            return buffer.Append(" = ").Append(expression.Value);
        }

        public static StringBuilder Append(this StringBuilder buffer, TextFile* files, in AST.RaceTree.MoveTypeAssignExpression expression)
        {
            buffer.Append("movetype");
            if (expression.ScenarioVariant.Length != 0)
                buffer.Append('@').Append(files, expression.ScenarioVariant);
            buffer.Append(" = ");
            if (expression.Value.Length == 0)
                return buffer.Append('@');
            return buffer.Append(files, expression.Value);
        }

        public static StringBuilder Append(this StringBuilder buffer, TextFile* files, in AST.RaceTree.NameAssignExpression expression)
        {
            buffer.Append("name");
            if (expression.ScenarioVariant.Length != 0)
                buffer.Append('@').Append(files, expression.ScenarioVariant);
            buffer.Append(" = ");
            if (expression.Value.Length == 0)
                return buffer.Append('@');
            return buffer.Append(files, expression.Value);
        }

        public static StringBuilder Append(this StringBuilder buffer, TextFile* files, in AST.RaceTree.ConstiAssignExpression expression, in IdentifierNumberPairList list)
        {
            buffer.Append("consti");
            if (expression.ScenarioVariant.Length != 0)
                buffer.Append('@').Append(files, expression.ScenarioVariant);
            buffer.Append(" = ");
            if (expression.Length == 0)
                return buffer.Append('@');
            buffer.Append(files, list.Values[0].Span).Append('*').Append(list.Values[0].Number);
            for (int i = expression.Start + 1, end = expression.Start + expression.Length; i < end; i++)
                buffer.Append(", ").Append(files, list.Values[i].Span).Append('*').Append(list.Values[i].Number);
            return buffer;
        }

        public static unsafe StringBuilder Append(this StringBuilder buffer, in AST.RaceTree tree, TextFile* files, in AST.RaceParserTempData tempData, in IdentifierNumberPairList list, in AST.ASTValueTypePairList astValueTypePairList)
        {
            buffer.Append("race ").Append(files, tree.Name);
            if (tree.ParentName.Length == 0)
            {
                buffer.Append("\n{");
            }
            else
            {
                buffer.Append('@').Append(files, tree.ParentName).Append("\n{");
            }
            for (int i = tree.Start, end = tree.Start + tree.Length; i < end; i++)
            {
                var pair = astValueTypePairList.Values[i];
                buffer.Append("\n  ");
                switch (pair.Type)
                {
                    case AST.RaceTree.name:
                        buffer.Append(files, tempData.Names[pair.Value]);
                        break;
                    case AST.RaceTree.align:
                        buffer.Append(files, tempData.Aligns[pair.Value]);
                        break;
                    case AST.RaceTree.brave:
                        buffer.Append(files, tempData.Braves[pair.Value]);
                        break;
                    case AST.RaceTree.consti:
                        buffer.Append(files, tempData.Constis[pair.Value], list);
                        break;
                    case AST.RaceTree.movetype:
                        buffer.Append(files, tempData.MoveTypes[pair.Value]);
                        break;
                }
            }
            return buffer.Append("\n}");
        }

        public static StringBuilder Append(this StringBuilder buffer, in TryInterpretReturnValue value, AST.ScriptAnalyzeDataManager script)
        {
            if (value.Status == InterpreterStatus.Error)
            {
                buffer.Append("Error - ").AppendLine(ErrorSentence.Contents[value.DataIndex]);
                if (ErrorSentence.SubContents[value.DataIndex] != null && ErrorSentence.SubContents[value.DataIndex].Length != 0)
                    buffer.AppendLine(ErrorSentence.SubContents[value.DataIndex][value.SubDataIndex]);
            }
            else if (value.Status == InterpreterStatus.Pending)
            {
                buffer.AppendLine("Pending");
            }
            buffer.Append("at (").Append(script.FullPaths[value.Span.File]).Append(':').Append(value.Span.Line + 1).Append('(').Append(value.Span.Column + 1);
            if (value.Span.Length > 1)
                buffer.Append('-').Append(value.Span.Column + 1 + value.Span.Length);
            return buffer.Append(")\n").Append(script, value.Span);
        }
    }
}