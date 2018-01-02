<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalRelatedFieldAdd" Trace="false"%>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form runat="server">
        <ctrl:alerts text="前缀及后缀为联动字段显示时在下拉列表之前及之后显示的文字，可以为空" runat="server" />

        <div class="form-horizontal">

          <div class="form-group">
            <label class="col-xs-3 text-right control-label">联动字段名称</label>
            <div class="col-xs-8">
              <asp:TextBox id="TbRelatedFieldName" class="form-control" runat="server" />
            </div>
            <div class="col-xs-1 help-block">
              <asp:RequiredFieldValidator ControlToValidate="TbRelatedFieldName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbRelatedFieldName" ValidationExpression="[^',]+" errorMessage=" *"
                foreColor="red" display="Dynamic" />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 text-right control-label">几级联动</label>
            <div class="col-xs-8">
              <asp:DropDownList ID="DdlTotalLevel" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="DdlTotalLevel_SelectedIndexChanged"
                runat="server">
                <asp:ListItem Text="一级" Value="1"></asp:ListItem>
                <asp:ListItem Text="二级" Value="2" Selected="true"></asp:ListItem>
                <asp:ListItem Text="三级" Value="3"></asp:ListItem>
                <asp:ListItem Text="四级" Value="4"></asp:ListItem>
                <asp:ListItem Text="五级" Value="5"></asp:ListItem>
              </asp:DropDownList>
            </div>
            <div class="col-xs-1 help-block"></div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 text-right control-label">一级前缀字符串</label>
            <div class="col-xs-8">
              <asp:TextBox class="form-control" id="TbPrefix1" runat="server" />
            </div>
            <div class="col-xs-1">
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbPrefix1" ValidationExpression="[^',]+" errorMessage=" *"
                foreColor="red" display="Dynamic" />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 text-right control-label">一级后缀字符串</label>
            <div class="col-xs-8">
              <asp:TextBox class="form-control" id="TbSuffix1" runat="server" />
            </div>
            <div class="col-xs-1">
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbSuffix1" ValidationExpression="[^',]+" errorMessage=" *"
                foreColor="red" display="Dynamic" />
            </div>
          </div>

          <asp:PlaceHolder ID="PhFix2" runat="server">
            <div class="form-group">
              <label class="col-xs-3 text-right control-label">二级前缀字符串</label>
              <div class="col-xs-8">
                <asp:TextBox class="form-control" id="TbPrefix2" runat="server" />
              </div>
              <div class="col-xs-1">
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbPrefix2" ValidationExpression="[^',]+" errorMessage=" *"
                  foreColor="red" display="Dynamic" />
              </div>
            </div>

            <div class="form-group">
              <label class="col-xs-3 text-right control-label">二级后缀字符串</label>
              <div class="col-xs-8">
                <asp:TextBox class="form-control" id="TbSuffix2" runat="server" />
              </div>
              <div class="col-xs-1">
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbSuffix2" ValidationExpression="[^',]+" errorMessage=" *"
                  foreColor="red" display="Dynamic" />
              </div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder ID="PhFix3" runat="server">
            <div class="form-group">
              <label class="col-xs-3 text-right control-label">三级前缀字符串</label>
              <div class="col-xs-8">
                <asp:TextBox class="form-control" id="TbPrefix3" runat="server" />
              </div>
              <div class="col-xs-1">
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbPrefix3" ValidationExpression="[^',]+" errorMessage=" *"
                  foreColor="red" display="Dynamic" />
              </div>
            </div>

            <div class="form-group">
              <label class="col-xs-3 text-right control-label">三级后缀字符串</label>
              <div class="col-xs-8">
                <asp:TextBox class="form-control" id="TbSuffix3" runat="server" />
              </div>
              <div class="col-xs-1">
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbSuffix3" ValidationExpression="[^',]+" errorMessage=" *"
                  foreColor="red" display="Dynamic" />
              </div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder ID="PhFix4" runat="server">
            <div class="form-group">
              <label class="col-xs-3 text-right control-label">四级前缀字符串</label>
              <div class="col-xs-8">
                <asp:TextBox class="form-control" id="TbPrefix4" runat="server" />
              </div>
              <div class="col-xs-1">
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbPrefix4" ValidationExpression="[^',]+" errorMessage=" *"
                  foreColor="red" display="Dynamic" />
              </div>
            </div>

            <div class="form-group">
              <label class="col-xs-3 text-right control-label">四级后缀字符串</label>
              <div class="col-xs-8">
                <asp:TextBox class="form-control" id="TbSuffix4" runat="server" />
              </div>
              <div class="col-xs-1">
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbSuffix4" ValidationExpression="[^',]+" errorMessage=" *"
                  foreColor="red" display="Dynamic" />
              </div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder ID="PhFix5" runat="server">
            <div class="form-group">
              <label class="col-xs-3 text-right control-label">五级前缀字符串</label>
              <div class="col-xs-8">
                <asp:TextBox class="form-control" id="TbPrefix5" runat="server" />
              </div>
              <div class="col-xs-1">
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbPrefix5" ValidationExpression="[^',]+" errorMessage=" *"
                  foreColor="red" display="Dynamic" />
              </div>
            </div>

            <div class="form-group">
              <label class="col-xs-3 text-right control-label">五级后缀字符串</label>
              <div class="col-xs-8">
                <asp:TextBox class="form-control" id="TbSuffix5" runat="server" />
              </div>
              <div class="col-xs-1">
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbSuffix5" ValidationExpression="[^',]+" errorMessage=" *"
                  foreColor="red" display="Dynamic" />
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