using Textensions.Core;
using Textensions.Effects.Base;
using UnityEngine;

namespace Textensions.Reveals.Effects
{
	[CreateAssetMenu(menuName = "Text Reveals/Effects/Translate", fileName = "New Translate Effect")]
	public class Translate : Effect
	{
		public bool applyOnX;
		public bool applyOnY;
		public bool applyOnZ;

		public override void Calculate(Character _character)
		{
			// Allocate memory for the movement
			Vector3 result = Vector3.zero;

			// X
			if (applyOnX) result.x = m_uniform.Evaluate(_character.timeSinceReveal);
			// Y
			if (applyOnY) result.y = m_uniform.Evaluate(_character.timeSinceReveal);
			// Z
			if (applyOnZ) result.z = m_uniform.Evaluate(_character.timeSinceReveal);

			// Move the character
			_character.AddPosition(result);
		}
	}
}