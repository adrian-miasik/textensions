using System;
using System.Collections.Generic;
using UnityEngine;

namespace Textensions.Editor
{
	public class TextensionsConsole
	{
		/// <summary>
		/// The types of logs you can send to the console.
		/// </summary>
		public enum Types
		{
			DEFAULT,
			WARNING,
			SUCCESS,
			ASSERT
		}

		/// <summary>
		/// The log structure. Each log requires a type and a message.
		/// </summary>
		private struct Log
		{
			private Types status;
			private string message;

			public Log(Types _status, string _message)
			{
				status = _status;
				message = _message;
			}

			public void Print()
			{
				Debug.Log(status + ": " + message);
			}
		}

		/// <summary>
		/// A dictionary that contains a reference to all the types of logs.
		/// </summary>
		/// <returns></returns>
		private Dictionary<Types, List<Log>> console = new Dictionary<Types, List<Log>>();

		/// <summary>
		/// Records a log into our console
		/// </summary>
		/// <param name="_type">The type of message you would like to log. <example>Warning</example></param>
		/// <param name="_message">The message associated with that type. <example>"Missing text reference!"</example></param>
		public void Record(Types _type, string _message)
		{
			// Create a log to store for later.
			Log _log = new Log(_type, _message);

			// Checks if it has your message type...
			if (console.TryGetValue(_type, out List<Log> _recordedLogs))
			{
				// We do have these type of logs.
				// Let add this log to that list.
				_recordedLogs.Add(_log);
			}
			// We can't find out message type, it's the first of it's entry. Let's create a list of logs for it.
			else
			{
				// Create new list of logs.
				List<Log> _logs = new List<Log>();

				// Add the new log to our list.
				_logs.Add(_log);

				// Add that list to the console.
				console.Add(_type, _logs);
			}
		}

		/// <summary>
		/// Print out all our console logs into Unity's console
		/// </summary>
		public void PrintAllLogs()
		{
			// Iterate through the Types enum...
			foreach (Types _type in Enum.GetValues(typeof(Types)))
			{
				// Check the console to see if it contains our type...
				if (console.TryGetValue(_type, out List<Log> _recordedLogs))
				{
					// Let's iterate through all the logs of this type...
					foreach (Log _log in _recordedLogs)
					{
						// Print this log to the unity console
						_log.Print();
					}
				}
			}
		}

		/// <summary>
		/// Gets the number of logs of this type that have been logged to the console thus far.
		/// </summary>
		/// <summary>
		/// Note: If we are unable to find any of your type of logs, we will return 0.
		/// </summary>
		/// <param name="_type"></param>
		/// <returns></returns>
		public int GetTypeCount(Types _type)
		{
			return console.TryGetValue(_type, out List<Log> _recordedLogs) ? _recordedLogs.Count : 0;
		}
	}
}
