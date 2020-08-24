using System;
using System.Collections.Generic;
using Textensions.Editor.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Textensions.Editor
{
    public class TextensionsConsole
    {
        /// <summary>
        /// The types of logs you can send to the console.
        /// </summary>
        public enum Types
        {
            MESSAGE,
            WARNING,
            SUCCESS,
            ASSERT
        }

        /// <summary>
        /// The log structure. Each log requires a type and a message.
        /// </summary>
        private struct Log
        {
            private readonly Types status;
            private readonly string message;

            public Log(Types _status, string _message)
            {
                status = _status;
                message = _message;
            }

            public void Print()
            {
                Debug.Log(status + ": " + message);
            }

            public Types GetStatus()
            {
                return status;
            }

            public string GetMessage()
            {
                return message;
            }
        }

        /// <summary>
        /// A dictionary that contains a reference to all the types of logs.
        /// </summary>
        /// <returns></returns>
        private readonly Dictionary<Types, List<Log>> console = new Dictionary<Types, List<Log>>();

        // Colors
        private static readonly Color32 BackgroundColor = new Color32(40, 40, 40, 255);
        private static readonly Color32 MessageColor = new Color32(248, 248, 248, 255);
        private static readonly Color32 AssetColor = new Color32(255, 28, 41, 255);
        private static readonly Color32 WarningColor = new Color32(255, 192, 41, 255);
        private static readonly Color32 SuccessColor = new Color32(44, 255, 99, 255);

        // Styles
        public readonly StyleColor backgroundStyleColor = new StyleColor(BackgroundColor);
        public readonly StyleColor messageStyleColor = new StyleColor(MessageColor);
        public readonly StyleColor assetStyleColor = new StyleColor(AssetColor);
        public readonly StyleColor warningStyleColor = new StyleColor(WarningColor);
        public readonly StyleColor successStyleColor = new StyleColor(SuccessColor);

        // Cache
        private TextElement lastUsedStatusPrefix;
        private Types lastUsedType;

        /// <summary>
        /// Records a log into our console
        /// </summary>
        /// <param name="_type">The type of message you would like to log. <example>WARNING</example></param>
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
                // Create a list of logs starting with this log
                List<Log> _logs = new List<Log> { _log };

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
        /// Generates and injects all our console logs into a certain visual element
        /// </summary>
        public void InjectLogs(VisualElement _parent)
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
                        _parent.Add(GenerateLogComponent(_log));
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

        /// <summary>
        /// Returns the total number of logs
        /// </summary>
        /// <returns></returns>
        public int GetLogCount()
        {
            int count = 0;

            // Iterate through the Types enum...
            foreach (Types _type in Enum.GetValues(typeof(Types)))
            {
                // Check the console to see if it contains our type...
                if (console.TryGetValue(_type, out List<Log> _recordedLogs))
                {
                    count += _recordedLogs.Count;
                }
            }

            return count;
        }

        /// <summary>
        /// Generates and returns a specific visual element based on the provided log.
        /// </summary>
        /// <param name="_log"></param>
        /// <returns></returns>
        private VisualElement GenerateLogComponent(Log _log)
        {
            // Create and stylize the status row
            VisualElement _statusRow = new VisualElement {name = "Status Row*"};
            _statusRow.style.flexGrow = 0;
            _statusRow.style.flexDirection = FlexDirection.Row;
            _statusRow.style.alignItems = Align.Center;
            _statusRow.style.marginTop = 1;
            _statusRow.style.borderBottomWidth = 2f;			        // Border
            _statusRow.style.borderBottomColor = backgroundStyleColor;	// Border

            // Create and stylize the status prefix
            TextElement _statusPrefix = new TextElement {name = "Status Prefix*"};
            _statusPrefix.style.flexShrink = 1;
            _statusPrefix.style.flexGrow = 0;
            _statusPrefix.style.marginLeft = 2;
            _statusPrefix.style.marginTop = 1;
            _statusPrefix.style.fontSize = 12;

            // Create and stylize the status text
            TextElement _statusText = new TextElement {name = "Status Prefix*"};
            _statusText.style.flexShrink = 1;
            _statusText.style.flexGrow = 0;
            _statusText.style.flexWrap = Wrap.Wrap;

            _statusPrefix.text = _log.GetStatus().ToString();
            _statusText.text = _log.GetMessage();

            // Cache log
            lastUsedStatusPrefix = _statusPrefix;
            lastUsedType = _log.GetStatus();

            StyleCachedLog();

            // Sort hierarchy
            _statusRow.Add(_statusPrefix);
            _statusRow.Add(_statusText);

            return _statusRow;
        }
        
        /// <summary>
        /// Stylizes the last cached log depending on the log data.
        /// </summary>
        private void StyleCachedLog()
        {
            // Make the title bold
            TextensionStyling.MarkBold(lastUsedStatusPrefix);

            // Show type and append a colon for formatting
            lastUsedStatusPrefix.text = lastUsedType.ToString();
            lastUsedStatusPrefix.text += ": ";

            // Change title color based on log type
            switch (lastUsedType)
            {
                case Types.MESSAGE:
                    lastUsedStatusPrefix.style.color = messageStyleColor;
                    break;
                case Types.WARNING:
                    lastUsedStatusPrefix.style.color = warningStyleColor;
                    break;
                case Types.SUCCESS:
                    lastUsedStatusPrefix.style.color = successStyleColor;
                    break;
                case Types.ASSERT:
                    lastUsedStatusPrefix.style.color = assetStyleColor;
                    break;
                default:
                    Debug.LogWarning("T.ext: Unable to style this type.");
                    break;
            }
        }
    }
}
