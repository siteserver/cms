<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.PageAutoUpdate" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <title>SiteServer CMS 升级向导</title>
      <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
      <link href="./images/siteserver_icon.png" rel="icon" type="image/png">
      <link href="./assets/bootstrap/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
      <link href="./assets/css/core.css" rel="stylesheet" type="text/css" />
      <link href="./assets/css/components.css" rel="stylesheet" type="text/css" />
      <link href="./assets/css/pages.css" rel="stylesheet" type="text/css" />
      <link href="./assets/css/menu.css" rel="stylesheet" type="text/css" />
      <link href="./assets/css/responsive.css" rel="stylesheet" type="text/css" />
      <link href="./assets/css/ionicons.min.css" rel="stylesheet" type="text/css" />
      <script src="./assets/jquery/jquery-1.9.1.min.js" type="text/javascript"></script>
      <script src="./assets/sweetalert/sweetalert.min.js" type="text/javascript"></script>
      <script src="./assets/layer/layer.min.js" type="text/javascript"></script>
      <script src="./inc/script.js" type="text/javascript"></script>
      <link href="./assets/showLoading/css/showLoading.css" rel="stylesheet" media="screen" />
      <script type="text/javascript" src="./assets/showLoading/js/jquery.showLoading.js"></script>
      <style>body{ padding: 20px 0; }</style>
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">

        <div id="main" class="card-box">
          <h4 class="text-dark  header-title m-t-0">
            SiteServer CMS
            <asp:Literal ID="LtlVersionInfo" runat="server"></asp:Literal> 升级向导
          </h4>
          <p class="text-muted m-b-25 font-13">
            欢迎来到SiteServer CMS 升级向导！
          </p>

          <ctrl:alerts runat="server" />

          <!-- step 1 place -->
          <asp:PlaceHolder ID="PhStep1" runat="server">

            <ul class="nav nav-pills nav-fill bg-muted m-b-20">
              <li class="nav-item active">
                <a class="nav-link" href="javascript:;">升级准备</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">检查更新</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">下载升级包</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级系统</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级完成</a>
              </li>
            </ul>

            <div class="jumbotron">
              <h4 class="display-5">欢迎来到SiteServer CMS 升级向导！</h4>
              <p class="lead">
                升级向导将在线检查并自动下载、安装可用的更新，升级范围包括 SiteServer CMS 产品及已安装的插件。
              </p>
              <hr class="my-4">
              <p>
                如果已手动下载产品升级包并解压覆盖，请点击
                <asp:button OnClick="BtnStep4_Click" class="btn btn-primary" Text="直接升级" runat="server"></asp:button>
              </p>
            </div>

            <hr />

            <div class="text-center">
              <asp:button OnClick="BtnStep1_Click" class="btn btn-primary" Text="下一步" runat="server"></asp:button>
            </div>

          </asp:PlaceHolder>

          <!-- step 2 place -->
          <asp:PlaceHolder ID="PhStep2" visible="false" runat="server">

            <ul class="nav nav-pills nav-fill bg-muted m-b-20">
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级准备</a>
              </li>
              <li class="nav-item active">
                <a class="nav-link" href="javascript:;">检查更新</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">下载升级包</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级系统</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级完成</a>
              </li>
            </ul>

            <div class="panel panel-border panel-primary">
              <div class="panel-heading">
                <h3 class="panel-title">产品更新</h3>
                <p class="panel-sub-title font-13 text-muted">SiteServer CMS 新版本</p>
              </div>
              <div class="panel-body">

                <div class="jumbotron text-center">
                  <img src="./pic/animated_loading.gif" />
                  <br />
                  <small class="form-text text-muted">正在检查更新...</small>
                </div>

                <div class="jumbotron">
                  <h4 class="display-5">当前版本已经是最新版本</h4>
                </div>

                <div class="table-responsive">
                  <table class="table tablesaw table-hover m-0">
                    <thead>
                      <tr class="thead">
                        <th class="text-center" width="200">选择</th>
                        <th>版本</th>
                        <th>更新简介</th>
                        <th class="text-center">发布日期</th>
                        <th class="text-center"></th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr>
                        <td class="text-center checkbox checkbox-primary text-nowrap">
                          <input type="checkbox" name="choose" id="choose_" value="" />
                          <label for="choose_">选中</label>
                        </td>
                        <td>
                          当前版本：
                          <strong>5.1</strong>
                          <br /> 新版本：
                          <strong>5.1.1</strong>
                        </td>
                        <td>
                          快速制作各类表单,如报名表,登记表,邀请表,反馈表,预约表,订单表等。
                        </td>
                        <td class="text-center">
                          2018年1月2日
                        </td>
                        <td class="text-center">
                          <a href="/siteserver/plugins/pageconfig.aspx?pluginId=ss-shopping" target="_blank">发行说明</a>
                        </td>
                      </tr>

                    </tbody>
                  </table>
                </div>
              </div>
            </div>

            <div class="panel panel-border panel-primary">
              <div class="panel-heading">
                <h3 class="panel-title">插件更新</h3>
                <p class="panel-sub-title font-13 text-muted">插件新版本</p>
              </div>
              <div class="panel-body">

                <div class="jumbotron text-center">
                  <img src="./pic/animated_loading.gif" />
                  <br />
                  <small class="form-text text-muted">正在检查更新...</small>
                </div>

                <div class="jumbotron text-center">
                  <h4 class="display-5">无可用插件升级包</h4>
                </div>

                <div class="table-responsive">
                  <table class="table tablesaw table-hover m-0">
                    <thead>
                      <tr class="thead">
                        <th class="text-center" width="200">选择</th>
                        <th>插件Id</th>
                        <th>插件名称</th>
                        <th>版本</th>
                        <th>作者</th>
                        <th>更新简介</th>
                        <th class="text-center">发布日期</th>
                        <th></th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr>
                        <td class="text-center checkbox checkbox-primary text-nowrap">
                          <input type="checkbox" name="choose" id="choose_" value="" />
                          <label for="choose_">选中</label>
                        </td>
                        <td>
                          ss-shopping
                        </td>
                        <td>
                          <img src="/SiteFiles/Plugins/ss-shopping/logo.svg" width="48" height="48"> 购物
                        </td>
                        <td>
                          当前版本：
                          <strong>5.1</strong>
                          <br /> 新版本：
                          <strong>5.1.1</strong>
                        </td>
                        <td>
                          <a href="/siteserver/plugins/pageconfig.aspx?pluginId=ss-shopping" target="_blank">siteserver</a>
                        </td>
                        <td>
                          快速制作各类表单,如报名表,登记表,邀请表,反馈表,预约表,订单表等。
                        </td>
                        <td class="text-center">
                          2018年1月2日
                        </td>
                        <td class="text-center">
                          <a href="/siteserver/plugins/pageconfig.aspx?pluginId=ss-shopping" target="_blank">发行说明</a>
                        </td>
                      </tr>

                    </tbody>
                  </table>

                </div>
              </div>
            </div>

            <div style="display: ">

              <hr />

              <div class="text-center">
                <asp:button OnClick="BtnStep2_Click" class="btn btn-primary" Text="下一步" runat="server"></asp:button>
              </div>

            </div>

          </asp:PlaceHolder>

          <!-- step 3 place -->
          <asp:PlaceHolder ID="PhStep3" visible="false" runat="server">

            <ul class="nav nav-pills nav-fill bg-muted m-b-20">
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级准备</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">检查更新</a>
              </li>
              <li class="nav-item active">
                <a class="nav-link" href="javascript:;">下载升级包</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级系统</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级完成</a>
              </li>
            </ul>

            <div class="jumbotron text-center">
              <img src="./pic/animated_loading.gif" />
              <br />
              <small class="form-text text-muted">正在下载升级包...</small>
            </div>

            <div class="jumbotron text-center">
              <h4 class="display-5">升级包已全部下载完成，点击下一步升级系统</h4>
            </div>

            <hr />

            <div class="text-center">
              <asp:button OnClick="BtnStep3_Click" class="btn btn-primary" Text="下一步" runat="server"></asp:button>
            </div>

          </asp:PlaceHolder>

          <!-- step 4 place -->
          <asp:PlaceHolder ID="PhStep4" visible="false" runat="server">

            <ul class="nav nav-pills nav-fill bg-muted m-b-20">
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级准备</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">检查更新</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">下载升级包</a>
              </li>
              <li class="nav-item active">
                <a class="nav-link" href="javascript:;">升级系统</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级完成</a>
              </li>
            </ul>

            <div class="jumbotron text-center">
              <img src="./pic/animated_loading.gif" />
              <br />
              <small class="form-text text-muted">正在升级系统...</small>
            </div>

            <hr />

            <div class="text-center">
              <asp:button OnClick="BtnStep4_Click" class="btn btn-primary" Text="下一步" runat="server"></asp:button>
            </div>

          </asp:PlaceHolder>

          <!-- step 5 place -->
          <asp:PlaceHolder ID="PhStep5" visible="false" runat="server">

            <ul class="nav nav-pills nav-fill bg-muted m-b-20">
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级准备</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">检查更新</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">下载升级包</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级系统</a>
              </li>
              <li class="nav-item active">
                <a class="nav-link" href="javascript:;">升级完成</a>
              </li>
            </ul>

            <div class="alert alert-success" role="alert">
              <h4 class="alert-heading">升级完成！</h4>
              <p>
                恭喜，您已经完成了 SiteServer CMS 的升级
                <asp:Literal id="LtlGo" runat="server" />
              </p>
              <hr>
              <p class="mb-0">
                获取更多使用帮助请访问
                <a href="http://docs.siteserver.cn" target="_blank">SiteServer CMS 文档中心</a>
              </p>
            </div>

          </asp:PlaceHolder>

        </div>

      </form>
    </body>

    </html>
    <!--#include file="./inc/foot.html"-->
    <script>
      var validate = window.Page_ClientValidate;
      $(function () {
        $('.btn-primary').click(function () {
          if (!validate || validate()) {
            $('#main').showLoading();
          }
          return true;
        });
      });
    </script>