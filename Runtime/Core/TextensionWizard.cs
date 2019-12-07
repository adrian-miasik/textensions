using Textensions.Editor;
using UnityEditor;
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
		/// Resets the wizard state and forces a redraw on the editor reference.
		/// </summary>
		[ContextMenu("Convert to Textension")]
		public void ConvertToTextension()
		{
			if (editor != null)
			{				
				int stepCount = MoveComponentToTop(this);
				UnityEditorInternal.ComponentUtility.CopyComponent(this);

				int stepsToMoveUp = ((GetComponents<Component>().Length - 1) - stepCount) - 1; // Ignore first element, then account for index
				
				// Create a new textension and cache it
				Textension createdTextension = gameObject.AddComponent<Textension>();

				for (int i = 0; i < stepsToMoveUp; i++)
				{
					UnityEditorInternal.ComponentUtility.MoveComponentUp(createdTextension);
				}

				UnityEditorInternal.ComponentUtility.PasteComponentValues(createdTextension);
				
				// Destroy self
				DestroyImmediate(this); // This spits out an instance id error
			}
		}

		private int MoveComponentToTop(Textension textension)
		{
			Component[] allComponents = GetComponents<Component>();
			
			// You can't move anything above the transform component.
			// We need to get the 1st component under that.
			int topMostInstanceID = allComponents[1].GetInstanceID();
			
			int movingComponentInstanceID = textension.GetInstanceID();

			int timesMovedUpwards = 0;

			for (int i = 0; i < allComponents.Length; i++)
			{
				if (topMostInstanceID != movingComponentInstanceID)
				{
					timesMovedUpwards++;
					UnityEditorInternal.ComponentUtility.MoveComponentUp(textension);

					// Fetch a refreshed order of the components.
					topMostInstanceID = GetComponents<Component>()[1].GetInstanceID();
				}
				else
				{
					return timesMovedUpwards;
				}
			}

			return timesMovedUpwards;
		}
		
		public void CompleteWizard()
		{
			hasWizardCompleted = true;
			state = WizardStates.COMPLETED;
		}
	}
}