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
        public bool isRevealed;
        public bool effectCompleted;
        private TMP_CharacterInfo _info;

        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        
        private Vector3 _positionCached;
        private Quaternion _rotationCached;
        private Vector3 _scaleCached;
        
        public bool updatePosition;
        public bool updateRotation;
        public bool updateScale;
        
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
            effectCompleted = false;
            isRevealed = false;
        }

        // TODO: Revisit
        public void AddPosition(Vector3 position)
        {
            updatePosition = true;
            this.position += position;
        }

        // TODO: Revisit
        public void RemovePosition(Vector3 position)
        {
            updatePosition = true;
            this.position -= position;
        }

        // TODO: Revisit
        public void AddRotation(Quaternion rotation)
        {
            updateRotation = true;
            this.rotation *= rotation;
        }

        // TODO: Revisit
        public void RemoveRotation(Quaternion rotation)
        {
            updateRotation = true;
            this.rotation = rotation * Quaternion.Inverse(this.rotation);
        }

        public void AddScale(Vector3 scale)
        {
            updateScale = true;
            _scaleCached += scale;
        }

        public void ApplyScale()
        {
            if (updateScale)
            {
                updateScale = false;
                scale = _scaleCached;
                _scaleCached = Vector3.zero;
            }
        }

        // TODO: Revisit
        public void SetScale(Vector3 scale)
        {
            updateScale = true;
            this.scale = scale;
        }

        // TODO: Revisit
        public void RemoveScale(Vector3 scale)
        {
            updateScale = true;
            this.scale -= scale;
        }

        public void Reveal()
        {
            isRevealed = true;
        }

        public void Unreveal()
        {
            isRevealed = false;
        }

        // Getters have a performance overhead when they are not optimized out. Most getters and setters are optimized out by the compiler anyways
        public TMP_CharacterInfo Info()
        {
            return _info;
        }
    }
}