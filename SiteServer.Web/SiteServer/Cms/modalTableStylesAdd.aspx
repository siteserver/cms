<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalTableStylesAdd" Trace="false" %>
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
        <ctrl:alerts text="多个字段之间用换行分割，字段显示名称可以放到括号中，如：字段名称(显示名称)，不设置显示名称将默认使用字段名称" runat="server" />

        <div class="form-horizontal">

          <div class="form-group">
            <label class="col-xs-1 text-right control-label">字段</label>
            <div class="col-xs-8">
              <asp:TextBox class="form-control" id="TbAttributeNames" TextMode="MultiLine" Rows="8" runat="server" />
            </div>
            <div class="col-xs-3 help-block">
              <asp:RequiredFieldValidator ControlToValidate="TbAttributeNames" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbAttributeNames" ValidationExpression="[^',]+" errorMessage=" *"
                foreColor="red" display="Dynamic" />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-1 text-right control-label">是否启用</label>
            <div class="col-xs-8">
              <asp:DropDownList id="DdlIsVisible" class="form-control" runat="server">
                <asp:ListItem Text="是" Selected="True" />
                <asp:ListItem Text="否" />
              </asp:DropDownList>
            </div>
            <div class="col-xs-3 help-block"></div>
          </div>

          <div class="form-group">
            <label class="col-xs-1 text-right control-label">表单提交类型</label>
            <div class="col-xs-8">
              <asp:DropDownList ID="DdlInputType" class="form-control" OnSelectedIndexChanged="ReFresh" AutoPostBack="true" runat="server"></asp:DropDownList>
            </div>
            <div class="col-xs-3 help-block"></div>
          </div>

          <asp:PlaceHolder id="PhHeightAndWidth" runat="server">
            <div class="form-group">
              <label class="col-xs-1 text-right control-label">显示宽度</label>
              <div class="col-xs-8">
                <asp:TextBox class="form-control" Text="0" id="TbWidth" runat="server" />
              </div>
              <div class="col-xs-3 help-block">
                px
                <asp:RegularExpressionValidator ControlToValidate="TbWidth" ValidationExpression="\d+" Display="Dynamic" errorMessage=" *"
                  foreColor="red" runat="server" /> （0代表默认）
              </div>
            </div>
            <div class="form-group">
              <label class="col-xs-1 text-right control-label">显示高度</label>
              <div class="col-xs-8">
                <asp:TextBox class="form-control" Text="0" id="TbHeight" runat="server" />
              </div>
              <div class="col-xs-3 help-block">
                px
                <asp:RegularExpressionValidator ControlToValidate="TbHeight" ValidationExpression="\d+" Display="Dynamic" errorMessage=" *"
                  foreColor="red" runat="server" /> （0代表默认）
              </div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder id="PhDefaultValue" runat="server">
            <div class="form-group">
              <label class="col-xs-1 text-right control-label">默认显示值</label>
              <div class="col-xs-8">
                <asp:TextBox class="form-control" TextMode="MultiLine" id="TbDefaultValue" runat="server" />
              </div>
              <div class="col-xs-3">
                <span class="help-block" id="SpDateTip" runat="server">
                  <br> {Current}代表当前日期/日期时间
                </span>
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbDefaultValue" ValidationExpression="[^']+" errorMessage=" *"
                  foreColor="red" display="Dynamic" />
              </div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder id="PhRepeat" runat="server">
            <div class="form-group">
              <label class="col-xs-1 text-right control-label">排列方向</label>
              <div class="col-xs-8">
                <asp:DropDownList id="DdlIsHorizontal" class="form-control" runat="server">
                  <asp:ListItem Text="水平" Selected="True" />
                  <asp:ListItem Text="垂直" />
                </asp:DropDownList>
              </div>
              <div class="col-xs-3">
              </div>
            </div>
            <div class="form-group">
              <label class="col-xs-1 text-right control-label">列数</label>
              <div class="col-xs-8">
                <asp:TextBox class="form-control" Text="0" id="TbColumns" runat="server" />
              </div>
              <div class="col-xs-3">
                <asp:RegularExpressionValidator ControlToValidate="TbColumns" ValidationExpression="\d+" Display="Dynamic" errorMessage=" *"
                  foreColor="red" runat="server" /> （0代表未设置此属性）
              </div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder id="PhItemCount" runat="server">
            <div class="form-group">
              <label class="col-xs-1 text-right control-label">设置选项数目</label>
              <div class="col-xs-8">
                <asp:TextBox class="form-control" Text="2" id="TbItemCount" runat="server" />
              </div>
              <div class="col-xs-3">
                <asp:RequiredFieldValidator ControlToValidate="TbItemCount" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                />
                <asp:Button class="btn" style="margin-bottom:0px;" id="SetCount" text="设 置" onclick="SetCount_OnClick" CausesValidation="false"
                  runat="server" />
                <asp:RegularExpressionValidator ControlToValidate="TbItemCount" ValidationExpression="\d+" Display="Dynamic" ErrorMessage="此项必须为数字"
                  foreColor="red" runat="server" />
              </div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder id="PhSetItems" runat="server">
            <asp:Repeater ID="RptContents" runat="server">
              <itemtemplate>
                <table width="100%" border="0" cellspacing="2" cellpadding="2">
                  <tr>
                    <td class="center" style="width:20px;">
                      <strong>
                        <%# Container.ItemIndex + 1 %>
                      </strong>
                    </td>
                    <td>
                      <table width="100%" border="0" cellspacing="3" cellpadding="3">
                        <tr>
                          <td width="120">
                              选项标题
                          </td>
                          <td colspan="3">
                            <asp:TextBox ID="ItemTitle" Columns="40" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"ItemTitle") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ControlToValidate="ItemTitle" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                            />
                          </td>
                        </tr>
                        <tr>
                          <td width="120">
                              选项值
                          </td>
                          <td colspan="3">
                            <asp:TextBox ID="ItemValue" Columns="40" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"ItemValue") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ControlToValidate="ItemValue" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                            />
                          </td>
                        </tr>
                        <tr>
                          <td width="120">
                              初始化时选定
                          </td>
                          <td colspan="3">
                            <asp:CheckBox ID="IsSelected" runat="server" Checked="False" Text="选定"></asp:CheckBox>
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </table>
              </itemtemplate>
            </asp:Repeater>
          </asp:PlaceHolder>

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