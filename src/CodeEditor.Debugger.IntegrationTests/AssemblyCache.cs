using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace CodeEditor.Debugger.IntegrationTests
{
	public static class AssemblyCache
	{
		public static string GetCacheFolder(string fixtureName)
		{
			var cacheFolder = Path.Combine(Path.GetTempPath(), "cil2as", fixtureName);
			Directory.CreateDirectory(cacheFolder);
			return cacheFolder;
		}

		private static string MD5String(string text)
		{
			var md5 = MD5.Create();
			var inputBytes = Encoding.UTF8.GetBytes(text);
			var hashBytes = md5.ComputeHash(inputBytes);

			var sb = new StringBuilder();
			for (int i = 0; i < hashBytes.Length; i++)
				sb.Append(hashBytes[i].ToString("x2"));
			return sb.ToString();
		}

		public static string GetCacheKey(string fixtureName, string assemblyName, CachedAssemblyInfo info)
		{
			var cacheFolder = GetCacheFolder(fixtureName);

			var infoText = info.ToString();
			var infoMD5 = MD5String(infoText);

			var subFolder = Path.Combine(cacheFolder, infoMD5);
			Directory.CreateDirectory(subFolder);

			var assemblyNameShort = Path.GetFileNameWithoutExtension(assemblyName);
			return Path.Combine(subFolder, assemblyNameShort);
		}

		public static bool CheckCachedAssembly(string fixtureName, string assemblyName, CachedAssemblyInfo info)
		{
			var xmlPath = XmlCachePathFor(fixtureName, assemblyName, info);
			if (!File.Exists(xmlPath))
				return false;
			var cached = XmlRead(xmlPath);
			return info.Equals(cached);
		}

		public static void SaveCachedAssembly(string fixtureName, string assemblyName, CachedAssemblyInfo info)
		{
			var xmlPath = XmlCachePathFor(fixtureName, assemblyName, info);
			XmlWrite(xmlPath, info);
		}

		private static string XmlCachePathFor(string fixtureName, string assemblyName, CachedAssemblyInfo info)
		{
			return GetCacheKey(fixtureName, assemblyName, info) + ".xml";
		}

		private static void XmlWrite(string path, CachedAssemblyInfo info)
		{
			var serializer = new XmlSerializer(info.GetType());
			using (var s = File.Open(path, FileMode.Create, FileAccess.Write))
				serializer.Serialize(s, info);
		}

		private static CachedAssemblyInfo XmlRead(string path)
		{
			var serializer = new XmlSerializer(typeof(CachedAssemblyInfo));
			using (var s = File.OpenRead(path))
				return serializer.Deserialize(s) as CachedAssemblyInfo;
		}
	}

	[Serializable]
	public struct CachedFileInfo
	{
		public string Path;
		public long Size;
		public DateTime LastModified;
		public bool Exists;

		public CachedFileInfo(string existingFilePath)
		{
			Path = System.IO.Path.GetFullPath(existingFilePath).ToLowerInvariant();

			try
			{
				var fi = new FileInfo(existingFilePath);
				Size = fi.Length;
				LastModified = fi.LastWriteTimeUtc;
				Exists = true;
			}
			catch (Exception)
			{
				Size = 0;
				LastModified = DateTime.MinValue;
				Exists = false;
			}
		}

		public bool Equals(CachedFileInfo rhs)
		{
			var deltaSeconds = Math.Abs((LastModified - rhs.LastModified).TotalSeconds);

			return Path == rhs.Path
				&& Size == rhs.Size
				&& deltaSeconds <= 1.0
				&& Exists == rhs.Exists;
		}

		public override bool Equals(object obj)
		{
			if (obj is CachedFileInfo)
				return Equals((CachedFileInfo)obj);
			return base.Equals(obj);
		}

		public override string ToString()
		{
			return Path;
		}
	}

	[Serializable]
	public class CachedAssemblyInfo
	{
		public CachedFileInfo[] SourceFiles;
		public string[] References;

		private CachedAssemblyInfo()
		{
		}

		public CachedAssemblyInfo(string[] sourceFiles, string[] references)
		{
			SourceFiles = sourceFiles.Select(fn => new CachedFileInfo(fn)).ToArray();
			References = references;
		}

		// Checks to see if the output path and source files all match exactly.
		// Note that the actual output isn't compared.
		public bool Equals(CachedAssemblyInfo rhs)
		{
			if ((References == null) || (rhs.References == null))
				return false;
			if ((SourceFiles == null) || (rhs.SourceFiles == null))
				return false;

			return SourceFiles.Length == rhs.SourceFiles.Length
				&& SourceFiles.OrderBy(sf => sf.Path)
					.SequenceEqual(rhs.SourceFiles.OrderBy(sf => sf.Path))
				&& References.Length == rhs.References.Length
				&& References.OrderBy(r => r)
					.SequenceEqual(rhs.References.OrderBy(r => r));
		}

		public override bool Equals(object obj)
		{
			var rhs = obj as CachedAssemblyInfo;
			return rhs != null ? Equals(rhs) : base.Equals(obj);
		}

		public override string ToString()
		{
			var sb = new StringBuilder();

			sb.AppendLine("Generated from inputs");
			foreach (var sourceFile in SourceFiles.OrderBy(sf => sf.Path))
				sb.AppendLine(sourceFile.ToString());

			sb.AppendLine("With references");
			foreach (var reference in References)
				sb.AppendLine(reference);

			return sb.ToString();
		}
	}
}
