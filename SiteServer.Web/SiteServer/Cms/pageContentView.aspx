<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.PageContentView" %>
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
            内容查看
          </div>
          <p class="text-muted font-13 m-b-25"></p>

          <div class="form-group form-row">
            <label class="col-sm-1 col-form-label">栏目名</label>
            <div class="col-sm-10 form-control-plaintext">
              <asp:Literal ID="LtlNodeName" runat="server" />
            </div>
            <div class="col-sm-1"></div>
          </div>

          <asp:PlaceHolder id="PhTags" runat="server">
            <div class="form-group form-row">
              <label class="col-sm-1 col-form-label">内容标签</label>
              <div class="col-sm-10 form-control-plaintext">
                <asp:Literal ID="LtlTags" runat="server" />
              </div>
              <div class="col-sm-1"></div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder id="PhContentGroup" runat="server">
            <div class="form-group form-row">
              <label class="col-sm-1 col-form-label">所属内容组</label>
              <div class="col-sm-10 form-control-plaintext">
                <asp:Literal ID="LtlContentGroup" runat="server" />
              </div>
              <div class="col-sm-1"></div>
            </div>
          </asp:PlaceHolder>

          <div class="form-group form-row">
            <label class="col-sm-1 col-form-label">最后修改日期</label>
            <div class="col-sm-10 form-control-plaintext">
              <asp:Literal ID="LtlLastEditDate" runat="server" />
            </div>
            <div class="col-sm-1"></div>
          </div>

          <div class="form-group form-row">
            <label class="col-sm-1 col-form-label">添加人</label>
            <div class="col-sm-10 form-control-plaintext">
              <asp:Literal id="LtlAddUserName" runat="server" />
            </div>
            <div class="col-sm-1"></div>
          </div>

          <div class="form-group form-row">
            <label class="col-sm-1 col-form-label">最后修改人</label>
            <div class="col-sm-10 form-control-plaintext">
              <asp:Literal id="LtlLastEditUserName" runat="server" />
            </div>
            <div class="col-sm-1"></div>
          </div>

          <div class="form-group form-row">
            <label class="col-sm-1 col-form-label">状态</label>
            <div class="col-sm-10 form-control-plaintext">
              <asp:Literal ID="LtlContentLevel" runat="server" />
            </div>
            <div class="col-sm-1"></div>
          </div>

          <asp:Repeater ID="RptContents" runat="server">
            <itemtemplate>
              <asp:Literal id="ltlHtml" runat="server" />
            </itemtemplate>
          </asp:Repeater>

          <hr />

          <div class="text-center">
            <asp:Button id="BtnSubmit" class="btn btn-primary" text="审 核" runat="server" />
            <asp:HyperLink id="HlPreview" target="_blank" class="btn btn-success m-l-5" text="预 览" runat="server" />
            <input class="btn m-l-5" type="button" onClick="location.href='<%=ReturnUrl%>';return false;" value="返 回" />
          </div>

        </div>

      </form>
    </body>

    </html>