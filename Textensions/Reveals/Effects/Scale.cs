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
        public bool isEffectOver = false;

        public override float Calculate(Character c)
		{
            Debug.Log(isEffectOver);
            if (isEffectOver)
            {
                // TODO: Destroy effect
                Debug.Log("TODO: Destroy effect");
                return 0f;
            }
//            
			// If our current character is not revealed or not visible, then skip it.
            if (!c.isRevealed || !c.Info().isVisible) return 0f;

            // Accumulate time to this specific character
			c.timeSinceReveal += Time.deltaTime;
            
            // TODO: Detect when this effect is over (We will then take that data to remove certain effects that are not playing)

//            if (cachedCalculatedValue - uniform.Evaluate(c.timeSinceReveal) == 0)
//            {
//                isEffectOver = true;
//                Debug.Log("No change in previous frame");
//            }

//            Debug.Log(uniform[uniform.length - 1].time);

            if (c.timeSinceReveal > uniform[uniform.length - 1].time)
            {
//                isEffectOver = true;
                Debug.Log("Effect has completed");
                isEffectOver = true;
            }
            
            cachedCalculatedValue = uniform.Evaluate(c.timeSinceReveal);
            
			return cachedCalculatedValue;
		}
	}
}