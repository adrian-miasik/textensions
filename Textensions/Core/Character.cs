// Author: Adrian Miasik
// Personal Portfolio: http://AdrianMiasik.com
// Github Account: https://github.com/AdrianMiasik

using TMPro;

namespace Textensions.Core
{
	public class Character
	{
		public float timeSinceReveal;
		public bool isRevealed = false;
		private TMP_CharacterInfo _info;
		public float cachedScale;

		public Character(TMP_CharacterInfo info)
		{
			_info = info;
		}

		public TMP_CharacterInfo Info()
		{
			return _info;
		}
	}
}