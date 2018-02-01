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

        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">用户名</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlUserName" runat="server" />
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">姓名</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlDisplayName" runat="server" />
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">电子邮箱</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlEmail" runat="server" />
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">手机号码</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlMobile" runat="server" />
          </div>
        </div>
        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">登录次数</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlLoginCount" runat="server" />
          </div>
        </div>
        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">投稿数量</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlWritingCount" runat="server" />
          </div>
        </div>
        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">单位</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlOrganization" runat="server" />
          </div>
        </div>
        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">部门</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlDepartment" runat="server" />
          </div>
        </div>
        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">职位</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlPosition" runat="server" />
          </div>
        </div>
        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">出生日期</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlBirthday" runat="server" />
          </div>
        </div>
        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">性别</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlGender" runat="server" />
          </div>
        </div>
        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">毕业院校</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlGraduation" runat="server" />
          </div>
        </div>
        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">学历</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlEducation" runat="server" />
          </div>
        </div>
        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">地址</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlAddress" runat="server" />
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">微信</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlWeiXin" runat="server" />
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">QQ</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlQq" runat="server" />
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">微博</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlWeiBo" runat="server" />
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">兴趣</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlInterests" runat="server" />
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">签名</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlSignature" runat="server" />
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">注册时间</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlCreateDate" runat="server" />
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">最后修改密码时间</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlLastResetPasswordDate" runat="server" />
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">最后登录时间</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlLastActivityDate" runat="server" />
          </div>
        </div>

        <hr />

        <div class="text-right mr-1">
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">关 闭</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->