// Author: Adrian Miasik
// Personal Portfolio: http://AdrianMiasik.com
// Github Account: https://github.com/AdrianMiasik

using Textensions.Core;
using UnityEngine;

namespace Textensions.Examples.Scripts
{
	/// <summary>
	/// An example script that demonstrates how to use invoke Reveal functions.
	/// </summary>
	public class TextRevealUsageExample : MonoBehaviour
	{
		[SerializeField] [Tooltip("The keycode that starts the reveal. When pressing this key down, the reveal will start.")]
		private KeyCode revealKey = KeyCode.F1;
        
        [SerializeField] private Textension textension;

        private const string MESSAGE = "This is a test sentence that was passed in by TextRevealUsageExample.cs";

        private void Update()
		{
			if (Input.GetKeyDown(revealKey))
			{
                textension.Initialize();
            }
		}
	}
}
