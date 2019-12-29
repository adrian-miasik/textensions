using System;
using TMPro;
using UnityEngine;

namespace Textensions.Core
{
    [Serializable]
    public class Character
    {
        private readonly TMP_CharacterInfo info;

        private Vector3 positionCached;
        private Quaternion rotationCached;
        private Vector3 scaleCached;
        
        // TODO: Utilize this
        public bool effectCompleted;

        [HideInInspector] public bool hasPositionUpdated;
        [HideInInspector] public bool hasRotationUpdated;
        [HideInInspector] public bool hasScaleUpdated;

        /// <summary>
        /// Index position within the given text component. E.g. ("Hello", "o" would be index 4)
        /// </summary>
        public int index;

        public bool isRevealed;

        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public float timeSinceReveal;

        public Character(TMP_CharacterInfo _info)
        {
            this.info = _info;
            position = Vector3.zero;
            rotation = Quaternion.identity;
            scale = Vector3.one;
            timeSinceReveal = 0f;
            index = _info.index;
            effectCompleted = false;
            isRevealed = false;
        }

        public void CleanFlags()
        {
            hasPositionUpdated = false;
            hasRotationUpdated = false;
            hasScaleUpdated = false;
        }

        /// <summary>
        /// Adds a vector3 to our cached position.
        /// </summary>
        /// <param name="_position"></param>
        public void AddPosition(Vector3 _position)
        {
            hasPositionUpdated = true;
            positionCached += _position;
        }

        /// <summary>
        /// Subtracts a vector3 from our cached position.
        /// </summary>
        /// <param name="_position"></param>
        public void RemovePosition(Vector3 _position)
        {
            hasPositionUpdated = true;
            positionCached -= _position;
        }

        /// <summary>
        /// Directly sets the position of this character.
        /// </summary>
        /// <param name="_position"></param>
        public void SetPosition(Vector3 _position)
        {
            hasPositionUpdated = true;
            this.position = _position;
        }

        /// <summary>
        /// If our character position has been dirtied, we will move the character to the new position, reset the cached position, and return True.
        /// </summary>
        /// <summary>
        /// If our character position has not been dirtied, we will return False.
        /// </summary>
        public bool ApplyPosition()
        {
            if (!hasPositionUpdated) return false;
            hasPositionUpdated = false;
            position = positionCached;
            positionCached = Vector3.zero;
            return true;

        }

        /// <summary>
        /// Multiples a quaternion rotation to our cached (quaternion) rotation.
        /// </summary>
        /// <param name="_rotation"></param>
        public void AddRotation(Quaternion _rotation)
        {
            hasRotationUpdated = true;
            rotationCached *= _rotation;
        }

        /// <summary>
        /// Adds the provided euler rotation to our cached (quaternion) rotation.
        /// </summary>
        /// <param name="_rotation"></param>
        public void AddRotation(Vector3 _rotation)
        {
            hasRotationUpdated = true;
            rotationCached *= Quaternion.Euler(_rotation);
        }

        /// <summary>
        /// Multiplies the provided quaternion rotation with the inverse of our cached (quaternion) rotation.
        /// </summary>
        /// <param name="_rotation"></param>
        public void RemoveRotation(Quaternion _rotation)
        {
            hasRotationUpdated = true;
            rotationCached = _rotation * Quaternion.Inverse(rotationCached);
        }

        /// <summary>
        /// Multiplies the provided euler rotation with the inverse of our cached (quaternion) rotation.
        /// </summary>
        /// <param name="_rotation"></param>
        public void RemoveRotation(Vector3 _rotation)
        {
            hasRotationUpdated = true;
            rotationCached = Quaternion.Euler(_rotation) * Quaternion.Inverse(rotationCached);
        }

        /// <summary>
        /// Directly sets the rotation of this character.
        /// </summary>
        /// <param name="_rotation"></param>
        public void SetRotation(Quaternion _rotation)
        {
            hasRotationUpdated = true;
            this.rotation = _rotation;
        }

        /// <summary>
        /// If our character rotation has been dirtied, we will rotate the character to our new rotation, reset the cached rotation, and return True.
        /// </summary>
        /// <summary>
        /// If our character rotation has not been dirtied, we will return False.
        /// </summary>
        public bool ApplyRotation()
        {
            if (hasRotationUpdated)
            {
                hasRotationUpdated = false;
                rotation = rotationCached;
                rotationCached = Quaternion.identity;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a vector3 to our cached scale.
        /// </summary>
        /// <param name="_scale"></param>
        public void AddScale(Vector3 _scale)
        {
            hasScaleUpdated = true;
            scaleCached += _scale;
        }

        /// <summary>
        /// Subtracts a vector3 from our cached scale.
        /// </summary>
        /// <param name="_scale"></param>
        public void RemoveScale(Vector3 _scale)
        {
            hasScaleUpdated = true;
            scaleCached -= _scale;
        }

        /// <summary>
        /// Directly sets the scale of this character.
        /// </summary>
        /// <param name="_scale"></param>
        public void SetScale(Vector3 _scale)
        {
            hasScaleUpdated = true;
            this.scale = _scale;
        }

        /// <summary>
        /// If our character scale has been dirtied, we will scale the character to our new scale, reset the cached scale, and return True.
        /// </summary>
        /// <summary>
        /// If our character scale has not been dirtied, we will return False.
        /// </summary>
        public bool ApplyScale()
        {
            if (hasScaleUpdated)
            {
                hasScaleUpdated = false;
                scale = scaleCached;
                scaleCached = Vector3.zero;
                return true;
            }

            return false;
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
            return info;
        }
    }
}