// Author: Adrian Miasik
// Personal Portfolio: http://AdrianMiasik.com
// Github Account: https://github.com/AdrianMiasik

using Textensions.Core;
using TMPro;
using UnityEngine;

namespace Textensions.Reveals.Base
{
	public abstract class TextReveal : MonoBehaviour
	{
		// PUBLIC MEMBERS
		[Tooltip("The text we want to reveal.")]
		public Textension textension;
		
		[Tooltip("The amount of time (in seconds) between each character reveal.")]
		public float characterDelay = 0.05f;

		// PRIVATE MEMBERS
		private bool _isRevealing;
		private float _characterTime;
		private float _totalRevealTime;
		private int _numberOfCharacters;
		private int _numberOfCharactersRevealed;
		private bool _vertexUpdate;
		private bool _colorUpdate;
		
		protected virtual void RevealCharacter(int index)
		{
			textension.GetCharacter(index).isRevealed = true;
		}
		
		protected virtual void HideAllCharacters()
		{
			for (int i = 0; i < textension.GetCharacters().Count; i++)
			{
				textension.GetCharacter(i).isRevealed = false;
			}
		}
				
		private void Reset()
		{
			// Quickly Fetch References.
			textension = GetComponent<Textension>();
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
			SetTextString(source.text);
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
			textension.SetTextString(message);
		}

		/// <summary>
		/// Initializes the text by defaulting some variables, and hiding the text.
		/// </summary>
		private void Initialize()
		{
			_numberOfCharactersRevealed = 0;
			_numberOfCharacters = textension.GetTextLength();
			_totalRevealTime = 0f;
			
			HideAllCharacters();
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
			}
		}
	}
}