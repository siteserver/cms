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
        创建新站点
        <input type="hidden" id="HihSiteTemplateDir" value="" runat="server" />
        <input type="hidden" id="HihOnlineTemplateName" value="" runat="server" />
      </div>

      <asp:PlaceHolder id="PhSource" runat="server">
        <p class="text-muted font-13 m-b-25">
          欢迎使用创建新站点向导，请选择创建站点的方式
        </p>

        <div class="form-group">
          <label class="col-form-label">创建站点方式</label>
          <asp:RadioButtonList id="RblSource" class="radio radio-primary" runat="server"></asp:RadioButtonList>
        </div>
      </asp:PlaceHolder>

      <asp:PlaceHolder id="PhSiteTemplates" visible="false" runat="server">
        <p class="text-muted font-13 m-b-25">
          请选择站点模板
        </p>

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

      <asp:PlaceHolder id="PhOnlineTemplates" visible="false" runat="server">
        <p class="text-muted font-13 m-b-25">
          请选择在线模板，本页面只显示部分免费模板，更多站点模板请访问官网：
          <a href="http://templates.siteserver.cn" target="_blank">http://templates.siteserver.cn</a>
        </p>

        <div class="panel panel-default">
          <div class="panel-body p-0">
            <div class="table-responsive">
              <table id="contents" class="table tablesaw table-hover m-0">
                <thead>
                  <tr class="thead">
                    <th class="text-center"></th>
                    <th class="text-nowrap">名称</th>
                    <th>简介</th>
                    <th class="text-center text-nowrap">模板作者</th>
                    <th class="text-center text-nowrap">更新时间</th>
                    <th class="text-center text-nowrap"></th>
                  </tr>
                </thead>
                <tbody>
                  <asp:Repeater ID="RptOnlineTemplates" runat="server">
                    <ItemTemplate>
                      <tr>
                        <td class="text-center radio radio-primary text-nowrap">
                          <asp:Literal id="ltlChoose" runat="server" />
                        </td>
                        <td class="text-nowrap">
                          <asp:Literal ID="ltlTitle" runat="server"></asp:Literal>
                        </td>
                        <td>
                          <asp:Literal ID="ltlDescription" runat="server"></asp:Literal>
                        </td>
                        <td class="text-center text-nowrap">
                          <asp:Literal ID="ltlAuthor" runat="server"></asp:Literal>
                        </td>
                        <td class="text-center text-nowrap">
                          <asp:Literal ID="ltlLastEditDate" runat="server"></asp:Literal>
                        </td>
                        <td class="text-center text-nowrap">
                          <asp:Literal ID="ltlPreviewUrl" runat="server"></asp:Literal>
                        </td>
                      </tr>
                    </ItemTemplate>
                  </asp:Repeater>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </asp:PlaceHolder>

      <asp:PlaceHolder id="PhSubmit" visible="false" runat="server">

        <p class="text-muted font-13 m-b-25">
          在此设置新建站点的名称、文件夹以及内容表等信息
        </p>

        <div class="form-group">
          <label class="col-form-label">创建站点方式</label>
          <div class="form-control-plaintext">
            <asp:Literal ID="LtlSource" runat="server"></asp:Literal>
          </div>
        </div>

        <div class="form-group">
          <label class="col-form-label">
            站点名称
            <asp:RequiredFieldValidator ControlToValidate="TbSiteName" errorMessage=" *" foreColor="red" Display="Dynamic"
              runat="server" />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbSiteName" ValidationExpression="[^']+"
              errorMessage=" *" foreColor="red" Display="Dynamic" />
          </label>
          <asp:TextBox id="TbSiteName" class="form-control" runat="server" />
        </div>

        <div class="form-group">
          <label class="col-form-label">
            站点级别
          </label>
          <asp:RadioButtonList ID="RblIsRoot" cssClass="radio radio-primary" AutoPostBack="true" OnSelectedIndexChanged="RblIsRoot_SelectedIndexChanged"
            RepeatDirection="Horizontal" runat="server">
            <asp:ListItem Text="主站" Value="True"></asp:ListItem>
            <asp:ListItem Text="子站" Value="False" Selected="true"></asp:ListItem>
          </asp:RadioButtonList>
        </div>

        <asp:PlaceHolder ID="PhIsNotRoot" runat="server">
          <div class="form-group">
            <label class="col-form-label">
              上级站点
            </label>
            <asp:DropDownList ID="DdlParentId" cssClass="form-control" runat="server"></asp:DropDownList>
          </div>
          <div class="form-group">
            <label class="col-form-label">
              文件夹名称
              <asp:RequiredFieldValidator ControlToValidate="TbSiteDir" errorMessage=" *" foreColor="red" Display="Dynamic"
                runat="server" />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbSiteDir" ValidationExpression="[\\.a-zA-Z0-9_-]+"
                foreColor="red" ErrorMessage=" 只允许包含字母、数字、下划线、中划线及小数点" Display="Dynamic" />
            </label>
            <asp:TextBox id="TbSiteDir" class="form-control" runat="server" />
            <small class="form-text text-muted">实际在服务器中保存此网站的文件夹名称，此路径必须以英文或拼音命名</small>
          </div>
        </asp:PlaceHolder>

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
            <asp:CheckBox id="CbIsImportTableStyles" class="checkbox checkbox-primary" runat="server" Checked="true"
              Text="导入"></asp:CheckBox>
          </div>
        </asp:PlaceHolder>

        <div class="form-group">
          <label class="col-form-label">
            内容表
          </label>
          <asp:RadioButtonList id="RblTableRule" class="radio radio-primary" AutoPostBack="true" OnSelectedIndexChanged="RblTableRule_OnSelectedIndexChanged"
            RepeatDirection="Horizontal" runat="server"></asp:RadioButtonList>
          <asp:PlaceHolder id="PhTableChoose" runat="server">
            <asp:DropDownList ID="DdlTableChoose" class="form-control" runat="server"></asp:DropDownList>
            <small class="form-text text-muted">请选择已存在的内容表</small>
          </asp:PlaceHolder>
          <asp:PlaceHolder id="PhTableHandWrite" runat="server">
            <asp:TextBox ID="TbTableHandWrite" class="form-control" runat="server"></asp:TextBox>
            <small class="form-text text-muted">请输入内容表名称，系统将检测数据库是否已存在指定的内容表，如果不存在系统将创建此内容表。</small>
          </asp:PlaceHolder>
        </div>

        <div class="form-group">
          <label class="col-form-label">
            内容审核机制
          </label>
          <asp:RadioButtonList id="RblIsCheckContentUseLevel" class="radio radio-primary" AutoPostBack="true"
            OnSelectedIndexChanged="RblIsCheckContentUseLevel_OnSelectedIndexChanged" RepeatDirection="Horizontal"
            runat="server"></asp:RadioButtonList>
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

        <div class="form-group">
          <label class="col-form-label">
            网页编码
          </label>
          <asp:DropDownList id="DdlCharset" class="form-control" runat="server"></asp:DropDownList>
        </div>

      </asp:PlaceHolder>

      <hr />

      <div class="text-center">
        <asp:Button cssClass="btn" id="BtnPrevious" onclick="BtnPrevious_Click" CausesValidation="false" Enabled="false"
          runat="server" text="上一步"></asp:button>
        <asp:Button class="btn btn-primary m-l-5" id="BtnNext" onclick="BtnNext_Click" runat="server" text="下一步"></asp:button>
        <asp:Button class="btn btn-primary m-l-5" id="BtnSubmit" onclick="BtnSubmit_Click" visible="false" runat="server"
          text="创建站点"></asp:button>
      </div>

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->