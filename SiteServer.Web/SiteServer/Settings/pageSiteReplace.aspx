<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageSiteReplace" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">

        <div class="card-box">
          <ul class="nav nav-pills">
            <li class="nav-item active">
              <a class="nav-link" href="pageSite.aspx">系统站点管理</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageSiteUrlWeb.aspx">Web访问地址</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageSiteUrlAssets.aspx">资源文件访问地址</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageSiteUrlApi.aspx">API访问地址</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageSiteAuxiliaryTable.aspx">辅助表管理</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageSiteKeyword.aspx">敏感词管理</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageSiteTemplate.aspx">站点模板管理</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageSiteTemplateOnline.aspx">在线站点模板</a>
            </li>
          </ul>
        </div>

        <ctrl:alerts runat="server" />

        <div class="card-box">
          <div class="m-t-0 header-title">
            整站替换
          </div>

          <asp:PlaceHolder id="PhChooseSiteTemplate" runat="server">

            <p class="text-muted font-13 m-b-25">
              整站替换将改变现有网站，请谨慎使用，您选择的站点为
              <asp:Literal ID="LtlSiteName" runat="server"></asp:Literal>
            </p>

            <input type="hidden" id="HihSiteTemplateDir" value="" runat="server" />

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
                      <asp:Repeater ID="RptContents" runat="server">
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

          <asp:PlaceHolder id="PhCreateSiteParameters" runat="server" Visible="false">

            <p class="text-muted font-13 m-b-25">
              整站替换将改变现有网站，请谨慎使用，您选择的站点模板为
              <asp:Literal ID="LtlSiteTemplateName" runat="server"></asp:Literal>
            </p>

            <div class="form-group">
              <label class="col-form-label">清除站点栏目及内容</label>
              <asp:RadioButtonList ID="RblIsDeleteChannels" runat="server" RepeatDirection="Horizontal" class="radio radio-primary">
                <asp:ListItem Text="清除站点栏目及内容" Value="True"></asp:ListItem>
                <asp:ListItem Text="保留站点栏目及内容" Value="False" Selected="true"></asp:ListItem>
              </asp:RadioButtonList>
            </div>

            <div class="form-group">
              <label class="col-form-label">清除站点显示模板</label>
              <asp:RadioButtonList ID="RblIsDeleteTemplates" runat="server" RepeatDirection="Horizontal" class="radio radio-primary">
                <asp:ListItem Text="清除站点显示模板" Value="True"></asp:ListItem>
                <asp:ListItem Text="保留站点显示模板" Value="False" Selected="true"></asp:ListItem>
              </asp:RadioButtonList>
            </div>

            <div class="form-group">
              <label class="col-form-label">清除站点文件</label>
              <asp:RadioButtonList ID="RblIsDeleteFiles" runat="server" RepeatDirection="Horizontal" class="radio radio-primary">
                <asp:ListItem Text="清除站点文件" Value="True"></asp:ListItem>
                <asp:ListItem Text="保留站点文件" Value="False" Selected="true"></asp:ListItem>
              </asp:RadioButtonList>
            </div>

            <div class="form-group">
              <label class="col-form-label">是否覆盖同名数据</label>
              <asp:RadioButtonList ID="RblIsOverride" runat="server" RepeatDirection="Horizontal" class="radio radio-primary">
                <asp:ListItem Text="覆盖同名数据" Value="True" Selected="true"></asp:ListItem>
                <asp:ListItem Text="不覆盖同名数据" Value="False"></asp:ListItem>
              </asp:RadioButtonList>
            </div>

          </asp:PlaceHolder>

          <hr />

          <div class="text-center">
            <asp:Button class="btn btn-primary m-r-5" id="BtnNext" onclick="BtnNext_Click" runat="server" text="下一步"></asp:button>
            <asp:Button class="btn btn-danger m-r-5" id="BtnSubmit" onclick="BtnSubmit_Click" visible="false" runat="server" text="整站替换"></asp:button>
            <asp:Button class="btn m-r-5" text="返 回" onclick="Return_OnClick" runat="server" />
          </div>

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->