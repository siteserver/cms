<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTemplateReference" enableviewstate="false"%>
<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>

<head>
  <meta charset="utf-8">
  <!--#include file="../inc/head.html"-->
</head>

<body>
  <form class="m-l-15 m-r-15" runat="server">
    <ctrl:alerts runat="server" />

    <asp:PlaceHolder id="PhRefenreces" visible="false" runat="server">
      <div class="card-box">
        <div class="tab-content">
          <asp:Literal ID="LtlReferences" runat="server"></asp:Literal>
        </div>
      </div>
    </asp:PlaceHolder>

    <div class="card-box">

      <div class="m-t-0 header-title">
        STL语言参考
      </div>
      <p class="text-muted font-13 m-b-25">
        STL语言为SiteServer模板语言(SiteServer Template Language)的缩写，是一种和HTML语言类似的服务器端语言。
        <a href="https://sscms.com/docs/v6/stl/" target="_blank">STL 语言参考手册</a>
      </p>

      <asp:Literal ID="LtlAll" runat="server"></asp:Literal>

    </div>

  </form>
</body>

</html>
