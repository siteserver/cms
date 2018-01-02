<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.ModalFileEdit" Trace="false"%>
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
            <label class="col-xs-2 control-label text-right">文件名</label>
            <div class="col-xs-6">
              <asp:TextBox class="form-control" id="TbFileName" runat="server" />
            </div>
            <div class="col-xs-4">
              <asp:RequiredFieldValidator ControlToValidate="TbFileName" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
              />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-2 control-label text-right">文件编码</label>
            <div class="col-xs-6">
              <asp:DropDownList id="DdlCharset" class="form-control" runat="server"></asp:DropDownList>
            </div>
            <div class="col-xs-4"></div>
          </div>

          <div class="form-group">
            <label class="col-xs-2 control-label text-right">编辑方式</label>
            <div class="col-xs-6">
              <asp:DropDownList ID="DdlIsPureText" AutoPostBack="true" OnSelectedIndexChanged="DdlIsPureText_OnSelectedIndexChanged" class="form-control"
                runat="server">
                <asp:ListItem Text="纯文本编辑" Value="True" Selected="true"></asp:ListItem>
                <asp:ListItem Text="使用编辑器" Value="False"></asp:ListItem>
              </asp:DropDownList>
            </div>
            <div class="col-xs-4"></div>
          </div>

          <asp:PlaceHolder ID="PhPureText" runat="server">
            <div class="form-group">
              <div class="col-xs-1"></div>
              <div class="col-xs-10">
                <asp:TextBox ID="TbFileContent" runat="server" TextMode="MultiLine" class="form-control" Height="300"></asp:TextBox>
              </div>
              <div class="col-xs-1"></div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder ID="PhFileContent" visible="false" runat="server">
            <div class="form-group">
              <div class="col-xs-1"></div>
              <div class="col-xs-10">
                <ctrl:UEditor id="UeFileContent" width="540" height="300" runat="server"></ctrl:UEditor>
              </div>
              <div class="col-xs-1"></div>
            </div>
          </asp:PlaceHolder>

          <hr />

          <div class="form-group m-b-0">
            <div class="col-xs-11 text-right">
              <asp:Button class="btn btn-primary m-l-10" ID="BtnCheck" Text="确 定" OnClick="Save_OnClick" runat="server" />
              <asp:Literal ID="LtlOpen" runat="server"></asp:Literal>
              <asp:Literal ID="LtlView" runat="server"></asp:Literal>
              <button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">关 闭</button>
            </div>
            <div class="col-xs-1"></div>
          </div>

        </div>

      </form>
    </body>

    </html>