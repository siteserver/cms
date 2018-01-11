<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.ModalKeywordAdd" Trace="false" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form runat="server">
        <ctrl:alerts runat="server" />

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">敏感词</label>
          <div class="col-6">
            <asp:TextBox ID="TbKeyword" class="form-control" runat="server" />
          </div>
          <div class="col-3">
            <asp:RequiredFieldValidator ControlToValidate="TbKeyword" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
            />
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">替换为</label>
          <div class="col-6">
            <asp:TextBox ID="TbAlternative" class="form-control" runat="server" />
          </div>
          <div class="col-3">
            <small class="form-text text-muted">可以为空</small>
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">等级</label>
          <div class="col-6">
            <asp:DropDownList ID="DdlGrade" class="form-control" runat="server"></asp:DropDownList>
          </div>
          <div class="col-3"></div>
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