<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageKeywordConfiguration" %>
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
  <asp:Literal id="LtlBreadCrumb" runat="server" />
  <bairong:alerts runat="server"></bairong:alerts>

  <script type="text/javascript">
    var selectWelcomeKeyword = function(keyword){
      $('#TbWelcomeKeyword').val(keyword);
    };
    var selectDefaultReplyKeyword = function(keyword){
      $('#TbDefaultReplyKeyword').val(keyword);
      };
     
  </script>

  <div class="popover popover-static">
    <h3 class="popover-title">智能回复设置</h3>
    <div class="popover-content">
    
      <table class="table noborder table-hover">
        <tr>
          <td width="160">开启关注时回复：</td>
          <td>
              <asp:CheckBox ID="CbIsWelcome" class="checkbox" text="开启" runat="server"></asp:CheckBox>
          </td>
        </tr>
        <tr>
          <td>关注时回复关键词：</td>
          <td>
            <asp:TextBox id="TbWelcomeKeyword" runat="server"/>
            &nbsp;<asp:Button id="BtnWelcomeKeywordSelect" class="btn btn-info" text="选择" runat="server" />
          </td>
        </tr>
        <tr>
          <td>开启默认无匹配回复：</td>
          <td>
              <asp:CheckBox ID="CbIsDefaultReply" class="checkbox" text="开启" runat="server"></asp:CheckBox>
          </td>
        </tr>
        <tr>
          <td>默认无匹配回复关键词：</td>
          <td>
            <asp:TextBox id="TbDefaultReplyKeyword" runat="server"/>
            &nbsp;<asp:Button id="BtnDefaultReplyKeywordSelect" class="btn btn-info" text="选择" runat="server" />
          </td>
        </tr>
      </table>
  
      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />
          </td>
        </tr>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
