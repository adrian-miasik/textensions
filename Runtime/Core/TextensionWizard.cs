using Textensions.Editor;
using Textensions.Editor.Components;
using Textensions.Utilities;
using UnityEngine;

namespace Textensions.Core
{
    public class TextensionWizard : Textension, IWizard
    {
        public enum WizardStates
        {
            SPLASH_SCREEN,
            TEXTENSION_CONNECT,
            HIDE_ON_INITIALIZATION,
            COMPLETED
        }

        public WizardStates state = WizardStates.SPLASH_SCREEN;

        public bool hasWizardCompleted;

        public bool IsReady()
        {
            if (text == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Cached reference of the editor (Can only be set)
        /// </summary>
        public TextensionWizardEditor Editor { private get; set; }

        /// <summary>
        /// Resets the wizard state and forces a redraw on the editor reference.
        /// </summary>
        [ContextMenu("Reset Wizard")]
        public void ResetWizard()
        { 
            if (Editor == null) return;
            
            hasWizardCompleted = false;
            state = WizardStates.SPLASH_SCREEN;
            Editor.CreateInspectorGUI(); 
        }
		
        /// <summary>
        /// Converts this wizard into a basic textension component.
        /// </summary>
        [ContextMenu("Convert to Textension")]
        public void ConvertToTextension()
        {
            GameObject myObject = gameObject;
            
            // Create Textension
            Textension createdTextension = myObject.AddComponent<Textension>();

            // Sample our components, then move our new textension to our wizard location
            ComponentOrder.SampleComponents(myObject);
            ComponentOrder.MoveComponentToIndex(createdTextension, ComponentOrder.GetComponentIndex(this));
            
            // Carry over component data
            ComponentClipboard.CopyComponent(this);
            ComponentClipboard.PasteComponent(createdTextension);
				
            // Remove our wizard component
            DestroyImmediate(this); // This spits out an instance id error
        }
		
        public void CompleteWizard()
        {
            hasWizardCompleted = true;
            state = WizardStates.COMPLETED;
        }
    }
}