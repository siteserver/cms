<%@ Page Language="C#" Trace="false" Inherits="SiteServer.BackgroundPages.Cms.ModalContentExport" %>
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
        $(document).ready(function () {
          $('#DdlPeriods').click(function () {
            if ($('#DdlPeriods').val() == '-1') {
              $('#periods').show();
            } else {
              $('#periods').hide();
            }
          });

          if ($('#DdlPeriods').val() == '-1') {
            $('#periods').show();
          } else {
            $('#periods').hide();
          }
        });
      </script>
    </head>

    <body>
      <form runat="server">
        <ctrl:alerts text="导出压缩包能够将内容以及内容相关的图片、附件等文件一道导出，导出Access或Excel则仅能导出数据。" runat="server" />

        <div class="form-group form-row">
          <label class="col-2 col-form-label text-right">导出类型</label>
          <div class="col-9">
            <asp:DropDownList ID="DdlExportType" AutoPostBack="true" OnSelectedIndexChanged="DdlExportType_SelectedIndexChanged" runat="server"
              class="form-control">
              <asp:ListItem Text="导出压缩包" Value="ContentZip" Selected="true"></asp:ListItem>
              <asp:ListItem Text="导出Access" Value="ContentAccess"></asp:ListItem>
              <asp:ListItem Text="导出Excel" Value="ContentExcel"></asp:ListItem>
            </asp:DropDownList>
          </div>
          <div class="col-1"></div>
        </div>

        <div class="form-group form-row">
          <label class="col-2 col-form-label text-right">时间段选择</label>
          <div class="col-9">
            <asp:DropDownList id="DdlPeriods" class="form-control" runat="server">
              <asp:ListItem Text="全部" Value="0" selected="true"></asp:ListItem>
              <asp:ListItem Text="一周" Value="7"></asp:ListItem>
              <asp:ListItem Text="一月" Value="30"></asp:ListItem>
              <asp:ListItem Text="半年" Value="180"></asp:ListItem>
              <asp:ListItem Text="一年" Value="365"></asp:ListItem>
              <asp:ListItem Text="自定义" Value="-1"></asp:ListItem>
            </asp:DropDownList>
            <div id="periods" class="row" style="display:none">
              <hr />
              <div class="col-2 col-form-label text-right">
                开始
              </div>
              <div class="col-4">
                <ctrl:DateTimeTextBox id="TbStartDate" class="form-control" runat="server" />
              </div>
              <div class="col-2 col-form-label text-right">
                结束
              </div>
              <div class="col-4">
                <ctrl:DateTimeTextBox id="TbEndDate" class="form-control" runat="server" />
              </div>
            </div>
          </div>
          <div class="col-1"></div>
        </div>

        <asp:PlaceHolder ID="PhDisplayAttributes" runat="server">
          <div class="form-group form-row">
            <label class="col-2 col-form-label text-right">选择需要导出的字段</label>
            <div class="col-9">
              <input type="checkbox" id="check_groups" onClick="checkAll(document.getElementById('Group'),this.checked);">
              <label for="check_groups">全选</label>
              <br />
              <span id="Group">
                <asp:CheckBoxList ID="CblDisplayAttributes" RepeatColumns="3" RepeatDirection="Horizontal" class="table checkbox checkbox-primary"
                  Width="100%" runat="server" />
              </span>
            </div>
            <div class="col-1"></div>
          </div>
        </asp:PlaceHolder>

        <div class="form-group form-row">
          <label class="col-2 col-form-label text-right">内容状态</label>
          <div class="col-9">
            <asp:DropDownList id="DdlIsChecked" class="form-control" runat="server">
              <asp:ListItem Text="全部内容" Value="All" selected="true"></asp:ListItem>
              <asp:ListItem Text="未审核内容" Value="False"></asp:ListItem>
              <asp:ListItem Text="已审核内容" Value="True"></asp:ListItem>
            </asp:DropDownList>
          </div>
          <div class="col-1"></div>
        </div>

        <hr />

        <div class="text-right mr-1">
          <asp:Button class="btn btn-primary m-l-5" Text="确 定" OnClick="Submit_OnClick" runat="server" />
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->