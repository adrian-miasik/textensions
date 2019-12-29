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
    // This script is for the textension wizard component
    [CustomEditor(typeof(TextensionWizard))]
    public class TextensionWizardEditor : TextensionEditor
    {
        /// <summary>
        /// The class that has the state data
        /// </summary>
        private TextensionWizard textensionWizard;
        
        private TextElement instructionTitleTE;
        private TextElement instructionTextTE;

        private VisualElement wizardVE;
        
        private Button step0;

        private VisualElement step1;
        private ObjectField step1ObjectField;

        private VisualElement step2;
        private Button step2ButtonYes;
        private Button step2ButtonNo;
        
        public override void OnEnable()
        {
            // Cache the wizard reference
            textensionWizard = (TextensionWizard) target;
            textensionWizard.Editor = this;

            InitializeUIElements(
                "Packages/com.adrianmiasik.textensions/Editor/Components/TextensionWizardEditor.uxml",
                "Packages/com.adrianmiasik.textensions/Editor/Components/TextensionWizardEditor.uss");
        }
        
        protected override void QueryElements()
        {
            // Query base elements first
            base.QueryElements();
            
            #region Root
            #region Banner/
            #endregion
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
            #endregion
        }

        protected override void CheckElements()
        {
            base.CheckElements();

            if (wizardVE == null)
            {
                console.Record(TextensionsConsole.Types.ASSERT,
                    "Unable to locate the wizard container element. This is used to hide the " +
                    "entire wizard window when the user completes the setup process.");
            }
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
        protected override void Paint()
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
                default:
                    return;
            }
            
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
