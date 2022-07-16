<%@ Assembly Name="DCMA.SP.TrackingPII, Version=1.0.0.0, Culture=neutral, PublicKeyToken=8593ae0ac35dd13e" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CreatePIIListWPUserControl.ascx.cs" Inherits="DCMA.SP.TrackingPII.CreatePIIListWP.CreatePIIListWPUserControl" %>

<%--<asp:GridView ID="searchGridView" runat="server">

</asp:GridView>--%>
<div>
<asp:Label ID="TitleLabel" runat="server" Font-Size="Medium" ForeColor="#0066CC" Text="Scan PII Content and Save to the PII Tracking List" Font-Bold="True"></asp:Label>
</div>
</br>
<div>
<asp:Label ID="InsLabel" runat="server"  ForeColor="Black" Text="Use button below to scan and save content containing the words &quot;PII&quot; or &quot;SSN&quot; or &quot;DOB&quot; or &quot;SENSITIVE&quot; in this site collection, including subsites. It creates a list titled as 'PII Tracking List' where the scan results will be saved and adds to the left navigation of this site.</br> If the list has been created previously through this function, clicking the button updates the list by adding PII content created after last scan.</br></br><i>You may refresh the browser after clicking the button to see changes if success message is not displayed.</i></br> "></asp:Label>
</div>
</br>
<div>
<asp:Button ID="ButtonPII" runat="server" Text="Scan and Save PII Content" OnClick="ButtonPII_Click" />
</div>

<div>

<asp:Label ID="InfoLabel" runat="server" Font-Italic="True" ForeColor="Black"></asp:Label>
 </div>

<div>

<asp:Literal ID="ListPermLiteral" runat="server"></asp:Literal>

</div>
