<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTemplatePreview"%>
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
          <div class="header-title m-t-0">STL在线解析</div>
          <p class="text-muted m-b-20 font-13">
            在此输入STL模板标签，点击查看预览按钮在线查看解析结果。如果选择内容模板，系统将采用栏目下的第一篇内容进行解析。
          </p>

          <ul class="nav nav-pills m-b-20">
            <li class="nav-item active">
              <a class="nav-link" href="javascript:;" onclick="$('#containerTemplate').show();$('#containerCode').hide();$('#containerPreview').hide();$('.nav-item').removeClass('active');$(this).parent().addClass('active');">STL 标签</a>
            </li>
            <li class="nav-item">
              <a id="linkCode" class="nav-link" href="javascript:;" onclick="$('#containerTemplate').hide();$('#containerCode').show();$('#containerPreview').hide();$('.nav-item').removeClass('active');$(this).parent().addClass('active');">解析结果</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="javascript:;" onclick="$('#containerTemplate').hide();$('#containerCode').hide();$('#containerPreview').show();$('.nav-item').removeClass('active');$(this).parent().addClass('active');">界面预览</a>
            </li>
          </ul>

          <div id="containerTemplate">
            <asp:TextBox ID="TbTemplate" TextMode="Multiline" style="height: 500px;" class="form-control" runat="server"></asp:TextBox>
            <hr />

            <div class="m-t-10">
              <div class="form-inline">

                <div class="form-group">
                  <label class="col-form-label m-r-10">模板类型</label>
                  <asp:DropDownList id="DdlTemplateType" AutoPostBack="true" OnSelectedIndexChanged="DdlTemplateType_SelectedIndexChanged"
                    runat="server" class="form-control"></asp:DropDownList>
                </div>

                <asp:PlaceHolder id="PhTemplateChannel" runat="server" visible="false">
                  <div class="form-group m-l-10">
                    <label class="col-form-label m-r-10">栏目</label>
                    <asp:DropDownList id="DdlChannelId" runat="server" class="form-control"></asp:DropDownList>
                  </div>

                </asp:PlaceHolder>

                <div class="form-group m-l-10">
                  <asp:Button class="btn btn-success m-r-10" onclick="BtnPreview_OnClick" Text="在线解析" runat="server" />
                  <asp:Button id="BtnReturn" class="btn" onclick="BtnReturn_OnClick" Text="返 回" visible="false" runat="server" />
                </div>

              </div>
            </div>
          </div>

          <div id="containerCode" style="display: none">
            <asp:TextBox ID="TbCode" TextMode="Multiline" style="height: 500px;" class="form-control" runat="server"></asp:TextBox>
          </div>

          <div id="containerPreview" style="display: none">
            <asp:Literal ID="LtlPreview" runat="server"></asp:Literal>
          </div>

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->