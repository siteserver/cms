<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.ModalKeywordAdd" Trace="false" %>
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
            <label class="col-xs-3 text-right control-label">敏感词</label>
            <div class="col-xs-6">
              <asp:TextBox ID="TbKeyword" class="form-control" runat="server" />
            </div>
            <div class="col-xs-3 help-block">
              <asp:RequiredFieldValidator ControlToValidate="TbKeyword" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
              />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 text-right control-label">替换为</label>
            <div class="col-xs-6">
              <asp:TextBox ID="TbAlternative" class="form-control" runat="server" />
            </div>
            <div class="col-xs-3 help-block">
              可以为空
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 text-right control-label">等级</label>
            <div class="col-xs-6">
              <asp:DropDownList ID="DdlGrade" class="form-control" runat="server"></asp:DropDownList>
            </div>
            <div class="col-xs-3 help-block"></div>
          </div>

          <hr />

          <div class="form-group m-b-0">
            <div class="col-xs-11 text-right">
              <asp:Button class="btn btn-primary m-l-10" text="确 定" runat="server" onClick="Submit_OnClick" />
              <button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">取 消</button>
            </div>
            <div class="col-xs-1"></div>
          </div>

        </div>

      </form>
    </body>

    </html>