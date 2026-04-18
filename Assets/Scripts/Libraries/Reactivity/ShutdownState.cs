using UnityEngine;

namespace Reactivity
{
	public static class ShutdownState
	{
		public static bool IsQuitting { get; private set; }

		[RuntimeInitializeOnLoadMethod]
		static void Init()
		{
			Application.quitting += () => IsQuitting = true;
		}
	}
}