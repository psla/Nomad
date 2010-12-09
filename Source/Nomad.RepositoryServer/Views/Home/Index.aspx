<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Nomad.RepositoryServer.Models.RepositoryModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        List of avaliable modules within this Nomad Modules Center:
    </h2>
    <table>
        <tr>
            <th>
                Module Name
            </th>
            <th>
                Module Version
            </th>
            <th>
                Signature Issuer
            </th>
            <th colspan="3">
                Actions
            </th>
        </tr>
        <% foreach (var item in Model.ModuleInfosList)
           { %>
        <tr>
            <td>
                <%= item.Manifest.ModuleName %>
            </td>
            <td>
                <%= item.Manifest.ModuleVersion.ToString() %>
            </td>
            <td>
                <%= item.Manifest.Issuer %>
            </td>
            <td>
                <%= Html.ActionLink("Details","Details", new { id = item.Id}, HttpVerbs.Get) %>
            </td>
            <td>
                <%= Html.ActionLink("Download","GetModulePackage","Modules",new { urlId = item.Id},HttpVerbs.Get) %>
            </td>
            <td>
                <%= Html.ActionLink("Remove","Remove",new {id = item.Id}, HttpVerbs.Get) %>
            </td>
        </tr>
        <% } %>
    </table>
    <!--
        FIXME: Add better way of viewing this, beacause my UI skills sux
    -->
    <div>
        <p>
            <h3>
                Add new module here</h3>
        </p>
        <form action="/Home/AddModule" method="post" enctype="multipart/form-data">
        <table>
            <tr>
                <td>
                    <label>
                        Assembly File:
                    </label>
                </td>
                <td>
                    <input type="file" name="file" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <input type="submit" value="Send" />
                </td>
            </tr>
        </table>
        </form>
    </div>
</asp:Content>
