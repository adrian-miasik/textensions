using System;
using Textensions.Core;
using Textensions.Reveals.Base;

namespace Textensions.Reveals
{
	/// <summary>
	/// Reveals text characters over time much like a typewriter (Using the TMP_Text component).
	/// </summary>
	/// <summary>
	/// Render Reveal: Characters aren't even being displayed by TMP until we tell it to. 
	/// (This means that the mesh for each character isn't being generated/calculated until told)
	/// The way we are revealing each character is by incrementing the maxVisibleCharacters int on the TextMeshProUGUI class.
	/// </summary>
    [Obsolete("Temporarily unsupported script.")]
    public class RenderTypeWriter : TextReveal
	{
		/// <summary>
		/// Hides the text by not rendering any of the character
		/// </summary>
		protected override void HideAllCharacters()
		{
			// Hide the text
			textension.GetText().maxVisibleCharacters = 0;
            base.HideAllCharacters();
		}

		protected override void RevealCharacter(int revealNumber)
		{
            // TODO: Add support for render reveals
			// Reveal a character
            textension.GetText().maxVisibleCharacters = revealNumber + 1;
        }
	}
}