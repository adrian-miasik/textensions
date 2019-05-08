using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Textensions.Core
{
	/// <summary>
	/// A textension is an abstraction layer on top of the TMP 'TMP_Text' class including all it's relevant components. This will be used for later to keep track of effects and each character state.
	/// </summary>
	public class Textension: MonoBehaviour
	{
		// PUBLIC MEMBERS
		// Note: There should only probably be one textension per given text, but that's to be decided.
		public TMP_Text text;
		
		[Tooltip("The base scale of each character.")]
		public float characterScale;
		
		
		// PRIVATE MEMBERS
		// TODO: Convert to array?
		public List<Character> _characters = new List<Character>();
		
		private bool _refreshVertex;
		
		private TMP_MeshInfo[] _originalMesh;
		private Vector3[] _targetVertices = new Vector3[0];
		[SerializeField] private Color32 cachedColor;
		
		private void Start()
		{
			// TODO: Re-Initialize on text change
			Initialize();
		}

		private void Initialize()
		{
			// Force the mesh update so we don't have to wait a frame to get the data.
			// Since we need to get information from the mesh we will have to update the mesh a bit earlier than normal.
			// "TMP generates/processes the mesh once per frame (if needed) just before Unity renders the frame."
			// Source: https://www.youtube.com/watch?v=ZHU3AcyDKik&feature=youtu.be&t=164
			// In most cases it's fine for TMP to render at it's normal timings but as mentioned above if we are going
			// to manipulate or fetch data from the mesh we should force the mesh to update so the data remains accurate.
			text.ForceMeshUpdate();

			cachedColor = text.color;
			_originalMesh = text.textInfo.CopyMeshInfoVertexData();
			
			// Create and cache a character class (This will be used for later to keep track of effects and each character state)
			for (int i = 0; i < text.text.Length; i++)
			{
				Character c = new Character(text.textInfo.characterInfo[i]);
				_characters.Add(c);
			}
		}
		
		// TODO: Create and enforce application loop as described below
		private void Update()
		{
			// Step 0: Init / Reinit dirty text
			
			// Step 1: Let each modifier/component do its thing
			
			// Step 2: Then finally take all that information and apply it to the TMP_Text
			Render();
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

//			text.textInfo.meshInfo[0].mesh.SetVertices(_targetVertices.ToList());
			
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
			return _characters;
		}
		
		/// <summary>
		/// Returns a single character class for this textension at a given index.
		/// </summary>
		/// <param name="i">Index of the character you want to fetch</param>
		/// <returns></returns>
		public Character GetCharacter(int i)
		{
			return _characters[i];
		}

		public int GetIndexOfLastCharacter()
		{
			return _characters.Count - 1;
		}

		/// <summary>
		/// Changes the TMP text string to the provided message.
		/// </summary>
		/// <param name="message"></param>
		public void SetTextString(string message)
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
                // Cache the scale 
                character.cachedScale = scale;
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
            DirtyVertex();
        }
	}
}