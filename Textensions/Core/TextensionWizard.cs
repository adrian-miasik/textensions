namespace Textensions.Core
{
	public class TextensionWizard: Textension
	{
		public enum WizardStates
		{
			NOT_DETERMINED,
			SPLASH_SCREEN,
			TEXTENSION_CONNECT,
			HIDE_ON_INITIALIZATION,
			COMPLETED
		}

		public WizardStates state = WizardStates.NOT_DETERMINED;

	}
}