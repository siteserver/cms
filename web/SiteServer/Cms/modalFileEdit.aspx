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

        <div class="form-group form-row">
          <label class="col-2 col-form-label text-right">文件名</label>
          <div class="col-6">
            <asp:TextBox cssClass="form-control" id="TbFileName" runat="server" />
          </div>
          <div class="col-4">
            <asp:RequiredFieldValidator ControlToValidate="TbFileName" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
            />
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-2 col-form-label text-right">文件编码</label>
          <div class="col-6">
            <asp:DropDownList id="DdlCharset" class="form-control" runat="server"></asp:DropDownList>
          </div>
          <div class="col-4"></div>
        </div>

        <div class="form-group form-row">
          <label class="col-2 col-form-label text-right">编辑方式</label>
          <div class="col-6">
            <asp:DropDownList ID="DdlIsPureText" AutoPostBack="true" OnSelectedIndexChanged="DdlIsPureText_OnSelectedIndexChanged" class="form-control"
              runat="server">
              <asp:ListItem Text="纯文本编辑" Value="True" Selected="true"></asp:ListItem>
              <asp:ListItem Text="使用编辑器" Value="False"></asp:ListItem>
            </asp:DropDownList>
          </div>
          <div class="col-4"></div>
        </div>

        <asp:PlaceHolder ID="PhPureText" runat="server">
          <div class="form-group form-row">
            <div class="col-1"></div>
            <div class="col-10">
              <asp:TextBox ID="TbFileContent" runat="server" TextMode="MultiLine" class="form-control" Height="300"></asp:TextBox>
            </div>
            <div class="col-1"></div>
          </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="PhFileContent" visible="false" runat="server">
          <div class="form-group form-row">
            <div class="col-1"></div>
            <div class="col-10">
              <ctrl:UEditor id="UeFileContent" width="540" height="300" runat="server"></ctrl:UEditor>
            </div>
            <div class="col-1"></div>
          </div>
        </asp:PlaceHolder>

        <hr />

        <div class="text-right mr-1">
          <asp:Button class="btn btn-primary m-l-5" ID="BtnCheck" Text="确 定" OnClick="Save_OnClick" runat="server" />
          <asp:Literal ID="LtlOpen" runat="server"></asp:Literal>
          <asp:Literal ID="LtlView" runat="server"></asp:Literal>
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">关 闭</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->