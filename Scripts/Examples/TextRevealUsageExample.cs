// Author: Adrian Miasik
// Personal Portfolio: http://AdrianMiasik.com
// Github Account: https://github.com/AdrianMiasik

using Textensions.Reveals.Base;
using UnityEngine;

namespace Textensions.Examples
{
	/// <summary>
	/// An example class that demonstrates how to use invoke Reveal functions.
	/// </summary>
	public class TextRevealUsageExample : MonoBehaviour
	{
		[SerializeField] [Tooltip("The keycode that starts the reveal. When pressing this key down, the reveal will start.")]
		private KeyCode revealKey = KeyCode.F1;

		[SerializeField] [Tooltip("The amount of time (in seconds) between each character reveal.")]
		private float characterDelay = 0.05f;

		[SerializeField] private TextReveal reveal = null;

		private string message = "This is a test sentence that was passed in by TextRevealUsageExample.cs";
		
		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyDown(revealKey))
			{
				reveal.Reveal(characterDelay, message);
			}
		}
	}
}
