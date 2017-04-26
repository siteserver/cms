<%@ Page Language="c#" Inherits="SiteServer.BackgroundPages.PageError"%>

<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>

<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<bairong:Code type="jQuery" runat="server" />
<bairong:Code type="calendar" runat="server" />
<bairong:Code type="bootstrap" runat="server" />
<bairong:Code type="html5shiv" runat="server" />
<link rel="stylesheet" href="inc/style.css" type="text/css" />
<script language="javascript" src="inc/script.js"></script>
<title>错误提示</title>
</head>

<body>
<form class="form-inline" runat="server">
  <asp:Literal id="ltlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />

    <style type="text/css">
    .errorMessage {
      color:red;
    }
    .errorMessage a {
      font-size: 14px;
    }
    </style>

  <div class="popover popover-static">
    <h3 class="popover-title">错误信息</h3>
    <div class="popover-content">
    
      <table class="table noborder">
        <tr>
          <td class="errorMessage">
          	<asp:Literal id="ltlErrorMessage" runat="server"/>
          </td>
        </tr>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
