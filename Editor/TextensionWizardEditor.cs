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
		private VisualElement statusIndicatorVisualElement;
		private Texture2D logo;
		private VisualElement logoVisualElement;
		private VisualElement statusRowVisualElement;
		private VisualElement bodyVisualElement;
		private VisualElement contentVisualElement;
		private TextElement statusTitleTextElement;
		private TextElement statusTextTextElement;
		private VisualElement instructionsRow;
		private TextElement instructionTitleTextElement;
		private TextElement instructionTextTextElement;
		private Button step0ButtonConnect;
		private ObjectField step1ObjectField;
		private Button step2ButtonYes;
		private Button step2ButtonNo;
		private VisualElement historyVisualElement;
		private ObjectField textMeshProTextRef;
		private Toggle hideOnInitializationRef;
		private VisualElement wizardVisualElement;
		private TextElement historyTitleTextElement;

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
			// Fetch our references
			statusIndicatorVisualElement = resultElement.Q("Status Indicator");
			logoVisualElement = resultElement.Q("Logo");
			bodyVisualElement = resultElement.Q("Body");
			contentVisualElement = resultElement.Q("Content");
			statusRowVisualElement = resultElement.Q("Status Row");
			statusTitleTextElement = resultElement.Q<TextElement>("Status Text");
			statusTextTextElement = resultElement.Q<TextElement>("Display Message Text");
			instructionsRow = resultElement.Q("Instructions Row");
			instructionTitleTextElement = resultElement.Q<TextElement>("Instruction Title");
			instructionTextTextElement = resultElement.Q<TextElement>("Instruction Text");
			step0ButtonConnect = resultElement.Q<Button>("Step-0");
			step1ObjectField = resultElement.Q<ObjectField>("Step-1");
			step2ButtonYes = resultElement.Q<Button>("Yes");
			step2ButtonNo = resultElement.Q<Button>("No");
			historyVisualElement = resultElement.Q("History");
			textMeshProTextRef = resultElement.Q<ObjectField>("TextMeshProTextRef");
			hideOnInitializationRef = resultElement.Q<Toggle>("HideOnInitializationRef");
			wizardVisualElement = resultElement.Q("Wizard");
			historyTitleTextElement = resultElement.Q<TextElement>("History Title");
		}

		// TODO: Refactor
		private bool CheckElements()
		{
			// ReSharper disable once ReplaceWithSingleAssignment.True
			bool isSuccessful = true;

			// Check our elements
			// Status Title (E.g. WARNING)
			if (statusTitleTextElement == null)
			{
				console.RecordLog(StatusCodes.WARNING, "Missing Status Title Reference");
				isSuccessful = false;
			}

			// Status Message (E.g. Unable to find textension reference)
			if (statusTextTextElement == null)
			{
				console.RecordLog(StatusCodes.WARNING, "Missing Status Message Reference");
				isSuccessful = false;
			}

			// Instruction Title (E.g. "Step 1:")
			if (instructionTitleTextElement == null)
			{
				console.RecordLog(StatusCodes.WARNING, "Missing Instruction Title Reference");
				isSuccessful = false;
			}

			// Instruction Title (E.g. "Please connect your textension.")
			if (instructionTextTextElement == null)
			{
				console.RecordLog(StatusCodes.WARNING, "Missing Instruction Message Reference");
				isSuccessful = false;
			}

			// Step 0 Button Connect
			if (step0ButtonConnect == null)
			{
				console.RecordLog(StatusCodes.WARNING, "Missing Connect Button Reference");
				isSuccessful = false;
			}

			// Step 1 Object Field
			if (step1ObjectField == null)
			{
				console.RecordLog(StatusCodes.WARNING, "Missing Object Field Reference");
				isSuccessful = false;
			}

			// Step 2 Button Yes
			if (step2ButtonYes == null)
			{
				console.RecordLog(StatusCodes.WARNING, "Missing Yes Button Reference");
				isSuccessful = false;
			}

			// Step 2 Button No
			if (step2ButtonNo == null)
			{
				console.RecordLog(StatusCodes.WARNING, "Missing No Button Reference");
				isSuccessful = false;
			}

			if (wizardVisualElement == null)
			{
				console.RecordLog(StatusCodes.WARNING, "Missing Wizard Element");
				isSuccessful = false;
			}

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
			console = new ConsoleRecorder(statusRowVisualElement, statusTitleTextElement, defaultStyleColor,
				statusTextTextElement);

			// Setup all our references, and null check our queries...
			if (Setup())
			{
				// Load Logo
				LoadImage(logoVisualElement, LOGO_PATH, 40, 40);

				logoVisualElement.RegisterCallback<MouseUpEvent>(_e =>
				{
					textensionWizard.state = TextensionWizard.WizardStates.SPLASH_SCREEN;
					CreateInspectorGUI();
				});

				// Load page
				DeterminePath();
			}

			console.PrintRecords();
//			resultElement.Add(console.GenerateRecordVisuals());

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
		private void DeterminePath()
		{
			// Remove status
			statusRowVisualElement.style.display = DisplayStyle.None;

			// Remove all steps
			step0ButtonConnect.style.display = DisplayStyle.None;
			step1ObjectField.style.display = DisplayStyle.None;
			step2ButtonYes.style.display = DisplayStyle.None;
			step2ButtonNo.style.display = DisplayStyle.None;

			// Remove the history
			historyVisualElement.style.display = DisplayStyle.None;

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

					statusRowVisualElement.style.display = DisplayStyle.None;
					instructionTitleTextElement.text = "Setup Wizard";
					instructionTextTextElement.text = "";

					step0ButtonConnect.style.display = DisplayStyle.Flex;

					historyVisualElement.style.display = DisplayStyle.None;

					// When the user presses the connect button...
					step0ButtonConnect.RegisterCallback<MouseUpEvent>(_e =>
					{
						// We will transition to the other state
						textensionWizard.state = TextensionWizard.WizardStates.TEXTENSION_CONNECT;
						DeterminePath();
					});
					break;

				case TextensionWizard.WizardStates.TEXTENSION_CONNECT:

					step1ObjectField.style.display = DisplayStyle.Flex;
					step1ObjectField.Q<Label>().style.minWidth = 0;
					step1ObjectField.objectType = typeof(TMP_Text);

					instructionTitleTextElement.text = "Step 1 of 2:";
					instructionTextTextElement.text = "Please connect your text";

					// History
					historyVisualElement.style.display = DisplayStyle.Flex;
					textMeshProTextRef.SetEnabled(false);
					textMeshProTextRef.objectType = typeof(TMP_Text);
//					textMeshProTextRef.Q<Label>().style.minWidth = 0;

					hideOnInitializationRef.style.display = DisplayStyle.Flex;
					hideOnInitializationRef.SetEnabled(false);
//					hideOnInitializationRef.Q<VisualElement>("unity-checkmark").parent.style.justifyContent =
//						Justify.FlexEnd;

					textMeshProTextRef.AddToClassList("active-field");
					hideOnInitializationRef.RemoveFromClassList("active-field");

					step1ObjectField.RegisterCallback<ChangeEvent<UnityEngine.Object>>((_field) =>
					{
						step1ObjectField.value = _field.newValue;
						textensionWizard.text = _field.newValue as TMP_Text;
						textMeshProTextRef.value = _field.newValue;

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

					instructionTitleTextElement.text = "Step 2 of 2:";
					instructionTextTextElement.text = "Hide text on initialization?";

					// Step Buttons
					step2ButtonYes.parent.style.display = DisplayStyle.Flex;
					step2ButtonYes.style.display = DisplayStyle.Flex;
					step2ButtonNo.style.display = DisplayStyle.Flex;

					// History
					historyVisualElement.style.display = DisplayStyle.Flex;
					textMeshProTextRef.SetEnabled(true);
//					textMeshProTextRef.Q<Label>().style.minWidth = 0;

					textMeshProTextRef.RegisterCallback<ChangeEvent<UnityEngine.Object>>(_field =>
					{
						textMeshProTextRef.value = _field.newValue;
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

					hideOnInitializationRef.style.display = DisplayStyle.Flex;
					hideOnInitializationRef.SetEnabled(false);
//					hideOnInitializationRef.Q<VisualElement>("unity-checkmark").parent.style.justifyContent =
//						Justify.FlexEnd;

					textMeshProTextRef.RemoveFromClassList("active-field");
					hideOnInitializationRef.AddToClassList("active-field");

					step2ButtonYes.RegisterCallback<MouseUpEvent>(_e =>
					{
						// We will transition to the other state
						textensionWizard.hideOnInitialization = true;
						hideOnInitializationRef.value = true;
						textensionWizard.state = TextensionWizard.WizardStates.COMPLETED;
						DeterminePath();
					});

					step2ButtonNo.RegisterCallback<MouseUpEvent>(_e =>
					{
						// We will transition to the other state
						textensionWizard.hideOnInitialization = false;
						hideOnInitializationRef.value = false;
						textensionWizard.state = TextensionWizard.WizardStates.COMPLETED;
						DeterminePath();
					});

					break;

				case TextensionWizard.WizardStates.COMPLETED:

					instructionsRow.style.display = DisplayStyle.None;

					contentVisualElement.style.display = DisplayStyle.None;
					historyVisualElement.style.display = DisplayStyle.Flex;
					textMeshProTextRef.SetEnabled(true);
					hideOnInitializationRef.SetEnabled(true);
//					hideOnInitializationRef.Q<VisualElement>().style.justifyContent = Justify.FlexEnd;
					wizardVisualElement.style.display = DisplayStyle.None;

					textMeshProTextRef.value = textensionWizard.text;
					textMeshProTextRef.RemoveFromClassList("active-field");
					hideOnInitializationRef.RemoveFromClassList("active-field");
//					resultElement.Add(new IMGUIContainer(OnInspectorGUI));

					hideOnInitializationRef.style.display = DisplayStyle.Flex;
					hideOnInitializationRef.value = textensionWizard.hideOnInitialization;
//					hideOnInitializationRef.Q<VisualElement>("unity-checkmark").parent.style.justifyContent =
//						Justify.FlexEnd;

					console.RecordLog(StatusCodes.NULL, "test");
					// TODO: Expand the console recorder class - pull data from console instead
					if (console.GetLogCount() > 0)
					{
//				DisplayStatusMessage(console.GetFirstLog());
						DisplayStatusMessage(StatusCodes.SUCCESS, "Ready to use!");

						var hideStatusMessage = resultElement.schedule.Execute(() =>
						{
							statusRowVisualElement.style.display = DisplayStyle.None;
						});

						hideStatusMessage.ExecuteLater(2000);
					}

					break;

				default:
					break;
			}

		}

		private void DisplayStatusMessage(KeyValuePair<StatusCodes, string> log)
		{
			Debug.Log(log.Key);
			Debug.Log(log.Value);

			statusRowVisualElement.style.display = DisplayStyle.Flex;
			DisplayStatus(StatusCodes.ASSERT);
			statusTextTextElement.text = log.Value;
		}

		private void DisplayStatusMessage(StatusCodes _statusCode,  string _message)
		{
			statusRowVisualElement.style.display = DisplayStyle.Flex;
			DisplayStatus(_statusCode);
			statusTextTextElement.text = _message;
		}

		/// <summary>
		/// Displays a color for the status indicator and status title. Also renames the status title to the status code.
		/// </summary>
		/// <param name="_statusCode">A status code<see cref="StatusCodes"/></param>
		private void DisplayStatus(StatusCodes _statusCode)
		{
			statusTitleTextElement.text = _statusCode + ": ";

			switch (_statusCode)
			{
				case StatusCodes.NULL:
					statusIndicatorVisualElement.style.unityBackgroundImageTintColor = defaultStyleColor;
					statusTitleTextElement.style.color = defaultStyleColor;
					break;
				case StatusCodes.ASSERT:
					statusIndicatorVisualElement.style.unityBackgroundImageTintColor = assetStyleColor;
					statusTitleTextElement.style.color = assetStyleColor;
					break;
				case StatusCodes.WARNING:
					statusIndicatorVisualElement.style.unityBackgroundImageTintColor = warningStyleColor;
					statusTitleTextElement.style.color = warningStyleColor;
					break;
				case StatusCodes.SUCCESS:
					statusIndicatorVisualElement.style.unityBackgroundImageTintColor = successStyleColor;
					statusTitleTextElement.style.color = successStyleColor;
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