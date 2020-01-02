using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using NewRelic.Api.Agent;
using Timer = System.Threading.Timer;

namespace FxConsole
{
	public class Program
	{
		public static Dictionary<string, float> ParamDictionary= new Dictionary<string, float>();
		public const string Gen0SizeName = "Gen0Size";
		public const string Gen1SizeName = "Gen1Size";
		public static System.Timers.Timer timer1 = new System.Timers.Timer();
		public static System.Timers.Timer timer2 = new System.Timers.Timer();
		public static NewRelic.Api.Agent.IAgent Agent;

		static void Main(string[] args)
		{
			Console.WriteLine("enter any key to begin...");
			Console.ReadLine();

			timer1.Elapsed += new ElapsedEventHandler(OnTimer1);
			timer1.Interval = 20000;
			timer1.Enabled = true;

			timer2.Elapsed += new ElapsedEventHandler(OnTimer2);
			timer2.Interval = 30000;
			timer2.Enabled = true;

			ParamDictionary.Add(Gen0SizeName, 34f);
			ParamDictionary.Add(Gen1SizeName, 44f);

			var d = new longListOfParams(ParamDictionary);

			Agent = NewRelic.Api.Agent.NewRelic.GetAgent();

			while (true) { var x = DoSomething("wash your car"); }
		}

		private static void OnTimer1(object source, ElapsedEventArgs e)
		{
			Console.WriteLine("Clearing Dictionary");
			ParamDictionary.Clear();
		}

		private static void OnTimer2(object source, ElapsedEventArgs e)
		{
			Console.WriteLine("GC Collecting");
			GC.Collect();
		}

		[Transaction]
		public static int DoSomething(string whatToDo)
		{
			//var y = whatToDo;
			//Console.WriteLine($"what I'm doing: {y}");

			ParamDictionary.Add(Guid.NewGuid().ToString(), 88);
			Thread.Sleep(1000);
			try
			{
				var localTmd = Agent.TraceMetadata;
				var tid = localTmd.TraceId;
				var sid = localTmd.SpanId;
				var sam = localTmd.IsSampled;
				Console.WriteLine($"TraceId: {tid}; SpanId: {sid}; Sampled: {sam.ToString()}.");

				var lm = Agent.GetLinkingMetadata();
				foreach (KeyValuePair<string, string> kvp in lm)
				{
					//Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
				}

			}
			catch (Exception ex)
			{
				var x = ex.Message;
			}
			return 10;
		}

		public class longListOfParams
		{
			public float gen0size;
			public float gen1size;

			public longListOfParams(Dictionary<string, float> paramDictionary)
			{
				gen0size = ParamDictionary[Gen0SizeName];
				gen1size = ParamDictionary[Gen1SizeName];
			}
		} 
	}
}