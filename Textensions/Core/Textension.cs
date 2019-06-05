using System;
using System.Collections.Generic;
using Textensions.Effects.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;

namespace Textensions.Core
{
    /// <summary>
    /// A textension is an abstraction layer on top of the TMP 'TMP_Text' class including all it's relevant components. This will be used for later to keep track of effects and each character state.
    /// </summary>
    public class Textension : MonoBehaviour
    {
        // PUBLIC MEMBERS
        // TODO: There should only be one textension per given text. Enforce this.
        public TMP_Text text;

        [Tooltip("The base scale of each character.")]
        public Vector3 characterScale = Vector3.one;

        public bool hasInitialized = false;

        [Tooltip(
            "Filled-in if you want to hide the text when it's initialized. If filled in then you are probably looking to attach a reveal to the textension.")]
        public bool hideOnInitialization = true;

        // TODO: Convert to array?
        public List<Character> characters = new List<Character>();
        public List<Character> unrevealedCharacters = new List<Character>();

        // These are the effects that we will be updating in EffectTick() every frame
        public List<Effect> effectsToApply = new List<Effect>();

        // PRIVATE MEMBERS
        private bool _refreshVertex;

        private TMP_MeshInfo[] _originalMesh;
        private Vector3[] _targetVertices = new Vector3[0];
        private Color32 _cachedColor;

        /// <summary>
        /// An action that only gets invoked if we are hiding text.
        /// </summary>
        public event Action OnHideInitialize;

        public event Action RevealTick;

        private void Reset()
        {
            text = GetComponent<TMP_Text>();
        }

        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// Recalculates the TMP_Text and recreates the character list to match the text. This should not be called more than once per frame.
        /// </summary>
        private void Initialize()
        {
            // Force the mesh update so we don't have to wait a frame to get the data.
            // Since we need to get information from the mesh we will have to update the mesh a bit earlier than normal.
            // "TMP generates/processes the mesh once per frame (if needed) just before Unity renders the frame."
            // Source: https://www.youtube.com/watch?v=ZHU3AcyDKik&feature=youtu.be&t=164
            // In most cases it's fine for TMP to render at it's normal timings but as mentioned above if we are going
            // to manipulate or fetch data from the mesh we should force the mesh to update so the data remains accurate.
            text.ForceMeshUpdate();

            // Cache our current text color and original mesh data
            _cachedColor = text.color;
            _originalMesh = text.textInfo.CopyMeshInfoVertexData();

            // Clear our lists
            characters.Clear();
            unrevealedCharacters.Clear();

            // Create and cache a character class (This will be used for later to keep track of effects and each character state)
            for (int i = 0; i < text.text.Length; i++)
            {
                Character character = new Character(text.textInfo.characterInfo[i]);
                characters.Add(character);

                // TODO: Effect init here?

                // If we are hiding on initialization...
                if (hideOnInitialization)
                {
                    // We just created the character so it won't be marked as revealed
                    unrevealedCharacters.Add(character);
                    continue;
                }

                // In this case we are not hiding the text on initialization so we will be marking this character as already revealed.
                character.isRevealed = true;
            }

            if (hideOnInitialization)
            {
                OnHideInitialize?.Invoke();
            }

            hasInitialized = true;
        }

        // TODO: Create and enforce application loop as described below
        private void Update()
        {
            if (hasInitialized)
            {
                // Step 0: Init / Re-init dirty text
                // TODO: Don't let each component access Initialize() since we might have them invoke it more than once per frame.

                // Step 1: Let each reveal do its thing
                RevealTick?.Invoke();

                // Step 2: Calculate the effect for each character
                EffectTick();

                // Step 3: Apply the effect to the character class
                RenderEffects();

                // Step 4: Then finally take all that information and apply it to the TMP_Text only once this frame
                Render();
            }
        }

        /// <summary>
        /// Checks if the provided effect will modify the characters, if it does modify the characters we will add the effect to use it next time we Initialize().
        /// </summary>
        /// <summary>
        /// However, if the effect does not modify the characters, we will simply ignore it. (To see if an effect has been ignored use the "DEBUG_TEXT" scripting define)
        /// </summary>>
        public void AddEffects(List<Effect> fxs)
        {
            foreach (Effect fx in fxs)
            {
                if (fx.indexToEffect.Count > 0)
                {
                    ForceAddEffect(fx);
                }
#if DEBUG_TEXT
                else
                {
                    Debug.Log("Effect '" + fx.title +
                              " does not modify characters and has not been added to the textension");
                }
#endif
            }
        }

        /// <summary>
        /// Adds the provided effect without checking if it will apply to all characters.
        /// </summary>
        /// <param name="fx"></param>
        private void ForceAddEffect(Effect fx)
        {
#if DEBUG_TEXT
            Debug.Log("Applying '" + fx.title + "' to this textension: " + gameObject, gameObject);
#endif
            effectsToApply.Add(fx);
        }

        // TODO: Improve performance & architecture
        private void EffectTick()
        {
            // For each effect...
            foreach (Effect effect in effectsToApply)
            {
                // Iterate through all the index you will need to effect
                for (int i = 0; i < effect.indexToEffect.Count; i++)
                {
                    if (effect.indexToEffect[i] >= GetCharacters().Count)
                    {
#if DEBUG_TEXT
                        Debug.LogWarning("Effect out of range");
#endif
                        return;
                    }

                    // TODO: Don't cache this for each effect. Just do it once for each character. (Maybe store each effect in a list at specific indexes)
                    Character character = GetCharacter(effect.indexToEffect[i]);

                    // Don't even bother effecting characters that aren't revealed
                    if (!character.isRevealed)
                    {
                        return;
                    }

                    // TODO: Determine when the effect is over, when it is over. Delete data
                    // Set scale to that individual character using it's own data
                    character.AddScale(effect.Calculate(character) * Vector3.one - character.scale);
                }
            }
        }

        /// <summary>
        /// Update each character that needs to be updated. (If an effect doesn't not effect a character, we will not update the characters mesh)
        /// </summary>
        private void RenderEffects()
        {
            // Iterate through each character...
            foreach (Character character in characters)
            {
                // TODO: Maybe split UpdateCharacter() into different functions? Will need to stress test and see if certain matrix modifications are cheaper than others.
                // If the character's position, rotation or scale has been modified...
                if (character.updatePosition || character.updateRotation || character.updateScale)
                {
#if DEBUG_TEXT
//                    Debug.Log("Updating character: " + character.Info().character);
#endif
                    // Update the character mesh data
                    UpdateCharacter(character);
                }
            }
        }

        public void DirtyVertex()
        {
            _refreshVertex = true;
        }

        /// <summary>
        /// See if we need to apply changes to the mesh this frame, if we do we will apply the changes to the mesh and copy the data for next time.
        /// </summary>
        private void Render()
        {
            if (_refreshVertex)
            {
                ApplyMeshChanges();
                _refreshVertex = false;
            }
        }

        /// <summary>
        /// Returns a list of character structs for this textension.
        /// </summary>
        /// <returns></returns>
        private List<Character> GetCharacters()
        {
            return characters;
        }

        /// <summary>
        /// Returns a list of character structs that have not been revealed.
        /// </summary>
        /// <returns></returns>
        public List<Character> GetUnrevealedCharacters()
        {
            return unrevealedCharacters;
        }

        /// <summary>
        /// Returns a single character struct for this textension at a given index.
        /// </summary>
        /// <param name="i">Index of the character you want to fetch</param>
        /// <returns></returns>
        public Character GetCharacter(int i)
        {
            return characters[i];
        }

        /// <summary>
        /// Returns a single character struct for this textension at a given index.
        /// </summary>
        /// <param name="i">Index of the character you want to fetch</param>
        /// <returns></returns>
        public Character GetUnrevealedCharacter(int i)
        {
            return unrevealedCharacters[i];
        }

        /// <summary>
        /// Changes the TMP text string to the provided message.
        /// </summary>
        /// <param name="message"></param>
        private void SetTextString(string message)
        {
            text.text = message;
        }

        /// <summary>
        /// Returns the length of the string
        /// </summary>
        /// <returns></returns>
        public int GetTextLength()
        {
            return text.text.Length;
        }

        /// <summary>
        /// Returns the TextInfo class for this text
        /// </summary>
        /// <returns></returns>
        public TMP_TextInfo Info()
        {
            return text.textInfo;
        }

        public TMP_Text GetText()
        {
            return text;
        }

        public Color32 GetCachedColor()
        {
            return _cachedColor;
        }

        // TODO: Optimize this function
        /// <summary>
        /// WORK IN PROGRESS
        /// </summary>
        /// <summary>
        /// Note: Remember to update your mesh vertex position data. Update the mesh geometry or use UpdateVertexData() 
        /// </summary>
        /// <param name="character"></param>
        private void UpdateCharacter(Character character)
        {
            Profiler.BeginSample("Update Character");
            
            // Don't update characters that haven't been revealed by a TextReveal.cs or don't update when the character is not visible (Determined by TMP)
            if (!character.isRevealed || !character.Info().isVisible)
            {
                // Early exit and also necessary for spaces
                return;
            }
            
            // Clean the update flags
            character.updatePosition = false;
            character.updateRotation = false;
            character.updateScale = false;

            // TODO: Move these 2 lines outside of this function since they aren't specific to this character (It can probably just done once in initialization)
            // Get the characters material vertices from the texts mesh
            Vector3[] originalVertices = _originalMesh[character.Info().materialReferenceIndex].vertices;
            // Make a copy of those verts
            _targetVertices = originalVertices;

            int index = character.Info().vertexIndex;

            // TODO: Update at character height on the text. Don't scale from the center of the mesh since some characters render at different heights.
            // Locate the center of the mesh (Bottom left + top right) / 2
            Vector3 characterOrigin = (originalVertices[index + 0] + originalVertices[index + 3]) / 2;
            characterOrigin.y = 0;

            // Subtract the center of the mesh from target verts
            _targetVertices[index + 0] -= characterOrigin;
            _targetVertices[index + 1] -= characterOrigin;
            _targetVertices[index + 2] -= characterOrigin;
            _targetVertices[index + 3] -= characterOrigin;

            // Scale the mesh of this character using it's own data
            Matrix4x4 matrix = Matrix4x4.TRS(character.position, character.rotation, character.scale);
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
            DirtyVertex();

            Profiler.EndSample();
        }

        /// <summary>
        /// Takes the modified mesh data and apply it to our text, 
        /// </summary>
        private void ApplyMeshChanges()
        {
            // TODO: See if we need this
            if (_targetVertices.Length <= 0) return;

            // Pass in the modified data back into the text
            text.textInfo.meshInfo[0].mesh.vertices = _targetVertices;

            // Refresh data to render correctly
            text.UpdateGeometry(Info().meshInfo[0].mesh, 0);

            // Cache data for next time we need to apply changes.
            _originalMesh = Info().CopyMeshInfoVertexData();
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
    }
}