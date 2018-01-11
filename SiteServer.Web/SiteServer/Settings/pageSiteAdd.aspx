<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageSiteAdd" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">
        <ctrl:alerts runat="server" />

        <div class="card-box">
          <div class="m-t-0 header-title">
            创建站点
          </div>

          <asp:PlaceHolder id="PhSiteTemplate" runat="server">
            <input type="hidden" id="HihSiteTemplateDir" value="" runat="server" />

            <p class="text-muted font-13 m-b-25">
              欢迎使用新建站点向导，您可以选择使用站点模板作为新建站点的基础
            </p>

            <div class="form-group">
              <label class="col-form-label">是否使用站点模板</label>
              <asp:CheckBox id="CbIsSiteTemplate" class="checkbox checkbox-primary" AutoPostBack="true" OnCheckedChanged="CbIsSiteTemplate_CheckedChanged"
                runat="server" Checked="true" Text="使用"></asp:CheckBox>
            </div>

            <asp:PlaceHolder id="PhIsSiteTemplate" runat="server">
              <div class="panel panel-default">
                <div class="panel-body p-0">
                  <div class="table-responsive">
                    <table class="table tablesaw table-hover m-0">
                      <thead>
                        <tr class="thead">
                          <th class="text-center"></th>
                          <th>站点模板名称</th>
                          <th>站点模板文件夹</th>
                          <th>站点模板介绍</th>
                          <th>样图</th>
                        </tr>
                      </thead>
                      <tbody>
                        <asp:Repeater ID="RptSiteTemplates" runat="server">
                          <itemtemplate>
                            <tr>
                              <td class="text-center radio radio-primary">
                                <asp:Literal id="ltlChoose" runat="server" />
                              </td>
                              <td>
                                <asp:Literal id="ltlTemplateName" runat="server" />
                              </td>
                              <td>
                                <asp:Literal id="ltlName" runat="server" />
                              </td>
                              <td>
                                <asp:Literal id="ltlDescription" runat="server" />
                              </td>
                              <td>
                                <asp:Literal id="ltlSamplePic" runat="server" />
                              </td>
                            </tr>
                          </itemtemplate>
                        </asp:Repeater>
                      </tbody>
                    </table>

                  </div>
                </div>
              </div>
            </asp:PlaceHolder>

          </asp:PlaceHolder>

          <asp:PlaceHolder id="PhParameters" runat="server" Visible="false">

            <p class="text-muted font-13 m-b-25">
              在此设置新建站点的名称、文件夹以及辅助表等信息
            </p>

            <asp:PlaceHolder id="PhSiteTemplateName" runat="server">
              <div class="form-group">
                <label class="col-form-label">使用的站点模板名称</label>
                <div class="form-control-plaintext">
                  <asp:Literal ID="LtlSiteTemplateName" runat="server"></asp:Literal>
                </div>
              </div>
            </asp:PlaceHolder>

            <div class="form-group">
              <label class="col-form-label">
                站点名称
                <asp:RequiredFieldValidator ControlToValidate="TbPublishmentSystemName" errorMessage=" *" foreColor="red" Display="Dynamic"
                  runat="server" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbPublishmentSystemName" ValidationExpression="[^']+" errorMessage=" *"
                  foreColor="red" Display="Dynamic" />
              </label>
              <asp:TextBox id="TbPublishmentSystemName" class="form-control" runat="server" />
            </div>

            <div class="form-group">
              <label class="col-form-label">
                站点级别
              </label>
              <asp:RadioButtonList ID="RblIsHeadquarters" class="radio radio-primary" AutoPostBack="true" OnSelectedIndexChanged="RblIsHeadquarters_SelectedIndexChanged"
                RepeatDirection="Horizontal" runat="server">
                <asp:ListItem Text="主站" Value="True"></asp:ListItem>
                <asp:ListItem Text="子站" Value="False" Selected="true"></asp:ListItem>
              </asp:RadioButtonList>
            </div>

            <asp:PlaceHolder ID="PhIsNotHeadquarters" runat="server">
              <div class="form-group">
                <label class="col-form-label">
                  上级站点
                </label>
                <asp:DropDownList ID="DdlParentPublishmentSystemId" class="form-control" runat="server"></asp:DropDownList>
              </div>
              <div class="form-group">
                <label class="col-form-label">
                  文件夹名称
                  <asp:RequiredFieldValidator ControlToValidate="TbPublishmentSystemDir" errorMessage=" *" foreColor="red" Display="Dynamic"
                    runat="server" />
                  <asp:RegularExpressionValidator runat="server" ControlToValidate="TbPublishmentSystemDir" ValidationExpression="[\\.a-zA-Z0-9_-]+"
                    foreColor="red" ErrorMessage=" 只允许包含字母、数字、下划线、中划线及小数点" Display="Dynamic" />
                </label>
                <asp:TextBox id="TbPublishmentSystemDir" class="form-control" runat="server" />
                <small class="form-text text-muted">实际在服务器中保存此网站的文件夹名称，此路径必须以英文或拼音命名</small>
              </div>
            </asp:PlaceHolder>

            <div class="form-group">
              <label class="col-form-label">
                网页编码
              </label>
              <asp:DropDownList id="DdlCharset" class="form-control" runat="server"></asp:DropDownList>
            </div>

            <asp:PlaceHolder id="PhIsImportContents" runat="server">
              <div class="form-group">
                <label class="col-form-label">
                  是否导入栏目及内容
                </label>
                <asp:CheckBox id="CbIsImportContents" class="checkbox checkbox-primary" runat="server" Checked="true" Text="导入"></asp:CheckBox>
              </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder id="PhIsImportTableStyles" runat="server">
              <div class="form-group">
                <label class="col-form-label">
                  是否导入表单样式
                </label>
                <asp:CheckBox id="CbIsImportTableStyles" class="checkbox checkbox-primary" runat="server" Checked="true" Text="导入"></asp:CheckBox>
              </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder id="PhIsUserSiteTemplateAuxiliaryTables" runat="server">
              <div class="form-group">
                <label class="col-form-label">
                  站点表结构设置
                </label>
                <asp:RadioButtonList ID="RblIsUserSiteTemplateAuxiliaryTables" class="radio radio-primary" AutoPostBack="true" OnSelectedIndexChanged="RblIsUserSiteTemplateAuxiliaryTables_SelectedIndexChanged"
                  RepeatDirection="Horizontal" runat="server">
                  <asp:ListItem Text="使用站点模板中的辅助表" Value="True"></asp:ListItem>
                  <asp:ListItem Text="使用指定的辅助表" Value="False" Selected="true"></asp:ListItem>
                </asp:RadioButtonList>
              </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="PhAuxiliaryTable" runat="server" Visible="false">
              <div class="form-group">
                <label class="col-form-label">
                  内容辅助表
                  <asp:RequiredFieldValidator ControlToValidate="DdlAuxiliaryTableForContent" ErrorMessage="辅助表不能为空！" foreColor="red" Display="Dynamic"
                    runat="server" />
                </label>
                <asp:DropDownList ID="DdlAuxiliaryTableForContent" class="form-control" runat="server"></asp:DropDownList>
              </div>
            </asp:PlaceHolder>

            <div class="form-group">
              <label class="col-form-label">
                内容审核机制
              </label>
              <asp:RadioButtonList id="RblIsCheckContentUseLevel" class="radio radio-primary" AutoPostBack="true" OnSelectedIndexChanged="RblIsCheckContentUseLevel_OnSelectedIndexChanged"
                RepeatDirection="Horizontal" runat="server"></asp:RadioButtonList>
            </div>

            <asp:PlaceHolder ID="PhCheckContentLevel" runat="server" Visible="false">
              <div class="form-group">
                <label class="col-form-label">
                  内容审核级别
                </label>
                <asp:DropDownList id="DdlCheckContentLevel" class="form-control" runat="server">
                  <asp:ListItem Value="2" Text="二级" Selected="true"></asp:ListItem>
                  <asp:ListItem Value="3" Text="三级"></asp:ListItem>
                  <asp:ListItem Value="4" Text="四级"></asp:ListItem>
                  <asp:ListItem Value="5" Text="五级"></asp:ListItem>
                </asp:DropDownList>
                <small class="form-text text-muted">指此内容在添加后需要经多少次审核才能正式发布</small>
              </div>
            </asp:PlaceHolder>

          </asp:PlaceHolder>

          <hr />

          <div class="text-center">
            <asp:Button cssClass="btn" id="BtnPrevious" onclick="BtnPrevious_Click" CausesValidation="false" Enabled="false" runat="server"
              text="上一步"></asp:button>
            <asp:Button class="btn btn-primary" id="BtnSiteTemplateNext" onclick="BtnSiteTemplateNext_Click" runat="server" text="下一步"></asp:button>
            <asp:Button class="btn btn-primary" id="BtnParameters" onclick="BtnParameters_Click" visible="false" runat="server" text="创建站点"></asp:button>
          </div>

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->