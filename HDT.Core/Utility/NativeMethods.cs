using System;
using System.Runtime.InteropServices;

namespace HDT.Core.Utility
{
	public class NativeMethods
	{
		[DllImport("user32.dll")]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
	}
}
