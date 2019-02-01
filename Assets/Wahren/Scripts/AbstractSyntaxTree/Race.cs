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
                    case 0:
                        buffer.Append("\n  ");
                        tempData.NameList[pair.Value].Append(ref script, buffer);
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
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
        }
        public struct BraveAssignExpression // 2
        {
            public Span ScenarioVariant;
            public sbyte Value;
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
        }
        public struct MoveTypeAssignExpression // 4
        {
            public Span ScenarioVariant;
            public Span Value;
        }
    }
}