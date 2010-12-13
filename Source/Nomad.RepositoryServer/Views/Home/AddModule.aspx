<%@ Page Title="" Language="C#" 
Inherits="System.Web.Mvc.ViewPage<Nomad.RepositoryServer.Models.ServerSigner.VirtualModuleAdder>" 
MasterPageFile="~/Views/Shared/Site.Master" %>
<%@ Import Namespace="System.IO" %>
<asp:Content runat="server" ID="Title" ContentPlaceHolderID="TitleContent">
	Adding module - step 2
</asp:Content>
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">
<!--
	Manifest basic control for displaying info about the manifest
-->
  <div>
	<table>
		<tr>
			<th>
				Name:
			</th>
			<td>
				<%= Model.ModuleInfo.Manifest.ModuleName %>
			</td>
		</tr>
		<tr>
			<th>
				Version:
			</th>
			<td>
				<%= Model.ModuleInfo.Manifest.ModuleVersion.ToString() %>
			</td>
		</tr>
	</table>
  </div>

  <!--
	Adding files to the virtual folder
  -->
  <div>
		<h4>Add aditional files here, which might be requested during manifest building</h4>
		<%using(Html.BeginForm("AddFile","Home",FormMethod.Post,new{enctype="multipart/form-data"}))
		{
		%>
		<table>
			<tr>
				<td>
					<label>
						Additional file:
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
		<%  } %>
	</div>

	<!-- 
		List of files with checkboxe 
	-->
   <div style="background-color:White;">
   <h4>List of files in your virtual folder</h4>
   <% using( Html.BeginForm("PublishModule","Home")) { %> 
	<table>
		<tr>
		<th>File</th>
		<th>Size</th>
		<th>Include in package</th>
		</tr>
		<%foreach (var item in Model.InFolderFiles)                       
		  {
		%>
			<tr>
				<td><%= item.FileName %></td>
				<td><%= item.Size %></td>
				<td>
					<%= Html.CheckBox(item.FileName,item.ToPackage) %>
				</td>
			</tr>  
		  <% 
			} 
		  %>
	</table>
		<input type="submit" value="Publish Module" />
	<%}%>
	</div>


  
</asp:Content>
