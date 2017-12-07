<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalFileView" Trace="false" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <!--#include file="../inc/openWindow.html"-->

      <form runat="server">
        <ctrl:alerts runat="server" />

        <div class="form-horizontal">

          <div class="form-group">
            <label class="col-xs-2 control-label text-right">文件名称</label>
            <div class="col-xs-4">
              <asp:Literal ID="LtlFileName" runat="server"></asp:Literal>
            </div>
            <label class="col-xs-2 control-label text-right">文件类型</label>
            <div class="col-xs-4">
              <asp:Literal ID="LtlFileType" runat="server"></asp:Literal>
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-2 control-label text-right">位置</label>
            <div class="col-xs-10">
              <asp:Literal ID="LtlFilePath" runat="server"></asp:Literal>
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-2 control-label text-right">大小</label>
            <div class="col-xs-4">
              <asp:Literal ID="LtlFileSize" runat="server"></asp:Literal>
            </div>
            <label class="col-xs-2 control-label text-right">创建时间</label>
            <div class="col-xs-4">
              <asp:Literal ID="LtlCreationTime" runat="server"></asp:Literal>
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-2 control-label text-right">最后修改时间</label>
            <div class="col-xs-4">
              <asp:Literal ID="LtlLastWriteTime" runat="server"></asp:Literal>
            </div>
            <label class="col-xs-2 control-label text-right">最后访问时间</label>
            <div class="col-xs-4">
              <asp:Literal ID="LtlLastAccessTime" runat="server"></asp:Literal>
            </div>
          </div>

          <hr />

          <div class="form-group m-b-0">
            <div class="col-xs-11 text-right">
              <asp:Literal ID="LtlOpen" runat="server"></asp:Literal>
              <asp:Literal ID="LtlEdit" runat="server"></asp:Literal>
              <asp:Literal ID="LtlChangeName" runat="server"></asp:Literal>
              <button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">关 闭</button>
            </div>
            <div class="col-xs-1"></div>
          </div>

        </div>

      </form>
    </body>

    </html>