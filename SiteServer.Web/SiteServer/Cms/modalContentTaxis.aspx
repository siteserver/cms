<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalContentTaxis" Trace="false"%>
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
            <label class="col-xs-3 text-right control-label">排序方向</label>
            <div class="col-xs-8">
              <asp:DropDownList ID="DdlTaxisType" class="form-control" runat="server"></asp:DropDownList>
            </div>
            <div class="col-xs-1"></div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 text-right control-label">移动数目</label>
            <div class="col-xs-8">
              <asp:TextBox class="form-control" Text="1" MaxLength="50" id="TbTaxisNum" runat="server" />
            </div>
            <div class="col-xs-1">
              <asp:RequiredFieldValidator ControlToValidate="TbTaxisNum" errorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbTaxisNum" ValidationExpression="^([1-9]|[1-9][0-9]{1,})$"
                errorMessage=" *" foreColor="red" Display="Dynamic" />
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