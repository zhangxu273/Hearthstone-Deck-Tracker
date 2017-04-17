#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using HDT.Core.Utility.Extensions;

#endregion

namespace HDT.Core.Utility.Logging
{
	[DebuggerStepThrough]
	public class Log
	{
		private const int MaxLogFileAge = 2;
		private const int KeepOldLogs = 25;
		private const string BaseFileName = "hdt_log";
		private static readonly Queue<string> LogQueue = new Queue<string>();
		public static bool Initialized { get; private set; }

		private static string GetLogFile(string path)
		{
			var timeStamp = DateTime.Now.ToUnixTime();
			string logFile;
			while(File.Exists(logFile = Path.Combine(path, $"{BaseFileName}_{timeStamp}.txt")))
				timeStamp++;
			return logFile;
		}

		public static void Initialize()
		{
			if(Initialized)
				return;
			Trace.AutoFlush = true;
			var directory = Helper.GetDirectory("Logs");
			if(!directory.Exists)
				directory.Create();
			else
			{
				try
				{
					var oldLogs = directory.GetFiles($"{BaseFileName}*")
						.Where(x => x.LastWriteTime < DateTime.Now.AddDays(-MaxLogFileAge))
						.OrderByDescending(x => x.LastWriteTime)
						.Skip(KeepOldLogs);
					foreach(var file in oldLogs)
					{
						try
						{
							File.Delete(file.FullName);
						}
						catch
						{
						}
					}
				}
				catch(Exception)
				{
				}
			}
			var logFile = GetLogFile(directory.FullName);
			File.Create(logFile).Dispose();
			try
			{
				Trace.Listeners.Add(new TextWriterTraceListener(new StreamWriter(logFile, false)));	
			}
			catch (Exception ex)
			{
			}
			Initialized = true;
			foreach(var line in LogQueue)
				Trace.WriteLine(line);
		}

		public static void WriteLine(string msg, LogType type, [CallerMemberName] string memberName = "",
									 [CallerFilePath] string sourceFilePath = "")
		{
#if (!DEBUG)
			if(type == LogType.Debug && Config.Instance.LogLevel == 0)
				return;
#endif
			var file = sourceFilePath?.Split('/', '\\').LastOrDefault()?.Split('.').FirstOrDefault();
			var line = $"{DateTime.Now.ToLongTimeString()}|{type}|{file}.{memberName} >> {msg}";
			if(Initialized)
				Trace.WriteLine(line);
			else
				LogQueue.Enqueue(line);
		}

		public static void Debug(string msg, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
			=> WriteLine(msg, LogType.Debug, memberName, sourceFilePath);

		public static void Info(string msg, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
			=> WriteLine(msg, LogType.Info, memberName, sourceFilePath);

		public static void Warn(string msg, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
			=> WriteLine(msg, LogType.Warning, memberName, sourceFilePath);

		public static void Error(string msg, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
			=> WriteLine(msg, LogType.Error, memberName, sourceFilePath);

		public static void Error(Exception ex, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
			=> WriteLine(ex.ToString(), LogType.Error, memberName, sourceFilePath);
	}
}
