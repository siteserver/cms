<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.ModalContentView" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">
        <ctrl:alerts runat="server" />

        <div class="form-group form-row">
          <label class="col-sm-2 col-form-label">标题</label>
          <div class="col-sm-9 form-control-plaintext">
            <asp:Literal ID="LtlTitle" runat="server" />
          </div>
          <div class="col-sm-1"></div>
        </div>

        <div class="form-group form-row">
          <label class="col-sm-2 col-form-label">栏目</label>
          <div class="col-sm-9 form-control-plaintext">
            <asp:Literal ID="LtlChannelName" runat="server" />
          </div>
          <div class="col-sm-1"></div>
        </div>

        <asp:Repeater ID="RptContents" runat="server">
          <itemtemplate>
            <asp:Literal id="ltlHtml" runat="server" />
          </itemtemplate>
        </asp:Repeater>

        <asp:PlaceHolder id="PhTags" runat="server">
          <div class="form-group form-row">
            <label class="col-sm-2 col-form-label">内容标签</label>
            <div class="col-sm-9 form-control-plaintext">
              <asp:Literal ID="LtlTags" runat="server" />
            </div>
            <div class="col-sm-1"></div>
          </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder id="PhContentGroup" runat="server">
          <div class="form-group form-row">
            <label class="col-sm-2 col-form-label">所属内容组</label>
            <div class="col-sm-9 form-control-plaintext">
              <asp:Literal ID="LtlContentGroup" runat="server" />
            </div>
            <div class="col-sm-1"></div>
          </div>
        </asp:PlaceHolder>

        <div class="form-group form-row">
          <label class="col-sm-2 col-form-label">最后修改日期</label>
          <div class="col-sm-9 form-control-plaintext">
            <asp:Literal ID="LtlLastEditDate" runat="server" />
          </div>
          <div class="col-sm-1"></div>
        </div>

        <div class="form-group form-row">
          <label class="col-sm-2 col-form-label">添加人</label>
          <div class="col-sm-9 form-control-plaintext">
            <asp:Literal id="LtlAddUserName" runat="server" />
          </div>
          <div class="col-sm-1"></div>
        </div>

        <div class="form-group form-row">
          <label class="col-sm-2 col-form-label">最后修改人</label>
          <div class="col-sm-9 form-control-plaintext">
            <asp:Literal id="LtlLastEditUserName" runat="server" />
          </div>
          <div class="col-sm-1"></div>
        </div>

        <div class="form-group form-row">
          <label class="col-sm-2 col-form-label">状态</label>
          <div class="col-sm-9 form-control-plaintext">
            <asp:Literal ID="LtlContentLevel" runat="server" />
          </div>
          <div class="col-sm-1"></div>
        </div>

        <hr />

        <div class="text-right mr-1">
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">关 闭</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->