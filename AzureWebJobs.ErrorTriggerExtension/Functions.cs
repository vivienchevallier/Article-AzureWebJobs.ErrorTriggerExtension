using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions;
using Microsoft.Azure.WebJobs.Host;

namespace AzureWebJobs.ErrorTriggerExtension
{
	public class Functions
	{
		// This function will get triggered/executed when a new message is written 
		// on an Azure Queue called queue.
		public static void ProcessQueueAMessage([QueueTrigger("queuea")] string message, TextWriter log)
		{
			log.WriteLine(message);

			ProcessMessage(message);
		}

		public static void ProcessQueueBMessage([QueueTrigger("queueb")] string message, TraceWriter logger)
		{
			logger.Info(message);

			try
			{
				ProcessMessage(message);
			}
			catch (Exception ex)
			{
				logger.Error($"An error occurred in: '{nameof(ProcessQueueBMessage)}'", ex, nameof(Functions));
			}
		}

		/// <summary>
		/// Triggered when an error is reported in other functions.
		/// Called whenever 2 errors occur within a 3 minutes sliding window (throttled at a maximum of 2 notifications per 10 minutes).
		/// </summary>
		public static void GlobalErrorMonitor([ErrorTrigger("0:03:00", 2, Throttle = "0:10:00")] TraceFilter filter, TextWriter log)
		{
			Console.Error.WriteLine("An error has been detected in a function.");

			log.WriteLine(filter.GetDetailedMessage(1));
		}

		private static void ProcessMessage(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				throw new ArgumentNullException(nameof(message));
			}

			//Do some work here...
		}
	}
}
