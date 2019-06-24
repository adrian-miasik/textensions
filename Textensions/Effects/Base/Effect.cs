// Author: Adrian Miasik
// Personal Portfolio: http://AdrianMiasik.com
// Github Account: https://github.com/AdrianMiasik

using System.Collections.Generic;
using Textensions.Core;
using UnityEngine;

namespace Textensions.Effects.Base
{
	public abstract class Effect : ScriptableObject {

        public string title = "Default Effect Title";
        
        public enum Style
        {
            ALL_CHARACTERS,
            ALL_ODD_CHARACTERS,
            ALL_EVEN_CHARACTERS,
            CUSTOM_INDICES
        }

        public Style style;
        
        public abstract float Calculate(Character character);

        protected float cachedCalculatedValue;
    }
}