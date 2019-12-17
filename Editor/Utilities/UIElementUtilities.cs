using System.IO;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Textensions.Editor.Utilities
{
    /// <summary>
    /// A static class of helper functions related to UI element manipulations.
    /// </summary>
    public static class UIElementUtilities
    {
        /// <summary>
        /// Provides a background image for a specific VisualElement
        /// </summary>
        /// <param name="_target">The visual element that will get a background image</param>
        /// <param name="_imagePath">The directory of the image asset</param>
        /// <param name="_imageWidth">The image width in pixels</param>
        /// <param name="_imageHeight">The image height in pixels</param>
        public static void LoadImageIntoElement(VisualElement _target, string _imagePath, int _imageWidth, int _imageHeight)
        {
            // If our image asset exists...
            if (File.Exists(_imagePath))
            {
                // Create a 2D Texture
                Texture2D _logoTexture = new Texture2D(_imageWidth, _imageHeight);

                // Load image into 2D Texture (Convert .png to a texture2D)
                _logoTexture.LoadImage(File.ReadAllBytes(_imagePath));

                // Load 2D texture into the background image style
                _target.style.backgroundImage = _logoTexture;
            }
            else
            {
                Debug.LogAssertion("T.ext: Unable to find asset in: " + _imagePath);
            }
        }
        public static VisualElement CreateRootElement()
        {
            VisualElement result = new VisualElement();
            result.AddToClassList("root");
            return result;
        }
        
        public static void StyleElement(VisualElement _element, string _styleSheetPath)
        {
            _element.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(_styleSheetPath));
        }
        
        internal static void ShowDisplay(VisualElement _element)
        {
            _element.style.display = DisplayStyle.Flex;
        }

        internal static void HideDisplay(VisualElement _element)
        {
            _element.style.display = DisplayStyle.None;
        }

        internal static void ToggleDisplay(VisualElement _element)
        {
            _element.style.display =
                _element.style.display == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
        }

        internal static void ChangeDisplay(VisualElement _element, DisplayStyle _style)
        {
            _element.style.display = _style;
        }
    }
}