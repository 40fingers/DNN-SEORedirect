<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="FortyFingers.SeoRedirect.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table width="550" cellspacing="0" cellpadding="4" border="0" width=100%>
	<tr>
		<td class="SubHead" width="150" valign="top"><dnn:label id="plNoOfEntries" controlname="NoOfEntries" runat="server" Text="Number of entries to show" suffix=":" /></td>
		<td valign="top">
		    <asp:TextBox ID="NoOfEntries" runat="server" CssClass="NormalTextBox" Width="60"></asp:TextBox>
            <asp:RangeValidator runat="server" ID="ValNoE" ControlToValidate="NoOfEntries" MinimumValue="0" MaximumValue="1000" ErrorMessage="Please enter a number between 0 and 1000" Type="Integer" />
		</td>
	</tr>
	<tr>
        <td class="SubHead" width="150" valign="top"><dnn:label id="plEnableCleaner" controlname="EnableCleaner" runat="server" Text="Enable cleaner" suffix=":" /></td>
        <td valign="top">
            <asp:CheckBox ID="EnableCleaner" runat="server" CssClass="NormalTextBox" Width="60"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top"><dnn:label id="plMaxAgeDays" controlname="MaxAgeDays" runat="server" Text="Max age in days" suffix=":" /></td>
        <td valign="top">
            <asp:TextBox ID="MaxAgeDays" runat="server" CssClass="NormalTextBox" Width="60"></asp:TextBox>
            <asp:RangeValidator runat="server" ID="ValMaxAge" ControlToValidate="MaxAgeDays" MinimumValue="0" MaximumValue="1000" ErrorMessage="Please enter a number between 0 and 1000" Type="Integer" />
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top"><dnn:label id="plMaxEntries" controlname="MaxEntries" runat="server" Text="Max number of entries" suffix=":" /></td>
        <td valign="top">
            <asp:TextBox ID="MaxEntries" runat="server" CssClass="NormalTextBox" Width="60"></asp:TextBox>
            <asp:RangeValidator runat="server" ID="ValMaxEntries" ControlToValidate="MaxEntries" MinimumValue="0" MaximumValue="1000" ErrorMessage="Please enter a number between 0 and 1000" Type="Integer" />
        </td>
    </tr>
</table>

