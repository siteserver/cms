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

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">需要显示的项</label>
          <div class="col-9">
            <input type="checkbox" id="check_groups" onClick="checkAll(document.getElementById('Group'), this.checked);">
            <label for="check_groups">全选</label>
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-1 text-right col-form-label"></label>
          <div id="Group" class="col-10">
            <asp:CheckBoxList ID="CblDisplayAttributes" RepeatColumns="3" RepeatDirection="Horizontal" class="checkbox checkbox-primary"
              runat="server" />
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