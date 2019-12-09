using Textensions.Core;
using TMPro;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static Textensions.Editor.Utilities.UIElementUtilities;
using static Textensions.Editor.Utilities.TextensionStyling;

namespace Textensions.Editor.Components
{
    // TODO: Inherit from TextensionEditor.cs
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

        /// <summary>
        /// Textensions Logo (Pre-determined size, image gets loaded into this)
        /// </summary>
        private VisualElement logoVE;

        /// <summary>
        /// Status Indicator that lights a certain color at the top right. This color matches the statusPrefixTE color
        /// </summary>
        private VisualElement statusIndicatorVE;

        private TextElement instructionTitleTE;
        private TextElement instructionTextTE;

        private VisualElement wizardVE;
        
        private Button step0;

        private VisualElement step1;
        private ObjectField step1ObjectField;

        private VisualElement step2;
        private Button step2ButtonYes;
        private Button step2ButtonNo;

        private ObjectField directTextOF;
        private Toggle directHideOnInitT;
        
        private VisualElement consoleLogContainer;

        private const string LOGO_PATH =
            "Packages/com.adrianmiasik.textensions/Resources/TextensionLogo40x40.png";

        private TextensionsConsole console;

        public void OnEnable()
        {
            // Cache the script reference
            textensionWizard = (TextensionWizard) target;

            // Send reference of us to the wizard so it can get context menu logic
            textensionWizard.Editor = this;

            // Cache the uxml structure reference so we can pull out the visual elements when needed as necessary
            uxmlReference = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
                ("Packages/com.adrianmiasik.textensions/Editor/Components/TextensionWizardEditor.uxml");

            // Create a new visual element object that we will use to draw every inspector rebuild
            resultElement = new VisualElement();

            // Link the style sheet to the result element so each element within the result element is styled properly
            resultElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>
                ("Packages/com.adrianmiasik.textensions/Editor/Components/TextensionWizardEditor.uss"));

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
            // Create a console
            console = new TextensionsConsole();

            // Clear the result element before we add stuff to render to it
            resultElement.Clear();

            // Read and draw the uxml structure visual elements to the result element
            uxmlReference.CloneTree(resultElement);

            // Setup all our references, and null check our queries...
            if (Setup())
            {
                // Load Logo
                LoadImageIntoElement(logoVE, LOGO_PATH, 40, 40);

                // Load page
                Paint();
            }

            return resultElement;
        }

        /// <summary>
        /// Query our elements and checks the validity of our queried data.
        /// </summary>
        /// <returns>Returns true if the setup was successful. Otherwise this will return false</returns>
        private bool Setup()
        {
            // Get references
            QueryElements();

            // Verify element validity
            return CheckElements();
        }

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
            consoleLogContainer = resultElement.Q("ConsoleLogContainer*");
            wizardVE = resultElement.Q("Wizard*");
            #region Wizard/Wizard Center
            #region Wizard/Wizard Center/Instructions Row
            instructionTitleTE = resultElement.Q<TextElement>("Instruction Title*");
            instructionTextTE = resultElement.Q<TextElement>("Instruction Text*");
            #endregion
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
            #region Direct/
            #region Direct/Direct Center/Direct Content/
            directTextOF = resultElement.Q<ObjectField>("Direct Text*");
            directHideOnInitT = resultElement.Q<Toggle>("Direct Hide On Init*");
            #endregion
            #endregion
            #endregion
        }

        private bool CheckElements()
        {
            if (logoVE == null)
            {
                console.Record(TextensionsConsole.Types.WARNING, "Unable to locate logo element." +
                                                                 " This is used to display the Textensions logo.");
            }

            if (statusIndicatorVE == null)
            {
                console.Record(TextensionsConsole.Types.ASSERT, "Unable to locate the status indicator. " +
                                                                "This is used to show the user the highest priority log on this component.");
            }

            if (wizardVE == null)
            {
                console.Record(TextensionsConsole.Types.ASSERT, "Unable to locate the wizard container element. This is used to hide the " +
                                                                "entire wizard window when the user completes the setup process.");
            }

            // TODO: The rest of the visual elements.

            // If we are missing any necessary elements to build this wizard...
            if (console.GetTypeCount(TextensionsConsole.Types.ASSERT) > 0)
            {
                Debug.LogAssertion("Unable to draw the Textension Wizard component. Please see the asset logs in the Textensions console.");
                return false;
            }

            return true;
        }
        
        private void ChangeWizardInstruction(string _prefix = "", string _message = "")
        {
            instructionTitleTE.text = _prefix;
            instructionTextTE.text = _message;
        }
        
        /// <summary>
        /// Add functionality to the connect button that changes the wizard state to TEXTENSION_CONNECT and redraw.
        /// </summary>
        private void HandleStep0()
        {
            step0.RegisterCallback<MouseUpEvent>(_e =>
            {
                // We will transition to the other state
                textensionWizard.state = TextensionWizard.WizardStates.TEXTENSION_CONNECT;
                Paint();
            });
        }

        private void HandleStep1()
        {
            step1ObjectField.RegisterCallback<ChangeEvent<Object>>((_field) =>
            {
                textensionWizard.text = _field.newValue as TMP_Text;

                // If you are adding a valid reference.
                if (_field.newValue != null && _field.previousValue == null)
                {
                    textensionWizard.state = TextensionWizard.WizardStates.HIDE_ON_INITIALIZATION;
                    Paint();
                }
                // else if you de-referenced it.
                else if (_field.previousValue != null && _field.newValue == null)
                {
                    textensionWizard.state = TextensionWizard.WizardStates.SPLASH_SCREEN;
                    Paint();
                }
            });
        }

        private void HandleStep2()
        {
            step2ButtonYes.RegisterCallback<MouseUpEvent>(_e =>
            {
                textensionWizard.hideOnInitialization = true;
                if (textensionWizard.IsReady())
                {
                    textensionWizard.CompleteWizard();
                }
                else
                {
                    textensionWizard.state = TextensionWizard.WizardStates.TEXTENSION_CONNECT;
                }
                Paint();
            });

            step2ButtonNo.RegisterCallback<MouseUpEvent>(_e =>
            {
                textensionWizard.hideOnInitialization = false;

                if (textensionWizard.IsReady())
                {
                    textensionWizard.CompleteWizard();
                }
                else
                {
                    textensionWizard.state = TextensionWizard.WizardStates.TEXTENSION_CONNECT;
                }
                Paint();
            });
        }

        private void HandleDirectReferences()
        {
            directTextOF.RegisterCallback<ChangeEvent<Object>>((_field) =>
            {
                // If you de-referenced a valid value.
                if (_field.previousValue != null && _field.newValue == null)
                {
                    console.Record(TextensionsConsole.Types.WARNING, "Missing Text Reference.");
                    textensionWizard.state = TextensionWizard.WizardStates.TEXTENSION_CONNECT;
                }
            });

            // Whatever the direct bool is, apply it on the textensionWizard.hideOnInitialization
            directHideOnInitT.RegisterCallback<ChangeEvent<bool>>((_field) =>
            {
                textensionWizard.hideOnInitialization = _field.newValue;
            });

            // Enable the direct fields
            directHideOnInitT.SetEnabled(true);
            directTextOF.SetEnabled(true);
        }

        /// TODO: Refactor this so I'm not changing the (virtual?) DOM unnecessarily, which I am.
        /// <summary>
        /// Draw the frame based on the textension wizard state
        /// </summary>
        private void Paint()
        {
            // Hide the connect button, text object field, and the hide on init bool
            HideDisplay(step0);
            HideDisplay(step1);
            HideDisplay(step2);

            // Set interactivity
            DisableDirectInteractivity();

            // Remove styles by default
            CleanActiveField(directTextOF);
            CleanActiveField(directHideOnInitT);

            // Initialize the direct object field to always be of type TMP_Text
            InitializeObjectField(directTextOF);

            if (textensionWizard.state == TextensionWizard.WizardStates.SPLASH_SCREEN)
            {
                textensionWizard.text = null;
                textensionWizard.hideOnInitialization = false;
            }

            // Copy data
            directTextOF.value = textensionWizard.text;
            directHideOnInitT.value = textensionWizard.hideOnInitialization;

            statusIndicatorVE.style.unityBackgroundImageTintColor = console.messageStyleColor;

            HandleDirectReferences();

            // Check state of wizard
            switch (textensionWizard.state)
            {
                case TextensionWizard.WizardStates.SPLASH_SCREEN:

                    // Change the instruction text
                    ChangeWizardInstruction("Setup Wizard");

                    // Display the connect button
                    ShowDisplay(step0);

                    // Hook up the connect button functionality.
                    HandleStep0();

                    directTextOF.SetEnabled(false);
                    directHideOnInitT.SetEnabled(false);

                    break;

                case TextensionWizard.WizardStates.TEXTENSION_CONNECT:

                    // Change the instruction text
                    ChangeWizardInstruction("Step 1 of 2:", "Please connect your text");

                    // Display the text object field
                    ShowDisplay(step1);

                    // Change the object field type and the direct object field to a TMP_Text
                    InitializeObjectField(step1ObjectField);

                    // Handle the object field
                    HandleStep1();

                    // Make the textensions text reference the active field.
                    MarkActiveField(directTextOF);

                    directTextOF.SetEnabled(false);
                    directHideOnInitT.SetEnabled(false);

                    break;

                case TextensionWizard.WizardStates.HIDE_ON_INITIALIZATION:

                    // Change the instruction text
                    ChangeWizardInstruction("Step 2 of 2:", "Hide text on initialization?");

                    // Display the yes and no buttons
                    ShowDisplay(step2);

                    // Handle the yes and no buttons
                    HandleStep2();

                    // Make the textensions text reference the bool
                    MarkActiveField(directHideOnInitT);

                    directHideOnInitT.SetEnabled(false);

                    break;

                // TODO: Revise this
                case TextensionWizard.WizardStates.COMPLETED:

                    HideDisplay(wizardVE);

                    // If our textensions component has everything it needs...
                    if (textensionWizard.IsReady())
                    {
                        textensionWizard.hasWizardCompleted = true;

                        // If we have no assets and no warnings, we can say we are ready to use.
                        if (console.GetTypeCount(TextensionsConsole.Types.ASSERT) <= 0 &&
                            console.GetTypeCount(TextensionsConsole.Types.WARNING) <= 0)
                        {
                            console.Record(TextensionsConsole.Types.SUCCESS, "Ready to use!");
                        }

                        statusIndicatorVE.style.unityBackgroundImageTintColor = console.successStyleColor;
                    }

                    break;
            }

            // Print to unity console
            // console.PrintAllLogs();

            // Add elements to flag
            console.InjectLogs(consoleLogContainer);

            // TODO: Schedule all the logs so they disappear
            // var hideStatusMessage = resultElement.schedule.Execute(() => 
            // 	{
            // 		statusRowVE.style.display = DisplayStyle.None;
            // 	});
            //
            // 	hideStatusMessage.ExecuteLater(2000);

            // Debug.LogWarning("injecting logs: " + console.GetLogCount());
        }

        /// <summary>
        /// Disables the direct references interactivity.
        /// </summary>
        private void DisableDirectInteractivity()
        {
            directTextOF.SetEnabled(false);
            directHideOnInitT.SetEnabled(false);
        }

        /// <summary>
        /// Marks this element as the "actively' working element.
        /// </summary>
        /// <param name="_element"></param>
        private static void MarkActiveField(VisualElement _element)
        {
            _element.AddToClassList("active-field");
        }

        /// <summary>
        /// Cleans (or "Un-marks") this element as the "active" working element.
        /// </summary>
        /// <param name="_element"></param>
        private static void CleanActiveField(VisualElement _element)
        {
            _element.RemoveFromClassList("active-field");
        }
    }
}
