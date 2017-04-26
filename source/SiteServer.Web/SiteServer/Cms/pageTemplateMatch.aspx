<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTemplateMatch" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<!--#include file="../inc/header.aspx"-->
</head>

<body>
<!--#include file="../inc/openWindow.html"-->
<form class="form-inline" runat="server">
  <asp:Literal id="ltlBreadCrumb" runat="server" />

  <bairong:alerts text="选择左侧栏目（可多选）与右侧模版（单选），点击“匹配”按钮进行模版匹配。<br />左侧栏目列表中第一个括号代表对应栏目所匹配的栏目模版，第二个括号代表栏目下内容页面所匹配的显示模版。" runat="server"></bairong:alerts>

  <table class="table table-bordered">
    <tr>
      <td>
        <table class="table table-noborder">
          <tr>
            <td class="align-right">栏目列表：</td>
            <td width="100">&nbsp;</td>
            <td class="center">栏目模板列表：</td>
            <td width="100">&nbsp;</td>
            <td>内容模板列表：</td>
          </tr>
          <tr>
            <td class="align-right">
              <asp:ListBox ID="NodeIDCollectionToMatch" style="width:auto" SelectionMode="Multiple" Rows="25" runat="server"></asp:ListBox>
            </td>
            <td class="center" style="vertical-align:middle">
              <asp:Button class="btn" id="MatchChannelTemplateButton" text="<- 匹配" onclick="MatchChannelTemplateButton_OnClick" runat="server" />
              <br />
              <br />
              <asp:Button class="btn" id="RemoveChannelTemplateButton" text="-> 取消" onclick="RemoveChannelTemplateButton_OnClick" runat="server" />
            </td>
            <td class="center">
              <asp:ListBox ID="ChannelTemplateID" style="width:auto" DataTextField="TemplateName" DataValueField="TemplateID" SelectionMode="Single" Rows="25" runat="server"></asp:ListBox>
            </td>
            <td class="center" style="vertical-align:middle">
              <asp:Button class="btn" id="MatchContentTemplateButton" text="<- 匹配" onclick="MatchContentTemplateButton_OnClick" runat="server" />
              <br />
              <br />
              <asp:Button class="btn" id="RemoveContentTemplateButton" text="-> 取消" onclick="RemoveContentTemplateButton_OnClick" runat="server" />
            </td>
            <td>
              <asp:ListBox ID="ContentTemplateID" style="width:auto" DataTextField="TemplateName" DataValueField="TemplateID" SelectionMode="Single" Rows="25" runat="server"></asp:ListBox>
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>

  <asp:PlaceHolder ID="phCreate" visible="false" runat="server">
    <ul class="breadcrumb breadcrumb-button">
      <asp:Button class="btn" ID="CreateChannelTemplate" Text="生成栏目模版" OnClick="CreateChannelTemplate_Click" runat="server" />
      <asp:Button class="btn" ID="CreateSubChannelTemplate" Text="生成下级栏目模版" OnClick="CreateSubChannelTemplate_Click" runat="server" />
      <asp:Button class="btn" ID="CreateContentTemplate" Text="生成内容模版" OnClick="CreateContentTemplate_Click" runat="server" />
      <asp:Button class="btn" ID="CreateSubContentTemplate" Text="生成下级内容模版" OnClick="CreateSubContentTemplate_Click" runat="server" />
    </ul>
  </asp:PlaceHolder>

</form>
</body>
</html>
