<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Add Template
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<ul class="path floatContainer">
		<li class="first"><%=Html.ActionLink("Admin", "Dashboard", "Admin") %></li>
		<li><%=Html.ActionLink("Templates", "ListTemplates", "Admin")%></li>
	</ul>
    <h1>Add Template</h1>
    <%=Html.ValidationSummary("<h3>Please check the following errors:</h3>", new Dictionary<string, object>
		{
			{"postedFile", new Dictionary<ValidationErrorType, string>()
						{ 
							{ValidationErrorType.AccessRights, "The application does not file Write access on the server's template folder."}
							,{ValidationErrorType.FileFormat, "File format is invalid. Please include only the allowed files."},
						}}
		}, null)%>
	<% Html.BeginForm(null, null, FormMethod.Post, new{enctype="multipart/form-data"}); %>
	<div class="formItem floatContainer">
		<label for="txtKey">Key</label><input type="text" name="key" id="txtKey" value="demo1" />
	</div>
	<div class="formItem floatContainer">
		<label for="txtPostedFile">Template File (.zip)</label><input type="file" name="postedFile" id="txtPostedFile" />
	</div>
	<div class="formItem buttons floatContainer">
		<input type="submit" value="Submit" />
	</div>
	<% Html.EndForm(); %>
	<h2 style="padding-top: 30px;">File format</h2>
	<p>Compressed zip file with the following allowed files and folders.</p>
	<ul>
		<li><strong>template.html</strong>: html file. The html file must contain the following place holders: {-TitleContent-}, {-HeadContent-}, {-LoginSmall-}, {-MainContent-}. Please mantain the footer links to the Nearforums website.</li>
		<li>
			<strong>template-contents</strong>: folder that contains any css, png, jpg or gif files.
			<ul>
				<li>picture1.png</li>
				<li>icon.png</li>
				<li>picture2.jpg</li>
				<li>styles.css</li>
				<li>...etc...</li>
			</ul>
		</li>
	</ul>
</asp:Content>