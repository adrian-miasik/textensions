using Textensions.Editor;
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
		public TextensionWizardEditor editor { private get; set; }

		/// <summary>
		/// Resets the wizard state and forces a redraw on the editor reference.
		/// </summary>
		[ContextMenu("Reset Wizard")]
		public void ResetWizard()
		{
			if (editor != null)
			{
				hasWizardCompleted = false;
				state = WizardStates.SPLASH_SCREEN;
				editor.CreateInspectorGUI();
			}
		}
		
		/// <summary>
		/// Converts this wizard into a basic textension component.
		/// </summary>
		[ContextMenu("Convert to Textension")]
		public void ConvertToTextension()
		{
			if (editor == null) return;
			
			// Create Textension
			Textension createdTextension = gameObject.AddComponent<Textension>();

			// Sample our components, then move our new textension to our wizard location
			ComponentUtilities.SampleComponents(gameObject);
			ComponentUtilities.MoveComponentToIndex(createdTextension, ComponentUtilities.GetComponentIndex(this));

			// Carry over component data
			UnityEditorInternal.ComponentUtility.CopyComponent(this);
			UnityEditorInternal.ComponentUtility.PasteComponentValues(createdTextension);
				
			// Remove our wizard component
			DestroyImmediate(this);
		}
		
		public void CompleteWizard()
		{
			hasWizardCompleted = true;
			state = WizardStates.COMPLETED;
		}
	}
}