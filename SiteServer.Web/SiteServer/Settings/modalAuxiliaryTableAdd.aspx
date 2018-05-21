<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.ModalAuxiliaryTableAdd" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form runat="server">
        <ctrl:alerts text="内容表标识为内容表的唯一标识，当在数据库中创建内容表时此标识作为被创建表的名称，只允许包含字母、数字以及下划线" runat="server" />

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">内容表标识</label>
          <div class="col-8">
            <asp:TextBox id="TbTableName" cssClass="form-control" runat="server" />
          </div>
          <div class="col-1">
            <asp:RequiredFieldValidator ControlToValidate="TbTableName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
            />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbTableName" ValidationExpression="[a-zA-Z0-9_]+" ErrorMessage=" *"
              foreColor="red" Display="Dynamic" />
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">内容表名称</label>
          <div class="col-8">
            <asp:TextBox id="TbDisplayName" cssClass="form-control" runat="server" />
          </div>
          <div class="col-1">
            <asp:RequiredFieldValidator ControlToValidate="TbDisplayName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
            />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbDisplayName" ValidationExpression="[^']+" errorMessage=" *"
              foreColor="red" display="Dynamic" />
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">内容表简介</label>
          <div class="col-8">
            <asp:TextBox id="TbDescription" cssClass="form-control" Columns="45" Rows="4" TextMode="MultiLine" runat="server" />
          </div>
          <div class="col-1">
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbDescription" ValidationExpression="[^']+" errorMessage=" *"
              foreColor="red" display="Dynamic" />
          </div>
        </div>

        <hr />

        <div class="text-right mr-1">
          <asp:Button class="btn btn-primary m-l-5" text="确 定" runat="server" onClick="Submit_OnClick" />
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->