<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalCrossSiteTransEdit" Trace="false" %>
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

        <div class="form-horizontal">

          <div class="form-group">
            <label class="col-xs-2 control-label text-right">跨站转发类型</label>
            <div class="col-xs-5">
              <asp:DropDownList ID="DdlTransType" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="DdlTransType_OnSelectedIndexChanged"
                runat="server"></asp:DropDownList>
            </div>
            <div class="col-xs-5"></div>
          </div>

          <asp:PlaceHolder ID="PhPublishmentSystem" Visible="false" runat="server">
            <div class="form-group">
              <label class="col-xs-2 control-label text-right">指定跨站转发站点</label>
              <div class="col-xs-5">
                <asp:DropDownList ID="DdlPublishmentSystemId" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="DdlPublishmentSystemId_OnSelectedIndexChanged"
                  runat="server"></asp:DropDownList>
              </div>
              <div class="col-xs-5"></div>
            </div>
            <div class="form-group">
              <label class="col-xs-2 control-label text-right">选择可跨站转发的栏目</label>
              <div class="col-xs-5">
                <asp:ListBox ID="LbNodeId" class="form-control" SelectionMode="Multiple" Rows="12" runat="server"></asp:ListBox>
              </div>
              <div class="col-xs-5"></div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder ID="PhNodeNames" Visible="false" runat="server">
            <div class="form-group">
              <label class="col-xs-2 control-label text-right">设置可跨站转发的栏目名称</label>
              <div class="col-xs-5">
                <asp:TextBox ID="TbNodeNames" class="form-control" TextMode="MultiLine" Columns="45" runat="server"></asp:TextBox>
              </div>
              <div class="col-xs-5">
                <span class="help-block">多个栏目以,分隔，不添加栏目代表可以对所有栏目进行跨站转发。</span>
              </div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder ID="PhIsAutomatic" Visible="false" runat="server">
            <div class="form-group">
              <label class="col-xs-2 control-label text-right">是否自动转发内容</label>
              <div class="col-xs-5">
                <asp:DropDownList ID="DdlIsAutomatic" class="form-control" runat="server"></asp:DropDownList>
              </div>
              <div class="col-xs-5"></div>
            </div>
            <div class="form-group">
              <label class="col-xs-2 control-label text-right">设置转发内容类型</label>
              <div class="col-xs-5">
                <asp:DropDownList ID="DdlTranslateDoneType" class="form-control" runat="server"></asp:DropDownList>
              </div>
              <div class="col-xs-5"></div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder ID="PhChecked" Visible="false" runat="server">
            <div class="form-group">
              <label class="col-xs-2 control-label text-right">状态</label>
              <div class="col-xs-5">
                <asp:DropDownList ID="DdlCheckedStaus" class="form-control" runat="server"></asp:DropDownList>
              </div>
              <div class="col-xs-5"></div>
            </div>
          </asp:PlaceHolder>

          <hr />

          <div class="form-group m-b-0">
            <div class="col-xs-11 text-right">
              <asp:Button class="btn btn-primary m-l-10" ID="BtnCheck" Text="确 定" OnClick="Submit_OnClick" runat="server" />
              <button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">取 消</button>
            </div>
            <div class="col-xs-1"></div>
          </div>

        </div>

      </form>
    </body>

    </html>