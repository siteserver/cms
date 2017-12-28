<%@ Page Language="C#" ValidateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.PageContentAdd" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
      <script type="text/javascript" src="../assets/validate.js"></script>
      <script type="text/javascript" src="../assets/jquery/jquery.form.js"></script>
      <script type="text/javascript" src="../assets/jscolor/jscolor.js"></script>
      <script type="text/javascript" src="js/contentAdd.js"></script>
    </head>

    <body>
      <form id="myForm" class="m-l-15 m-r-15" enctype="multipart/form-data" runat="server">
        <ctrl:alerts runat="server" />

        <div class="raw">
          <div class="card-box">
            <h4 class="m-t-0 header-title">
              <b>
                <asp:Literal ID="LtlPageTitle" runat="server" />
              </b>
            </h4>
            <p class="text-muted font-13 m-b-25"></p>

            <ul class="nav nav-pills m-b-30">
              <li class="active">
                <a href="javascript:;" onclick="$('.basic').show();$('.advanced').hide();$('.nav-pills li').removeClass('active');$(this).parent().addClass('active');">基础</a>
              </li>
              <li class="">
                <a href="javascript:;" onclick="$('.basic').hide();$('.advanced').show();$('.nav-pills li').removeClass('active');$(this).parent().addClass('active');">其他</a>
              </li>
            </ul>

            <div class="form-horizontal basic">

              <div class="form-group">
                <label class="col-sm-1 control-label">标题</label>
                <div class="col-sm-6">
                  <asp:TextBox ID="TbTitle" class="form-control" runat="server" />
                </div>
                <div class="col-sm-5">
                  <asp:RequiredFieldValidator ControlToValidate="TbTitle" errorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
                  />
                  <asp:Literal ID="LtlTitleHtml" runat="server" />
                </div>
              </div>

              <ctrl:AuxiliaryControl ID="AcAttributes" runat="server" />

            </div>

            <div class="form-horizontal advanced" style="display: none">
              <div class="form-group">
                <label class="col-sm-1 control-label">属性</label>
                <div class="col-sm-5">
                  <asp:CheckBoxList class="checkbox checkbox-primary" ID="CblContentAttributes" RepeatDirection="Horizontal" RepeatColumns="5"
                    runat="server" />
                </div>
                <div class="col-sm-6">

                </div>
              </div>
              <div class="form-group">
                <label class="col-sm-1 control-label">内容组</label>
                <div class="col-sm-6">
                  <span class="pull-left">
                    <asp:CheckBoxList ID="CblContentGroups" RepeatDirection="Horizontal" class="checkbox checkbox-primary" RepeatColumns="5"
                      runat="server" />
                  </span>
                  <asp:Button id="BtnContentGroupAdd" class="btn pull-left" text="新增内容组" runat="server" />
                </div>
                <div class="col-sm-5">

                </div>
              </div>

              <div class="form-group">
                <label class="col-sm-1 control-label">标签</label>
                <div class="col-sm-6">
                  <asp:TextBox ID="TbTags" class="form-control" runat="server" />
                </div>
                <div class="col-sm-5">
                  <asp:Literal ID="LtlTags" runat="server"></asp:Literal>
                  <span class="help-block">请用空格或英文逗号分隔</span>
                </div>
              </div>

              <asp:PlaceHolder ID="PhStatus" runat="server">
                <div class="form-group">
                  <label class="col-sm-1 control-label">状态</label>
                  <div class="col-sm-6">
                    <asp:DropDownList ID="DdlContentLevel" class="form-control" runat="server" />
                  </div>
                  <div class="col-sm-5">

                  </div>
                </div>
              </asp:PlaceHolder>

              <div class="form-group">
                <label class="col-sm-1 control-label">外部链接</label>
                <div class="col-sm-6">
                  <asp:TextBox ID="TbLinkUrl" class="form-control" runat="server" />
                </div>
                <div class="col-sm-5">
                  <span class="help-block">设置后链接将指向此地址</span>
                </div>
              </div>

              <div class="form-group">
                <label class="col-sm-1 control-label">添加时间</label>
                <div class="col-sm-6">
                  <ctrl:DateTimeTextBox ID="TbAddDate" ShowTime="true" class="form-control" MaxLength="50" Width="180" runat="server" />
                </div>
                <div class="col-sm-5">

                </div>
              </div>

              <asp:PlaceHolder ID="PhTranslate" runat="server">
                <div class="form-group">
                  <label class="col-sm-1 control-label">转移到</label>
                  <div class="col-sm-10">
                    <span class="pull-left m-t-5" id="translateContainer"></span>
                    <asp:Button id="BtnTranslate" class="btn pull-left" text="选择栏目" runat="server" />
                  </div>
                  <div class="col-sm-1"></div>
                </div>
                <div class="form-group" id="translateType" style="display: none">
                  <label class="col-sm-1 control-label">转移方式</label>
                  <div class="col-sm-6">
                    <input id="translateCollection" name="translateCollection" value="" type="hidden">
                    <asp:DropDownList ID="DdlTranslateType" class="form-control" runat="server"></asp:DropDownList>
                  </div>
                  <div class="col-sm-5"></div>
                </div>
              </asp:PlaceHolder>
            </div>

            <hr />

            <div class="form-group m-b-0">
              <div class="col-sm-12 text-center">
                <asp:Button class="btn btn-primary m-r-5" itemIndex="1" ID="BtnSubmit" Text="确 定" OnClick="Submit_OnClick" runat="server"
                />
                <input class="btn btn-success m-r-5" type="button" onClick="previewSave();" value="预 览" />
                <input class="btn" type="button" onclick="location.href='<%=ReturnUrl%>';return false;" value="返 回" />
                <div class="help-block">
                  提示：按CTRL+回车可以快速提交
                </div>
              </div>
            </div>

            <div class="clearfix"></div>

          </div>
        </div>


      </form>
    </body>

    </html>