<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomFax.aspx.cs" Inherits="AimsHub.Views.PatientLog.CustomFax" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=14.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<script type="text/javascript">
    function getMessage() {
        var ans;
        ans = window.confirm('Do you want to send the PCP communication?');
        if (ans == true)
        {
            document.form1.hdnbox.value = 'Yes';
        }
        else {
            document.form1.hdnbox.value = 'No';
        }
    }
</script>
<script src="../../Scripts/jquery-3.2.1.min.js"></script>
<script src="../../Scripts/bootstrap.js"></script>
<script src="../../Scripts/AIMScommon.js"></script>
<link href="../../Content/bootstrap.css" rel="stylesheet" />
<link href="../../Content/AimsStyle.css" rel="stylesheet" />
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <input type="hidden" id="hdnbox" />
        <span class="col-lg-12" style="margin-top: 8px;">
            <span class="col-lg-3" id="message">
                <asp:Label ID="lblMessage" runat="server" Text="Please select fax type:" ></asp:Label>
            </span>
            <span class="col-lg-2">
                <asp:DropDownList AutoPostBack="true" ID="drpFaxTypes" runat="server">
                    <asp:ListItem Text="Please select a fax type.." Value="default" ></asp:ListItem>
                    <asp:ListItem Text="Admit Notice" Value="AdmitNotice" ></asp:ListItem>
                    <asp:ListItem Text="Discharge Notice" Value="DischargeNotice" ></asp:ListItem>
                    <asp:ListItem Text="General Communication" Value="GeneralCommunication" ></asp:ListItem>
                    <asp:ListItem Text="Discharge Summary" Value="DischargeSummary" ></asp:ListItem>
                </asp:DropDownList>
            </span>
        </span>
        <span class="col-lg-12">
            <span class="col-lg-2">
                <asp:Button CssClass="btn btn-primary" ID="btnEdit" runat="server" Text="Edit" Visible="false" />
                <asp:Button CssClass="btn btn-danger" ID="btnCancel" runat="server" Text="Cancel Changes" Visible="false" />
            </span>
            <span class="col-lg-2">
                <asp:Button CssClass="btn btn-success" ID="btnSend" runat="server" Text="Send Fax" Visible="false" />
                <asp:Button CssClass="btn btn-primary" ID="btnSave" runat="server" Text="Save Changes" Visible="false" />
            </span>
        </span>
        <span class="col-lg-12">
            <asp:Panel ID="editPanel" runat="server" Visible="false">

            </asp:Panel>
        </span>
        <span class="col-lg-12">
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" ProcessingMode="Local" Visible="false"
                Font-Size="8pt" Height="11in" Width="8.5in" DocumentMapWidth="100px"
                Font-Names="Verdana" ExportContentDisposition="AlwaysAttachment" SizeToReportContent="true"
                ShowFindControls="false" ShowPageNavigationControls="false">
            </rsweb:ReportViewer>
        </span>
        <%--<span class="col-lg-12">
            <asp:Button ID="btnCloseWindow" runat="server" Text="Close Popup" />
            <asp:Button ID="btnFax" OnClientClick="getMessage()" OnClick="btnFax_Clicked" runat="server" Text="Submit Fax" />
        </span>--%>
        <div>
        </div>
    </form>
</body>
</html>
