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
		public AnimationCurve uniform;

		public override float Calculate(Character c)
		{
			// If our current character is not revealed or not visible, then skip it.
            if (!c.isRevealed || !c.Info().isVisible) return 0f;

            // Accumulate time to this specific character
			c.timeSinceReveal += Time.deltaTime;
            
            // TODO: Detect when this effect is over (We will then take that data to remove certain effects that are not playing)
            
			return uniform.Evaluate(c.timeSinceReveal);
		}
	}
}