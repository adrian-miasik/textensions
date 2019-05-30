// Author: Adrian Miasik
// Personal Portfolio: http://AdrianMiasik.com
// Github Account: https://github.com/AdrianMiasik

using System;
using System.Collections.Generic;
using Textensions.Effects.Base;
using TMPro;
using UnityEngine;

namespace Textensions.Core
{
    [Serializable]
    public class Character
    {
        public float timeSinceReveal;
        public bool isRevealed;
        private TMP_CharacterInfo _info;
        
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        
        /// <summary>
        /// Index position within the given text component. E.g. ("Hello", "o" would be index 4)
        /// </summary>
        public int index;

        public Character(TMP_CharacterInfo info)
        {
            _info = info;
            position = Vector3.zero;
            rotation = Quaternion.identity;
            scale = Vector3.one;
            timeSinceReveal = 0f;
            index = info.index;
            isRevealed = false;
        }
        
        public void AddPosition(Vector3 position)
        {
            this.position += position;
        }

        public void RemovePosition(Vector3 position)
        {
            this.position -= position;
        }

        public void AddRotation(Quaternion rotation)
        {
            this.rotation *= rotation;
        }

        public void RemoveRotation(Quaternion rotation)
        {
            this.rotation = rotation * Quaternion.Inverse(this.rotation);
        }

        public void AddScale(Vector3 scale)
        {
            this.scale += scale;
        }

        public void SetScale(Vector3 scale)
        {
            this.scale = scale;
        }

        public void RemoveScale(Vector3 scale)
        {
            this.scale -= scale;
        }
        
        public void Reveal() {
            isRevealed = true;
        }
        
        public void Unreveal() {
            isRevealed = false;
        }
        
        // TODO: Look if there is performance overhead for getters
        public TMP_CharacterInfo Info()
        {
            return _info;
        }
    }
}