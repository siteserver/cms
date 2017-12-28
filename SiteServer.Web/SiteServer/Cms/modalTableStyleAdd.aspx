<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalTableStyleAdd" Trace="false" %>
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
            <label class="col-xs-2 control-label">字段名称</label>
            <div class="col-xs-2">
              <asp:TextBox CssClass="form-control" id="TbAttributeName" runat="server" />
            </div>
            <label class="col-xs-2 control-label">显示名称</label>
            <div class="col-xs-3">
              <asp:TextBox class="form-control" Columns="25" MaxLength="50" id="TbDisplayName" runat="server" />
            </div>
            <div class="col-xs-3 help-block">
              <asp:RequiredFieldValidator ControlToValidate="TbAttributeName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbAttributeName" ValidationExpression="[a-zA-Z0-9_]+" ErrorMessage="字段名称只允许包含字母、数字以及下划线"
                foreColor="red" Display="Dynamic" />
              <asp:RequiredFieldValidator ControlToValidate="TbDisplayName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbDisplayName" ValidationExpression="[^']+" errorMessage=" *"
                foreColor="red" display="Dynamic" />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-2 control-label">排序</label>
            <div class="col-xs-2">
              <asp:TextBox class="form-control" id="TbTaxis" runat="server" />
            </div>
            <label class="col-xs-2 control-label">帮助提示</label>
            <div class="col-xs-3">
              <asp:TextBox class="form-control" id="TbHelpText" runat="server" />
            </div>
            <div class="col-xs-3 help-block">
              <asp:RequiredFieldValidator ControlToValidate="TbTaxis" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbHelpText" ValidationExpression="[^']+" errorMessage=" *"
              foreColor="red" display="Dynamic" />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-2 control-label">表单提交类型</label>
            <div class="col-xs-7">
              <asp:DropDownList class="form-control" ID="DdlInputType" OnSelectedIndexChanged="ReFresh" AutoPostBack="true" runat="server"></asp:DropDownList>
            </div>
            <div class="col-xs-3 help-block"></div>
          </div>
          <asp:PlaceHolder ID="PhIsFormatString" Visible="false" runat="server">
            <div class="form-group">
              <label class="col-xs-2 control-label">可否设置格式</label>
              <div class="col-xs-7">
                <asp:DropDownList id="DdlIsFormatString" RepeatDirection="Horizontal" class="form-control" runat="server">
                  <asp:ListItem Text="可设置" Value="True" />
                  <asp:ListItem Text="不可设置" Value="False" Selected="True" />
                </asp:DropDownList>
              </div>
              <div class="col-xs-3 help-block"></div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder ID="PhRelatedField" runat="server">
            <div class="form-group">
              <label class="col-xs-2 control-label">联动字段</label>
              <div class="col-xs-7">
                <asp:DropDownList class="form-control" id="DdlRelatedFieldId" runat="server" />
              </div>
              <div class="col-xs-3 help-block"></div>
            </div>
            <div class="form-group">
              <label class="col-xs-2 control-label">显示方式</label>
              <div class="col-xs-7">
                <asp:DropDownList id="DdlRelatedFieldStyle" class="form-control" runat="server" />
              </div>
              <div class="col-xs-3 help-block"></div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder ID="PhWidth" runat="server">
            <div class="form-group">
              <label class="col-xs-2 control-label">显示宽度</label>
              <div class="col-xs-7">
                <asp:TextBox class="form-control" MaxLength="50" id="TbWidth" runat="server" />
              </div>
              <div class="col-xs-3 help-block">
                px（不设置代表默认宽度）
                <asp:RegularExpressionValidator ControlToValidate="TbWidth" ValidationExpression="\d+" Display="Dynamic" errorMessage=" *"
                  foreColor="red" runat="server" />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="PhHeight" runat="server">
            <div class="form-group">
              <label class="col-xs-2 control-label">显示高度</label>
              <div class="col-xs-7">
                <asp:TextBox class="form-control" MaxLength="50" id="TbHeight" runat="server" />
              </div>
              <div class="col-xs-3 help-block">
                px（不设置代表默认高度）
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
                  <asp:ListItem Text="水平" Value="True" Selected="True" />
                  <asp:ListItem Text="垂直" Value="False" />
                </asp:DropDownList>
              </div>
              <div class="col-xs-3 help-block"></div>
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

          <asp:PlaceHolder ID="PhDefaultValue" runat="server">
            <div class="form-group">
              <label class="col-xs-2 control-label">默认显示值</label>
              <div class="col-xs-7">
                <asp:TextBox class="form-control" id="TbDefaultValue" runat="server" />
              </div>
              <div class="col-xs-3 help-block">
                <span id="SpanDateTip" runat="server">
                  {Current}代表当前日期/日期时间
                </span>
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbDefaultValue" ValidationExpression="[^']+" errorMessage=" *"
                  foreColor="red" display="Dynamic" />
              </div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder ID="PhIsSelectField" runat="server">
            <div class="form-group">
              <label class="col-xs-2 control-label">设置选项</label>
              <div class="col-xs-7">
                <asp:DropDownList ID="DdlIsRapid" class="form-control" OnSelectedIndexChanged="ReFresh" AutoPostBack="true" runat="server">
                  <asp:ListItem Text="快速设置" Value="True" Selected="True" />
                  <asp:ListItem Text="详细设置" Value="False" />
                </asp:DropDownList>
              </div>
              <div class="col-xs-3 help-block"></div>
            </div>

            <asp:PlaceHolder ID="PhRapid" runat="server">
              <div class="form-group">
                <label class="col-xs-2 control-label">选项可选值</label>
                <div class="col-xs-7">
                  <asp:TextBox class="form-control" TextMode="MultiLine" Rows="4" id="TbRapidValues" runat="server" />
                </div>
                <div class="col-xs-3 help-block">
                  <asp:RequiredFieldValidator ControlToValidate="TbRapidValues" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                  />
                  <span class="grey">英文","分隔</span>
                </div>
              </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="PhNotRapid" runat="server">
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
              <asp:Repeater ID="RptItems" runat="server">
                <itemtemplate>
                  <div class="form-group">
                    <label class="col-xs-2 control-label">
                      <asp:Literal id="ltlSeq" runat="server"></asp:Literal>
                    </label>
                    <div class="col-xs-3">
                      标题
                      <asp:TextBox class="form-control" ID="tbTitle" Columns="40" runat="server"></asp:TextBox>
                      <asp:RequiredFieldValidator ControlToValidate="tbTitle" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                      />
                    </div>
                    <div class="col-xs-3">
                      值
                      <asp:TextBox class="form-control" ID="tbValue" Columns="40" runat="server"></asp:TextBox>
                      <asp:RequiredFieldValidator ControlToValidate="tbValue" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                      />
                    </div>
                    <div class="col-xs-2">
                      &nbsp;
                      <asp:CheckBox ID="cbIsSelected" class="checkbox checkbox-primary" runat="server" Checked="False" Text="默认选择"></asp:CheckBox>
                    </div>
                    <div class="col-xs-3 help-block"> </div>
                  </div>
                </itemtemplate>
              </asp:Repeater>
            </asp:PlaceHolder>

          </asp:PlaceHolder>

          <asp:PlaceHolder ID="PhCustomize" runat="server">
            <div class="form-group">
              <label class="col-xs-2 control-label">左侧代码</label>
              <div class="col-xs-7">
                <asp:TextBox class="form-control" id="TbCustomizeLeft" TextMode="MultiLine" Rows="4" runat="server" />
              </div>
              <div class="col-xs-3">
                <span class="help-block">
                  左侧代码（Html）为包含字段名称的表单项，必填
                  <br /> {Value}代表表单项的值
                </span>
                <asp:RequiredFieldValidator ControlToValidate="TbCustomizeLeft" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                />
              </div>
            </div>
            <div class="form-group">
              <label class="col-xs-2 control-label">右侧代码</label>
              <div class="col-xs-7">
                <asp:TextBox class="form-control" id="TbCustomizeRight" TextMode="MultiLine" Rows="4" runat="server" />
              </div>
              <div class="col-xs-3">
                <span class="help-block">
                  右侧代码（Html、Css、Js）为按钮、说明等其他元素，可选
                </span>
              </div>
            </div>
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