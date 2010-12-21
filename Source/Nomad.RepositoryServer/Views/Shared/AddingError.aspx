<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<System.Exception>" MasterPageFile="~/Views/Shared/Site.Master" %>
<asp:Content runat="server" ID="Title" ContentPlaceHolderID="TitleContent">
    Error!
</asp:Content>
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">
    <h2>>Error occured!</h2>

    <h4>
        <%= Model.Message %>
    </h4>
</asp:Content>
