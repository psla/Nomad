<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Nomad.RepositoryServer.Models.IModuleInfo>"
	MasterPageFile="~/Views/Shared/Site.Master" %>
<%@ Import Namespace="Nomad.Signing.FileUtils" %>

<asp:Content runat="server" ID="Title" ContentPlaceHolderID="TitleContent">
	<h1>
		Details
		<%= Model.Manifest.ModuleName %>
	</h1>
</asp:Content>
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">
	<!-- 
		TODO: put content here about showing details, like signed files, signature, dependencies and so on
		using various controls
		-->
	<table>
		<tr>
			<th>
				Module Name:
			</th>
			<td>
				<%= Model.Manifest.ModuleName %>
			</td>
		</tr>
		<tr>
			<th>
				Module Version
			</th>
			<td>
				<%= Model.Manifest.ModuleVersion %>
			</td>
		</tr>
		<tr>
			<th>
				Issuer of the signature
			</th>
			<td>
				<%= Model.Manifest.Issuer %>
			</td>
		</tr>
		<tr>
			<th colspan="2">
				Module Dependencies
			</th>
		</tr>
		<tr>
			<th> 
				Dependency Name
			</th>
			<th>
				Dependency Version
			</th>
		</tr>
		<% foreach (var moduleDependency in Model.Manifest.ModuleDependencies)
		   {
		%>
		<tr>
			<td>
				<%= moduleDependency.ModuleName%>
			</td>
			<td>
				<%= moduleDependency.MinimalVersion %>
			</td>
		 </tr>
		<%}%>
		<tr>
			<th colspan="2">
				Signed files
			</th>
		</tr>
			<%foreach (var signedFile in Model.Manifest.SignedFiles)
			{ %>
			   
			   <tr>
					<td colspan="2">
						<%= signedFile.FilePath %>
					</td>
			   </tr>
			<% } %>
	</table>
</asp:Content>
