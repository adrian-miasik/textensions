using Textensions.Core;
using Textensions.Effects.Base;
using UnityEngine;

namespace Textensions.Reveals.Effects
{
	[CreateAssetMenu(menuName = "Text Reveals/Effects/Scale", fileName = "New Scale Effect")]
	public class Scale: Effect
	{
        public override void Calculate(Character character)
		{
            character.AddScale(uniform.Evaluate(character.timeSinceReveal) * Vector3.one);
        }
    }
}