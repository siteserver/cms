<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalAddToGroup" Trace="false"%>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
  <!DOCTYPE html>
  <html class="modalPage">

  <head>
    <meta charset="utf-8">
    <!--#include file="../inc/head.html"-->
    <script language="javascript">
      function selectChannel(nodeNames, nodeID) {
        $('#nodeNames').html(nodeNames);
        $('#nodeID').val(nodeID);
      }
    </script>
  </head>

  <body>
    <!--#include file="../inc/openWindow.html"-->

    <form runat="server">
      <bairong:alerts runat="server" />

      <div class="form-horizontal">
        
        <div class="form-group">
          <label class="col-xs-1 control-label"></label>
          <div class="col-xs-10">
            <asp:CheckBoxList ID="CblGroupNameCollection" RepeatDirection="Horizontal" class="checkbox checkbox-primary" runat="server"/>
          </div>
          <div class="col-xs-1">

          </div>
        </div>
        <div class="form-group">
            <label class="col-xs-1 control-label"></label>
            <div class="col-xs-10">
                <asp:Button id="BtnAddGroup" class="btn btn-success" runat="server"></asp:Button>
            </div>
            <div class="col-xs-1">
  
            </div>
          </div>

        <hr />

        <div class="form-group m-b-0">
            <div class="col-xs-11 text-right">
              <asp:Button class="btn btn-primary m-l-10" text="确 定" runat="server" onClick="Submit_OnClick" />
              <button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">取 消</button>
            </div>
            <div class="col-xs-1"></div>
          </div>
      </div>

    </form>
  </body>

  </html>