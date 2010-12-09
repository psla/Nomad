<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" 
Inherits="System.Web.Mvc.ViewPage<Nomad.RepositoryServer.Models.RepositoryModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <h2>
        List of avaliable modules within this Nomad Modules Center:
    </h2>
    
    <table>
    <tr>
        <th>Module Name</th>
        <th>Module Version</th>
        <th>Signature Issuer</th>
        <th>Details Link</th>
        <th>Download Link</th>
    </tr>
        <% foreach(var item in Model.ModuleInfosList)
           { %>
               <tr>
                <td> <%= item.Manifest.ModuleName %></td>
                <td> <%= item.Manifest.ModuleVersion.ToString() %></td>
                <td> <%= item.Manifest.Issuer %></td>
                
                
                <td>
                    <%= Html.ActionLink("Details","Details", new { id = item.Id}, HttpVerbs.Get) %>
                </td>
                <td>
                    <%= Html.ActionLink("Download","GetModulePackage","Modules",new { urlId = item.Id},HttpVerbs.Get) %>
                </td>
                <td>
                    <%= Html.ActionLink("Remove","Remove",new {id = item.Id}, HttpVerbs.Get) %>
                </td>
                <!-- 
                    TODO: add more properties with links and so on.
                    remove and maybe edit

                -->

               </tr>
           <% } %>
    </table>
    
</asp:Content>
