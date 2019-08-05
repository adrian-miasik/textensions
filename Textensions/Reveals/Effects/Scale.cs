// Author: Adrian Miasik
// Personal Portfolio: http://AdrianMiasik.com
// Github Account: https://github.com/AdrianMiasik

using Textensions.Core;
using Textensions.Effects.Base;
using UnityEngine;

namespace Textensions.Reveals.Effects
{
	[CreateAssetMenu(menuName = "Text Reveals/Effects/Scale", fileName = "New Scale Effect")]
	public class Scale: Effect
	{
        public override float Calculate(Character c)
		{
            return uniform.Evaluate(c.timeSinceReveal);
		}
	}
}