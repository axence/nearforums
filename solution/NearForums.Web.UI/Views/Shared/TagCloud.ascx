<%@ Control Language="C#" Inherits="NearForums.Web.UI.BaseViewUserControl<List<WeightTag>>" %>
<%
if (this.Model.Count > 0)
{
%>
	<div class="tagCloud floatContainer">
		<h2>Tags</h2>
		<ul>
<%
		foreach (WeightTag tag in this.Model)
		{
			int size;
			if (tag.Weight < 2)
			{
				size = 1;
			}
			else if (tag.Weight < 4)
			{
				size = 2;
			}
			else if (tag.Weight < 6)
			{
				size = 3;
			}
			else if (tag.Weight < 10)
			{
				size = 4;
			}
			else
			{
				size = 5;
			}
%>
			<li class="weight<%=size %>"><%= Html.ActionLink(tag.Value, "TagDetail", "Forums", new{tag=tag.Value}, null) %></li>
<%		
		}
%>
		</ul>
	</div>
<%
}
%>