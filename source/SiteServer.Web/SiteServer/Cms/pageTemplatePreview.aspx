<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTemplatePreview"%>
  <%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <link href="../assets/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
      <link href="../assets/css/core.css" rel="stylesheet" type="text/css" />
      <link href="../assets/css/components.css" rel="stylesheet" type="text/css" />
      <link href="../assets/css/pages.css" rel="stylesheet" type="text/css" />
      <link href="../assets/font-awesome/css/font-awesome.min.css" rel="stylesheet" type="text/css" />
    </head>

    <body>
      <bairong:alerts runat="server" />
      <!-- container start -->
      <div class="container">
        <div class="m-b-25"></div>

        <form runat="server">
          <div class="row">
            <div class="col-sm-12">
              <div class="card-box">
                <h4 class="text-dark  header-title m-t-0">STL在线解析</h4>
                <p class="text-muted m-b-30 font-13">
                  在此输入STL模板标签，点击查看预览按钮在线查看解析结果。如果选择内容模板，系统将采用栏目下的第一篇内容进行解析。
                </p>
                <asp:Literal id="LtlMessage" runat="server" />

                <div class="row m-b-30">
                  <div class="col-sm-12">
                    <div class="form-inline" role="form">
                      <div class="form-group">
                        <label for="DdlTemplateType">模板类型</label>
                        <asp:DropDownList id="DdlTemplateType" AutoPostBack="true" OnSelectedIndexChanged="DdlTemplateType_SelectedIndexChanged"
                          runat="server" class="form-control"></asp:DropDownList>
                      </div>

                      <asp:PlaceHolder id="PhTemplateChannel" runat="server" visible="false">
                        <div class="form-group m-l-10">
                          <label for="DdlNodeId">栏目</label>
                          <asp:DropDownList id="DdlNodeId" runat="server" class="form-control"></asp:DropDownList>
                        </div>
                      </asp:PlaceHolder>
                    </div>
                  </div>

                </div>

                <div class="form-horizontal">
                  <div class="form-group">
                    <div class="col-md-12">
                      <asp:TextBox ID="TbTemplate" TextMode="Multiline" style="height: 500px;" class="form-control" runat="server"></asp:TextBox>
                    </div>
                  </div>

                  <div class="m-b-25"></div>
                  <asp:Button class="btn btn-success" onclick="BtnPreview_OnClick" Text="查看预览" runat="server" />
                  <asp:Button id="BtnReturn" class="btn" onclick="BtnReturn_OnClick" Text="返 回" visible="false" runat="server" />
                </div>

              </div>
            </div>
          </div>
        </form>

        <asp:Placeholder id="PhPreview" runat="server" visible="false">
          <div class="row">
            <div class="col-sm-12">
              <div class="card-box">
                <h4 class="text-dark  header-title m-t-0">STL解析结果</h4>
                <p class="text-muted m-b-25 font-13"></p>

                <asp:Literal ID="LtlPreview" runat="server"></asp:Literal>

              </div>
            </div>
          </div>
        </asp:Placeholder>

      </div>
      <!-- container end -->
    </body>

    </html>