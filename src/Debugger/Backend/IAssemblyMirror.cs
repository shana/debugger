using Mono.Cecil;

namespace Debugger.Backend
{
	public interface IAssemblyMirror : IWrapper
	{
		string FullName { get; }
		string Path { get; }
		AssemblyDefinition Metadata { get; set; }
	}
}
