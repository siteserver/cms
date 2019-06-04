<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTemplate" %>
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
            模板管理
          </div>
          <p class="text-muted font-13 m-b-25"></p>

          <div class="form-inline">
            <div class="form-group">
              <label class="col-form-label m-r-10">模板类型</label>
              <asp:DropDownList ID="DdlTemplateType" AutoPostBack="true" OnSelectedIndexChanged="DdlTemplateType_OnSelectedIndexChanged"
                class="form-control" runat="server"></asp:DropDownList>
            </div>

            <div class="form-group m-l-10">
              <label class="col-form-label m-r-10">模板名称/文件名</label>
              <asp:TextBox ID="TbKeywords" class="form-control m-r-10" runat="server"></asp:TextBox>
              <asp:Button class="btn btn-success" onclick="BtnSearch_Click" runat="server" Text="搜 索"></asp:Button>
            </div>
          </div>

          <div class="panel panel-default m-t-10">
            <div class="panel-body p-0">
              <div class="table-responsive">
                <table class="table tablesaw table-hover m-0">
                  <thead>
                    <tr>
                      <th>模板名称 </th>
                      <th>模板文件 </th>
                      <th>生成文件名</th>
                      <th class="text-center" width="80">使用</th>
                      <th class="text-center" width="120">模板类型</th>
                      <th class="text-center"></th>
                    </tr>
                  </thead>
                  <tbody>
                    <asp:Repeater ID="RptContents" runat="server">
                      <itemtemplate>
                        <tr>
                          <td>
                            <asp:Literal ID="ltlTemplateName" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlRelatedFileName" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlFileName" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlUseCount" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlTemplateType" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlDefaultUrl" runat="server"></asp:Literal>
                            <asp:Literal ID="ltlCopyUrl" runat="server"></asp:Literal>
                            <asp:Literal ID="ltlLogUrl" runat="server"></asp:Literal>
                            <asp:Literal ID="ltlCreateUrl" runat="server"></asp:Literal>
                            <asp:Literal ID="ltlDeleteUrl" runat="server"></asp:Literal>
                          </td>
                        </tr>
                      </itemtemplate>
                    </asp:Repeater>
                  </tbody>
                </table>
              </div>
            </div>
          </div>

          <hr />

          <asp:Literal id="LtlCommands" runat="server" />

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->