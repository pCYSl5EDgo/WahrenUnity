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

        public int Length;
        public ASTValueTypePair* Contents;

        public void Dispose()
        {
            if (Length != 0)
            {
                UnsafeUtility.Free(Contents, Allocator.Persistent);
                Contents = null;
                Length = 0;
            }
        }

        public void CopyFrom(Unity.Collections.NativeList<ASTValueTypePair> list)
        {
            if (Length != 0) return;
            Length = list.Length;
            Contents = (ASTValueTypePair*)UnsafeUtility.Malloc(sizeof(ASTValueTypePair) * list.Length, 4, Allocator.Persistent);
            UnsafeUtility.MemCpy(Contents, list.GetUnsafePtr(), sizeof(ASTValueTypePair) * Length);
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
            for (int i = 0; i < Length; i++)
            {
                var pair = Contents[i];
                switch (pair.Type)
                {
                    case name:
                        buffer.Append("\n  ");
                        tempData.NameList[pair.Value].Append(ref script, buffer);
                        break;
                    case align:
                        buffer.Append("\n  ");
                        tempData.AlignList[pair.Value].Append(ref script, buffer);
                        break;
                    case brave:
                        buffer.Append("\n  ");
                        tempData.BraveList[pair.Value].Append(ref script, buffer);
                        break;
                    case consti:
                        buffer.Append("\n  ");
                        tempData.ConstiList[pair.Value].Append(ref script, buffer);
                        break;
                    case movetype:
                        buffer.Append("\n  ");
                        tempData.MoveTypeList[pair.Value].Append(ref script, buffer);
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
            public Span* Attributes;
            public sbyte* Values;
            public void Dispose()
            {
                if (Length != 0)
                {
                    UnsafeUtility.Free(Attributes, Allocator.Persistent);
                    UnsafeUtility.Free(Values, Allocator.Persistent);
                    Attributes = null;
                    Values = null;
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
                buffer.Append(Values[0]).Append('*').Append(Attributes[0]);
                for (int i = 1; i < Length; i++)
                {
                    buffer.Append(", ").Append(Values[i]).Append('*').Append(Attributes[i]);
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