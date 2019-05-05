// Author: Adrian Miasik
// Personal Portfolio: http://AdrianMiasik.com
// Github Account: https://github.com/AdrianMiasik

using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Textensions.Reveals.Base
{
	public abstract class TextReveal : MonoBehaviour
	{
		[SerializeField] [Tooltip("The text we want to reveal.")]
		internal TextMeshProUGUI displayText;

		//[SerializeField] [Tooltip("The text we will use to display statistics about this character reveal (Number of characters revealed, the delay in-between each character reveal, the current characters reveal time, and the total elapsed time).")]
		//private TextMeshProUGUI statistics;

		[SerializeField] [Tooltip("The amount of time (in seconds) between each character reveal.")]
		protected float characterDelay = 0.05f;

		protected Color32 CachedColor;
		private TMP_MeshInfo[] _originalMesh;
		private Vector3[] _targetVertices = new Vector3[0];
		
		private bool _isRevealing;
		private float _characterTime;
		private float _totalRevealTime;
		private int _numberOfCharacters;
		private int _numberOfCharactersRevealed;

		public float characterScale;

		protected Character[] AllCharacters;

		// Note: We are assuming this list will never contain a null element
		public List<Effect> allEffects = new List<Effect>();

		public bool vertexUpdate = false;
		public bool colorUpdate = false;
		
		protected virtual void RevealCharacter(int index)
		{
			AllCharacters[index].IsRevealed = true;
		}

		protected virtual void HideAllCharacters()
		{
			for (int i = 0; i < AllCharacters.Length; i++)
			{
				AllCharacters[AllCharacters.Length - 1].IsRevealed = false;
			}
		}
				
		private void Reset()
		{
			// Quickly Fetch References.
			displayText = GetComponent<TextMeshProUGUI>();
			//statistics = GetComponent<TextMeshProUGUI>();
		}


		private void Start()
		{
			Initialize();
			_isRevealing = true;
		}

		/// <summary>
		/// Initialize the text, and starts the character reveal.
		/// </summary>
		public void Reveal()
		{
			Initialize();
			Play();
		}

		/// <summary>
		/// Set the text string, initialize the text, and start the character reveal.
		/// </summary>
		/// <param name="message"></param>
		public void Reveal(string message)
		{
			Initialize(message);
			Play();
		}

		/// <summary>
		/// Set the time between each character reveal, set the text string, initialize the text, and start the character reveal.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="timeBetweenCharacterReveal"></param>
		public void Reveal(float timeBetweenCharacterReveal, string message)
		{
			SetCharacterDelay(timeBetweenCharacterReveal);
			Reveal(message);
		}

		// TMP INPUT FIELD - Incorrect character length value of the input fields text component:
		// The reason we are using TMP_InputField instead of getting the TextMeshProUGUI component within the input field component
		// is because the TextMeshProUGUI text.Length and textInfo.CharacterCount values are incorrect. The text field could be an "empty"
		// string, but it still seems to provides us with a length of 1. 
		// According to a Unity Technologies user "Stephan_B" we shouldn't be accessing the input fields text component anyways.
		// However, if you do need to access the text input's TextMeshProUGUI component and you do need to get the character count value from
		// that component, there does seem to be a workaround a Unity forum user named "Chris-Trueman" has found.
		// Simply trimming the Unicode character 'ZERO WIDTH SPACE' (Code 8203) from the ugui text string does seem to return the correct length.
		// I've tested Chris' solution and it does seem to work if you need to use it, however for this case I won't be doing that.
		// Instead I'll just be using TMP_InputField as mentioned above.
		// SOURCE: https://forum.unity.com/threads/textmesh-pro-ugui-hidden-characters.505493/
		/// <summary>
		/// Replaces the text string with the provided source's text string (Using the TextMeshProUGUI component).
		/// </summary>
		/// <param name="source"></param>
		public void ReplaceStringWithSources(TMP_InputField source)
		{
			displayText.text = source.text;
		}

		/// <summary>
		/// Set the amount of time between each character reveal.
		/// </summary>
		/// <param name="timeInSeconds"></param>
		private void SetCharacterDelay(float timeInSeconds)
		{
			characterDelay = timeInSeconds;
		}

		/// <summary>
		/// Initializes the text reveal by changing the string, defaulting some variables, and hiding the text.
		/// </summary>
		private void Initialize(string message)
		{
			SetTextString(message);
			Initialize();
		}

		/// <summary>
		/// Set the text string.
		/// </summary>
		/// <param name="message"></param>
		private void SetTextString(string message)
		{
			displayText.text = message;
		}

		/// <summary>
		/// Initializes the text by defaulting some variables, and hiding the text.
		/// </summary>
		private void Initialize()
		{
			_numberOfCharactersRevealed = 0;
			_numberOfCharacters = displayText.text.Length;
			_totalRevealTime = 0f;

			// Force the mesh update so we don't have to wait a frame to get the data.
			// Since we need to get information from the mesh we will have to update the mesh a bit earlier than normal.
			// "TMP generates/processes the mesh once per frame (if needed) just before Unity renders the frame."
			// Source: https://www.youtube.com/watch?v=ZHU3AcyDKik&feature=youtu.be&t=164
			// In most cases it's fine for TMP to render at it's normal timings but as mentioned above if we are going
			// to manipulate or fetch data from the mesh we should force the mesh to update so the data remains accurate.
			displayText.ForceMeshUpdate();

			CachedColor = displayText.color;
			_originalMesh = displayText.textInfo.CopyMeshInfoVertexData();
			
			// Define the size of our array
			AllCharacters = new Character[_numberOfCharacters];

			// Create and cache a character class (This will be used for later to keep track of effects and each character state)
			for (int i = 0; i < _numberOfCharacters; i++)
			{
				Character c = new Character(displayText.textInfo.characterInfo[i]);
				AllCharacters[i] = c;
			}

			HideAllCharacters();
		}

		/// <summary>
		/// Updates the statistics UI string.
		/// </summary>
		private void UpdateStatisticsText(TMP_Text text)
		{
			text.text = "Number of characters revealed: " + _numberOfCharactersRevealed + "/" + _numberOfCharacters + "\n" +
			                  "Delay in-between character reveal: " + characterDelay + " seconds.\n" +
			                  "Current character reveal time: " + _characterTime.ToString("F2") + " seconds.\n" +
			                  "Elapsed time: " + _totalRevealTime.ToString("F2") + " seconds.\n";
		}

		/// <summary>
		/// Starts revealing the characters.
		/// </summary>
		/// <summary>
		/// Note: You might need to call Initialize() first.
		/// </summary>
		private void Play()
		{
			_isRevealing = true;
		}
		
		public virtual void Update()
		{
			// Prevent a negative value from being assigned
			characterDelay = Mathf.Clamp(characterDelay, 0, characterDelay);
			
			// If we are text revealing...
			if (_isRevealing)
			{
				
				// If we don't have anything to reveal...
				if (_numberOfCharacters == 0)
				{
					//UpdateStatisticsText();

					// Early exit
					return;
				}

				_totalRevealTime += Time.deltaTime;
				_characterTime += Time.deltaTime;

				// While loop used to calculate how many letters on the same frame needs to be drawn
				while (_characterTime > characterDelay)
				{
					RevealCharacter(_numberOfCharactersRevealed);
					_numberOfCharactersRevealed++;

					_characterTime -= characterDelay;

					// If all characters are revealed, set the _isRevealing flag as dirty and break out of this while loop 
					if (_numberOfCharactersRevealed == _numberOfCharacters)
					{
						_characterTime = 0f;
						_isRevealing = false;
						break;
					}
				}

				//UpdateStatisticsText();
			}

			// Iterate through all the characters
			for (int i = 0; i < AllCharacters.Length; i++)
			{
				// If we have effects to apply to this character...
				if (allEffects.Count > 0)
				{
					// Iterate through all the effects to apply to this character
					for (int j = 0; j < allEffects.Count; j++)
					{
						// TODO: If calculations are the same for this frame and last, don't scale / update mesh.
						SetCharacterScale(AllCharacters[i], allEffects[j].Calculate(AllCharacters[i]));
					}
				}
				// We don't have any effects to apply, lets just do the regular scale.
				else
				{
					SetCharacterScale(AllCharacters[i], characterScale);
				}
			}
			
			if (vertexUpdate)
			{
				vertexUpdate = false;
				ApplyMeshChanges();
			}
		}

		public void ApplyMeshChanges()
		{
			if (_targetVertices.Length <= 0) return;
			
			displayText.textInfo.meshInfo[0].mesh.SetVertices(_targetVertices.ToList());

			// Vert changes
			displayText.textInfo.meshInfo[0].mesh.vertices = _targetVertices;
			
			// Refresh data to render correctly
			displayText.UpdateGeometry(displayText.textInfo.meshInfo[0].mesh, 0);

			// Cache data for next time we need to apply changes.
			_originalMesh = displayText.textInfo.CopyMeshInfoVertexData();
		}
		
		/// <summary>
		/// Scales the specified character to a specific size. (Uniform)
		/// </summary>
		/// <summary>
		/// Note: Remember to update your mesh vertex position data. Update the mesh geometry or use UpdateVertexData() 
		/// </summary>
		/// <param name="character"></param>
		/// <param name="scale"></param>
		public void SetCharacterScale(Character character, float scale)
		{
			if (character.cachedScale == scale)
			{
//				Debug.Log("Skipping");
				// Cache the scale 
				character.cachedScale = scale;
				return;
			}

			// Skip any characters that aren't visible
			if (!character.Info().isVisible)
				return;
			
			// Get the characters material vertices from the texts mesh
			Vector3[] originalVertices = _originalMesh[character.Info().materialReferenceIndex].vertices;
			
			// Make a copy of those verts
			_targetVertices = originalVertices;

			int index = character.Info().vertexIndex;
			
			// Locate the center of the mesh (Bottom left + top right) / 2
			Vector3 characterOrigin = (originalVertices[index + 0] + originalVertices[index + 3]) / 2;
			characterOrigin.y = 0;
			
			// Subtract the center of the mesh from target verts
			_targetVertices[index + 0] -= characterOrigin;
			_targetVertices[index + 1] -= characterOrigin;
			_targetVertices[index + 2] -= characterOrigin;
			_targetVertices[index + 3] -= characterOrigin;
			 
			// Scale the mesh of this character by our factor
			Matrix4x4 matrix = Matrix4x4.Scale((scale + characterScale) * Vector3.one);
			_targetVertices[index + 0] = matrix.MultiplyPoint3x4(_targetVertices[index + 0]);
			_targetVertices[index + 1] = matrix.MultiplyPoint3x4(_targetVertices[index + 1]);
			_targetVertices[index + 2] = matrix.MultiplyPoint3x4(_targetVertices[index + 2]);
			_targetVertices[index + 3] = matrix.MultiplyPoint3x4(_targetVertices[index + 3]);

			// Re-add the center of the mesh 
			_targetVertices[index + 0] += characterOrigin;
			_targetVertices[index + 1] += characterOrigin;
			_targetVertices[index + 2] += characterOrigin;
			_targetVertices[index + 3] += characterOrigin;

			
			// Set our vertex update to true so we can update all the vertex data at once instead of numerous times in a single frame.
			vertexUpdate = true;
		}
	}
}