<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageContentTranslate" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
      <script language="javascript" type="text/javascript">
        function translateNodeAdd(name, value) {
          $('#translateContainer').append("<span id='translate_" + value + "' class='label label-primary p-1 m-r-5'>" +
            name +
            "&nbsp;<i class='ion-android-close' style='cursor:pointer' onClick=\"translateNodeRemove('" + value +
            "')\"></i>&nbsp;</span>");
          $('#translateCollection').val(value + ',' + $('#translateCollection').val());
        }

        function translateNodeRemove(value) {
          $('#translate_' + value).remove();
          var val = '';
          var values = $('#translateCollection').val().split(",");
          for (i = 0; i < values.length; i++) {
            if (values[i] && value != values[i]) {
              val = values[i] + ',';
            }
          }
          $('#translateCollection').val(val);
        }
      </script>
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">
        <ctrl:alerts runat="server" />

        <div class="card-box">
          <div class="m-t-0 header-title">
            内容转移
          </div>
          <p class="text-muted font-13 m-b-25">
            可以同时选择多个栏目，内容将同时转移到对应栏目下；转移有四种方式：
            <br>
            <span class="text-danger">“复制”</span>将创建内容的副本，并拷贝到指定栏目下，副本和原始内容之间不存在关系；
            <br>
            <span class="text-danger">“剪切”</span>代表将内容转移到指定栏目下，系统不会创建内容副本；
            <br>
            <span class="text-danger">“引用地址”</span>将创建内容的副本，并拷贝到指定栏目下，内容副本仅是原内容的引用，内容副本链接将和原内容链接一致。
            <br>
            <span class="text-danger">“引用内容”</span>将创建内容的副本，并拷贝到指定栏目下，同时内容副本的数据与原内容保持同步，内容副本的链接指向副本内容。
          </p>

          <div class="form-group form-row">
            <label class="col-sm-2 col-form-label">需要转移的内容</label>
            <div class="col-sm-8">
              <asp:Literal ID="LtlContents" runat="server"></asp:Literal>
            </div>
            <div class="col-sm-2"></div>
          </div>

          <div class="form-group form-row">
            <label class="col-sm-2 col-form-label">转移到栏目</label>
            <div class="col-sm-8">
              <span class="pull-left m-t-5" id="translateContainer"></span>
              <asp:Button id="BtnTranslateAdd" class="btn pull-left" text="选择栏目" runat="server" />
            </div>
            <div class="col-sm-2"></div>
          </div>

          <div class="form-group form-row">
            <label class="col-sm-2 col-form-label">转移方式</label>
            <div class="col-sm-8">
              <input id="translateCollection" name="translateCollection" value="" type="hidden">
              <asp:RadioButtonList ID="RblTranslateType" class="radio radio-primary" repeatDirection="Horizontal" runat="server"></asp:RadioButtonList>
            </div>
            <div class="col-sm-2"></div>
          </div>

          <hr />

          <div class="text-center">
            <asp:Button class="btn btn-primary" ID="Submit" Text="转 移" OnClick="Submit_OnClick" runat="server" />
            <asp:Button class="btn m-l-5" ID="Return" Text="返 回" CausesValidation="false" OnClick="Return_OnClick" runat="server" />
          </div>

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->