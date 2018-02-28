<%@ Page Language="C#" Trace="false" Inherits="SiteServer.BackgroundPages.Cms.ModalContentImport" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
      <script type="text/javascript">
        $(document).ready(function () {
          $('#import select').change(function () {
            var tips =
              '<button type="button" class="close" data-dismiss="alert">&times;</button><strong>提示!</strong>&nbsp;&nbsp; ';
            if (this.value == 'ContentZip') {
              $(".alert").html(tips + '请选择后台导出的压缩包，系统能够将内容以及内容相关的图片、附件等文件一道导入')
            } else if (this.value == 'ContentAccess') {
              $(".alert").html(tips + '请选择Access文件，系统将导入Access文件对应的字段数据');
            } else if (this.value == 'ContentExcel') {
              $(".alert").html(tips + '请选择Excel文件，系统将导入Excel文件对应的字段数据');
            } else if (this.value == 'ContentTxtZip') {
              $(".alert").html(tips + '请选择以.txt结尾的纯文本文件的压缩包，系统将压缩包内的每一个文件作为一篇内容，文件中第一行作为内容标题，其余作为内容正文导入');
            }
          });
        });
      </script>
    </head>

    <body>
      <form enctype="multipart/form-data" method="post" runat="server">
        <ctrl:alerts text="请选择Access文件，系统将导入Access文件对应的字段数据" runat="server" />

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">导入类型</label>
          <div id="import" class="col-5">
            <asp:DropDownList class="form-control" ID="DdlImportType" runat="server">
              <asp:ListItem Text="导入压缩包" Value="ContentZip" Selected="true"></asp:ListItem>
              <asp:ListItem Text="导入Access" Value="ContentAccess"></asp:ListItem>
              <asp:ListItem Text="导入Excel" Value="ContentExcel"></asp:ListItem>
              <asp:ListItem Text="导入TXT压缩包" Value="ContentTxtZip"></asp:ListItem>
            </asp:DropDownList>
          </div>
          <div class="col-4"></div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">导入文件</label>
          <div class="col-5">
            <input type=file id="HifFile" class="form-control" runat="server" />
          </div>
          <div class="col-4">
            <asp:RequiredFieldValidator ControlToValidate="HifFile" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
            />
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">是否覆盖同名标题</label>
          <div class="col-5">
            <asp:DropDownList ID="DdlIsOverride" runat="server" class="form-control">
              <asp:ListItem Text="覆盖" Value="True" Selected="true"></asp:ListItem>
              <asp:ListItem Text="不覆盖" Value="False"></asp:ListItem>
            </asp:DropDownList>
          </div>
          <div class="col-4"></div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">从第几条开始导入</label>
          <div class="col-5">
            <asp:TextBox class="form-control" id="TbImportStart" runat="server" />
          </div>
          <div class="col-4">
            <small class="form-text text-muted">默认为第一条</small>
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">共导入几条</label>
          <div class="col-5">
            <asp:TextBox class="form-control" id="TbImportCount" runat="server" />
          </div>
          <div class="col-4">
            <small class="form-text text-muted">默认为全部导入</small>
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">内容的状态</label>
          <div class="col-5">
            <asp:DropDownList ID="DdlContentLevel" class="form-control" runat="server" />
          </div>
          <div class="col-4">
            <small class="form-text text-muted">设置导入后内容的状态</small>
          </div>
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