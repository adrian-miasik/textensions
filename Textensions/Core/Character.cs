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
    public struct CharacterStruct
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public CharacterStruct(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
    }

    [Serializable]
    public class Character
    {
        public float timeSinceReveal;
        public bool isRevealed = false;
        private TMP_CharacterInfo _info;

        private List<Effect> effects = new List<Effect>();

        public CharacterStruct cs;

        /// <summary>
        /// Index position within the given text component. E.g. ("Hello", "o" would be index 4)
        /// </summary>
        public int index;

        public void AddPosition(Vector3 position)
        {
            cs.position += position;
        }

        public void RemovePosition(Vector3 position)
        {
            cs.position -= position;
        }

        public void AddRotation(Quaternion rotation)
        {
            cs.rotation *= rotation;
        }

        public void RemoveRotation(Quaternion rotation)
        {
            cs.rotation = rotation * Quaternion.Inverse(cs.rotation);
        }

        public void AddScale(Vector3 scale)
        {
            cs.scale += scale;
        }

        public void SetScale(Vector3 scale)
        {
            cs.scale = scale;
        }

        public void RemoveScale(Vector3 scale)
        {
            cs.scale -= scale;
        }

        public void AddEffect(Effect fx)
        {
            effects.Add(fx);
        }

        /// <summary>
        /// Returns an effect at a given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Effect GetEffect(int index)
        {
            return effects[index];
        }

        public int GetEffectsCount()
        {
            return effects.Count;
        }

        public Character(TMP_CharacterInfo info)
        {
            _info = info;
            cs = new CharacterStruct(Vector3.zero, Quaternion.identity, Vector3.one);
        }

        public TMP_CharacterInfo Info()
        {
            return _info;
        }
    }
}