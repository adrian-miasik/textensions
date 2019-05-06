// Author: Adrian Miasik
// Personal Portfolio: http://AdrianMiasik.com
// Github Account: https://github.com/AdrianMiasik

using UnityEngine;

namespace Textensions.Core
{
	public abstract class Effect : ScriptableObject
	{
		public abstract float Calculate(Character character);
	}
}