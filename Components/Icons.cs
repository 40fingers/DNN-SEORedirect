using DotNetNuke;
using DotNetNuke.Common;

namespace FortyFingers.SeoRedirect.Components
{
    /// <summary>
    /// List icons by the MEANING, NOT filename. That way we can easily change an icon for a single purpose.
    /// </summary>
    public enum IconTypes
    {
        Add,
        Delete,
        Edit,
        Save,
        Cancel,
        Open,
        Close
    }
    public static class Icons
    {
        /// <summary>
        /// Extension method to provide for easy URLs for icons. Extends Educo.MyStek.Components.IconTypes
        /// </summary>
        /// <param name="iconType"></param>
        /// <returns></returns>
        public static string GetUrl(this IconTypes iconType)
        {
            var retval = "";

            switch (iconType)
            {
                case IconTypes.Open:
                    retval = Globals.ResolveUrl("<i class='demo-icon icon-down-open-big'></i>");
                    break;

                case IconTypes.Close:
                    retval = Globals.ResolveUrl("<i class='demo-icon icon-up-open-big'></i>");
                    break;
                case IconTypes.Delete:
                    retval = Globals.ResolveUrl("<i class='demo-icon icon-cancel-trash'></i>");
                    break;
                case IconTypes.Add:
                    retval = Globals.ResolveUrl("<i class='demo-icon icon-plus-circled'></i>");
                    break;
                case IconTypes.Edit:
                    retval = Globals.ResolveUrl("<i class='demo-icon icon-pencil'></i>"); 
                    break;
                case IconTypes.Save:
                    retval = Globals.ResolveUrl("<i class='demo-icon icon-drive'></i>");
                    break;
                case IconTypes.Cancel:
                    retval = Globals.ResolveUrl("<i class='demo-icon icon-cancel-circled'></i>");
                    break;
                default:
                    retval = Globals.ResolveUrl(Constants.DESKTOPMODULES_MODULEROOT_URL +
                                                             "img/icons/application.png");
                    break;
            }

            return retval;
        }
    }
}