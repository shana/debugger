using MDS=Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	class SdbVariable : Wrapper, IVariable
	{
		private MDS.LocalVariable localVariable { get { return obj as MDS.LocalVariable; } }

		public SdbVariable (MDS.LocalVariable variable)
			: base (variable)
		{}

		public string Name
		{
			get { return localVariable.Name; }
		}

		public ITypeMirror Type
		{
			get { return Cache.Lookup<SdbTypeMirror> (localVariable.Type); }
		}

		public bool IsArgument
		{
			get { return localVariable.IsArg; }
		}

		public object Value { get; set; }

		public override string ToString ()
		{
			//LogProvider.Log ("ValueObject: {0}", ValueObject);
			if (Value == null)
				return string.Empty;
			return Value.ToString ();
		}

		internal MDS.Value ValueObject { get; set; }
	}
}
