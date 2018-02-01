<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalAddToGroup" Trace="false"%>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
      <script language="javascript">
        function selectChannel(nodeNames, channelId) {
          $('#nodeNames').html(nodeNames);
          $('#channelId').val(channelId);
        }
      </script>
    </head>

    <body>
      <form runat="server">
        <ctrl:alerts runat="server" />

        <div class="form-group form-row">
          <label class="col-1 col-form-label"></label>
          <div class="col-10">
            <asp:CheckBoxList ID="CblGroupNameCollection" RepeatDirection="Horizontal" RepeatColumns="5" class="checkbox checkbox-primary"
              runat="server" />
          </div>
          <div class="col-1"></div>
        </div>
        <div class="form-group form-row">
          <label class="col-1 col-form-label"></label>
          <div class="col-10">
            <asp:Button id="BtnAddGroup" class="btn btn-success" runat="server"></asp:Button>
          </div>
          <div class="col-1"></div>
        </div>

        <hr />

        <div class="text-right mr-1">
          <asp:Button class="btn btn-primary m-l-5" text="确 定" runat="server" onClick="Submit_OnClick" />
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->