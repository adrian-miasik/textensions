using System.IO;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Textensions.Editor.Utilities
{
    // TODO: Expand and utilize
    /// <summary>
    /// A static class of helper functions related to global USS properties for Textensions
    /// </summary>
    public static class TextensionStyling
    {
        /// <summary>
        /// Gives this element `font : chivo-bold`.
        /// </summary>
        /// <param name="_element"></param>
        public static void MarkBold(VisualElement _element)
        {
            _element.AddToClassList("chivo-bold");
        }

        /// <summary>
        /// Removes the <see cref="MarkBold"/> effect on the element.
        /// </summary>
        /// <param name="_element"></param>
        public static void CleanBold(VisualElement _element)
        {
            _element.RemoveFromClassList("chivo-bold");
        }

        public static void InitializeObjectField(ObjectField _objectField)
        {
            _objectField.objectType = typeof(TMP_Text);
        }
    }
}
