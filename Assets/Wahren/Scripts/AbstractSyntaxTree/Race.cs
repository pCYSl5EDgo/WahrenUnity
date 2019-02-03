using System;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct RaceTree : IDisposable
    {
        public Span Name;
        public Span ParentName;

        public ASTValueTypePairList List;

        public void Dispose()
        {
            List.Dispose();
            this = default;
        }

        public string ToString(ref ScriptLoadReturnValue script, ref RaceParserTempData tempData)
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder(1024).Append("race ").Append(script.ToString(ref Name));
            if (ParentName.Length == 0)
            {
                buffer.Append("\n{");
            }
            else
            {
                buffer.Append('@').Append(script.ToString(ref ParentName)).Append("\n{");
            }
            for (int i = 0; i < List.Length; i++)
            {
                var pair = List.Values[i];
                switch (pair.Type)
                {
                    case name:
                        buffer.Append("\n  ");
                        tempData.Names[pair.Value].Append(ref script, buffer);
                        break;
                    case align:
                        buffer.Append("\n  ");
                        tempData.Aligns[pair.Value].Append(ref script, buffer);
                        break;
                    case brave:
                        buffer.Append("\n  ");
                        tempData.Braves[pair.Value].Append(ref script, buffer);
                        break;
                    case consti:
                        buffer.Append("\n  ");
                        tempData.Constis[pair.Value].Append(ref script, buffer);
                        break;
                    case movetype:
                        buffer.Append("\n  ");
                        tempData.MoveTypes[pair.Value].Append(ref script, buffer);
                        break;
                }
            }
            return buffer.Append("\n}").ToString();
        }

        internal const int name = 0, align = 1, brave = 2, consti = 3, movetype = 4;

        public struct NameAssignExpression // 0
        {
            public Span ScenarioVariant;
            public Span Value;

            public StringBuilder Append(ref ScriptLoadReturnValue script, StringBuilder buffer)
            {
                if (ScenarioVariant.Length == 0)
                    return buffer.Append("name = ").Append(script.ToString(ref Value));
                else
                    return buffer.Append("name@").Append(script.ToString(ref ScenarioVariant)).Append(" = ").Append(script.ToString(ref Value));
            }
        }
        public struct AlignAssignExpression // 1
        {
            public Span ScenarioVariant;
            public sbyte Value;

            public StringBuilder Append(ref ScriptLoadReturnValue script, StringBuilder buffer)
            {
                if (ScenarioVariant.Length == 0)
                    buffer.Append("align = ");
                else
                    buffer.Append("align@").Append(script.ToString(ref ScenarioVariant)).Append(" = ");
                return buffer.Append(Value);
            }
        }
        public struct BraveAssignExpression // 2
        {
            public Span ScenarioVariant;
            public sbyte Value;

            public StringBuilder Append(ref ScriptLoadReturnValue script, StringBuilder buffer)
            {
                if (ScenarioVariant.Length == 0)
                    buffer.Append("brave = ");
                else
                    buffer.Append("brave@").Append(script.ToString(ref ScenarioVariant)).Append(" = ");
                return buffer.Append(Value);
            }
        }
        public unsafe struct ConstiAssignExpression : IDisposable // 3
        {
            public Span ScenarioVariant;
            public int Length;
            public IdentifierNumberPair* Pairs;
            public void Dispose()
            {
                if (Length != 0)
                {
                    UnsafeUtility.Free(Pairs, Allocator.Persistent);
                    Pairs = null;
                }
            }

            public StringBuilder Append(ref ScriptLoadReturnValue script, StringBuilder buffer)
            {
                if (ScenarioVariant.Length == 0)
                    buffer.Append("consti = ");
                else
                    buffer.Append("consti@").Append(script.ToString(ref ScenarioVariant)).Append(" = ");
                if (Length == 0)
                    return buffer.Append('@');
                buffer.Append(script.ToString(ref Pairs[0].Span)).Append('*').Append(Pairs[0].Number);
                for (int i = 1; i < Length; i++)
                {
                    buffer.Append(", ").Append(script.ToString(ref Pairs[i].Span)).Append('*').Append(Pairs[i].Number);
                }
                return buffer;
            }
        }
        public struct MoveTypeAssignExpression // 4
        {
            public Span ScenarioVariant;
            public Span Value;

            public StringBuilder Append(ref ScriptLoadReturnValue script, StringBuilder buffer)
            {
                if (ScenarioVariant.Length == 0)
                    return buffer.Append("movetype = ").Append(script.ToString(ref Value));
                else
                    return buffer.Append("movetype@").Append(script.ToString(ref ScenarioVariant)).Append(" = ").Append(script.ToString(ref Value));
            }
        }
    }
}