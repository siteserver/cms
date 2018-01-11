<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalTaskAdd" Trace="false" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
      <script type="text/javascript" language="javascript">
        function selectAll(isChecked) {
          for (var i = 0; i < document.getElementById('<%=LbCreateChannelId.ClientID%>').options.length; i++) {
            document.getElementById('<%=LbCreateChannelId.ClientID%>').options[i].selected =
              isChecked;
          }
        }
      </script>
    </head>

    <body>
      <form runat="server">
        <ctrl:alerts runat="server" />

        <div class="form-group form-row">
          <label class="col-2 text-right col-form-label">任务名称</label>
          <div class="col-7">
            <asp:TextBox id="TbTaskName" cssClass="form-control" runat="server" />
          </div>
          <div class="col-3 help-block">
            <asp:RequiredFieldValidator ControlToValidate="TbTaskName" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
            />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbTaskName" ValidationExpression="[^']+" ErrorMessage=" *"
              ForeColor="red" Display="Dynamic" />
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-2 text-right col-form-label">任务执行频率</label>
          <div class="col-7">
            <asp:DropDownList ID="DdlFrequencyType" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="DdlFrequencyType_SelectedIndexChanged"
              runat="server"></asp:DropDownList>
          </div>
          <div class="col-3 help-block">

          </div>
        </div>

        <asp:PlaceHolder ID="PhPeriodIntervalMinute" Visible="false" runat="server">
          <div class="form-group form-row">
            <label class="col-2 text-right col-form-label">周期</label>
            <div class="col-2">
              <asp:TextBox class="form-control" MaxLength="50" Text="30" ID="TbPeriodInterval" runat="server" />
            </div>
            <div class="col-2">
              <asp:DropDownList ID="DdlPeriodIntervalType" class="form-control" runat="server"></asp:DropDownList>
            </div>
            <div class="col-3"></div>
            <div class="col-3 help-block">
              <asp:RequiredFieldValidator ControlToValidate="TbPeriodInterval" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator ControlToValidate="TbPeriodInterval" ValidationExpression="\d+" Display="Dynamic" ErrorMessage="必须为大于零的整数"
                foreColor="red" runat="server" />
              <asp:CompareValidator ControlToValidate="TbPeriodInterval" Operator="GreaterThan" ValueToCompare="0" Display="Dynamic" ErrorMessage="必须为大于零的整数"
                foreColor="red" runat="server" />
            </div>
          </div>
        </asp:PlaceHolder>


        <asp:PlaceHolder ID="PhNotPeriod" runat="server">
          <div class="form-group form-row">
            <label class="col-2 text-right col-form-label">任务开始时刻</label>
            <div class="col-7">
              <div class="row">
                <div class="col-1 text-right col-form-label">
                  日期
                </div>
                <div class="col-3">
                  <asp:DropDownList ID="DdlStartDay" CssClass="form-control" runat="server"></asp:DropDownList>
                </div>
                <div class="col-1 text-right col-form-label">
                  星期
                </div>
                <div class="col-3">
                  <asp:DropDownList ID="DdlStartWeekday" CssClass="form-control" runat="server"></asp:DropDownList>
                </div>
                <div class="col-1 text-right col-form-label">
                  小时
                </div>
                <div class="col-3">
                  <asp:DropDownList ID="DdlStartHour" CssClass="form-control" runat="server"></asp:DropDownList>
                </div>
              </div>
            </div>
            <div class="col-3 help-block">

            </div>
          </div>
        </asp:PlaceHolder>

        <div class="form-group form-row">
          <label class="col-2 text-right col-form-label">任务描述</label>
          <div class="col-7">
            <asp:TextBox class="form-control" TextMode="MultiLine" Rows="2" ID="TbDescription" runat="server" />
          </div>
          <div class="col-3 help-block">

          </div>
        </div>


        <asp:PlaceHolder ID="PhCreate" Visible="false" runat="server">
          <div class="form-group form-row">
            <label class="col-2 text-right col-form-label">需要生成的对象</label>
            <div class="col-7">
              <asp:ListBox ID="LbCreateChannelId" class="form-control" SelectionMode="Multiple" Rows="13" runat="server"></asp:ListBox>
              <asp:CheckBox ID="CbCreateIsCreateAll" class="checkbox checkbox-primary" Text="生成全部" runat="server"></asp:CheckBox>
            </div>
            <div class="col-3 help-block">

            </div>
          </div>
          <div class="form-group form-row">
            <label class="col-2 text-right col-form-label">生成类型</label>
            <div class="col-7">
              <asp:CheckBoxList ID="CblCreateCreateTypes" RepeatDirection="Horizontal" class="checkbox checkbox-primary" runat="server"></asp:CheckBoxList>
            </div>
            <div class="col-3 help-block">

            </div>
          </div>
        </asp:PlaceHolder>


        <asp:PlaceHolder ID="PhGather" Visible="false" runat="server">
          <div class="form-group form-row">
            <label class="col-2 text-right col-form-label">
              <asp:Literal ID="LtlGather" runat="server" />
            </label>
            <div class="col-7">
              <asp:ListBox ID="LbGather" CssClass="form-control" SelectionMode="Multiple" Rows="10" runat="server"></asp:ListBox>
            </div>
            <div class="col-3 help-block">
              <asp:RequiredFieldValidator ControlToValidate="LbGather" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
              />
            </div>
          </div>
        </asp:PlaceHolder>


        <asp:PlaceHolder ID="PhBackup" Visible="false" runat="server">
          <div class="form-group form-row">
            <label class="col-2 text-right col-form-label">
              选择备份类型
            </label>
            <div class="col-7">
              <asp:DropDownList ID="DdlBackupType" cssClass="form-control" runat="server"></asp:DropDownList>
            </div>
            <div class="col-3 help-block">

            </div>
          </div>

          <asp:PlaceHolder ID="PhBackupPublishmentSystem" Visible="false" runat="server">
            <div class="form-group form-row">
              <label class="col-2 text-right col-form-label">
                需要备份的站点
              </label>
              <div class="col-7">
                <asp:ListBox ID="LbBackupPublishmentSystemId" cssClass="form-control" SelectionMode="Multiple" Rows="8" runat="server"></asp:ListBox>
                <asp:CheckBox ID="CbBackupIsBackupAll" cssClass="checkbox checkbox-primary" runat="server" onClick="selectAll(this.checked);"
                  Text="全部"></asp:CheckBox>
              </div>
              <div class="col-3 help-block">

              </div>
            </div>
          </asp:PlaceHolder>
        </asp:PlaceHolder>

        <hr />

        <div class="text-right mr-1">
          <asp:Button class="btn btn-primary m-l-5" text="确 定" runat="server" onClick="Submit_OnClick" />
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->