using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Mono.Cecil;
using MDS=Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	public class SdbStackFrame : Wrapper, IStackFrame
	{
		private MDS.StackFrame stackframe { get { return obj as MDS.StackFrame; } }

		public SdbStackFrame (MDS.StackFrame obj)
			: base (obj)
		{
		}

		public IThreadMirror Thread
		{
			get { return Cache.Lookup<SdbThreadMirror> (stackframe.Thread); }
		}

		public IMethodMirror Method
		{
			get { return Cache.Lookup<SdbMethodMirror> (stackframe.Method); }
		}

		public int ILOffset { get { return stackframe.ILOffset; } }

		public ILocation Location
		{
			get { return Cache.Lookup<SdbLocation> (stackframe.Location); }
		}

		public IList<IVariable> Locals
		{
			get { return stackframe.Method.GetLocals ().Select (x => new SdbVariable (x)).ToArray (); }
		}

		public IList<IVariable> VisibleVariables 
		{
			get { return stackframe.GetVisibleVariables ().Select (x => new SdbVariable (x)).ToArray (); }
		}

		public object GetValue (IVariable variable)
		{
			SdbVariable val = variable as SdbVariable;
			val.ValueObject = stackframe.GetValue (variable.Unwrap<MDS.LocalVariable> ());
			//LogProvider.Log ("GetValue {0}", val.ValueObject);
			if (val.ValueObject is MDS.PrimitiveValue)
				val.Value = ((MDS.PrimitiveValue)val.ValueObject).Value;
			if (val.ValueObject is MDS.StringMirror)
				val.Value = ((MDS.StringMirror)val.ValueObject).Value;
			if (val.ValueObject is MDS.EnumMirror)
				val.Value = ((MDS.EnumMirror)val.ValueObject).Value;
//			if (val is MDS.ObjectMirror) {
//				return ((MDS.ObjectMirror)val).InvokeMethod (Thread.Unwrap<MDS.ThreadMirror> (), , new List<MDS.Value> ());
//			}
			return val.Value;
		}
	}

	abstract class SdbValue : Wrapper
	{
		private MDS.Value val { get { return obj as MDS.Value; } }

		public SdbValue (MDS.Value value) :
			base (value)
		{}

		public object GetValue (FieldDefinition def)
		{
			
			if (obj is MDS.StringMirror)
				return ((MDS.StringMirror)obj).Value;
			if (obj is MDS.EnumMirror)
				return ((MDS.EnumMirror)obj).Value;
			return null;
		}

		public object Value { get { return null; }}
	}
}
