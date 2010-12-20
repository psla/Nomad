<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<System.Web.Mvc.HandleErrorInfo>" %>

<asp:Content ID="errorTitle" ContentPlaceHolderID="TitleContent" runat="server">
	Error
</asp:Content>

<asp:Content ID="errorContent" ContentPlaceHolderID="MainContent" runat="server">
	<h2>
		Sorry, an error occurred while processing your request.
	</h2>
	<h5>
		Error message:
	</h5>
	<p>
		<%if (!string.IsNullOrEmpty((string)ViewData["Message"]))
		Html.Encode(ViewData["Message"]);
	else
		Html.Encode("No message passed");
			%>
	</p>
</asp:Content>
