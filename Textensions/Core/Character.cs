// Author: Adrian Miasik
// Personal Portfolio: http://AdrianMiasik.com
// Github Account: https://github.com/AdrianMiasik

using System;
using TMPro;
using UnityEngine;

namespace Textensions.Core
{
    // TODO: Ability to explicitly set the position, rotation, and/or scale of this character for one frame. (So we set variables directly)
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

        /// <summary>
        /// If this bool is on, then the character will tell itself to update and rebuild despite not being modified.
        /// </summary>
        /// <summary>
        /// Note: This is good for live editing in the editor.
        /// </summary>
        public bool forceUpdate;
        
        public bool hasPositionUpdated;
        public bool hasRotationUpdated;
        public bool hasScaleUpdated;
        
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

        /// <summary>
        /// Flags all position, rotation, and scale flags as being modified this frame. Causing rebuilding later on in the lifecycle/tick.
        /// </summary>
        public void DirtyCharacter()
        {
            hasPositionUpdated = true;
            hasRotationUpdated = true;
            hasScaleUpdated = true;
        }
        
        public void AddPosition(Vector3 position)
        {
            hasPositionUpdated = true;
            _positionCached += position;
        }
        
        public void RemovePosition(Vector3 position)
        {
            hasPositionUpdated = true;
            _positionCached -= position;
        }

        public void ApplyPosition()
        {
            if (hasPositionUpdated)
            {
                hasPositionUpdated = false;
                position = _positionCached;
                _positionCached = Vector3.zero;
            }
        }
        
        public void AddRotation(Quaternion rotation)
        {
            hasRotationUpdated = true;
            _rotationCached *= rotation;
        }
        
        public void RemoveRotation(Quaternion rotation)
        {
            hasRotationUpdated = true;
            _rotationCached = rotation * Quaternion.Inverse(_rotationCached);
        }

        public void ApplyRotation()
        {
            if (hasRotationUpdated)
            {
                hasRotationUpdated = false;
                rotation = _rotationCached;
                _rotationCached = Quaternion.identity;
            }
        }

        public void AddScale(Vector3 scale)
        {
            hasScaleUpdated = true;
            _scaleCached += scale;
        }
        
        public void RemoveScale(Vector3 scale)
        {
            hasScaleUpdated = true;
            _scaleCached -= scale;
        }

        public void ApplyScale()
        {
            if (hasScaleUpdated)
            {
                hasScaleUpdated = false;
                scale = _scaleCached;
                _scaleCached = Vector3.zero;
            }
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