<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalContentDiggSet" Trace="false"%>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <!--#include file="../inc/openWindow.html"-->

      <form runat="server">
        <ctrl:alerts runat="server" />

        <div class="form-horizontal">

          <div class="form-group">
            <label class="col-xs-3 control-label text-right">赞同数</label>
            <div class="col-xs-8">
              <asp:TextBox class="form-control" MaxLength="50" id="TbGoodNum" runat="server" />
            </div>
            <div class="col-xs-1">
              <asp:RequiredFieldValidator ControlToValidate="TbGoodNum" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator ControlToValidate="TbGoodNum" ValidationExpression="\d+" Display="Dynamic" ErrorMessage="赞同数必须为数字"
                foreColor="red" runat="server" />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 control-label text-right">不赞同数</label>
            <div class="col-xs-8">
              <asp:TextBox class="form-control" MaxLength="50" Text="0" id="TbBadNum" runat="server" />
            </div>
            <div class="col-xs-1">
              <asp:RequiredFieldValidator ControlToValidate="TbBadNum" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator ControlToValidate="TbBadNum" ValidationExpression="[\d\.]+" Display="Dynamic" ErrorMessage="不赞同数必须为数字,可以带小数点"
                foreColor="red" runat="server" />
            </div>
          </div>

          <hr />

          <div class="form-group m-b-0">
            <div class="col-xs-11 text-right">
              <asp:Button class="btn btn-primary m-l-10" Text="确 定" OnClick="Submit_OnClick" runat="server" />
              <button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">取 消</button>
            </div>
            <div class="col-xs-1"></div>
          </div>

        </div>

      </form>
    </body>

    </html>