<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageContentAddAfter" %>
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
            后续操作
          </div>
          <p class="text-muted font-13 m-b-25"></p>

          <div class="form-group form-row">
            <label class="col-sm-2 col-form-label">请选择后续操作</label>
            <div class="col-sm-4">
              <asp:RadioButtonList ID="RblOperation" runat="server" AutoPostBack="true" cssClass="radio radio-primary table" RepeatDirection="Vertical"
                OnSelectedIndexChanged="RblOperation_SelectedIndexChanged"></asp:RadioButtonList>
            </div>
            <div class="col-sm-6 help-block"></div>
          </div>

          <asp:PlaceHolder id="PhSiteId" runat="server">
            <div class="form-group form-row">
              <label class="col-sm-2 col-form-label">选择站点</label>
              <div class="col-sm-4">
                <asp:DropDownList ID="DdlSiteId" class="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DdlSiteId_SelectedIndexChanged"></asp:DropDownList>
              </div>
              <div class="col-sm-6 help-block">

              </div>
            </div>
            <div class="form-group form-row">
              <label class="col-sm-2 col-form-label">投稿到</label>
              <div class="col-sm-4">
                <asp:ListBox ID="LbChannelId" SelectionMode="Multiple" Height="320" class="form-control" runat="server"></asp:ListBox>
              </div>
              <div class="col-sm-6">
                <asp:RequiredFieldValidator ControlToValidate="LbChannelId" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
                />
              </div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder id="PhSubmit" runat="server">
            <hr />
            <asp:Button class="btn btn-primary" text="确 定" onclick="Submit_OnClick" runat="server" />
          </asp:PlaceHolder>

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->