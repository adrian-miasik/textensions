using System.Collections.Generic;
using System.IO;
using Textensions.Core;
using TMPro;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Textensions.Editor
{
	public class ConsoleRecorder
	{
		private VisualElement parent;
		private TextElement titleTE;
		private StyleColor titleColor;
		private TextElement textTE;

		private Queue<KeyValuePair<TextensionWizardEditor.StatusCodes, string>> console;

		public ConsoleRecorder(VisualElement _statusRow, TextElement _statusTitle, StyleColor _titleColor,
			TextElement _statusText)
		{
			// Hook up references
			parent = _statusRow;
			titleTE = _statusTitle;
			titleColor = _titleColor;
			textTE = _statusText;

			// Create a console
			console = new Queue<KeyValuePair<TextensionWizardEditor.StatusCodes, string>>();
		}

		public void RecordLog(KeyValuePair<TextensionWizardEditor.StatusCodes, string> _log)
		{
			console.Enqueue(_log);
		}

		public void RecordLog(TextensionWizardEditor.StatusCodes _type, string _message)
		{
			console.Enqueue(new KeyValuePair<TextensionWizardEditor.StatusCodes, string>(_type, _message));
		}

		public void PrintRecords()
		{
			foreach (KeyValuePair<TextensionWizardEditor.StatusCodes, string> log in console)
			{
				Debug.Log(log.Key + ": " + log.Value);
			}
		}

		public VisualElement GenerateRecordVisuals()
		{
			VisualElement result = new VisualElement();

			foreach (KeyValuePair<TextensionWizardEditor.StatusCodes, string> log in console)
			{
				result.Add(titleTE);
			}

			return result;
		}

		public KeyValuePair<TextensionWizardEditor.StatusCodes, string> GetFirstLog()
		{
			return console.Peek();
		}

		/// <summary>
		/// Returns the number of errors, warnings, or successes we've received
		/// </summary>
		/// <returns></returns>
		public int GetLogCount()
		{
			return console.Count;
		}
	}

	// This script is for the textension wizard component
	[CustomEditor(typeof(TextensionWizard))]
	public class TextensionWizardEditor : UnityEditor.Editor
	{
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
		private readonly string[] propertiesToExclude = {"m_Script"};

		// Cache
		private VisualElement statusIndicatorVE;
		private Texture2D logo;
		private VisualElement logoVE;
		private VisualElement statusRowVE;
		private VisualElement wizardContentVE;
		private TextElement statusPrefixTE;
		private TextElement statusTextTE;
		private VisualElement instructionsRowVE;
		private TextElement instructionTitleTE;
		private TextElement instructionTextTE;
		private Button step0;
		private VisualElement step1;
		private ObjectField step1ObjectField;
		private VisualElement step2;
		private Button step2ButtonYes;
		private Button step2ButtonNo;
		private VisualElement direct;
		private ObjectField directTextOF;
		private Toggle directHideOnInitT;
		private VisualElement wizardVE;
		private TextElement directTitleTE;

		private const string LOGO_PATH =
			"Packages/com.adrianmiasik.textensions/Resources/TextensionLogo40x40.png";

		// Colors
		private static Color32 defaultColor = new Color32(255, 255, 255, 255);
		private static Color32 assetColor = new Color32(255, 28, 41, 255);
		private static Color32 warningColor = new Color32(255, 192, 41, 255);
		private static Color32 successColor = new Color32(44, 255, 99, 255);

		// Styles
		private StyleColor defaultStyleColor = new StyleColor(defaultColor);
		private StyleColor assetStyleColor = new StyleColor(assetColor);
		private StyleColor warningStyleColor = new StyleColor(warningColor);
		private StyleColor successStyleColor = new StyleColor(successColor);

		public enum StatusCodes
		{
			NULL,
			ASSERT,
			WARNING,
			SUCCESS
		}

		private ConsoleRecorder console;

		/// <summary>
		/// Queries and caches all our elements needed on creation
		/// </summary>
		private void QueryElements()
		{
			#region Root
			#region Banner/
			logoVE = resultElement.Q("Logo*");
			statusIndicatorVE = resultElement.Q("Status Indicator*");
			#endregion
			statusRowVE = resultElement.Q("Status Row*");
			#region Status Row/
			statusPrefixTE = resultElement.Q<TextElement>("Status Prefix*");
			statusTextTE = resultElement.Q<TextElement>("Status Text*");
			#endregion
			wizardVE = resultElement.Q("Wizard*");
			#region Wizard/Wizard Center
			instructionsRowVE = resultElement.Q("Instructions Row*");
			#region Wizard/Wizard Center/Instructions Row
			instructionTitleTE = resultElement.Q<TextElement>("Instruction Title*");
			instructionTextTE = resultElement.Q<TextElement>("Instruction Text*");
			#endregion
			wizardContentVE = resultElement.Q("Wizard Content*");
			#region Wizard/Wizard Center/Wizard Content
			step0 = resultElement.Q<Button>("Step 0*");
			step1 = resultElement.Q("Step 1*");
			#region Wizard/Wizard Center/Wizard Content/Step 1/
			step1ObjectField = resultElement.Q<ObjectField>("Step 1 Object Field*");
			#endregion
			step2 = resultElement.Q("Step 2*");
			#region Wizard/Wizard Center/Wizard Content/Step 2/
			step2ButtonYes = resultElement.Q<Button>("Step 2 Yes*");
			step2ButtonNo = resultElement.Q<Button>("Step 2 No*");
			#endregion
			#endregion
			#endregion
			direct = resultElement.Q("Direct*");
			#region Direct/
			directTitleTE = resultElement.Q<TextElement>("Direct Title*");
			#region Direct/Direct Center/Direct Content/
			directTextOF = resultElement.Q<ObjectField>("Direct Text*");
			directHideOnInitT = resultElement.Q<Toggle>("Direct Hide On Init*");
			#endregion
			#endregion
			#endregion
		}

		private bool CheckElements()
		{
			bool isSuccessful = true;

			// TODO: Implement console system

			return isSuccessful;
		}

		public void OnEnable()
		{
			// Cache the script
			textensionWizard = (TextensionWizard) target;

			// Cache the uxml structure reference so we can pull out the visual elements when needed as necessary
			uxmlReference = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
				("Packages/com.adrianmiasik.textensions/Editor/TextensionWizardEditor.uxml");

			// Create a new visual element object that we will use to draw every inspector rebuild
			resultElement = new VisualElement();

			// Link the style sheet to the result element so each element within the result element is styled properly
			resultElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>
				("Packages/com.adrianmiasik.textensions/Editor/TextensionWizardEditor.uss"));

			resultElement.AddToClassList("root");
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

			// Create a console
			console = new ConsoleRecorder(statusRowVE, statusPrefixTE, defaultStyleColor,
				statusTextTE);

			// Setup all our references, and null check our queries...
			if (Setup())
			{
				// Load Logo
				LoadImage(logoVE, LOGO_PATH, 40, 40);

				logoVE.RegisterCallback<MouseUpEvent>(_e =>
				{
					textensionWizard.state = TextensionWizard.WizardStates.SPLASH_SCREEN;
					CreateInspectorGUI();
				});

				// Load page
				DeterminePath();

				// Load the default inspector
//				resultElement.Add(new IMGUIContainer(OnInspectorGUI));
			}

			console.PrintRecords();

			return resultElement;
		}

		/// <summary>
		/// Query our elements and checks the validity of our queried data.
		/// </summary>
		/// <returns>Returns true if the setup was successful. Otherwise this will return false</returns>
		private bool Setup()
		{
			// Get references...
			QueryElements();

			// Verify element validity
			return CheckElements();
		}

		// TODO: Refactor and modularize the redundant code
		private void DeterminePath()
		{
			// Remove status
			statusRowVE.style.display = DisplayStyle.None;

			// Remove all steps
			step0.style.display = DisplayStyle.None;
			step1ObjectField.parent.style.display = DisplayStyle.None;
			step2.style.display = DisplayStyle.None;

			// Remove the history
			direct.style.display = DisplayStyle.None;

			// Check state of wizard
			switch (textensionWizard.state)
			{
				case TextensionWizard.WizardStates.NOT_DETERMINED:
					textensionWizard.state = TextensionWizard.WizardStates.SPLASH_SCREEN;
					DeterminePath();

//					console.RecordLog(ConsoleRecorder.MessageType.NULL, "Not determined");
//
//					// Is our required references done?
//					if (!textensionWizard.IsWizardComplete())
//					{
//						// If not...start the wizard
//						textensionWizard.state = TextensionWizard.WizardStates.SPLASH_SCREEN;
//					}

					break;

				case TextensionWizard.WizardStates.SPLASH_SCREEN:

					statusRowVE.style.display = DisplayStyle.None;
					instructionTitleTE.text = "Setup Wizard";
					instructionTextTE.text = "";

					textensionWizard.text = null;
					step0.style.display = DisplayStyle.Flex;

					direct.style.display = DisplayStyle.None;

					// When the user presses the connect button...
					step0.RegisterCallback<MouseUpEvent>(_e =>
					{
						// We will transition to the other state
						textensionWizard.state = TextensionWizard.WizardStates.TEXTENSION_CONNECT;
						DeterminePath();
					});

					direct.style.display = DisplayStyle.Flex;
					directTextOF.SetEnabled(false);
					directTextOF.objectType = typeof(TMP_Text);
					directHideOnInitT.SetEnabled(false);
//					hideOnInitializationRef.Q<VisualElement>().style.justifyContent = Justify.FlexEnd;

					directTextOF.value = textensionWizard.text;
					directTextOF.RemoveFromClassList("active-field");
					directHideOnInitT.RemoveFromClassList("active-field");

					directHideOnInitT.style.display = DisplayStyle.Flex;
					directHideOnInitT.value = textensionWizard.hideOnInitialization;

					break;

				case TextensionWizard.WizardStates.TEXTENSION_CONNECT:

					step1ObjectField.parent.style.display = DisplayStyle.Flex;
					step1ObjectField.Q<Label>().style.minWidth = 0;
					step1ObjectField.objectType = typeof(TMP_Text);

					instructionTitleTE.text = "Step 1 of 2:";
					instructionTextTE.text = "Please connect your text";

					// History
					direct.style.display = DisplayStyle.Flex;
					directTextOF.SetEnabled(false);
					directTextOF.objectType = typeof(TMP_Text);

					directHideOnInitT.style.display = DisplayStyle.Flex;
					directHideOnInitT.SetEnabled(false);

					directTextOF.AddToClassList("active-field");
					directHideOnInitT.RemoveFromClassList("active-field");

					step1ObjectField.RegisterCallback<ChangeEvent<UnityEngine.Object>>((_field) =>
					{
						step1ObjectField.value = _field.newValue;
						textensionWizard.text = _field.newValue as TMP_Text;
						directTextOF.value = _field.newValue;

						// If your you are adding a valid reference
						if (_field.newValue != null && _field.previousValue == null)
						{
							textensionWizard.state = TextensionWizard.WizardStates.HIDE_ON_INITIALIZATION;
							DeterminePath();
						}
						// Else if you have something and you de-referenced it.
						else if (_field.previousValue != null && _field.newValue == null)
						{
							console.RecordLog(StatusCodes.WARNING, "Missing Text Reference");
							textensionWizard.state = TextensionWizard.WizardStates.SPLASH_SCREEN;
							DeterminePath();
						}
					});
					break;

				case TextensionWizard.WizardStates.HIDE_ON_INITIALIZATION:

					instructionTitleTE.text = "Step 2 of 2:";
					instructionTextTE.text = "Hide text on initialization?";

					// Step Buttons
					step2ButtonYes.parent.style.display = DisplayStyle.Flex;
					step2ButtonYes.style.display = DisplayStyle.Flex;
					step2ButtonNo.style.display = DisplayStyle.Flex;

					// History
					direct.style.display = DisplayStyle.Flex;
					directTextOF.SetEnabled(true);

					directTextOF.RegisterCallback<ChangeEvent<UnityEngine.Object>>(_field =>
					{
						directTextOF.value = _field.newValue;
						textensionWizard.text = _field.newValue as TMP_Text;

						// If your you are adding a valid reference
						if (_field.newValue != null && _field.previousValue == null)
						{
							textensionWizard.state = TextensionWizard.WizardStates.HIDE_ON_INITIALIZATION;
							DeterminePath();
						}
						// Else if you have something and you de-referenced it.
						else if (_field.previousValue != null && _field.newValue == null)
						{
							console.RecordLog(StatusCodes.WARNING, "Missing Text Reference");
						}
					});

					directHideOnInitT.style.display = DisplayStyle.Flex;
					directHideOnInitT.SetEnabled(false);

					directTextOF.RemoveFromClassList("active-field");
					directHideOnInitT.AddToClassList("active-field");

					step2ButtonYes.RegisterCallback<MouseUpEvent>(_e =>
					{
						// We will transition to the other state
						textensionWizard.hideOnInitialization = true;
						directHideOnInitT.value = true;
						textensionWizard.state = TextensionWizard.WizardStates.COMPLETED;
						DeterminePath();
					});

					step2ButtonNo.RegisterCallback<MouseUpEvent>(_e =>
					{
						// We will transition to the other state
						textensionWizard.hideOnInitialization = false;
						directHideOnInitT.value = false;
						textensionWizard.state = TextensionWizard.WizardStates.COMPLETED;
						DeterminePath();
					});

					break;

				case TextensionWizard.WizardStates.COMPLETED:

					instructionsRowVE.style.display = DisplayStyle.None;

					wizardContentVE.style.display = DisplayStyle.None;
					direct.style.display = DisplayStyle.Flex;
					directTextOF.SetEnabled(true);
					directHideOnInitT.SetEnabled(true);
//					hideOnInitializationRef.Q<VisualElement>().style.justifyContent = Justify.FlexEnd;
					wizardVE.style.display = DisplayStyle.None;

					directTextOF.value = textensionWizard.text;
					directTextOF.RemoveFromClassList("active-field");
					directHideOnInitT.RemoveFromClassList("active-field");
//					resultElement.Add(new IMGUIContainer(OnInspectorGUI));

					directHideOnInitT.style.display = DisplayStyle.Flex;
					directHideOnInitT.value = textensionWizard.hideOnInitialization;
//					hideOnInitializationRef.Q<VisualElement>("unity-checkmark").parent.style.justifyContent =
//						Justify.FlexEnd;

					console.RecordLog(StatusCodes.NULL, "test");
					// TODO: Expand the console recorder class - pull data from console instead
					if (console.GetLogCount() > 0)
					{
						DisplayStatusMessage(StatusCodes.SUCCESS, "Ready to use!");

						var hideStatusMessage = resultElement.schedule.Execute(() =>
						{
							statusRowVE.style.display = DisplayStyle.None;
						});

						hideStatusMessage.ExecuteLater(2000);
					}

					break;
			}
		}

		private void DisplayStatusMessage(KeyValuePair<StatusCodes, string> log)
		{
			Debug.Log(log.Key);
			Debug.Log(log.Value);

			statusRowVE.style.display = DisplayStyle.Flex;
			DisplayStatus(StatusCodes.ASSERT);
			statusTextTE.text = log.Value;
		}

		private void DisplayStatusMessage(StatusCodes _statusCode,  string _message)
		{
			statusRowVE.style.display = DisplayStyle.Flex;
			DisplayStatus(_statusCode);
			statusTextTE.text = _message;
		}

		/// <summary>
		/// Displays a color for the status indicator and status title. Also renames the status title to the status code.
		/// </summary>
		/// <param name="_statusCode">A status code<see cref="StatusCodes"/></param>
		private void DisplayStatus(StatusCodes _statusCode)
		{
			statusPrefixTE.text = _statusCode + ": ";

			switch (_statusCode)
			{
				case StatusCodes.NULL:
					statusIndicatorVE.style.unityBackgroundImageTintColor = defaultStyleColor;
					statusPrefixTE.style.color = defaultStyleColor;
					break;
				case StatusCodes.ASSERT:
					statusIndicatorVE.style.unityBackgroundImageTintColor = assetStyleColor;
					statusPrefixTE.style.color = assetStyleColor;
					break;
				case StatusCodes.WARNING:
					statusIndicatorVE.style.unityBackgroundImageTintColor = warningStyleColor;
					statusPrefixTE.style.color = warningStyleColor;
					break;
				case StatusCodes.SUCCESS:
					statusIndicatorVE.style.unityBackgroundImageTintColor = successStyleColor;
					statusPrefixTE.style.color = successStyleColor;
					break;
				default:
					Debug.LogWarning("T.ext: Unable to support this status code.");
					break;
			}
		}

		/// <summary>
		/// Provides a background image for a specific VisualElement
		/// </summary>
		/// <param name="target">The visual element that will get a background image</param>
		/// <param name="imagePath">The directory of the image asset</param>
		/// <param name="imageWidth">The image width in pixels</param>
		/// <param name="imageHeight">The image height in pixels</param>
		private void LoadImage(VisualElement target, string imagePath, int imageWidth, int imageHeight)
		{
			// If our image asset exists...
			if (File.Exists(imagePath))
			{
				// Create a 2D Texture
				Texture2D logoTexture = new Texture2D(imageWidth, imageHeight);

				// Load image into 2D Texture (Convert .png to a texture2D)
				logoTexture.LoadImage(File.ReadAllBytes(LOGO_PATH));

				// Load 2D texture into the background image style
				target.style.backgroundImage = logoTexture;
			}
			else
			{
				Debug.LogAssertion("Unable to find asset in: " + imagePath);
			}
		}
	}
}