<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" 
Inherits="System.Web.Mvc.ViewPage<Nomad.RepositoryServer.Models.RepositoryModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%= Html.Encode(ViewData["Message"]) %></h2>
    <p>
        List of avaliable modules within this Nomad Modules Center:
    </p>
    
    <table>
        <% foreach(var item in Model.ModuleInfosList)
           { %>
               <tr>
                <td> <%= item.Manifest.ModuleName %></td>
                <td> <%= item.Manifest.ModuleVersion.ToString() %></td>
                <td> <%= item.Manifest.Issuer %></td>
                
                <!-- 
                    TODO: add more properties with links and so on.
                    remove,edit,download (eventually on authorized)

                -->

               </tr>
           <% } %>
    </table>
    
</asp:Content>
