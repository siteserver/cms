<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTemplateMatch" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">
        <ctrl:alerts text="选择左侧栏目（可多选）与右侧模版（单选），点击“匹配”按钮进行模版匹配。<br />左侧栏目列表中第一个括号代表对应栏目所匹配的栏目模版，第二个括号代表栏目下内容页面所匹配的显示模版。" runat="server"
        />

        <div class="card-box">

          <div class="row">
            <div class="col-4">

              <div class="form-row">
                <label for="LbChannelId">栏目列表</label>
                <asp:ListBox ID="LbChannelId" class="form-control" SelectionMode="Multiple" Rows="25" runat="server"></asp:ListBox>
              </div>
            </div>
            <div class="col-1 text-center" style="padding-top: 200px;">
              <asp:Button class="btn btn-primary" id="MatchChannelTemplateButton" text="匹 配" onclick="MatchChannelTemplateButton_OnClick"
                runat="server" />
              <hr />
              <asp:Button class="btn" id="RemoveChannelTemplateButton" text="取 消" onclick="RemoveChannelTemplateButton_OnClick" runat="server"
              />
            </div>
            <div class="col-3">

              <div class="form-row">
                <label for="LbChannelTemplateId">栏目模板列表</label>
                <asp:ListBox ID="LbChannelTemplateId" class="form-control" DataTextField="TemplateName" DataValueField="Id" SelectionMode="Single"
                  Rows="25" runat="server"></asp:ListBox>
              </div>
            </div>
            <div class="col-1 text-center" style="padding-top: 200px;">
              <asp:Button class="btn btn-primary" id="MatchContentTemplateButton" text="匹 配" onclick="MatchContentTemplateButton_OnClick"
                runat="server" />
              <hr />
              <asp:Button class="btn" id="RemoveContentTemplateButton" text="取 消" onclick="RemoveContentTemplateButton_OnClick" runat="server"
              />
            </div>
            <div class="col-3">
              <div class="form-row">
                <label for="LbContentTemplateId">内容模板列表</label>
                <asp:ListBox ID="LbContentTemplateId" class="form-control" DataTextField="TemplateName" DataValueField="Id" SelectionMode="Single"
                  Rows="25" runat="server"></asp:ListBox>
              </div>
            </div>
          </div>

          <hr />
          <asp:Button id="BtnCreateChannelTemplate" class="btn" Text="创建栏目模版" runat="server" />
          <asp:Button id="BtnCreateChannelTemplateReal" style="display: none" OnClick="CreateChannelTemplate_Click" runat="server"
          />

          <asp:Button id="BtnCreateSubChannelTemplate" class="btn" Text="创建下级栏目模版" runat="server" />
          <asp:Button id="BtnCreateSubChannelTemplateReal" style="display: none" OnClick="CreateSubChannelTemplate_Click" runat="server"
          />

          <asp:Button id="BtnCreateContentTemplate" class="btn" Text="创建内容模版" runat="server" />
          <asp:Button id="BtnCreateContentTemplateReal" style="display: none" OnClick="CreateContentTemplate_Click" runat="server"
          />

          <asp:Button id="BtnCreateSubContentTemplate" class="btn" Text="创建下级内容模版" runat="server" />
          <asp:Button id="BtnCreateSubContentTemplateReal" style="display: none" OnClick="CreateSubContentTemplate_Click" runat="server"
          />

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->