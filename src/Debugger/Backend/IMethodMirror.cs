using System.Collections.Generic;

namespace Debugger.Backend
{
	public interface IMethodMirror : IWrapper
	{
		IEnumerable<ILocation> Locations { get; }
		string FullName { get; }
		ITypeMirror DeclaringType { get; }
		string Name { get; }
	}
}
