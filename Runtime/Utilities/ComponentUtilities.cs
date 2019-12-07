using System;
using UnityEngine;

namespace Textensions.Utilities
{
	public static class ComponentUtilities
	{
		private static Component[] cachedComponents;

		public static void SampleComponents(GameObject _gameObject)
		{
			cachedComponents =	_gameObject.GetComponents<Component>();
		}

		public static int GetComponentIndex(Component _component)
		{
			for (int i = 0; i < GetComponentLength(); i++)
			{
				if (cachedComponents[i] == _component)
				{
					return i;
				}
			}

			throw new Exception("Unable to find component index.");
		}

		public static int GetComponentLength()
		{
			return cachedComponents.Length;
		}
		
		private static bool IsComponentAtTheTop(Component _component)
		{
			return cachedComponents[1] == _component;
		}

		private static bool IsComponentAtTheBottom(Component _component)
		{
			return cachedComponents[GetComponentLength()] == _component;
		}

		/// <summary>
		/// Move a component up by one.
		/// </summary>
		/// <param name="_component"></param>
		public static void MoveComponentUp(Component _component)
		{
			UnityEditorInternal.ComponentUtility.MoveComponentUp(_component);
		}
		
		/// <summary>
		/// Moves a component down by one.
		/// </summary>
		/// <param name="_component"></param>
		public static void MoveComponentDown(Component _component)
		{
			UnityEditorInternal.ComponentUtility.MoveComponentDown(_component);
		}

		/// <summary>
		/// Moves a component upwards by N times.
		/// </summary>
		/// <param name="_component"></param>
		/// <param name="howManySteps"></param>
		public static void MoveComponentUpwards(Component _component, int howManySteps)
		{
			for (int i = 0; i < howManySteps; i++)
			{
				MoveComponentUp(_component);
			}
		}
		
		/// <summary>
		/// Moves a component downwards by N times.
		/// </summary>
		/// <param name="_component"></param>
		/// <param name="howManySteps"></param>
		public static void MoveComponentDownwards(Component _component, int howManySteps)
		{
			for (int i = 0; i < howManySteps; i++)
			{
				MoveComponentDown(_component);
			}
		}
		
		/// <summary>
		/// Moves a component to the top of the component structure.
		/// </summary>
		/// <summary>
		/// Note: Due to the way Unity was built, Unity's Transform / RectTransform component will always be first.
		/// This method will move your component just under that.
		/// </summary>
		/// <param name="_component">The component you want to move</param>
		public static void MoveComponentToTheTop(Component _component)
		{
			for (int i = 0; i < GetComponentLength(); i++)
			{
				if (IsComponentAtTheTop(_component))
				{
					return;
				}

				MoveComponentUp(_component);
			}
		}

		/// <summary>
		/// Moves a component to the bottom of the component structure.
		/// </summary>
		/// <param name="_component">The component you want to move</param>
		public static void MoveComponentToTheBottom(Component _component)
		{
			for (int i = 0; i < GetComponentLength(); i++)
			{
				if (IsComponentAtTheBottom(_component))
				{
					return;
				}

				MoveComponentDown(_component);
			}
		}

		private static int GetDistanceFromTheBottom(Component _component)
		{
			return Mathf.Abs(GetComponentIndex(_component) - (GetComponentLength() - 1));
		}

		// TODO: Refactor this method
		public static void MoveComponentToIndex(Component _component, int _index)
		{
			MoveComponentToTheTop(_component);
			MoveComponentDownwards(_component, _index);
		}
	}
}