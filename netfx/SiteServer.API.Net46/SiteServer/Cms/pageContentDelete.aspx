<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageContentDelete" %>
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
            删除内容
          </div>
          <p class="text-muted font-13 m-b-25"></p>

          <div class="form-group form-row">
            <label class="col-sm-2 col-form-label">需要删除的内容</label>
            <div class="col-sm-9">
              <asp:Literal ID="LtlContents" runat="server"></asp:Literal>
            </div>
            <div class="col-sm-1"></div>
          </div>

          <asp:PlaceHolder id="PhRetain" runat="server">
            <div class="form-group form-row">
              <label class="col-sm-2 col-form-label">是否保留文件</label>
              <div class="col-sm-9">
                <asp:RadioButtonList ID="RblRetainFiles" runat="server" RepeatDirection="Horizontal" class="radio radio-primary">
                  <asp:ListItem Text="保留生成的文件" Value="true"></asp:ListItem>
                  <asp:ListItem Text="删除生成的文件" Value="false" Selected="true"></asp:ListItem>
                </asp:RadioButtonList>
                <small class="form-text text-muted">选择保留文件则此操作将仅在数据库中删除内容。</small>
              </div>
              <div class="col-sm-1"></div>
            </div>
          </asp:PlaceHolder>

          <hr />

          <div class="text-center">
            <asp:Button class="btn btn-danger" Text="删 除" OnClick="Submit_OnClick" runat="server" />
            <asp:Button class="btn m-l-5" type="button" CausesValidation="false" OnClick="Return_OnClick" text="返 回" runat="server" />
          </div>

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->