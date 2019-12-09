using System;
using Textensions.Core;
using UnityEngine;

namespace Textensions.Examples.Scripts
{
	/// <summary>
	/// An example script that demonstrates how to use invoke Reveal functions.
	/// </summary>
	[Obsolete("This class is not being supported anymore.")]
	public class TextRevealUsageExample : MonoBehaviour
	{
		private const string MESSAGE = "This is a test sentence that was passed in by TextRevealUsageExample.cs";

		[SerializeField] [Tooltip("The keycode that starts the reveal. When pressing this key down, the reveal will start.")]
		private KeyCode revealKey = KeyCode.F1;

		[SerializeField] private Textension textension;

		private void Update()
		{
			if (Input.GetKeyDown(revealKey))
			{
//                textension();
			}
		}
	}
}