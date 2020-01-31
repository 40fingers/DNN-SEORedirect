<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="FortyFingers.SeoRedirect.View" %>
<%@ Import Namespace="FortyFingers.SeoRedirect.Components" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>

<script type="text/javascript">
    ff_seo_selectedPageChanged = function (selectedNode, arg2) {
        // no way to figure out which textbox to clear
        //console.log(JSON.stringify(selectedNode));
        FF.log("triggering change on : " + $("#" + arg2[0].previousElementSibling.id).attr('id'));

        $("#" + arg2[0].previousElementSibling.id).trigger('change');
    };

</script>
<script type="text/javascript">
    var srPid = <%= PortalId %>;
    var srMid = <%= ModuleId %>;
    var srVM;
    $(document).ready(function () {
        SR.service = $.ServicesFramework(srMid);
        SR.service.baseUrl = SR.service.getServiceRoot("40Fingers") + "SeoRedirect/";

        srVM = new SR.UnhandledUrlsVm({ urls: [] });
        srVM.load();
    });
</script>
<asp:PlaceHolder runat="server" ID="LoggingPlaceholder"></asp:PlaceHolder>
<asp:Panel runat="server" ID="UnhandledUrlsPanel" Visible="False">
    <div class="ffsr ffsr-view">
        <h4>
            <asp:Label runat="server" ID="UnhandledUrlsPanelHeader"></asp:Label></h4>
        <div id="koUnhandledUrlsWrapper">
            <div class="loader" data-bind="visible: isLoading"></div>
            <table class="dnnGrid">
                <thead>
                    <tr class="dnnGridHeader">
                        <th><%= Localization.GetString("Url.Header", LocalResourceFile) %></th>
                        <th><%= Localization.GetString("Occurrences.Header", LocalResourceFile) %></th>
                        <th><%= Localization.GetString("Actions.Header", LocalResourceFile) %></th>
                    </tr>
                </thead>
                <tbody data-bind="foreach: urls">
                    <tr class="dnnGridItem ffsr-item" data-bind="css: { hidden : isHandled }">
                        <td><a target="_new"><span data-bind="text: url"></span></a></td>
                        <td><span data-bind="text: occurrences"></span></td>
                        <td>
                            <a href="#" data-bind="click: toggleAddRedirectPanel($index)">
                                <span class="icon" data-bind="attr: {id: 'showAddRedirect_' + $index()}"><%= Icons.GetUrl(IconTypes.Open) %></span>
                                <span class="icon hidden" data-bind="attr: {id: 'hideAddRedirect_' + $index()}"><%= Icons.GetUrl(IconTypes.Close) %></span>
                            </a>
                        </td>
                    </tr>
                    <tr class="ffsr-edit ffrs-addRedirectPanel hidden" data-bind="attr: {id: 'addRedirectPanel_' + $index()} ">
                        <td class="ffsr-edit-item" colspan="3">
                            <div class="loader" data-bind="visible: isLoading"></div>
                            <div class="ffsr-block">
                                <div class="ffsr-block-item">
                                    <div>
                                        <input type="radio" data-bind="checked: mapToType, attr: {id: 'redirectTypeRadioUrl_' + $index(), name: 'redirectType_' + $index(), value: 'URL'} " /><%= Localization.GetString("RedirectToUrl", LocalResourceFile) %>
                                    </div>
                                    <input type="text" data-bind="value: targetUrl, attr: {id: 'redirectUrl_' + $index(), name: 'redirectUrl_' + $index()} " />
                                </div>
                                <div class="ffsr-block-item" data-bind="attr: {id: 'redirectPageDropdown_' + $index()} ">
                                    <div>
                                        <input type="radio" data-bind="checked: mapToType, attr: {id: 'redirectTypeRadioTab_' + $index(), name: 'redirectType_' + $index(), value: 'TAB'} " /><%= Localization.GetString("RedirectToTabId", LocalResourceFile) %>
                                    </div>
                                    <div class="page dnnDropDownList" data-bind="attr: {id: 'createPageDropdown_' + $index()}">
                                        <div class="selected-item">
                                            <a href="javascript:void(0);" title="Click to expand" class="selected-value">Select A Web Page</a>
                                        </div>
                                        <input type="hidden" data-bind="attr: {id: 'createPageDropdownState_' + $index()}, event: { change: setSelectedTab }" />
                                    </div>
                                </div>
                                <div class="ffsr-block-item">
                                    <div>
                                        <input type="radio" data-bind="checked: mapToType, attr: {id: 'redirectTypeRadioNone_' + $index(), name: 'redirectType_' + $index(), value: 'NONE'} " /><%= Localization.GetString("NoRedirect", LocalResourceFile) %>
                                    </div>
                                    <span>&nbsp;</span>
                                </div>
                                <div class="ffsr-block-item">
                                    <div>
                                        <input type="radio" data-bind="checked: mapToType, attr: {id: 'redirectTypeRadioNoLog_' + $index(), name: 'redirectType_' + $index(), value: 'NOLOG'} " /><%= Localization.GetString("NoLogging", LocalResourceFile) %>
                                    </div>
                                    <span>&nbsp;</span>
                                </div>
                            </div>
                            <ul class="dnnActions dnnClear">
                                <li><a class="dnnPrimaryAction" data-bind="click: saveRedirect">Save</a></li>
                                <li><a class="dnnSecondaryAction">Cancel</a></li>
                            </ul>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</asp:Panel>

