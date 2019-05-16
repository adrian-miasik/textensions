// Author: Adrian Miasik
// Personal Portfolio: http://AdrianMiasik.com
// Github Account: https://github.com/AdrianMiasik

using Textensions.Core;
using UnityEngine;

namespace Textensions.Effects.Base
{
	public abstract class Effect : ScriptableObject
	{
		public abstract float Calculate(Character character);
	}
}