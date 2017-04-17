using System;
using System.IO;
using HDT.Core.HsReplay.Enums;
using HDT.Core.Utility;
using HDT.Core.Utility.Logging;
using Newtonsoft.Json;

namespace HDT.Core.HsReplay
{
	internal sealed class Account
	{
		public static string CacheFilePath { get; } = Path.Combine(Helper.DataDirectory, "hsreplay.cache");

		private static readonly Lazy<Account> Lazy = new Lazy<Account>(Load);

		private Account()
		{
		}

		public static Account Instance => Lazy.Value;

		public string UploadToken { get; set; }
		public AccountStatus Status { get; set; }
		public string Username { get; set; }
		public int Id { get; set; }
		public DateTime LastUpdated { get; set; }

		public static bool Save()
		{
			var json = JsonConvert.SerializeObject(Instance);
			try
			{
				using(var sw = new StreamWriter(CacheFilePath))
					sw.WriteLine(json);
				return true;
			}
			catch(Exception ex)
			{
				Log.Error(ex);
				return false;
			}
		}


		public static void DeleteCacheFile()
		{
			if(!File.Exists(CacheFilePath))
				return;
			try
			{
				File.Delete(CacheFilePath);
			}
			catch(Exception e)
			{
				Log.Error(e);
			}
		}

		private static Account Load()
		{
			if(!File.Exists(CacheFilePath))
				return new Account();
			try
			{
				using(var sr = new StreamReader(CacheFilePath))
					return JsonConvert.DeserializeObject<Account>(sr.ReadToEnd()) ?? new Account();
			}
			catch(Exception ex)
			{
				Log.Error(ex);
				return new Account();
			}
		}
	}
}
