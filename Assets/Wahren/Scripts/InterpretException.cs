using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pcysl5edgo.Wahren
{
    [System.Serializable]
    public class InterpretException : System.ApplicationException
    {
        public InterpretException() { }
        public InterpretException(ref ScriptLoadReturnValue script, ref Span span) : this(ref script, ref span, null) { }
        public InterpretException(ref ScriptLoadReturnValue script, ref Span span, string message) : base($"{script.FullPaths[span.File]} : {span.Line}({span.Column})\n{script.ToString(ref span)}" + (message is null ? "" : ('\n' + message))) { }
        public InterpretException(string message) : base(message) { }
        public InterpretException(string message, System.Exception inner) : base(message, inner) { }
        protected InterpretException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}