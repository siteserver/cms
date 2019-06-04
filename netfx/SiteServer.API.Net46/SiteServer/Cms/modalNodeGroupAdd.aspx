<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalNodeGroupAdd" %>
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

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">栏目组名称</label>
          <div class="col-8">
            <asp:TextBox class="form-control" Columns="25" MaxLength="50" id="TbNodeGroupName" runat="server" />
            <asp:Literal id="LtlNodeGroupName" runat="server" />
          </div>
          <div class="col-1 help-block">
            <asp:RequiredFieldValidator controlToValidate="TbNodeGroupName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
            />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbNodeGroupName" ValidationExpression="[^',]+" errorMessage=" *"
              foreColor="red" display="Dynamic" />
          </div>
        </div>
        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">栏目组简介</label>
          <div class="col-8">
            <asp:TextBox class="form-control" Columns="35" Rows="4" TextMode="MultiLine" id="TbDescription" runat="server" />
          </div>
          <div class="col-1 help-block">
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