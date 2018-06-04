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
        Cancel
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
                case IconTypes.Delete:
                    retval = Globals.ResolveUrl(Constants.DESKTOPMODULES_MODULEROOT_URL +
                                                             "img/icons/delete.png");
                    break;
                case IconTypes.Add:
                    retval = Globals.ResolveUrl(Constants.DESKTOPMODULES_MODULEROOT_URL +
                                                             "img/icons/add.png");
                    break;
                case IconTypes.Edit:
                    retval = Globals.ResolveUrl(Constants.DESKTOPMODULES_MODULEROOT_URL +
                                                             "img/icons/pencil.png");
                    break;
                case IconTypes.Save:
                    retval = Globals.ResolveUrl(Constants.DESKTOPMODULES_MODULEROOT_URL +
                                                             "img/icons/disk.png");
                    break;
                case IconTypes.Cancel:
                    retval = Globals.ResolveUrl(Constants.DESKTOPMODULES_MODULEROOT_URL +
                                                             "img/icons/cancel.png");
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