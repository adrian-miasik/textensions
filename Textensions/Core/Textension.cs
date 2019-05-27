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

        public bool isRunning = true;
        
        [Tooltip("Filled-in if you want to hide the text when it's initialized. If filled in then you are probably looking to attach a reveal to the textension.")]
        public bool hideOnInitialization = true;

        // TODO: Convert to array?
        public List<Character> characters = new List<Character>();
        public List<Character> unrevealedCharacters = new List<Character>();

        public List<Effect> effectsToApply = new List<Effect>();

        // PRIVATE MEMBERS
        private bool _refreshVertex;

        private TMP_MeshInfo[] _originalMesh;
        private Vector3[] _targetVertices = new Vector3[0];
        [SerializeField] private Color32 cachedColor;

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
            cachedColor = text.color;
            _originalMesh = text.textInfo.CopyMeshInfoVertexData();

            // Clear our lists
            characters.Clear();
            unrevealedCharacters.Clear();

            // Create and cache a character class (This will be used for later to keep track of effects and each character state)
            for (int i = 0; i < text.text.Length; i++)
            {
                Character character = new Character(text.textInfo.characterInfo[i]) {index = i};
                characters.Add(character);
                character.cs.scale = characterScale;

                // If this character is visible (like not a space)
                if (character.Info().isVisible)
                {
                    // Add our effects to the character
                    foreach (Effect fx in effectsToApply)
                    {
                        character.AddEffect(fx);
                    }
                }

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
        }

        // TODO: Create and enforce application loop as described below
        private void Update()
        {
            if (isRunning)
            {
                // Step 0: Init / Re-init dirty text
                // TODO: Don't let each component access Initialize() since we might have them invoke it more than once per frame.

                // Step 1: Let each reveal do its thing
                RevealTick?.Invoke();
                
                // Step 2: Calculate the effect for each character
                EffectTick();

                // Step 3: Then finally take all that information and apply it to the TMP_Text only once this frame
                Render();
            }
        }

        // TODO: Improve performance & architecture
        private void EffectTick()
        {
            // Iterate through all the characters
            for (int i = 0; i < GetCharacters().Count; i++)
            {
                // If our character has no effects on it...
                if (GetCharacter(i).GetEffectsCount() <= 0)
                {
//                    GetCharacter(i).SetScale(characterScale);
                    
                    // Go to the next character
                    continue;
                }

                // Iterate through all the effects on this character...
                for (int j = 0; j < GetCharacter(i).GetEffectsCount(); j++)
                {
                    // Set scale to that individual character using it's own data
                    GetCharacter(i).AddScale(GetCharacter(i).GetEffect(j).Calculate(GetCharacter(i)) * Vector3.one - GetCharacter(i).cs.scale);

                    // Update the character mesh data
                    UpdateCharacter(GetCharacter(i));
                }
            }
        }

        public void DirtyVertex()
        {
            _refreshVertex = true;
        }

        private void Render()
        {
            if (_refreshVertex)
            {
                ApplyMeshChanges();
                _refreshVertex = false;
            }
        }

        private void ApplyMeshChanges()
        {
            if (_targetVertices.Length <= 0) return;

//            text.textInfo.meshInfo[0].mesh.SetVertices(_targetVertices.ToList());

            // Pass in the modified data back into the text
            text.textInfo.meshInfo[0].mesh.vertices = _targetVertices;

            // Refresh data to render correctly
            text.UpdateGeometry(Info().meshInfo[0].mesh, 0);

            // Cache data for next time we need to apply changes.
            _originalMesh = Info().CopyMeshInfoVertexData();
        }

        /// <summary>
        /// Returns a list of character classes for this textension.
        /// </summary>
        /// <returns></returns>
        public List<Character> GetCharacters()
        {
            return characters;
        }

        /// <summary>
        /// Returns a list of character classes that have not been revealed.
        /// </summary>
        /// <returns></returns>
        public List<Character> GetUnrevealedCharacters()
        {
            return unrevealedCharacters;
        }

        /// <summary>
        /// Returns a single character class for this textension at a given index.
        /// </summary>
        /// <param name="i">Index of the character you want to fetch</param>
        /// <returns></returns>
        public Character GetCharacter(int i)
        {
#if DEBUG_TEXT
            if (characters[i].isRevealed)
            {
                Debug.LogWarning("This character has already been revealed.");
                return null;
            }
#endif
            return characters[i];
        }

        public int GetIndexOfLastCharacter()
        {
            return characters.Count - 1;
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
            return cachedColor;
        }

        // TODO: Optimize this function
        /// <summary>
        /// WORK IN PROGRESS
        /// </summary>
        /// <summary>
        /// Note: Remember to update your mesh vertex position data. Update the mesh geometry or use UpdateVertexData() 
        /// </summary>
        /// <param name="character"></param>
        public void UpdateCharacter(Character character)
        {
            Profiler.BeginSample("Update Character");

            // Early exit and also necessary for spaces
            if (!character.isRevealed || !character.Info().isVisible)
            {
                return;
            }

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

            // Scale the mesh of this character using it's own data
            Matrix4x4 matrix = Matrix4x4.TRS(character.cs.position, character.cs.rotation, character.cs.scale);
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