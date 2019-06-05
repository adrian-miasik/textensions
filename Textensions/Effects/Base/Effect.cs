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
        public List<int> indexToEffect;
        public abstract float Calculate(Character character);
	}
}