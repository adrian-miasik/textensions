// Author: Adrian Miasik
// Personal Portfolio: http://AdrianMiasik.com
// Github Account: https://github.com/AdrianMiasik

using Textensions.Reveals.Base;
using TMPro;
using UnityEngine;

namespace Textensions.Reveals
{
	/// <summary>
	/// Reveals text characters over time in a random sequence by changing each characters vertex colors. (Using the TextMeshProUGUI component).
	/// </summary>
	/// <summary>
	/// Color Reveal: Characters are being rendered by TMP but are hidden by changing the alpha of each character mesh to zero.
	/// The way we are revealing each character is by altering the color of the vertices on the characters mesh.
	/// Each character mesh has 4 (56) vertices and we are changing the color32 of each corner at once.
	/// </summary>
	public class ColorTypeWriter : TextReveal
	{
		protected override void RevealCharacter(int index)
		{
			Debug.Log("Attempting to reveal character " + textension.GetCharacter(index).Info().character);
			base.RevealCharacter(index);

			// Skip any characters that aren't visible
			if (!textension.GetCharacter(index).Info().isVisible)
				return;
			
			// Color
			ColorSingleCharacter(textension.GetCharacter(index).Info(), textension.GetCachedColor());

			// Update the color on the mesh
			textension.text.textInfo.meshInfo[0].mesh.colors32 = textension.text.textInfo.meshInfo[0].colors32;
			textension.text.UpdateGeometry(textension.text.textInfo.meshInfo[0].mesh, 0);
		}
		
		protected override void HideAllCharacters()
		{
			base.HideAllCharacters();
			
			Debug.Log("Hiding all characters!");
			ColorAllCharacters(new Color32(0,0,0,0));
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
		private void ColorSingleCharacter(TMP_CharacterInfo character, Color32 color)
		{
			// Bottom Left, Top Left, Top Right, Bottom Right
			textension.Info().meshInfo[0].colors32[character.vertexIndex + 0] = color;
			textension.Info().meshInfo[0].colors32[character.vertexIndex + 1] = color;
			textension.Info().meshInfo[0].colors32[character.vertexIndex + 2] = color;
			textension.Info().meshInfo[0].colors32[character.vertexIndex + 3] = color;
		}
	}
}
