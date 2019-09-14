using Textensions.Core;
using Textensions.Reveals.Base;
using TMPro;
using UnityEngine;

namespace Textensions.Reveals
{
	/// <summary>
	/// Reveals text characters over time (from [0] to [length - 1]) by changing each characters vertex colors. (Using the TMP_Text component).
	/// </summary>
	/// <summary>
	/// Color Reveal: Characters are being rendered by TMP but are hidden by changing the alpha of each character mesh to zero.
	/// The way we are revealing each character is by altering the color of the vertices on the characters mesh.
	/// Each character mesh has 4 (56) vertices and we are changing the color32 of each corner at once.
	/// </summary>
	public class ColorTypeWriter : TextReveal
	{
        // TODO: Remove unused revealNumber parameter?
        protected override void RevealCharacter(int revealNumber)
		{
            // If this character is not visible (it's a space or not being rendered by TMP) then lets skip it
            if (!textension.unrevealedCharacters[0].Info().isVisible)
            {
                // Mark this one as revealed so the text reveal can continue.
                MarkAsRevealed(0);
                
                // Early exit, we don't need to update the color of this character nor do we need to update the entire text mesh.
                return;
            }
            
            // Color
			ColorSingleCharacter(textension.unrevealedCharacters[0].Info(), textension.GetCachedColor());

            // TODO: Do the color update once in the Textension
			// Update the color on the mesh
			textension.text.textInfo.meshInfo[0].mesh.colors32 = textension.text.textInfo.meshInfo[0].colors32;
			textension.text.UpdateGeometry(textension.text.textInfo.meshInfo[0].mesh, 0);
            
#if DEBUG_TEXT
//            Debug.Log("Character: " + textension.unrevealedCharacters[0].Info().character + " has been revealed by ColorTypeWriter.cs [" + GetInstanceID() + "]");
#endif
            
            // Mark the character as revealed
            MarkAsRevealed(0);
        }
		
		protected override void HideAllCharacters()
		{
			ColorAllCharacters(new Color32(0,0,0,0));
			base.HideAllCharacters();
        }
		
		/// <summary>
		/// Hides the text by getting every characters mesh and setting the alpha of each vertex to zero rendering it invisible.
		/// </summary>
		/// <summary>
		/// Note: Remember to update your mesh vertex color data. Update the mesh colors & update geometry or use UpdateVertexData() 
		/// </summary>
		private void ColorAllCharacters(Color32 color)
		{						
			// Iterate through each character
			for (int i = 0; i < textension.Info().characterCount; i++)
			{
				// Bottom Left, Top Left, Top Right, Bottom Right
				textension.Info().meshInfo[0].colors32[textension.GetCharacter(i).Info().vertexIndex + 0] = color;
				textension.Info().meshInfo[0].colors32[textension.GetCharacter(i).Info().vertexIndex + 1] = color;
				textension.Info().meshInfo[0].colors32[textension.GetCharacter(i).Info().vertexIndex + 2] = color;
				textension.Info().meshInfo[0].colors32[textension.GetCharacter(i).Info().vertexIndex + 3] = color;
			}

			// TODO: Pass this data back to the associated textension
			// Update the color on the mesh
			for (int i = 0; i < textension.Info().meshInfo.Length; i++)
			{
				// TODO: Don't reach into an object twice? This is a programming principle I need to read more about
				textension.text.textInfo.meshInfo[i].mesh.colors32 = textension.text.textInfo.meshInfo[i].colors32;
				textension.text.UpdateGeometry(textension.text.textInfo.meshInfo[i].mesh, i);
			}
			
			textension.DirtyVertex();
		}

		/// <summary>
		/// Changes the color of a single character.
		/// </summary>
		/// <summary>
		/// Colorizes all the vertices on a characters mesh all at once to a certain color.
		/// Note: Remember to update your mesh vertex color data. Update the mesh colors & update geometry or use UpdateVertexData() 
		/// </summary>
		/// <param name="character"></param>
		/// <param name="color"></param>
		protected void ColorSingleCharacter(TMP_CharacterInfo character, Color32 color)
		{
			// Bottom Left, Top Left, Top Right, Bottom Right
			textension.Info().meshInfo[0].colors32[character.vertexIndex + 0] = color;
			textension.Info().meshInfo[0].colors32[character.vertexIndex + 1] = color;
			textension.Info().meshInfo[0].colors32[character.vertexIndex + 2] = color;
			textension.Info().meshInfo[0].colors32[character.vertexIndex + 3] = color;
		}
    }
}
