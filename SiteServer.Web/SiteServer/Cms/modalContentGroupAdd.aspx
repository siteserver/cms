<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalContentGroupAdd" Trace="false"%>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form runat="server">
        <ctrl:alerts runat="server" />

        <div class="form-horizontal">

          <div class="form-group">
            <label class="col-xs-3 text-right control-label">内容组名称</label>
            <div class="col-xs-8">
              <asp:TextBox class="form-control" Columns="25" MaxLength="50" id="TbContentGroupName" runat="server" />
              <asp:Literal id="LtlContentGroupName" runat="server" />
            </div>
            <div class="col-xs-1 help-block">
              <asp:RequiredFieldValidator controlToValidate="TbContentGroupName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbContentGroupName" ValidationExpression="[^',]+" errorMessage=" *"
                foreColor="red" display="Dynamic" />
            </div>
          </div>
          
          <div class="form-group">
            <label class="col-xs-3 text-right control-label">内容组简介</label>
            <div class="col-xs-8">
              <asp:TextBox class="form-control" Columns="35" Rows="4" TextMode="MultiLine" id="TbDescription" runat="server" />
            </div>
            <div class="col-xs-1 help-block">
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbDescription" ValidationExpression="[^']+" errorMessage=" *"
                foreColor="red" display="Dynamic" />
            </div>
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