using Textensions.Core;
using Textensions.Effects.Base;
using UnityEngine;

namespace Textensions.Reveals.Effects
{
	[CreateAssetMenu(menuName = "Text Reveals/Effects/Translate", fileName = "New Translate Effect")]
	public class Translate: Effect
    {
        public bool applyOnX;
        public bool applyOnY;
        public bool applyOnZ;

		public override void Calculate(Character character)
        {
            // Allocate memory for the movement
            Vector3 result = Vector3.zero;

            // X
            if (applyOnX)
            {
                result.x = uniform.Evaluate(character.timeSinceReveal);
            }
            // Y
            if (applyOnY)
            {
                result.y = uniform.Evaluate(character.timeSinceReveal);
            }
            // Z
            if (applyOnZ)
            {
                result.z = uniform.Evaluate(character.timeSinceReveal);
            }

            // Move the character
            character.AddPosition(result);
		}
	}
}