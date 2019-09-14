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

        /// <summary>
        /// Theses are the keys we are going to remove from appliedEffects after we are done iterating through them.
        /// </summary>
        private List<int> _keysToClean = new List<int>();

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

        private void Update()
        {
#if DEBUG_TEXT
            // If the user presses f5...
            if (Input.GetKeyDown(KeyCode.F5))
            {
                // Print to the console all the effects on the characters.
                LogAppliedEffects();
            }
#endif
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
            List<int> characterIndicesToAffect = new List<int>();

            switch (fx.style)
            {
                // This effect will apply to all characters in this textension
                case Effect.Style.ALL_CHARACTERS:
                    for (int i = 0; i < textension.characters.Count; i++)
                    {
                        // If this specific character is not visible to TMP...
                        if (!textension.characters[i].Info().isVisible)
                        {
#if DEBUG_TEXT
                            Debug.LogWarning("Not applying effect to character index [" + i + "], because it is not visible.");
#endif
                            // Skip it
                            continue;
                        }

                        characterIndicesToAffect.Add(i);
                    }
                    break;
                // This effect will only apply to characters in odd index positions...
                case Effect.Style.ALL_ODD_CHARACTERS:
                    for (int i = 0; i < textension.characters.Count; i++)
                    {
                        // Odd
                        if (i % 2 == 1)
                        {
                            // If this specific character is not visible to TMP...
                            if (!textension.characters[i].Info().isVisible)
                            {
#if DEBUG_TEXT
                                Debug.LogWarning("Not applying effect to character index [" + i + "], because it is not visible.");
#endif
                                // Skip it
                                continue;
                            }

                            characterIndicesToAffect.Add(i);
                        }
                    }
                    break;
                // This effect will only apply to character in the even index positions...
                case Effect.Style.ALL_EVEN_CHARACTERS:
                    for (int i = 0; i < textension.characters.Count; i++)
                    {
                        // Even
                        if (i % 2 == 0)
                        {
                            // If this specific character is not visible to TMP...
                            if (!textension.characters[i].Info().isVisible)
                            {
#if DEBUG_TEXT
                                Debug.LogWarning("Not applying effect to character index [" + i + "], because it is not visible.");
#endif
                                // Skip it
                                continue;
                            }

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
        /// Removes the provided effect at a certain key within appliedEffects.
        /// </summary>
        /// <summary>
        /// Note: If the key value has no more effects after removal, the key will remain intact.
        /// Meaning just because we are removing the effect value from the key, doesn't mean we will remove the key if there are no effects within.
        /// Instead we are leaving the key alone even if it has no effects within it's value.
        /// </summary>
        /// <param name="characterIndex">The character you want to target on this textension.</param>
        /// <param name="effectToRemove">The effect you want to remove from this character.</param>
        private void RemoveEffect(int characterIndex, Effect effectToRemove)
        {
            // If we have found the character index...
            if (appliedEffects.TryGetValue(characterIndex, out List<Effect> effectsList))
            {
                // Remove the specific effect from this character
                effectsList.Remove(effectToRemove);
            }
            else
            {
                Debug.LogWarning("Could not find " + characterIndex + " in the applied effects list." +
                                 "It looks like you are trying to access a character that has no effects on it." +
                                 "Are you sure this character index has been created on the appliedEffect list?");
            }
        }

        /// <summary>
        /// Adds the provided effects to our textension.
        /// </summary>
        /// <summary>
        /// If the provided effect does not exist within the dictionary we will create a key in the dictionary along with a new list.
        /// </summary>
        /// <summary>
        /// However if the provided effect does exist within the dictionary we will access that key and add the effect to the list within that key value.
        /// </summary>
        private void AddEffects(List<Effect> effectsToApply)
        {
            // Iterate through each effect...
            for (int i = 0; i < effectsToApply.Count; i++)
            {
                // Get the indices that this effect is going to be applied to. (We don't want to apply this effect to every character)
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
                        Debug.Log(effectsToApply[i].title + " is not being applied to " + indicesToEffect[j] + " therefore we will create a new list for it at index " + indicesToEffect[j]);
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
#if DEBUG_TEXT
            LogAppliedEffects();
#endif
        }

#if DEBUG_TEXT
        /// <summary>
        /// A developer function that Debug.Logs all the applied effects on each character.
        /// </summary>
        private void LogAppliedEffects()
        {
            Debug.Log(appliedEffects.Keys.Count + " characters have keys on them.");

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
                        // Add the effect name to the end of this string
                        allEffectsStr += stackingEffect[j].title;

                        // If we have another effect after this one...
                        if (j != stackingEffect.Count - 1)
                        {
                            allEffectsStr += ", ";
                        }
                    }

                    // If we have no effects on this character...
                    if (stackingEffect.Count <= 0)
                    {
                        allEffectsStr += "[NO EFFECTS FOUND]";
                    }

                    // Add a period to the end
                    allEffectsStr += ".";

                    // Print
                    Debug.Log("Index " + key + " has these effect(s): " + allEffectsStr);
                }
            }
        }
#endif

        /// <summary>
        /// Note: This gets invoked by the textension.
        /// </summary>
        private void EffectsTick()
        {
            // If we have no characters on the textension...
            if (textension.characters.Count <= 0)
            {
                // Then we don't have any objects to apply our effects to. So we will Unsubscribe() as a result
                Unsubscribe();
                return;
            }

            // For each key...(each character index to apply an effect to)
            foreach (int key in appliedEffects.Keys)
            {
//                if (key > textension.GetTextLength() - 1)
//                {
//                    Debug.Log("Our effects need to be re-initialized since the text has changed.");
//                    // TODO: Throw out all character data past the characters length
//                    return;
//                }

                Character character = textension.GetCharacter(key);

                // If our current character is not revealed, or not visible...
                if (!character.isRevealed || !character.Info().isVisible)
                {
                    // then skip it
                    continue;
                }

                // Accumulate time to this specific character
                character.timeSinceReveal += Time.deltaTime;

                // Iterate through all the effects at this dictionary key (character index)
                for (int j = 0; j < appliedEffects[key].Count; j++)
                {
                    // Cache the effect
                    Effect fx = appliedEffects[key][j];

                    // Execute/Play the effect
                    fx.Calculate(character);

                    // If this effect has surpassed the animation curve time...
                    if (character.timeSinceReveal >= fx.uniform[fx.uniform.length - 1].time)
                    {
                        // If this effect animation curve has either a ping pong effect or a loop effect at the end...
                        if (fx.uniform.postWrapMode == WrapMode.PingPong || fx.uniform.postWrapMode == WrapMode.Loop)
                        {
                            // This effect is still playing, let's go to the next effect instead
                            continue;
                        }

                        // Remove the completed effect from this character
                        RemoveEffect(key, fx);

                        // This character has no more effects, therefore we will mark it as effectCompleted.
                        character.effectCompleted = true;

                        // If the specific character index has a value...
                        if (appliedEffects.TryGetValue(key, out List<Effect> effectsList))
                        {
                            // If the specific character has no more effects...
                            if (effectsList.Count <= 0)
                            {
                                // Mark this key as ready to be removed. (The reason we are doing it this way
                                // is we can't remove it while we are still iterating through it.)
                                _keysToClean.Add(key);
                            }
                        }
                    }
                }
            }

            CleanUnusedEffects();
        }

        /// <summary>
        /// Removes the dictionary entry for character indices that no longer have any effect playing.
        /// </summary>
        private void CleanUnusedEffects()
        {
            // If we have keys to clean...
            if (_keysToClean.Count > 0)
            {
                // Since we are no longer iterating through the appliedEffects collection, we can now remove the effects
                // Iterate through all the keys to remove from appliedEffects...
                foreach (int i in _keysToClean)
                {
                    // Remove the cleaned key
                    appliedEffects.Remove(i);

#if DEBUG_TEXT
                    Debug.Log("Character index [" + i + "] has no more effects on it, therefore the dictionary key is being revoked.");
                    Debug.Log(textension.GetCharacter(i).Info().character);
#endif
                }

                // We have just cleaned the keys, we can now clear this memory up for the next tick.
                _keysToClean.Clear();
            }
        }
    }
}