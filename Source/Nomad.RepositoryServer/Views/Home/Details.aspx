<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Nomad.RepositoryServer.Models.IModuleInfo>" MasterPageFile="~/Views/Shared/Site.Master" %>
<asp:Content runat="server" ID="Title" ContentPlaceHolderID="TitleContent">
    <h1>Details <%= Model.Manifest.ModuleName %> </h1>
</asp:Content>
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">
    <!-- 
        TODO: put content here about showing details, like signed files, signature, dependencies and so on
        using various controls
        -->

</asp:Content>
