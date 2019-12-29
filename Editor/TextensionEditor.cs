using Textensions.Core;
using Textensions.Editor.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static Textensions.Editor.Utilities.UIElementUtilities;
using static Textensions.Editor.Utilities.TextensionStyling;

namespace Textensions.Editor.Components
{
    // TODO: Add the ability to remove the header from Textension components
    [CustomEditor(typeof(Textension))]
    public class TextensionEditor: UnityEditor.Editor
    {
        private Textension textension;

        /// <summary>
        /// The result element that we want to render and is used to tell the inspector what to render.
        /// <see cref="Editor.CreateInspectorGUI"/>
        /// </summary>
        protected VisualElement resultElement;

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
        protected VisualElement statusIndicatorVE;

        private const string LOGO_PATH =
            "Packages/com.adrianmiasik.textensions/Resources/TextensionLogo40x40.png";

        protected ObjectField directTextOF;
        protected Toggle directHideOnInitT;

        protected VisualElement consoleLogContainer;

        protected TextensionsConsole console;
        
        public virtual void OnEnable()
        {
            // Cache the script reference
            textension = (Textension) target;
            
            InitializeUIElements(
                "Packages/com.adrianmiasik.textensions/Editor/Components/TextensionEditor.uxml",
                "Packages/com.adrianmiasik.textensions/Editor/Components/TextensionEditor.uss");
        }

        /// <summary>
        /// Caches our uxml visual tree asset and created a root visual element.
        /// </summary>
        /// <param name="_uxmlPath"></param>
        /// <param name="_styleSheetPath"></param>
        protected void InitializeUIElements(string _uxmlPath, string _styleSheetPath)
        {
            // Cache the uxml structure reference so we can pull out the visual elements when needed as necessary
            uxmlReference = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_uxmlPath);

            // Create a new styled VisualElement that we will use to draw every inspector rebuild
            resultElement = CreateRootElement();
            
            // Style root element
            StyleElement(resultElement, _styleSheetPath);
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
            CheckElements();

            // If we are missing any necessary elements to build this wizard...
            if (console.GetTypeCount(TextensionsConsole.Types.ASSERT) > 0)
            {
                console.PrintAllLogs();
                console.InjectLogs(consoleLogContainer);
                
                Debug.LogAssertion("Unable to draw the Textension Wizard component. Please see the asset logs in the Textensions console.");
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Queries and caches all our elements needed on creation
        /// </summary>
        protected virtual void QueryElements()
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

        /// <summary>
        /// Verifies the integrity of our references.
        /// </summary>
        /// <returns></returns>
        protected virtual void CheckElements()
        {
            if (logoVE == null)
            {
                console.Record(TextensionsConsole.Types.WARNING, 
                    "Unable to locate logo element. This is used to display the Textensions logo.");
            }

            if (statusIndicatorVE == null)
            {
                console.Record(TextensionsConsole.Types.ASSERT, 
                    "Unable to locate the status indicator. This is used to show the user the highest " +
                    "priority log on this component.");
            }

            if (consoleLogContainer == null)
            {
                console.Record(TextensionsConsole.Types.ASSERT, 
                    "Unable to locate the console container. This reference is used to inject our " +
                    "console logs into the Unity Editor. If this reference doesn't exist, our logs will " +
                    "appear in the console only.");
            }

            if (directTextOF == null)
            {
                console.Record(TextensionsConsole.Types.ASSERT, 
                    "Unable to locate the object field 'Direct Text*'. This is used to keep track of " +
                    "the current Textension reference. If this reference doesn't exist, our component will " +
                    "not work at all.");
            }

            if (directHideOnInitT == null)
            {
                console.Record(TextensionsConsole.Types.WARNING, 
                    "Unable to locate the checkbox toggle 'Direct Hide On Init*'. This is used to " +
                    "determine if our text should be hidden/seen on start (Needed for text reveals). If this " +
                    "reference doesn't exist, our component should mostly remain functional but our text reveal " +
                    "functionality will possibly break.");
            }
        }
        
        // TODO: A validate function or flow controller
        // (Only move to the next step if the requirements for a previous step have been filled. Not: if this field has updated go to
        // this specific state)

        /// TODO: Refactor this so I'm not changing the (virtual?) DOM unnecessarily, which I am.
        /// <summary>
        /// Draw the frame based on the textension wizard state
        /// </summary>
        protected virtual void Paint()
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