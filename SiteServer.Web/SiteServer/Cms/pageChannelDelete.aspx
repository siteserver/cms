<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageChannelDelete" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">
        <ctrl:alerts runat="server" />

        <div class="card-box">
          <div class="m-t-0 header-title">
              <asp:Literal id="LtlPageTitle" runat="server"></asp:Literal>
          </div>
          <p class="text-muted font-13 m-b-25"></p>

          <div class="form-group form-row">
            <label class="col-sm-2 col-form-label">是否保留文件</label>
            <div class="col-sm-4">
              <asp:RadioButtonList ID="RblRetainFiles" runat="server" RepeatDirection="Horizontal" class="radio radio-primary">
                <asp:ListItem Text="保留生成的文件" Value="true"></asp:ListItem>
                <asp:ListItem Text="删除生成的文件" Value="false" Selected="true"></asp:ListItem>
              </asp:RadioButtonList>
            </div>
            <div class="col-sm-6"></div>
          </div>

          <hr />

          <div class="text-center">
            <asp:Button class="btn btn-danger" ID="BtnDelete" Text="删 除" OnClick="Delete_OnClick" runat="server" />
            <input class="btn m-l-5" type="button" onclick="location.href='<%=ReturnUrl%>';return false;" value="返 回" />
          </div>

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->