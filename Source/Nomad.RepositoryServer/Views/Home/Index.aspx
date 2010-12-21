<%@ Page CodeFile="Index.aspx.cs" Inherits="Nomad.RepositoryServer.Views.Home.Index" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Index of modules
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Available modules:
    </h2>
    <table>
        <tr>
            <th>
                ID
            </th>
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
        <% foreach (var item in Model)
           { %>
        <tr>
            <td>
                <%= item.Id %>
            </td>
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
                <%= Html.ActionLink("Details", "Details", "Home", new { itemId = item.Id }, HttpVerbs.Get)%>
            </td>
            <td>
                <%= Html.ActionLink("Download","GetModulePackage","Modules",new { urlId = item.Id},HttpVerbs.Get) %>
            </td>
            <td>
                <%= Html.ActionLink("Remove", "Remove", "Home", new { itemId = item.Id }, HttpVerbs.Get)%>
            </td>
        </tr>
        <% } %>
    </table>
    <!--
        FIXME: Add better way of viewing this, because my UI skills sux
    -->
    <div>
        <p>
            <b>Add new module here</b>
        </p>
        
        <% using (Html.BeginForm("UploadPackage", "Plain", FormMethod.Post, new { enctype = "multipart/form-data" }))
           {%> 
        <table>
            <tr>
                <td>
                    <label>
                        Module Package (*.zip)
                    </label>
                </td>
                <td>
                    <input type="file" name="file" />
                    
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <input type="submit" value="Upload" />
                </td>
            </tr>
        </table>
        <%
           }%>
        
    </div>
</asp:Content>
