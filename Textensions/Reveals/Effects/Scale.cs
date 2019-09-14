using Textensions.Core;
using Textensions.Effects.Base;
using UnityEngine;

namespace Textensions.Reveals.Effects
{
	[CreateAssetMenu(menuName = "Text Reveals/Effects/Scale", fileName = "New Scale Effect")]
	public class Scale : Effect
	{
		public override void Calculate(Character _character)
		{
			_character.AddScale(m_uniform.Evaluate(_character.timeSinceReveal) * Vector3.one);
		}
	}
}