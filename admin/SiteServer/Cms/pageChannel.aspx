<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageChannel" enableViewState="false" %>
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

          <div class="btn-toolbar" role="toolbar">
            <div class="btn-group">
              <asp:Literal id="LtlButtonsHead" runat="server" />
            </div>
          </div>

          <div class="panel panel-default m-t-20 m-b-20">
            <div class="panel-body p-0">
              <div class="table-responsive">
                <table id="channels" class="table-tree tablesaw table table-hover m-b-0 tablesaw-stack">
                  <thead>
                    <tr class="thead">
                      <th>栏目名</th>
                      <th class "text-nowrap">所属栏目组</th>
                      <th class="text-nowrap">栏目索引</th>
                      <th width="60">上升</th>
                      <th width="60">下降</th>
                      <th width="60">&nbsp;</th>
                      <th width="20">
                        <input type="checkbox" onClick="activeRows(document.getElementById('channels'), this.checked);">
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <asp:Repeater ID="RptContents" runat="server">
                      <itemtemplate>
                        <asp:Literal id="ltlHtml" runat="server" />
                      </itemtemplate>
                    </asp:Repeater>
                  </tbody>
                </table>
              </div>
            </div>
          </div>

          <div class="btn-toolbar" role="toolbar">
            <div class="btn-group">
              <asp:Literal id="LtlButtonsFoot" runat="server" />
            </div>
          </div>

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->
    <script type="text/javascript">
      function activeRow(e) {
        var tr = $(e);
        var cb = tr.find('input:checkbox:first');
        var checked = cb.is(':checked');
        cb[0].checked = !checked;
        checked ? tr.removeClass('table-active') : tr.addClass('table-active');
      }

      function activeRows(layer, bcheck) {
        for (var i = 0; i < layer.childNodes.length; i++) {
          if (layer.childNodes[i].childNodes.length > 0) {
            activeRows(layer.childNodes[i], bcheck);
          } else {
            if (layer.childNodes[i].type == "checkbox") {
              layer.childNodes[i].checked = bcheck;
              var cb = $(layer.childNodes[i]);
              var tr = cb.closest('tr');
              if (!tr.hasClass("thead")) {
                cb.is(':checked') ? tr.addClass('table-active') : tr.removeClass('table-active');
              }
            }
          }
        }
      }

      function checkboxClick(e) {
        event.stopPropagation();
        var cb = $(e);
        var checked = cb.is(':checked');
        var tr = cb.parent().parent();
        checked ?  tr.addClass('table-active') : tr.removeClass('table-active');
        return true;
      }

      function cancelSelectRow(e) {
        var tr = $(e);
        var cb = tr.find('input:checkbox:first');
        var checked = cb.is(':checked');
        cb[0].checked = false;
        tr.removeClass('table-active');
      }
    </script>