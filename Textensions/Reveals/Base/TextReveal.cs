// Author: Adrian Miasik
// Personal Portfolio: http://AdrianMiasik.com
// Github Account: https://github.com/AdrianMiasik

using System;
using Sirenix.OdinInspector;
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
        [ShowInInspector] private bool _isRevealing; // TODO: Make readonly in the inspector
        private float _characterTime;
        private float _totalRevealTime;
        private int _numberOfCharacters;
        private int _numberOfCharactersRevealed;
        private bool _vertexUpdate;
        private bool _colorUpdate;

        protected virtual void RevealCharacter(int revealNumber)
        {
            MarkAsRevealed(textension.unrevealedCharacters[revealNumber]);
        }

        protected virtual void HideAllCharacters()
        {
            for (int i = 0; i < textension.GetUnrevealedCharacters().Count; i++)
            {
                textension.GetCharacter(i).isRevealed = false;
            }
        }

        private void Reset()
        {
            // Quickly Fetch References.
            textension = GetComponent<Textension>();
        }

        private void Awake()
        {
            textension.OnHideInitialize += Reveal;
            textension.OnTick += Tick;
        }

        private void OnEnable()
        {
            textension.OnHideInitialize += Reveal;
            textension.OnTick += Tick;
        }

        private void OnDisable()
        {
            textension.OnHideInitialize -= Reveal;
            textension.OnTick -= Tick;
            _isRevealing = false;
        }

        private void OnDestroy()
        {
        private void Start()
        {
            Initialize();
            _isRevealing = true;
            textension.OnHideInitialize -= Reveal;
            textension.OnTick -= Tick;
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
            if (_isRevealing)
            {
                OnInterrupted();
            }

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

        /// <summary>
        /// Gets invoked when the reveal has successfully completed.
        /// </summary>
        /// <summary>
        /// Note: This does not take effects into consideration, meaning if a text reveal is playing and is
        /// finished but the effects are visually hiding it, this will still get invoker before the effect finishes.
        /// </summary>
        protected virtual void OnCompleted() { }

        /// <summary>
        /// Gets invoked when we re-initialize an in-progress reveal.
        /// </summary>
        protected virtual void OnInterrupted() { }

        /// <summary>
        /// An update function that gets subscribed to the textension delegate
        /// </summary>
        private void Tick()
        {
            if (textension.unrevealedCharacters.Count <= 0)
            {
                _isRevealing = false;
                return;
            }

            if (!enabled)
            {
                OnDisable();
                return;
            }

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
                        OnCompleted();
                        break;
                    }
                }
            }
        }

        protected void MarkAsRevealed(Character character)
        {
            character.isRevealed = true;
            textension.unrevealedCharacters.Remove(character);
        }
    }
}