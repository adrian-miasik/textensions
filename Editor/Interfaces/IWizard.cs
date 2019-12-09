namespace Textensions.Editor
{
    public interface IWizard
    {
        /// <summary>
        /// Returns True if the wizard has all the necessary components. Otherwise returns False.
        /// </summary>
        /// <returns></returns>
        bool IsReady();
    }
}
