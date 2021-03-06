using Textensions.Core;
using UnityEngine;

namespace Textensions.Effects.Base
{
	public abstract class Effect : ScriptableObject
	{
		public enum Style
		{
			/// <summary>
			/// This effect will apply to all characters in this textension
			/// </summary>
			ALL_CHARACTERS,

			/// <summary>
			/// This effect will only apply to characters in odd index positions.
			/// </summary>
			ALL_ODD_CHARACTERS,

			/// <summary>
			/// This effect will only apply to characters in even index positions.
			/// </summary>
			ALL_EVEN_CHARACTERS,
			CUSTOM_INDICES
		}

		public Style m_style;
		public string m_title = "Default Effect Title";
		public AnimationCurve m_uniform;

		public abstract void Calculate(Character character);
	}
}