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

            // If we are iterating through properties, we can ignore the first property reference;
            // Which is the script reference.
            int numberOfVisibleProperties = iterator.CountRemaining() - 1;

            // The iterator needs to be reset when calling CountRemaining.
            iterator.Reset();

            // Skip and check our first reference
            if (iterator.NextVisible(true).GetType() == copiedObject.GetType())
            {
                Debug.Log("These are the same objects");
                // TODO: Native Unity copy and paste
                return;
            }
            
            for (int i = 0; i < numberOfVisibleProperties; i++)
            {
                // If we have a visible property...
                if (iterator.NextVisible(true))
                {
                    SerializedProperty property = target.FindProperty(iterator.name);

                    // If our property can't be found, move on to the next property
                    if (property == null) 
                        continue;

                    // If our types don't match, move on to the next property
                    if (!DoPropertyTypesMatch(property, iterator)) 
                        continue;
                    
                    target.CopyFromSerializedProperty(iterator);
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