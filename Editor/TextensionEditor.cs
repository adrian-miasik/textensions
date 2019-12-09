using Textensions.Core;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static Textensions.Editor.Utilities.UIElementUtilities;
using static Textensions.Editor.Utilities.TextensionStyling;

namespace Textensions.Editor.Components
{
    // TODO: Look into creating this the base class for Textension components
    // TODO: Add the ability to remove the header from Textension components
    [CustomEditor(typeof(Textension))]
    public class TextensionEditor: UnityEditor.Editor
    {
        private Textension textension;

        /// <summary>
        /// The result element that we want to render and is used to tell the inspector what to render.
        /// <see cref="Editor.CreateInspectorGUI"/>
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

        private const string LOGO_PATH =
            "Packages/com.adrianmiasik.textensions/Resources/TextensionLogo40x40.png";

        private ObjectField directTextOF;
        private Toggle directHideOnInitT;

        private VisualElement consoleLogContainer;
        
        private TextensionsConsole console;
        
        public void OnEnable()
        {
            // Cache the script reference
            textension = (Textension) target;

            // Cache the uxml structure reference so we can pull out the visual elements when needed as necessary
            uxmlReference = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
                ("Packages/com.adrianmiasik.textensions/Editor/Components/TextensionEditor.uxml");

            // Create a new visual element object that we will use to draw every inspector rebuild
            resultElement = new VisualElement();

            // Link the style sheet to the result element so each element within the result element is styled properly
            resultElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>
                ("Packages/com.adrianmiasik.textensions/Editor/Components/TextensionEditor.uss"));
            
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
            // TODO: The rest of the visual elements.

            // If we are missing any necessary elements to build this wizard...
            if (console.GetTypeCount(TextensionsConsole.Types.ASSERT) > 0)
            {
                Debug.LogAssertion(
                    "Unable to draw the Textension Wizard component. Please see the asset logs in the Textensions console.");
                return false;
            }

            return true;
        }


        // TODO: A validate function or flow controller
        // (Only move to the next step if the requirements for a previous step have been filled. Not: if this field has updated go to
        // this specific state)

        /// TODO: Refactor this so I'm not changing the (virtual?) DOM unnecessarily, which I am.
        /// <summary>
        /// Draw the frame based on the textension wizard state
        /// </summary>
        private void Paint()
        {
            // Initialize the direct object field to always be of type TMP_Text
            InitializeObjectField(directTextOF);

            // Copy data
            directTextOF.value = textension.text;
            directHideOnInitT.value = textension.hideOnInitialization;

            statusIndicatorVE.style.unityBackgroundImageTintColor = console.messageStyleColor;

            HandleDirectReferences();
        }

        private void HandleDirectReferences()
        {
            directTextOF.RegisterCallback<ChangeEvent<Object>>((_field) =>
            {
                // If you de-referenced a valid value.
                if (_field.previousValue != null && _field.newValue == null)
                {
                    console.Record(TextensionsConsole.Types.WARNING, "Missing Text Reference.");
                }
            });

            // Whatever the direct bool is, apply it on the textensionWizard.hideOnInitialization
            directHideOnInitT.RegisterCallback<ChangeEvent<bool>>((_field) =>
            {
                textension.hideOnInitialization = _field.newValue;
            });

            // Enable the direct fields
            directHideOnInitT.SetEnabled(true);
            directTextOF.SetEnabled(true);
        }
    }
}