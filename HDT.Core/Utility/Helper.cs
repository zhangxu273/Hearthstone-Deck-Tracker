#region

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

#endregion

namespace HDT.Core.Utility
{
	public static class Helper
	{
		public static string DataDirectory { get; } =
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HearthstoneDeckTracker");

		public static Version AppVersion { get; } = Assembly.GetExecutingAssembly().GetName().Version;

		public static FileInfo GetFile(string fileName) => new FileInfo(Path.Combine(DataDirectory, fileName));

		public static DirectoryInfo GetDirectory(string directoryName) => new DirectoryInfo(Path.Combine(DataDirectory, directoryName));

		public static int GetExecutableBuild(Process proc) => FileVersionInfo.GetVersionInfo(proc.MainModule.FileName).FilePrivatePart;
	}
}