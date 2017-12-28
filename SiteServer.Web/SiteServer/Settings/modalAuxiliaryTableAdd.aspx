<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.ModalAuxiliaryTableAdd" %>
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
        <ctrl:alerts text="辅助表标识为辅助表的唯一标识，当在数据库中创建辅助表时此标识作为被创建表的名称，只允许包含字母、数字以及下划线" runat="server" />

        <div class="form-horizontal">

          <div class="form-group">
            <label class="col-xs-3 text-right control-label">辅助表标识</label>
            <div class="col-xs-8">
              <asp:TextBox id="TbTableEnName" cssClass="form-control" runat="server" />
            </div>
            <div class="col-xs-1">
              <asp:RequiredFieldValidator ControlToValidate="TbTableEnName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbTableEnName" ValidationExpression="[a-zA-Z0-9_]+" ErrorMessage=" *"
                foreColor="red" Display="Dynamic" />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 text-right control-label">辅助表名称</label>
            <div class="col-xs-8">
              <asp:TextBox id="TbTableCnName" cssClass="form-control" runat="server" />
            </div>
            <div class="col-xs-1">
              <asp:RequiredFieldValidator ControlToValidate="TbTableCnName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbTableCnName" ValidationExpression="[^']+" errorMessage=" *"
                foreColor="red" display="Dynamic" />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 text-right control-label">辅助表简介</label>
            <div class="col-xs-8">
              <asp:TextBox id="TbDescription" cssClass="form-control" Columns="45" Rows="4" TextMode="MultiLine" runat="server" />
            </div>
            <div class="col-xs-1">
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