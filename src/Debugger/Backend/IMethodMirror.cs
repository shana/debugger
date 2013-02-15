using System.Collections.Generic;

namespace Debugger.Backend
{
	public interface IMethodMirror : IWrapper
	{
		IList<ILocation> Locations { get; }
		string FullName { get; }
		ITypeMirror DeclaringType { get; }
		ITypeMirror ReturnType { get; }
		string Name { get; }
	}
}
