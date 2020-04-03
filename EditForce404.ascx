<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditForce404.ascx.cs" Inherits="FortyFingers.SeoRedirect.EditForce404" %>
<%@ Import Namespace="FortyFingers.SeoRedirect.Components" %>

<script type="text/javascript">
    var srPid = <%= PortalId %>;
    var srMid = <%= ModuleId %>;
    var ffsrdata = <%= TreeData %>;
    var selectedField = "<%= selectedField.ClientID %>";
    //var ffsrdata =  [
    //    { "id" : "ajson1", "parent" : "#", "text" : "Simple root node" },
    //    { "id" : "ajson2", "parent" : "#", "text" : "Root node 2" },
    //    { "id" : "ajson3", "parent" : "ajson2", "text" : "Child 1" },
    //    { "id" : "ajson4", "parent" : "ajson2", "text" : "Child 2" },
    //];
    $(document).ready(function () {
        $('#treeTabs').jstree({
            'plugins':["wholerow","checkbox"], 
            'core' : {
                'data' : ffsrdata
            },
            "checkbox" : {
                "three_state" : false,
            }
            });
        $('#treeTabs').on("changed.jstree", function (e, data) {
            $("#" + selectedField).val(data.selected);
        });
    });
</script>

<div class="dnnFormMessage dnnFormWarning">
    <asp:Label runat="server" resourcekey="PleaseReadDocs"></asp:Label>
</div>
<div class="dnnFormMessage dnnFormInfo">
    <asp:Label runat="server" resourcekey="ExplainForce404"></asp:Label>
</div>
<div class="ffsr ffsr-editforce404">
    <div id="treeTabs"></div>
</div>
<asp:HiddenField runat="server" id="selectedField"></asp:HiddenField>
<ul class="dnnActions dnnClear">
    <li><asp:LinkButton runat="server" id="lnkSave" CssClass="dnnPrimaryAction" OnClick="lnkSave_OnClick"><%= Localization.GetString("Save") %></asp:LinkButton></li>
    <li><asp:HyperLink runat="server" id="lnkBack" CssClass="dnnSecondaryAction"><%= Localization.GetString("Cancel") %></asp:HyperLink></li>
</ul>

