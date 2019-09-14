using System;
using System.Collections.Generic;
using Textensions.Core;
using Textensions.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Textensions.Reveals
{
    /// <summary>
    /// Reveals text characters over time in a random sequence by changing each characters vertex colors. (Using the TMP_Text component).
    /// </summary>
    /// <summary>
    /// Color Reveal: Characters are being rendered by TMP but are hidden by changing the alpha of each character mesh to zero.
    /// The way we are revealing each character is by altering the color of the vertices on the characters mesh. Each character mesh has 4 vertices and we are changing the color32 of each corner at once.
    /// </summary>
    public class ColorRandomWriter : ColorTypeWriter
    {
        private bool hasBeenInit = false;

        protected override void RevealCharacter(int revealNumber)
        {
            // Get random character within the unrevealed characters list
            int characterIndex = Random.Range(0, textension.unrevealedCharacters.Count);

            // Color
            ColorSingleCharacter(textension.unrevealedCharacters[characterIndex].Info(), textension.GetCachedColor());

            // TODO: Do this once in the Textension
            // Update the color on the mesh
            textension.text.textInfo.meshInfo[0].mesh.colors32 = textension.text.textInfo.meshInfo[0].colors32;
            textension.text.UpdateGeometry(textension.text.textInfo.meshInfo[0].mesh, 0);

#if DEBUG_TEXT
            Debug.Log("Character: " + textension.unrevealedCharacters[0].Info().character + " has been revealed by ColorRandomWriter.cs [" + GetInstanceID() + "]");
#endif

            // Mark the character as revealed
            MarkAsRevealed(characterIndex);
        }

        protected override void OnCompleted()
        {
            hasBeenInit = false;
        }

        protected override void OnInterrupted()
        {
            hasBeenInit = false;
        }
    }
}