<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageContentTranslate" %>
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
  <bairong:alerts text="所选内容将转移到指定站点下的栏目中，可以同时选择多个栏目，内容将同时转移到对应栏目下。<br>
  转移有四种方式：<br>
  “复制”将创建内容的副本，并拷贝到指定栏目下，副本和原始内容之间不存在关系；<br>
  “剪切”代表将内容转移到指定栏目下，系统不会创建内容副本；<br>
  “引用地址”将创建内容的副本，并拷贝到指定栏目下，内容副本仅是原内容的引用，内容副本链接将和原内容链接一致。<br>
  “引用内容”将创建内容的副本，并拷贝到指定栏目下，同时内容副本的数据与原内容保持同步，内容副本的链接指向副本内容。    " runat="server" />

  <script language="javascript" type="text/javascript">
  function translateNodeAdd(name, value){
    $('#translateContainer').append("<div id='translate_" + value + "' class='addr_base addr_normal'><b>" + name + "</b> <a class='addr_del' href='javascript:;' onClick=\"translateNodeRemove('" + value + "')\"></a></div>");
    $('#translateCollection').val(value + ',' + $('#translateCollection').val());
  }
  function translateNodeRemove(value){
    $('#translate_' + value).remove();
    var val = '';
    var values = $('#translateCollection').val().split(",");
    for (i=0;i<values.length ;i++ )
    {
      if (values[i] && value != values[i]){val = values[i] + ',';}
    }
    $('#translateCollection').val(val);
  }
  </script>

  <div class="popover popover-static">
    <h3 class="popover-title">内容转移</h3>
    <div class="popover-content">

      <table class="table noborder table-hover">
        <tr>
          <td width="160">需要转移的内容：</td>
          <td><asp:Literal ID="ltlContents" runat="server"></asp:Literal></td>
        </tr>
        <tr>
          <td>转移到栏目：</td>
          <td>
            <div class="fill_box" id="translateContainer" style="display:"></div>
            <input id="translateCollection" name="translateCollection" value="" type="hidden">
            <div ID="divTranslateAdd" class="btn_pencil" runat="server"><span class="pencil"></span>　选择</div>
          </td>
        </tr>
        <tr>
          <td>转移方式：</td>
          <td><asp:RadioButtonList ID="ddlTranslateType" class="radiobuttonlist" repeatDirection="Horizontal" runat="server"></asp:RadioButtonList></td>
        </tr>
      </table>

      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-primary" ID="Submit" Text="转 移" OnClick="Submit_OnClick" runat="server"/>
            <asp:Button class="btn" ID="Return" Text="返 回" CausesValidation="false" OnClick="Return_OnClick" runat="server"/>
          </td>
        </tr>
      </table>

    </div>
  </div>

</form>
</body>
</html>
