<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.ModalTableMetadataAdd" %>
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
            <label class="col-2 text-right col-form-label">字段名</label>
            <div class="col-5">
              <asp:TextBox id="TbAttributeName" class="form-control" runat="server" />
            </div>
            <div class="col-5">
              <asp:RequiredFieldValidator id="RequiredFieldValidator" ControlToValidate="TbAttributeName" errorMessage=" *" foreColor="red"
                display="Dynamic" runat="server" />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbAttributeName" ValidationExpression="[a-zA-Z0-9_]+" ErrorMessage=" *"
                foreColor="red" Display="Dynamic" />
              <small class="form-text text-muted">只允许包含字母、数字以及下划线</small>
            </div>
          </div>
          <div class="form-group form-row">
            <label class="col-2 text-right col-form-label">数据类型</label>
            <div class="col-5">
              <asp:DropDownList ID="DdlDataType" class="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DdlDataType_SelectedIndexChanged"
              />
            </div>
            <div class="col-5">

            </div>
          </div>

          <asp:PlaceHolder id="PhDataLength" runat="server">
            <div class="form-group form-row">
              <label class="col-2 text-right col-form-label">数据长度</label>
              <div class="col-5">
                <asp:TextBox id="TbDataLength" class="form-control" runat="server" />
              </div>
              <div class="col-5">
                <asp:RequiredFieldValidator ControlToValidate="TbDataLength" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                />
                <asp:RegularExpressionValidator ControlToValidate="TbDataLength" ValidationExpression="\d+" Display="Dynamic" ErrorMessage="数据长度必须为数字"
                  foreColor="red" runat="server" />
              </div>
            </div>
          </asp:PlaceHolder>

          <hr />

          <div class="text-right mr-1">
            <asp:Button class="btn btn-primary m-l-5" text="确 定" runat="server" onClick="Submit_OnClick" />
            <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
          </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->