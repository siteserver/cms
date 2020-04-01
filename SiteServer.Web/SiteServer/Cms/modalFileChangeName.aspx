<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalFileChangeName" Trace="false"%>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
      <script language="javascript">
        document.ready(function () {
          document.getElementById('<%=TbFileName.ClientID%>').focus();
        });
      </script>
    </head>

    <body>
      <form runat="server">
        <ctrl:alerts runat="server" />

        <div class="form-group form-row">
          <label class="col-3 col-form-label text-right">文件名</label>
          <div class="col-8">
            <asp:Literal ID="LtlFileName" runat="server"></asp:Literal>
          </div>
          <div class="col-1"></div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 col-form-label text-right">新文件名</label>
          <div class="col-8">
            <asp:TextBox Columns="25" class="form-control" id="TbFileName" runat="server" />
          </div>
          <div class="col-1">
            <asp:RequiredFieldValidator ControlToValidate="TbFileName" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
            />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbFileName" ValidationExpression="[^',]+" errorMessage=" *"
              foreColor="red" display="Dynamic" />
          </div>
        </div>

        <hr />

        <div class="text-right mr-1">
          <asp:Button class="btn btn-primary m-l-5" ID="BtnCheck" Text="确 定" OnClick="Submit_OnClick" runat="server" />
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->