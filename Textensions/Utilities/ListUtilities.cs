using System;
using System.Collections.Generic;

namespace Textensions.Utilities
{
	public static class ListUtilities
	{
		/// <summary>
		/// Returns the provided list in a shuffled order using the Fisher yates algorithm
		/// </summary>
		/// <param name="list"></param>
		/// <typeparam name="T"></typeparam>
		public static List<T> Shuffle<T>(List<T> list)
		{
			Random rng = new Random();
			int length = list.Count;

			while (length > 1)
			{
				int i = rng.Next(length--);
				T temp = list[length];
				list[length] = list[i];
				list[i] = temp;
			}

            return list;
        }
	}
}