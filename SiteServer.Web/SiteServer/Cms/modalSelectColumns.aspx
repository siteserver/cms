<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalSelectColumns" Trace="false"%>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
      <script type="text/javascript" language="javascript">
        function selectAll(isChecked) {
          var oEvent = document.getElementById('CblDisplayAttributes');
          var chks = oEvent.getElementsByTagName("INPUT");
          for (var i = 0; i < chks.length; i++) {
            if (chks[i].type == "checkbox") chks[i].checked = isChecked;
          }
        }
      </script>
    </head>

    <body>
      <form runat="server">
        <ctrl:alerts runat="server" />

        <div class="form-group form-row">
          <label class="col-1 text-right col-form-label"></label>
          <div id="Group" class="col-10">
            <asp:CheckBoxList ID="CblDisplayAttributes" RepeatColumns="3" RepeatDirection="Horizontal" class="checkbox checkbox-primary" style="width: 100%" runat="server"
            />
          </div>
          <div class="col-1"></div>
        </div>

        <hr />

        <div class="text-right mr-1">
          <span class="checkbox checkbox-primary">
            <input id="checkbox" type="checkbox" onClick="selectAll(this.checked);">
            <label for="checkbox">
              全选
            </label>
          </span>
          <asp:Button class="btn btn-primary m-l-5" text="确 定" runat="server" onClick="Submit_OnClick" />
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->