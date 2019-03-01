using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct VoiceParserTempData
    {
        public ListLinkedList Values;
        public ListLinkedList Powers;
        public ListLinkedList Spots;
        public ListLinkedList VoiceTypes;
        public ListLinkedList Delskills;
        public IdentifierListLinkedList Identifiers;
        public NativeStringListLinkedList Strings;
        public NativeStringMemorySoyrceLinkedList StringMemories;

        public VoiceParserTempData(int capacity, int stringCapacity, Allocator allocator)
        {
            this = default;
            if (capacity != 0)
            {
                Values = new ListLinkedList(capacity, sizeof(VoiceTree), allocator);
                Powers = new ListLinkedList(capacity, sizeof(VoiceTree.PowerAssignExpression), allocator);
                Spots = new ListLinkedList(capacity, sizeof(VoiceTree.SpotAssignExpression), allocator);
                VoiceTypes = new ListLinkedList(capacity, sizeof(VoiceTree.VoiceTypeAssignExpression), allocator);
                Delskills = new ListLinkedList(capacity, sizeof(VoiceTree.DelskillAssignExpression), allocator);
                Identifiers = new IdentifierListLinkedList(capacity, allocator);
                Strings = new NativeStringListLinkedList(capacity, allocator);
                StringMemories = new NativeStringMemorySoyrceLinkedList(stringCapacity, allocator);
            }
        }

        public void Dispose(Allocator allocator)
        {
            Values.Dispose(allocator);
            Powers.Dispose(allocator);
            Spots.Dispose(allocator);
            VoiceTypes.Dispose(allocator);
            Delskills.Dispose(allocator);
            Identifiers.Dispose(allocator);
            Strings.Dispose(allocator);
            StringMemories.Dispose(allocator);
            this = default;
        }
    }
}