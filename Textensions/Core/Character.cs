// Author: Adrian Miasik
// Personal Portfolio: http://AdrianMiasik.com
// Github Account: https://github.com/AdrianMiasik

using System;
using TMPro;
using UnityEngine;

namespace Textensions.Core
{
    [Serializable]
	public class Character
	{
		public float timeSinceReveal;
		public bool isRevealed = false;
		private TMP_CharacterInfo _info;
        
        /// <summary>
        /// Index position within the given text component. E.g. ("Hello", "o" would be index 4)
        /// </summary>
        public int index;
        
        private Vector3 _position;
        private Quaternion _rotation;
        private Vector3 _scale;
        
        public void AddPosition(Vector3 position) {
            _position += position;
        }
        
        public void RemovePosition(Vector3 position) {
            _position -= position;
        }
        
        public void AddRotation(Quaternion rotation) {
            _rotation *= rotation;
        }
        
        public void RemoveRotation(Quaternion rotation) {
            _rotation = rotation * Quaternion.Inverse(_rotation);
        }

        public void AddScale(Vector3 scale) {
            _scale += scale;
        }
        
        public void RemoveScale(Vector3 scale) {
            _scale -= scale;
        }
        
		public Character(TMP_CharacterInfo info)
		{
			_info = info;
		}

		public TMP_CharacterInfo Info()
		{
			return _info;
		}
    }
}