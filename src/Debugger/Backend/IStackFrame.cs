using System.Collections.Generic;

namespace Debugger.Backend
{
	public interface IStackFrame : IWrapper
	{
		IThreadMirror Thread { get; }
		IMethodMirror Method { get; }
		int ILOffset { get; }
		ILocation Location { get; }
		IList<IVariable> Locals { get; }
		IList<IVariable> VisibleVariables { get; }
		object GetValue (IVariable variable);
	}
}
