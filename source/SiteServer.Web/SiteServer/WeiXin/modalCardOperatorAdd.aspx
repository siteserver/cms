<%@ Page Language="C#" ValidateRequest="false" Inherits="SiteServer.BackgroundPages.WeiXin.ModalCardOperatorAdd" Trace="false" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <!--#include file="../inc/header.aspx"-->
    <style type="text/css">
        .city {
            width: 75px;
        }
    </style>
</head>

<body>
    <!--#include file="../inc/openWindow.html"-->
    <form class="form-inline" runat="server">
        <asp:Button id="BtnSubmit" UseSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" Style="display: none" />
        <bairong:Alerts runat="server"></bairong:Alerts>
      
        <script src="modal_cardOperatorAdd.js"></script>

        <script type="text/javascript"><asp:Literal id="LtlOperatorItems" runat="server" /></script>
        <script type="text/html" class="itemController">
            <input id="itemCount" name="itemCount" type="hidden" value="{{itemCount}}" />
            <table class="table table-bordered table-hover">
                <tr class="info thead">
                    <td width="120">姓名</td>
                    <td width="120">密码</td>
                    <td width="60"></td>
                </tr>
                {{each items}}
                      <tr>
                          <td>
                              <input type="text" name="itemUserName" value="{{$value.userName}}" class="itemUserName input-medium">
                          </td>
                          <td>
                              <input type="text" name="itemPassword" value="{{$value.password}}" class="itemPassword input-xlarge">
                          </td>
                          <td class="center">{{if $index > 0}}
                          <a href="javascript:;" onclick="itemController.removeItem({{$index}});">删除</a>
                              {{/if}}
                          </td>
                      </tr>
                {{/each}}
                      <tr>
                          <td colspan="6">
                              <a href="javascript:;" onclick="itemController.addItem({})" class="btn btn-success">再加一项</a>
                              <span>至少设置一项</span>
                          </td>
                      </tr>
            </table>
        </script>
    </form>
</body>
</html>

