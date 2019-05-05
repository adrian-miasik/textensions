// Author: Adrian Miasik
// Personal Portfolio: http://AdrianMiasik.com
// Github Account: https://github.com/AdrianMiasik

using Textensions.Reveals.Base;
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
			if (!c.IsRevealed || !c.Info().isVisible) return 0f;

			// Accumulate time to this specific character
			c.timeSinceReveal += Time.deltaTime;

			return uniform.Evaluate(c.timeSinceReveal);
		}
	}
}