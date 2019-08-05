using System.Collections.Generic;
using Textensions.Effects.Base;
using UnityEngine;

namespace Textensions.Core
{
    public class TextensionEffects : MonoBehaviour
    {
        /// <summary>
        /// The text we want to apply effects to
        /// </summary>
        public Textension textension;

        /// <summary>
        /// The effects we want to apply to this textension
        /// </summary>
        public List<Effect> fxs;

        /// <summary>
        /// These are the effects that we will be updating in EffectTick() every frame
        /// </summary>
        public Dictionary<int, List<Effect>> appliedEffects = new Dictionary<int, List<Effect>>();

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
        /// <summary>
        /// Subscribe to the textension action events so that this effect script can get invoked. (EffectsInitialize & EffectsTick)
        /// </summary>
        private void Subscribe()
        {
            // Subscribe to the effect action events
            textension.EffectsInitialize += Initialize;
            textension.EffectsTick += EffectsTick;
        }

        /// <summary>
        /// Unsubscribe to the textension action events so that this effect script will no longer get invoked. (EffectsInitialize & EffectsTick)
        /// </summary>
        private void Unsubscribe()
        {
            // Unsubscribe to the effect action events
            textension.EffectsInitialize -= Initialize;
            textension.EffectsTick -= EffectsTick;
        }
        
        /// <summary>
        /// Note: This gets invoked by the textension.
        /// </summary>
        private void Initialize()
        {
            appliedEffects.Clear();
            AddEffectsToTextension();
        }

        /// <summary>
        /// Adds our list of effects to the textensions class (which gets applied each applicable character).
        /// </summary>
        private void AddEffectsToTextension()
        {
            AddEffects(fxs);
        }

        /// <summary>
        /// Determine what character indices this effect will apply to.
        /// </summary>
        /// <summary>
        /// Note: Any characters that are not visible to TMP, will not be returned.
        /// </summary>
        /// <param name="fx"></param>
        private List<int> DetermineWhatCharactersToAffect(Effect fx)
        {
            Debug.Log(fx.style);

            List<int> characterIndicesToAffect = new List<int>();

            switch (fx.style)
            {
                // This effect will apply to all characters in this textension
                case Effect.Style.ALL_CHARACTERS:
                    for (int i = 0; i < textension.characters.Count; i++)
                    {
                        characterIndicesToAffect.Add(i);
                    }

                    break;

                case Effect.Style.ALL_ODD_CHARACTERS:
                    for (int i = 0; i < textension.characters.Count; i++)
                    {
                        // Odd
                        if (i % 2 == 1)
                        {
                            characterIndicesToAffect.Add(i);
                        }
                    }

                    break;

                case Effect.Style.ALL_EVEN_CHARACTERS:
                    for (int i = 0; i < textension.characters.Count; i++)
                    {
                        // Even
                        if (i % 2 == 0)
                        {
                            characterIndicesToAffect.Add(i);
                        }
                    }

                    break;

                default:
                    Debug.Log("This effect style has not been implemented properly.");
                    break;
            }

            return characterIndicesToAffect;
        }

        /// <summary>
        /// Adds the provided effects to our textension
        /// </summary>
        /// <summary>
        /// If the provided effect does not exist within the dictionary we will create a key in the dictionary along with a new list
        /// </summary>
        /// <summary>
        /// However if the provided effect does exist within the dictionary we will access that key and add the effect to the list within that key value
        /// </summary>
        private void AddEffects(List<Effect> effectsToApply)
        {
            Debug.Log("Adding effect!");

            // Iterate through each array
            for (int i = 0; i < effectsToApply.Count; i++)
            {
                List<int> indicesToEffect = DetermineWhatCharactersToAffect(effectsToApply[i]);
                
                // Iterate through all the character indices this effect will affect...
                for (int j = 0; j < indicesToEffect.Count; j++)
                {
                    // If this key index exists...
                    if (appliedEffects.TryGetValue(indicesToEffect[j], out List<Effect> effectsList))
                    {
#if DEBUG_TEXT
                        Debug.Log("Key value " + indicesToEffect[j] + " already exists.");
#endif
                        // This key already exists, lets add this effect to the effects list at this index.
                        effectsList.Add(effectsToApply[i]);
                    }
                    // This key index does not exist...
                    else
                    {
#if DEBUG_TEXT
                        Debug.Log(effectsToApply[i].title + " is not being applied to " + indicesToEffect[j] + " therefore we will create a new list for it at index " + j);
#endif
                        // Create a new effects list                  
                        List<Effect> stackingEffect = new List<Effect>();

                        // Add this effect to the list
                        stackingEffect.Add(effectsToApply[i]);

                        // Create an entry in our dictionary with our list (if the key exists then other effects will be added to this list)
                        appliedEffects.Add(indicesToEffect[j], stackingEffect);
                    }
                }
            }
            LogAppliedEffects();
        }
        
        /// <summary>
        /// A developer function that Debug.Logs all the applied effects on each character.
        /// </summary>
        private void LogAppliedEffects()
        {
            Debug.Log(appliedEffects.Keys.Count + " characters have effects on them.");

            foreach (int key in appliedEffects.Keys)
            {
                List<Effect> stackingEffect;
                if (appliedEffects.TryGetValue(key, out stackingEffect))
                {
                    // Create a string that will have all the effect titles
                    string allEffectsStr = "";

                    // Iterate through all the effects at a given character index / key
                    for (int j = 0; j < stackingEffect.Count; j++)
                    {
                        // Add the effect name to the end of this string with a space at the end
                        allEffectsStr += stackingEffect[j].title + " ";
                    }

                    // Remove the last space at the end
                    allEffectsStr = allEffectsStr.TrimEnd(allEffectsStr[allEffectsStr.Length - 1]);

                    // Add a period to the end
                    allEffectsStr += ".";

                    // Print
                    Debug.Log("Index " + key + " has these effect(s): " + allEffectsStr);
                }
            }
        }

        /// <summary>
        /// Note: This gets invoked by the textension.
        /// </summary>
        private void EffectsTick()
        {
            if (textension.characters.Count <= 0)
            {
                Unsubscribe();
                return;
            }

            // For each key (character index to apply and effect to)...
            for (int i = 0; i < appliedEffects.Keys.Count; i++)
            {
                if (i > textension.GetTextLength() - 1)
                {
                    Debug.Log("Our effects need to be re-initialized since the text has changed.");
                    // TODO: Throw out all character data past the characters length
                    return;
                }

                Character character = textension.GetCharacter(i);

                // Don't even bother effecting characters that aren't revealed
                if (!character.isRevealed)
                {
                    return;
                }

                // Skip blame characters
                if (!character.Info().isVisible)
                {
                    continue;
                }

                // Iterate through all the effects at this dictionary key (character index)
                for (int j = 0; j < appliedEffects[i].Count; j++)
                {
                    // Cache the effect
                    Effect fx = appliedEffects[i][j];
                    
                    // TODO: Support multiple character effect animations
                    // If this animation is completed...
                    if (character.timeSinceReveal > fx.uniform[fx.uniform.length - 1].time)
                    {
                        // TODO: Flicker happens cause I'm assuming the scale is set too late and is late a frame after textension.cs. Possible fix? Update the character after we set the scale on the character below.
                        // TODO: Set to characters base scale
                        // Dirty scale for now
                        character.SetScale(Vector3.one);

                        // TODO: Remove this character from the dictionary along with the associated character class
                        character.effectCompleted = true; // Redundant?
                    }
                    else
                    {
                        character.AddScale(fx.Calculate(character) * Vector3.one - character.scale);
                    }
                }
            }
        }
    }
}