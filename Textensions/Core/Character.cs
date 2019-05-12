// Author: Adrian Miasik
// Personal Portfolio: http://AdrianMiasik.com
// Github Account: https://github.com/AdrianMiasik

using System;
using TMPro;

namespace Textensions.Core
{
    [Serializable]
	public class Character
	{
		public float timeSinceReveal;
		public bool isRevealed = false;
		private TMP_CharacterInfo _info;
		public float cachedScale;
        
        /// <summary>
        /// Index position within the given text component. E.g. ("Hello", "o" would be index 4)
        /// </summary>
        public int index;

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