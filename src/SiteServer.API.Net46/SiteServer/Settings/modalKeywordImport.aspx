<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.ModalKeywordImport" validateRequest="false" trace="false" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form runat="server">
        <ctrl:alerts text="敏感词导入规则：以英文逗号(,)分隔敏感词，如果需要替换用竖线(|)分隔，如：敏感词1,敏感词2,敏感词3|替换词3,敏感词4" runat="server" />

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">等级</label>
          <div class="col-6">
            <asp:DropDownList ID="DdlGrade" class="form-control" runat="server"></asp:DropDownList>
          </div>
          <div class="col-3 help-block"></div>
        </div>

        <div class="form-group form-row">
          <label class="col-1 text-right col-form-label"></label>
          <div class="col-10">
            <asp:TextBox style="width:98%;height:230px" class="form-control" TextMode="MultiLine" id="TbKeywords" runat="server" />
          </div>
          <div class="col-1 help-block"></div>
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