<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalSelectImage" Trace="false"%>
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

  <script language="javascript" type="text/javascript">
  function selectImage(textBoxUrl, imageUrl) {
    window.parent.document.getElementById('<%=Request.QueryString["TextBoxClientID"]%>').value = textBoxUrl;
    window.parent.closeWindow();
  }
  </script>

  <table width="100%" cellspacing=0 cellpadding=0 border=0 style="line-height:28px;">
    <tr>
      <td width="50"><asp:ImageButton runat="server" ImageUrl="../assets/icons/filesystem/management/back.gif" CommandName="NavigationBar" CommandArgument="Back" OnCommand="LinkButton_Command"></asp:ImageButton></TD>
      <TD width="50"><asp:ImageButton runat="server" ImageUrl="../assets/icons/filesystem/management/up.gif" CommandName="NavigationBar" CommandArgument="Up" OnCommand="LinkButton_Command"></asp:ImageButton></TD>
      <TD width="50"><asp:ImageButton runat="server" ImageUrl="../assets/icons/filesystem/management/reload.gif" CommandName="NavigationBar" CommandArgument="Reload" OnCommand="LinkButton_Command"></asp:ImageButton></TD>
      <TD width="5"><asp:ImageButton runat="server" ImageUrl="../assets/icons/filesystem/management/seperator.gif"></asp:ImageButton></TD>
      <TD width="80"><nobr>
        <asp:HyperLink ID="hlUploadLink" runat="server">
          <asp:ImageButton  runat="server" ImageUrl="../assets/icons/add.gif" ImageAlign="AbsBottom"></asp:ImageButton>
          上传图片</asp:HyperLink>
        </nobr></TD>
      <TD align="right"><span>当前目录：
        <asp:Literal id="ltlCurrentDirectory" runat="server" />
        &nbsp;</span></td>
    </tr>
  </table>

  <hr />
  
  <asp:Literal id="ltlFileSystems" runat="server" enableViewState="false" />

</form>
</body>
</html>
