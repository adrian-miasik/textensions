using Textensions.Core;
using Textensions.Effects.Base;
using UnityEngine;

namespace Textensions.Effects
{
    public class CharacterScaleEffect : MonoBehaviour
    {
        public Textension textension;
        public Effect fx;

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
            textension.effectsToApply.Add(fx);
        }
    }
}
