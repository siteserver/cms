<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Sys.ModalChangePublishmentSystemType" Trace="false"%>
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
<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server"></bairong:alerts>

  <asp:PlaceHolder ID="HeadquartersExists" runat="server">
    <table class="table table-noborder">
      <tr>
        <td colspan="2"> 根目录站点已经存在，站点<%=GetSiteName()%>不能转移到根目录 </td>
      </tr>
    </table>
  </asp:PlaceHolder>

  <asp:PlaceHolder ID="ChangeToSite" runat="server">
    <table class="table table-noborder">
      <tr>
        <td colspan="2">
          <blockquote>
            <p>将站点<code><%=GetSiteName()%></code>转移到子目录</p>
          </blockquote>
        </td>
      </tr>
      <tr>
        <td width="100">站点文件夹名称：</td>
        <td><asp:TextBox Columns="25" MaxLength="50" id="PublishmentSystemDir" runat="server"/>
          <asp:RequiredFieldValidator
              ControlToValidate="PublishmentSystemDir"
              errorMessage=" *" foreColor="red"
              Display="Dynamic"
              runat="server"/>
          <asp:RegularExpressionValidator
              runat="server"
              ControlToValidate="PublishmentSystemDir"
              ValidationExpression="[^']+"
              errorMessage=" *" foreColor="red"
              Display="Dynamic" />
              <br />
              <span class="gray">实际在服务器中保存此网站的文件夹名称，此路径必须以英文或拼音命名。</span>
            </td>
      </tr>
      <tr>
        <td colspan="2">
          从系统根目录选择需要转移到子站点的文件夹及文件：
        </td>
      </tr>
      <tr>
        <td colspan="2"><asp:CheckBoxList ID="FilesToSite" RepeatDirection="Horizontal" class="noborder" RepeatColumns="3" runat="server"></asp:CheckBoxList></td>
      </tr>
    </table>
  </asp:PlaceHolder>

  <asp:PlaceHolder ID="ChangeToHeadquarters" runat="server">
    <table class="table table-noborder">
      <tr>
        <td colspan="2"> 将站点<code><%=GetSiteName()%></code>转移到根目录 </td>
      </tr>
      <tr>
        <td width="140">转移文件夹及文件：</td>
        <td><asp:RadioButtonList ID="IsMoveFiles" runat="server" RepeatDirection="Horizontal" class="noborder">
            <asp:ListItem Text="转移" Value="true" Selected="true"></asp:ListItem>
            <asp:ListItem Text="不转移" Value="false"></asp:ListItem>
          </asp:RadioButtonList>
          <br />
          <span class="gray">选择转移将把此站点内的文件夹及文件转移到系统根目录中。</span>
        </td>
      </tr>
    </table>
  </asp:PlaceHolder>

</form>
</body>
</html>
