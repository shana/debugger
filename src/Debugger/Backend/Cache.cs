using System;
using System.Linq;
using System.Collections.Generic;

namespace Debugger.Backend.Sdb
{
	public static class Cache
	{
		static Dictionary<WeakReference, WeakReference> cachedMirrors = new Dictionary<WeakReference, WeakReference> ();

		public static T Lookup<T> (object native, params object[] args)
			where T : Wrapper
		{
			var ret = cachedMirrors.FirstOrDefault (x => x.Key.IsAlive && x.Key.Target == native);
		    if (ret.Value != null && ret.Value.IsAlive)
		        return ret.Value.Target as T;
			else if (ret.Key != null)
				cachedMirrors.Remove (ret.Key);
			var parameters = new object[args.Length + 1];
			parameters[0] = native;
			if (args.Length > 0)
				args.CopyTo (parameters, 1);
			var mirror = Activator.CreateInstance (typeof(T), parameters) as T;
			cachedMirrors.Add (new WeakReference (native), new WeakReference (mirror));
			return mirror;
		}

		public static void Clear (object native)
		{
			if (native is IWrapper)
				return;

			var list = cachedMirrors.Keys.Where (x => cachedMirrors[x].Target == native);
			foreach (var key in list)
				cachedMirrors.Remove (key);
		}

		public static void Clear (IWrapper mirror)
		{
			var list = cachedMirrors.Keys.Where (x => cachedMirrors[x].Target == mirror);
			foreach (var key in list)
				cachedMirrors.Remove (key);
		}

		public static void Clear ()
		{
			cachedMirrors.Clear ();
		}
	}
}
