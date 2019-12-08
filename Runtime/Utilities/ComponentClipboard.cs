using UnityEditor;
using UnityEngine;

namespace Textensions.Utilities
{
	public static class ComponentClipboard
	{
		private static SerializedObject copiedObject;
		
		/// <summary>
		/// Create a serialized copy of our target component.
		/// </summary>
		/// <param name="_component"></param>
		public static void CopyComponent(Component _component)
		{
			copiedObject = new SerializedObject(_component);
		}

		/// <summary>
		/// Paste any matching property/properties from our copied object component to our target component.
		/// </summary>
		/// <param name="_component"></param>
		public static void PasteComponent(Component _component)
		{
			SerializedObject target = new SerializedObject(_component);
			SerializedProperty iterator = copiedObject.GetIterator();

			int numberOfVisibleProperties = iterator.CountRemaining();
			iterator.Reset(); // Needs to be reset when calling CountRemaining.
            
            // TODO: Check if the component is exactly the same instead of checking all properties
            
			for (int i = 0; i < numberOfVisibleProperties; i++)
			{
				// If we have a visible property...
                if (iterator.NextVisible(true))
                {
					SerializedProperty property = target.FindProperty(iterator.name);

					// If our property can't be found, move on to the next one.
					if (property == null) 
						continue;
                    
					if (DoPropertyTypesMatch(property, iterator))
					{
                        Debug.Log("Attempting to copy variable: " + property.displayName);
                        
                        // TODO: Fix copy
						target.CopyFromSerializedProperty(iterator);
					}
				}
				// No more visible properties found
				else
				{
					iterator.Reset();
					break;
				}
			}

			target.ApplyModifiedProperties();
		}

		private static bool DoPropertyTypesMatch(SerializedProperty _propertyA, SerializedProperty _propertyB)
		{
			return _propertyA.propertyType == _propertyB.propertyType;
		}
	}
}