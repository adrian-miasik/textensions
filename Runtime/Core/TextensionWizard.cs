namespace Textensions.Core
{
	public class TextensionWizard: Textension
	{
		public enum WizardStates
		{
			SPLASH_SCREEN,
			TEXTENSION_CONNECT,
			HIDE_ON_INITIALIZATION,
			COMPLETED
		}

		public WizardStates state = WizardStates.SPLASH_SCREEN;

		public bool IsWizardComplete()
		{
			if (text != null)
			{
				return false;
			}

			return true;
		}
	}
}