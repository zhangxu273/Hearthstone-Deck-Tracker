#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HDT.Core.Hearthstone;
using HDT.Core.Utility.Logging;

#endregion

namespace HDT.Core.HsReplay
{
	internal class LogUploader
	{
		private static readonly List<UploaderItem> InProgress = new List<UploaderItem>();

		public static async Task<bool> Upload(string[] logLines, MatchMetaData matchMetaData)
		{
			var log = string.Join(Environment.NewLine, logLines);
			var item = new UploaderItem(log.GetHashCode());
			if(InProgress.Contains(item))
			{
				Log.Info($"{item.Hash} already in progress. Waiting for it to complete...");
				InProgress.Add(item);
				return await item.Success;
			}
			InProgress.Add(item);
			Log.Info($"Uploading {item.Hash}...");
			var success = false;
			try
			{
				success = await TryUpload(logLines, matchMetaData);
			}
			catch(Exception ex)
			{
				Log.Error(ex);
			}
			Log.Info($"{item.Hash} complete. Success={success}");
			foreach(var waiting in InProgress.Where(x => x.Hash == item.Hash))
				waiting.Complete(success);
			InProgress.RemoveAll(x => x.Hash == item.Hash);
			return success;
		}

		private static async Task<bool> TryUpload(string[] logLines, MatchMetaData matchMetaData)
		{
			try
			{
				var lines = logLines.SkipWhile(x => !x.Contains("CREATE_GAME")).ToArray();
				var metaData = UploadMetaDataGenerator.Generate(matchMetaData);
				Log.Info("Creating upload request...");
				var uploadRequest = await ApiWrapper.CreateUploadRequest(metaData);
				Log.Info("Upload Id: " + uploadRequest.ShortId);
				await ApiWrapper.UploadLog(uploadRequest, lines);
				Log.Info("Upload complete");
				return true;
			}
			catch(WebException ex)
			{
				Log.Error(ex);
				return false;
			}
		}

		public class UploaderItem
		{
			private readonly TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();

			public int Hash { get; }

			public UploaderItem(int hash)
			{
				Hash = hash;
			}

			public override bool Equals(object obj)
			{
				var uObj = obj as UploaderItem;
				return uObj != null && Equals(uObj);
			}

			public override int GetHashCode() => Hash;

			public bool Equals(UploaderItem obj) => obj.Hash == Hash;

			public void Complete(bool result) => _tcs.SetResult(result);

			public Task<bool> Success => _tcs.Task;
		}
	}
}
