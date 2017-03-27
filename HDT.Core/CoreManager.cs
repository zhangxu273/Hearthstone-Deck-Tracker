#region

using System.Threading.Tasks;
using HDT.Core.LogEventHandlers;
using HDT.Core.LogParsers;

#endregion

namespace HDT.Core
{
	public class CoreManager
	{
		private readonly LogWatcherManager _logWatcherManager;
		private readonly PowerParser _powerParser;
		private readonly PowerHandler _powerHandler;

		public CoreManager()
		{
			_powerParser = new PowerParser();
			_powerHandler = new PowerHandler(_powerParser);

			_logWatcherManager = new LogWatcherManager();
			_logWatcherManager.OnPowerPowerTaskList += _powerParser.Parse;
		}

		public bool Running { get; private set; }

		public async Task Run()
		{
			if(Running)
				return;

			await _logWatcherManager.Start();

			Running = true;
		}
	}
}