<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageUtilityEncrypt" %>
<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>

<head>
  <meta charset="utf-8">
  <!--#include file="../inc/head.html"-->
</head>

<body>
  <form class="m-l-15 m-r-15" runat="server">

    <div class="card-box" style="padding: 10px; margin-bottom: 10px;">
      <ul class="nav nav-pills nav-justified">
        <li class="nav-item">
          <a class="nav-link" href="pageUtilityCache.aspx">系统缓存</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageUtilityParameter.aspx">系统参数查看</a>
        </li>
        <li class="nav-item active">
          <a class="nav-link" href="pageUtilityEncrypt.aspx">加密字符串</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageUtilityJsMin.aspx">JS脚本压缩</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageUtilityDbLogDelete.aspx">清空数据库日志</a>
        </li>
      </ul>
    </div>

    <ctrl:alerts runat="server" />

    <div class="card-box">
      <div class="form-group">
        <label class="col-form-label">需要加密的字符串
          <asp:RequiredFieldValidator id="RequiredFieldValidator" ControlToValidate="TbString" errorMessage=" *"
            foreColor="red" display="Dynamic" runat="server" />
        </label>
        <asp:TextBox TextMode="MultiLine" class="form-control" rows="5" id="TbString" runat="server" />
      </div>

      <asp:PlaceHolder id="PhEncrypted" visible="false" runat="server">
        <div class="form-group">
          <label class="col-form-label">加密后的字符串</label>
          <asp:TextBox TextMode="MultiLine" class="form-control" rows="5" id="TbEncrypted" runat="server" />
        </div>
      </asp:PlaceHolder>

      <hr />

      <asp:Button class="btn btn-primary" text="加 密" onclick="Submit_OnClick" runat="server" />

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->