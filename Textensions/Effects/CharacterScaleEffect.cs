using System.Collections.Generic;
using Textensions.Core;
using Textensions.Effects.Base;
using UnityEngine;

namespace Textensions.Effects
{
    public class CharacterScaleEffect : MonoBehaviour
    {
        public Textension textension;
        public List<Effect> fxs;

        private void Reset()
        {
            textension = GetComponent<Textension>();
        }

        private void Start()
        {
            AddEffectToCharacters();
        }

        private void AddEffectToCharacters()
        {
            textension.AddEffects(fxs);
        }
    }
}
