using Textensions.Editor;
using UnityEngine;

namespace Textensions.Core
{
	public class TextensionWizard: Textension, IWizard
	{
		public enum WizardStates
		{
			SPLASH_SCREEN,
			TEXTENSION_CONNECT,
			HIDE_ON_INITIALIZATION,
			COMPLETED
		}

		public WizardStates state = WizardStates.SPLASH_SCREEN;

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
				state = WizardStates.SPLASH_SCREEN;
				editor.CreateInspectorGUI();
			}
		}
	}
}
