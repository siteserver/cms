<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.ModalUserView" Trace="false"%>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form runat="server">
        <ctrl:alerts text="" runat="server" />

        <div class="form-horizontal">

          <div class="form-group">
            <label class="col-xs-4 text-right control-label">用户名</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlUserName" runat="server" />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-4 text-right control-label">姓名</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlDisplayName" runat="server" />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-4 text-right control-label">电子邮箱</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlEmail" runat="server" />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-4 text-right control-label">手机号码</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlMobile" runat="server" />
            </div>
          </div>
          <div class="form-group">
            <label class="col-xs-4 text-right control-label">登录次数</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlLoginCount" runat="server" />
            </div>
          </div>
          <div class="form-group">
            <label class="col-xs-4 text-right control-label">投稿数量</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlWritingCount" runat="server" />
            </div>
          </div>
          <div class="form-group">
            <label class="col-xs-4 text-right control-label">单位</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlOrganization" runat="server" />
            </div>
          </div>
          <div class="form-group">
            <label class="col-xs-4 text-right control-label">部门</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlDepartment" runat="server" />
            </div>
          </div>
          <div class="form-group">
            <label class="col-xs-4 text-right control-label">职位</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlPosition" runat="server" />
            </div>
          </div>
          <div class="form-group">
            <label class="col-xs-4 text-right control-label">出生日期</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlBirthday" runat="server" />
            </div>
          </div>
          <div class="form-group">
            <label class="col-xs-4 text-right control-label">性别</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlGender" runat="server" />
            </div>
          </div>
          <div class="form-group">
            <label class="col-xs-4 text-right control-label">毕业院校</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlGraduation" runat="server" />
            </div>
          </div>
          <div class="form-group">
            <label class="col-xs-4 text-right control-label">学历</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlEducation" runat="server" />
            </div>
          </div>
          <div class="form-group">
            <label class="col-xs-4 text-right control-label">地址</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlAddress" runat="server" />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-4 text-right control-label">微信</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlWeiXin" runat="server" />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-4 text-right control-label">QQ</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlQq" runat="server" />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-4 text-right control-label">微博</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlWeiBo" runat="server" />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-4 text-right control-label">兴趣</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlInterests" runat="server" />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-4 text-right control-label">签名</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlSignature" runat="server" />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-4 text-right control-label">注册时间</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlCreateDate" runat="server" />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-4 text-right control-label">最后修改密码时间</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlLastResetPasswordDate" runat="server" />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-4 text-right control-label">最后登录时间</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlLastActivityDate" runat="server" />
            </div>
          </div>

          <hr />

          <div class="form-group m-b-0">
            <div class="col-xs-11 text-right">
              <button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">关 闭</button>
            </div>
            <div class="col-xs-1"></div>
          </div>

        </div>

      </form>
    </body>

    </html>