<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalCreateChannels" Trace="false"%>
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
          <label class="col-4 col-form-label text-right">是否生成下级栏目</label>
          <div class="col-7">
            <asp:DropDownList ID="DdlIsIncludeChildren" class="form-control" runat="server">
              <asp:ListItem Text="生成下级栏目" Value="True"></asp:ListItem>
              <asp:ListItem Text="仅生成选中栏目" Value="False" Selected="true"></asp:ListItem>
            </asp:DropDownList>
          </div>
          <div class="col-1"></div>
        </div>

        <div class="form-group form-row">
          <label class="col-4 col-form-label text-right">是否生成内容页</label>
          <div class="col-7">
            <asp:DropDownList ID="DdlIsCreateContents" class="form-control" runat="server">
              <asp:ListItem Text="生成内容页" Value="True"></asp:ListItem>
              <asp:ListItem Text="不生成内容页" Value="False" Selected="true"></asp:ListItem>
            </asp:DropDownList>
          </div>
          <div class="col-1"></div>
        </div>

        <hr />

        <div class="text-right mr-1">
          <asp:Button class="btn btn-primary m-l-5" ID="BtnCheck" Text="确 定" OnClick="Submit_OnClick" runat="server" />
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->