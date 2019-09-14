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

        /// <summary>
        /// If this bool is on, then the character will tell itself to update and rebuild despite not being modified.
        /// </summary>
        /// <summary>
        /// Note: This is good for live editing in the editor.
        /// </summary>
        public bool forceUpdate;

        public bool isDirty;
        [HideInInspector] public bool hasPositionUpdated;
        [HideInInspector] public bool hasRotationUpdated;
        [HideInInspector] public bool hasScaleUpdated;

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
        /// Dirty this character so it can rebuild later on in the lifecycle/tick.
        /// </summary>
        public void DirtyCharacter()
        {
            isDirty = true;
        }
        
        /// <summary>
        /// Adds a vector3 to our cached position.
        /// </summary>
        /// <param name="position"></param>
        public void AddPosition(Vector3 position)
        {
            hasPositionUpdated = true;
            _positionCached += position;
        }

        /// <summary>
        /// Subtracts a vector3 from our cached position.
        /// </summary>
        /// <param name="position"></param>
        public void RemovePosition(Vector3 position)
        {
            hasPositionUpdated = true;
            _positionCached -= position;
        }

        /// <summary>
        /// Directly sets the position of this character.
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(Vector3 position)
        {
            hasPositionUpdated = true;
            this.position = position;
        }

        /// <summary>
        /// If our character position has been dirtied, we will move the character to the new position and reset the cached position.
        /// </summary>
        public void ApplyPosition()
        {
            if (hasPositionUpdated)
            {
                hasPositionUpdated = false;
                position = _positionCached;
                _positionCached = Vector3.zero;
            }
        }

        /// <summary>
        /// Multiples a quaternion rotation to our cached rotation.
        /// </summary>
        /// <param name="rotation"></param>
        public void AddRotation(Quaternion rotation)
        {
            hasRotationUpdated = true;
            _rotationCached *= rotation;
        }

        /// <summary>
        /// Multiplies the provided rotation with the inverse of our cached rotation.
        /// </summary>
        /// <param name="rotation"></param>
        public void RemoveRotation(Quaternion rotation)
        {
            hasRotationUpdated = true;
            _rotationCached = rotation * Quaternion.Inverse(_rotationCached);
        }

        /// <summary>
        /// Directly sets the rotation of this character.
        /// </summary>
        /// <param name="rotation"></param>
        public void SetRotation(Quaternion rotation)
        {
            hasRotationUpdated = true;
            this.rotation = rotation;
        }

        /// <summary>
        /// If our character rotation has been dirtied, we will rotate the character to our new rotation and reset the cached rotation.
        /// </summary>
        public void ApplyRotation()
        {
            if (hasRotationUpdated)
            {
                hasRotationUpdated = false;
                rotation = _rotationCached;
                _rotationCached = Quaternion.identity;
            }
        }

        /// <summary>
        /// Adds a vector3 to our cached scale.
        /// </summary>
        /// <param name="scale"></param>
        public void AddScale(Vector3 scale)
        {
            hasScaleUpdated = true;
            _scaleCached += scale;
        }

        /// <summary>
        /// Subtracts a vector3 from our cached scale.
        /// </summary>
        /// <param name="scale"></param>
        public void RemoveScale(Vector3 scale)
        {
            hasScaleUpdated = true;
            _scaleCached -= scale;
        }

        /// <summary>
        /// Directly sets the scale of this character.
        /// </summary>
        /// <param name="scale"></param>
        public void SetScale(Vector3 scale)
        {
            hasScaleUpdated = true;
            this.scale = scale;
        }

        /// <summary>
        /// If our character scale has been dirtied, we will scale the character to our new scale and reset the cached scale.
        /// </summary>
        public void ApplyScale()
        {
            if (hasScaleUpdated)
            {
                hasScaleUpdated = false;
                scale = _scaleCached;
                _scaleCached = Vector3.zero;
            }
        }

        /// <summary>
        /// Marks this character as revealed. (Dirty Flag)
        /// </summary>
        public void Reveal()
        {
            isRevealed = true;
        }

        /// <summary>
        /// Marks this character as unrevealed. (Dirty Flag)
        /// </summary>
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