using Textensions.Core;
using UnityEngine;

namespace Textensions.Reveals.Base
{
    public abstract class TextReveal : MonoBehaviour
    {
        [Tooltip("The amount of time (in seconds) between each character reveal.")]
        public float characterDelay = 0.05f;

        private float characterTime;
        private bool colorUpdate;

        // PRIVATE MEMBERS
        private bool isRevealing; // TODO: Make readonly in the inspector
        private int numberOfCharacters;

        private int numberOfCharactersRevealed;

        // PUBLIC MEMBERS
        [Tooltip("The text we want to reveal.")]
        public Textension textension;
        
        private float totalRevealTime;
        private bool vertexUpdate;

        /// <summary>
        /// Reveals the character at a specific index
        /// </summary>
        /// <param name="_revealNumber"></param>
        protected virtual void RevealCharacter(int _revealNumber)
        {
            MarkAsRevealed(_revealNumber);
        }

        protected virtual void HideAllCharacters()
        {
            for (int i = 0; i < textension.GetUnrevealedCharacters().Count; i++) textension.GetCharacter(i).Unreveal();
        }

        private void Reset()
        {
            // Quickly fetch the textension reference
            textension = GetComponent<Textension>();
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            if (textension == null)
            {
                return;
            }
            
            textension.OnHideInitialize += Reveal;
            textension.RevealsTick += Tick;
        }

        private void Unsubscribe()
        {
            if (textension == null)
            {
                return;
            }
            
            textension.OnHideInitialize -= Reveal;
            textension.RevealsTick -= Tick;
            isRevealing = false;
        }

        /// <summary>
        /// Initialize the text, and starts the character reveal.
        /// </summary>
        private void Reveal()
        {
            Initialize();
            Play();
        }

        /// <summary>
        /// Initializes the text by defaulting some variables, and hiding the text.
        /// </summary>
        private void Initialize()
        {
            if (isRevealing) OnInterrupted();

            numberOfCharactersRevealed = 0;
            numberOfCharacters = textension.GetTextLength();
            totalRevealTime = 0f;

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
            isRevealing = true;
        }

        /// <summary>
        /// Gets invoked when the reveal has successfully completed.
        /// </summary>
        /// <summary>
        /// Note: This does not take effects into consideration, meaning if a text reveal is playing and is
        /// finished but the effects are visually hiding it, this will still get invoker before the effect finishes.
        /// </summary>
        protected virtual void OnCompleted()
        {
        }

        /// <summary>
        /// Gets invoked when we re-initialize an in-progress reveal.
        /// </summary>
        protected virtual void OnInterrupted()
        {
        }

        /// <summary>
        /// An update function that gets subscribed to the textension delegate
        /// </summary>
        private void Tick()
        {
            // Prevent a negative value from being assigned
            characterDelay = Mathf.Clamp(characterDelay, 0, characterDelay);

            // If we are text revealing...
            if (isRevealing)
            {
                // If we have no character to reveal...
                if (textension.unrevealedCharacters.Count <= 0)
                {
                    // Stop the reveal
                    isRevealing = false;
                    return;
                }

                // Add time
                totalRevealTime += Time.deltaTime;
                characterTime += Time.deltaTime;

                // While loop used to calculate how many letters on the same frame needs to be drawn
                while (characterTime > characterDelay)
                {
                    RevealCharacter(numberOfCharactersRevealed);
                    numberOfCharactersRevealed++;

                    characterTime -= characterDelay;

                    // If all characters are revealed, set the _isRevealing flag as dirty and break out of this while loop
                    if (numberOfCharactersRevealed == numberOfCharacters)
                    {
                        characterTime = 0f;
                        isRevealing = false;
                        OnCompleted();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Marks the specific character index as revealed and removes it from the unrevealedCharacters list.
        /// </summary>
        /// <param name="_index"></param>
        protected void MarkAsRevealed(int _index)
        {
            textension.unrevealedCharacters[_index].Reveal();
            textension.unrevealedCharacters.RemoveAt(_index);
        }
    }
}