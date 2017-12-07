<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalContentStarSet" Trace="false"%>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <!--#include file="../inc/openWindow.html"-->

      <form runat="server">
        <ctrl:alerts text="在此可以设置指定内容的总评分人数以及平均评分值，0代表将取消设置" runat="server" />

        <div class="form-horizontal">

          <div class="form-group">
            <label class="col-xs-3 text-right control-label">总评分人数</label>
            <div class="col-xs-8">
              <asp:TextBox class="form-control" MaxLength="50" Text="0" id="TbTotalCount" runat="server" />
            </div>
            <div class="col-xs-1 help-block">
              <asp:RequiredFieldValidator ControlToValidate="TbTotalCount" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator ControlToValidate="TbTotalCount" ValidationExpression="\d+" Display="Dynamic" ErrorMessage="总评分人数必须为数字"
                foreColor="red" runat="server" />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 text-right control-label">平均评分值</label>
            <div class="col-xs-8">
              <asp:TextBox class="form-control" MaxLength="50" Text="0" id="TbPointAverage" runat="server" />
            </div>
            <div class="col-xs-1 help-block">
              <asp:RequiredFieldValidator ControlToValidate="TbPointAverage" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator ControlToValidate="TbPointAverage" ValidationExpression="[\d\.]+" Display="Dynamic" ErrorMessage="平均评分值必须为数字,可以带小数点"
                foreColor="red" runat="server" />
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