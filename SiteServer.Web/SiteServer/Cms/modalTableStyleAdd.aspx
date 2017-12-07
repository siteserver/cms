<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalTableStyleAdd" Trace="false" %>
  <%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <!--#include file="../inc/openWindow.html"-->

      <form runat="server">
        <bairong:alerts runat="server" />

        <div class="form-horizontal">

          <div class="form-group">
            <label class="col-xs-2 control-label">字段名称</label>
            <div class="col-xs-7">
              <asp:TextBox class="form-control" Columns="25" MaxLength="50" id="TbAttributeName" runat="server" />
            </div>
            <div class="col-xs-3 help-block">
              <asp:RequiredFieldValidator ControlToValidate="TbAttributeName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbAttributeName" ValidationExpression="[a-zA-Z0-9_]+" ErrorMessage="字段名称只允许包含字母、数字以及下划线"
                foreColor="red" Display="Dynamic" />
            </div>
          </div>
          <div class="form-group">
            <label class="col-xs-2 control-label">显示名称</label>
            <div class="col-xs-7">
              <asp:TextBox class="form-control" Columns="25" MaxLength="50" id="TbDisplayName" runat="server" />
            </div>
            <div class="col-xs-3 help-block">
              <asp:RequiredFieldValidator ControlToValidate="TbDisplayName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbDisplayName" ValidationExpression="[^']+" errorMessage=" *"
                foreColor="red" display="Dynamic" />
            </div>
          </div>
          <div class="form-group">
            <label class="col-xs-2 control-label">显示帮助提示</label>
            <div class="col-xs-7">
              <asp:TextBox class="form-control" Columns="60" id="TbHelpText" runat="server" />
            </div>
            <div class="col-xs-3 help-block">
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbHelpText" ValidationExpression="[^']+" errorMessage=" *"
                foreColor="red" display="Dynamic" />
            </div>
          </div>
          <div class="form-group">
            <label class="col-xs-2 control-label">是否启用</label>
            <div class="col-xs-7">
              <asp:DropDownList id="DdlIsVisible" RepeatDirection="Horizontal" class="form-control" runat="server">
                <asp:ListItem Text="是" Selected="True" />
                <asp:ListItem Text="否" />
              </asp:DropDownList>
            </div>
            <div class="col-xs-3 help-block">

            </div>
          </div>
          <div class="form-group">
            <label class="col-xs-2 control-label">表单提交类型</label>
            <div class="col-xs-7">
              <asp:DropDownList class="form-control" ID="DdlInputType" OnSelectedIndexChanged="ReFresh" AutoPostBack="true" runat="server"></asp:DropDownList>
            </div>
            <div class="col-xs-3 help-block">

            </div>
          </div>
          <asp:PlaceHolder ID="PhIsFormatString" Visible="false" runat="server">
            <div class="form-group">
              <label class="col-xs-2 control-label">可否设置格式</label>
              <div class="col-xs-7">
                <asp:DropDownList id="DdlIsFormatString" RepeatDirection="Horizontal" class="form-control" runat="server">
                  <asp:ListItem Text="可设置" />
                  <asp:ListItem Text="不可设置" Selected="True" />
                </asp:DropDownList>
              </div>
              <div class="col-xs-3 help-block">

              </div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder ID="PhRelatedField" runat="server">
            <div class="form-group">
              <label class="col-xs-2 control-label">联动字段</label>
              <div class="col-xs-7">
                <asp:DropDownList class="form-control" id="DdlRelatedFieldId" runat="server" />
              </div>
              <div class="col-xs-3 help-block">

              </div>
            </div>
            <div class="form-group">
              <label class="col-xs-2 control-label">显示方式</label>
              <div class="col-xs-7">
                <asp:DropDownList id="DdlRelatedFieldStyle" class="form-control" runat="server" />
              </div>
              <div class="col-xs-3 help-block">

              </div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder ID="PhHeightAndWidth" runat="server">
            <div class="form-group">
              <label class="col-xs-2 control-label">显示宽度</label>
              <div class="col-xs-7">
                <asp:TextBox class="form-control" MaxLength="50" Text="0" id="TbWidth" runat="server" />
              </div>
              <div class="col-xs-3 help-block">
                px（0代表默认）
                <asp:RegularExpressionValidator ControlToValidate="TbWidth" ValidationExpression="\d+" Display="Dynamic" errorMessage=" *"
                  foreColor="red" runat="server" />
              </div>
            </div>
            <div class="form-group">
              <label class="col-xs-2 control-label">显示高度</label>
              <div class="col-xs-7">
                <asp:TextBox class="form-control" MaxLength="50" Text="0" id="TbHeight" runat="server" />
              </div>
              <div class="col-xs-3 help-block">
                px（0代表默认）
                <asp:RegularExpressionValidator ControlToValidate="TbHeight" ValidationExpression="\d+" Display="Dynamic" errorMessage=" *"
                  foreColor="red" runat="server" />
              </div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder ID="PhRepeat" runat="server">
            <div class="form-group">
              <label class="col-xs-2 control-label">排列方向</label>
              <div class="col-xs-7">
                <asp:DropDownList id="DdlIsHorizontal" class="form-control" runat="server">
                  <asp:ListItem Text="水平" Selected="True" />
                  <asp:ListItem Text="垂直" />
                </asp:DropDownList>
              </div>
              <div class="col-xs-3 help-block">

              </div>
            </div>
            <div class="form-group">
              <label class="col-xs-2 control-label">列数</label>
              <div class="col-xs-7">
                <asp:TextBox class="form-control" MaxLength="50" Text="0" id="TbColumns" runat="server" />
              </div>
              <div class="col-xs-3 help-block">
                <asp:RegularExpressionValidator ControlToValidate="TbColumns" ValidationExpression="\d+" Display="Dynamic" errorMessage=" *"
                  foreColor="red" runat="server" />
                <span class="gray">（0代表未设置此属性）</span>
              </div>
            </div>
          </asp:PlaceHolder>

          <div class="form-group">
            <label class="col-xs-2 control-label">默认显示值</label>
            <div class="col-xs-7">
              <asp:TextBox class="form-control" Columns="60" id="TbDefaultValue" runat="server" />
            </div>
            <div class="col-xs-3 help-block">
              <span id="SpanDateTip" runat="server">
                <br> {Current}代表当前日期/日期时间
              </span>
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbDefaultValue" ValidationExpression="[^']+" errorMessage=" *"
                foreColor="red" display="Dynamic" />
            </div>
          </div>

          <asp:PlaceHolder ID="PhItemsType" runat="server">
            <div class="form-group">
              <label class="col-xs-2 control-label">设置选项</label>
              <div class="col-xs-7">
                <asp:DropDownList ID="DdlItemType" class="form-control" OnSelectedIndexChanged="ReFresh" AutoPostBack="true" runat="server">
                  <asp:ListItem Text="快速设置" Value="True" Selected="True" />
                  <asp:ListItem Text="详细设置" Value="False" />
                </asp:DropDownList>
              </div>
              <div class="col-xs-3 help-block">

              </div>
            </div>
            <asp:PlaceHolder ID="PhItemCount" runat="server">
              <div class="form-group">
                <label class="col-xs-2 control-label">共有</label>
                <div class="col-xs-7">
                  <asp:TextBox class="form-control" id="TbItemCount" runat="server" />
                </div>
                <div class="col-xs-3 help-block">
                  <asp:RequiredFieldValidator ControlToValidate="TbItemCount" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                  /> 个选项&nbsp;
                  <asp:Button class="btn btn-success" style="margin-bottom:0px;" id="SetCount" text="设 置" onclick="SetCount_OnClick" CausesValidation="false"
                    runat="server" />
                  <asp:RegularExpressionValidator ControlToValidate="TbItemCount" ValidationExpression="\d+" Display="Dynamic" ErrorMessage="此项必须为数字"
                    foreColor="red" runat="server" />
                </div>
              </div>
            </asp:PlaceHolder>

          </asp:PlaceHolder>

          <asp:PlaceHolder ID="PhItemsRapid" runat="server">
            <div class="form-group">
              <label class="col-xs-2 control-label">选项可选值</label>
              <div class="col-xs-7">
                <asp:TextBox class="form-control" Columns="60" id="TbItemValues" runat="server" />
              </div>
              <div class="col-xs-3 help-block">
                <asp:RequiredFieldValidator ControlToValidate="TbItemValues" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                />
                <span class="grey">英文","分隔</span>
              </div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder ID="PhItems" runat="server">

            <asp:Repeater ID="RptItems" runat="server">
              <itemtemplate>
                <div class="form-group">
                  <label class="col-xs-2 control-label">
                    <%# Container.ItemIndex + 1 %>
                  </label>
                  <div class="col-xs-3">
                    标题
                    <asp:TextBox class="form-control" ID="ItemTitle" Columns="40" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"ItemTitle") %>'></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="ItemTitle" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                    />
                  </div>
                  <div class="col-xs-3">
                    值
                    <asp:TextBox class="form-control" ID="ItemValue" Columns="40" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"ItemValue") %>'></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="ItemValue" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                    />
                  </div>
                  <div class="col-xs-1">
                    &nbsp;
                    <asp:CheckBox class="checkbox checkbox-primary" ID="IsSelected" runat="server" Checked="False" Text="默认选择"></asp:CheckBox>
                  </div>
                  <div class="col-xs-3 help-block">

                  </div>
                </div>
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