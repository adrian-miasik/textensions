using System;
using System.IO;
using Textensions.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Textensions.Editor
{
	// This script is for the textension wizard component
	[CustomEditor(typeof(TextensionWizard))]
	public class TextensionWizardEditor : UnityEditor.Editor
	{
		private const string LOGO_PATH =
			"Packages/com.adrianmiasik.textensions/Textensions/Sprites/TextensionLogo128x128.png";

		private const string STATUS_INDICATOR_PATH =
			"Packages/com.adrianmiasik.textensions/Textensions/Sprites/StatusIndicator10x10.png";

		/// <summary>
		/// The class that has the state data
		/// </summary>
		private TextensionWizard textensionWizard;

		/// <summary>
		/// The result element that we want to render and is used to tell the inspector what to render.
		/// <see cref="CreateInspectorGUI"/>
		/// </summary>
		private VisualElement resultElement;

		/// <summary>
		/// The visual tree uxml structure this script is targeting
		/// </summary>
		private VisualTreeAsset uxmlReference;

		/// <summary>
		/// The properties we don't want the default inspector to draw
		/// </summary>
		private string[] propertiesToExclude = {"m_Script"};

		private Texture2D logo;
		private Texture2D statusIndicator;

		/// <summary>
		/// The logo visual element
		/// </summary>
		private VisualElement logoVE;

		/// <summary>
		/// The status indicator at the top right
		/// </summary>
		private Image statusIndicatorVE;

		private Color32 assetColor = new Color32(255,28,41, 255);
		private Color32 warningColor = new Color32(255,136,51, 255);
		private Color32 successColor = new Color32(44,255,99, 255);

		private enum StatusCodes
		{
			NULL,
			ASSERT,
			WARNING,
			SUCCESS
		}

		private StatusCodes status;

		public void OnEnable()
		{
			// Cache the script
			textensionWizard = (TextensionWizard) target;

			// Cache the uxml structure reference so we can pull out the visual elements as necessary when needed
			uxmlReference = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
				("Packages/com.adrianmiasik.textensions/Textensions/Editor/TextensionWizardEditor.uxml");

			// Create a new visual element object that we will use to draw every inspector rebuild
			resultElement = new VisualElement();

			// Link the style sheet to the result element so each element within the result element is styled properly
			resultElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>
				("Packages/com.adrianmiasik.textensions/Textensions/Editor/TextensionWizardEditor.uss"));

			resultElement.AddToClassList("root");

			// Default
			status = StatusCodes.SUCCESS;

			// If our image asset exists...
			if (File.Exists(LOGO_PATH))
			{
				// Create a 2D Texture
				Texture2D logoTexture = new Texture2D(32, 32);

				// Load image into 2D Texture (Convert .png to a texture2D)
				logoTexture.LoadImage(File.ReadAllBytes(LOGO_PATH));

				// Cache the new logo 2D Texture
				logo = logoTexture;
			}

			// If our image asset exists...
			if (File.Exists(STATUS_INDICATOR_PATH))
			{
				// Create a 2D Texture
				Texture2D statusIndicatorTexture = new Texture2D(10, 10);

				// Load image into 2D Texture (Convert .png to a texture2D)
				statusIndicatorTexture.LoadImage(File.ReadAllBytes(STATUS_INDICATOR_PATH));

				// Cache the new status indicator 2D Texture
				statusIndicator = statusIndicatorTexture;
			}
		}

		public override void OnInspectorGUI()
		{
			// Update our object, don't draw all the properties, and apply the new properties only
			// TL;DR: We don't render all inspector variables
			serializedObject.Update();
			DrawPropertiesExcluding(serializedObject, propertiesToExclude);
			serializedObject.ApplyModifiedProperties();
		}

		public override VisualElement CreateInspectorGUI()
		{
			// Clear the result element before we add stuff to render to it
			resultElement.Clear();

			// Read and draw the uxml structure visual elements to the result element
			uxmlReference.CloneTree(resultElement);

			// Apply the logo
			LoadLogo(logo);

			// Apply the status
			LoadStatus();



			// Show the status of the component (I.e. "Is it fully ready?")
			Compile(status);

			// Render the default inspector
			IMGUIContainer defaultInspector = new IMGUIContainer(OnInspectorGUI);
			resultElement.Add(defaultInspector);

			return resultElement;
		}

		private void Compile(StatusCodes _statusCode)
		{
			switch (_statusCode)
			{
				case StatusCodes.NULL:
					statusIndicatorVE.style.unityBackgroundImageTintColor = new StyleColor(Color.white);
					break;
				case StatusCodes.ASSERT:
					statusIndicatorVE.style.unityBackgroundImageTintColor = new StyleColor(assetColor);
					break;
				case StatusCodes.WARNING:
					statusIndicatorVE.style.unityBackgroundImageTintColor = new StyleColor(warningColor);
					break;
				case StatusCodes.SUCCESS:
					statusIndicatorVE.style.unityBackgroundImageTintColor = new StyleColor(successColor);
					break;
				default:
					Debug.LogWarning("T.ext: Unable to support this status code.");
					break;
			}
		}

		private void LoadStatus()
		{
			// Fetch our status indicator element
			statusIndicatorVE = resultElement.Q("status-indicator") as Image;

			// Load 2D texture into the background image style
			statusIndicatorVE.style.backgroundImage = statusIndicator;
		}

		/// <summary>
		/// Fetches our logoVisualElement and applies the given Texture2D
		/// </summary>
		private void LoadLogo(Texture2D _logo)
		{
			// Fetch our logo element
			logoVE = resultElement.Q("logo");

			// Load 2D texture into the background image style
			logoVE.style.backgroundImage = _logo;
		}
	}
}