<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalSelectColumns" Trace="false"%>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
      <script type="text/javascript" language="javascript">
        function checkAll(layer, bcheck) {
          for (var i = 0; i < layer.children.length; i++) {
            if (layer.children[i].children.length > 0) {
              checkAll(layer.children[i], bcheck);
            } else {
              if (layer.children[i].type == "checkbox") {
                layer.children[i].checked = bcheck;
              }
            }
          }
        }
      </script>
    </head>

    <body>
      <form runat="server">
        <ctrl:alerts runat="server" />

        <div class="form-horizontal">

          <div class="form-group">
            <label class="col-xs-3 text-right control-label">需要显示的项</label>
            <div class="col-xs-9">
              <input type="checkbox" id="check_groups" onClick="checkAll(document.getElementById('Group'), this.checked);">
              <label for="check_groups">全选</label>
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-1 text-right control-label"></label>
            <div id="Group" class="col-xs-10">
              <asp:CheckBoxList ID="CblDisplayAttributes" RepeatColumns="3" RepeatDirection="Horizontal" class="checkbox checkbox-primary"
                runat="server" />
            </div>
            <div class="col-xs-1"></div>
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