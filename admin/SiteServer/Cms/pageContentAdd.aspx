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
  <script>
    function translateNodeAdd(name, value) {
      $('#translateContainer').append("<span id='translate_" + value + "' class='label label-primary p-1'>" + name +
        "&nbsp;<i class='ion-android-close' style='cursor:pointer' onClick=\"translateNodeRemove('" + value +
        "')\"></i>&nbsp;</span>&nbsp;&nbsp;");
      $('#translateCollection').val(value + ',' + $('#translateCollection').val());
      $('#translateType').show();
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
      if (val == '') {
        $('#translateType').hide();
      }
    }

    $(document).keypress(function (e) {
      if (e.ctrlKey && e.which == 13 || e.which == 10) {
        e.preventDefault();
        $("#BtnSubmit").click();
      } else if (e.shiftKey && e.which == 13 || e.which == 10) {
        e.preventDefault();
        $("#BtnSubmit").click();
      }
    });

    var isPreviewSaving = false;

    function preview() {
      if (!$('#TbTitle').val()) return;
      if (isPreviewSaving) return;

      isPreviewSaving = true;
      var options = {
        beforeSubmit: function () {
          return true;
        },
        url: '<%=PageContentAddHandlerUrl%>',
        type: 'POST',
        dataType: 'json',
        success: function (data) {
          isPreviewSaving = false;
          if (data && data.previewUrl) {
            window.open(data.previewUrl);
          }
        }
      };

      if (UE) {
        $.each(UE.instants, function (index, editor) {
          editor.sync();
        });
      }
      $('#myForm').ajaxSubmit(options);
    }
  </script>
</head>

<body>
  <form id="myForm" class="m-l-15 m-r-15" enctype="multipart/form-data" runat="server">
    <ctrl:alerts runat="server" />

    <div class="card-box">
      <div class="m-t-0 header-title">
        <asp:Literal ID="LtlPageTitle" runat="server" />
      </div>
      <p class="text-muted font-13 m-b-25"></p>

      <ul class="nav nav-pills m-b-30">
        <li class="nav-item active">
          <a class="nav-link" href="javascript:;" onclick="$('.basic').show();$('.advanced').hide();$('.nav-pills li').removeClass('active');$(this).parent().addClass('active');">基 础</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="javascript:;" onclick="$('.basic').hide();$('.advanced').show();$('.nav-pills li').removeClass('active');$(this).parent().addClass('active');">其 他</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="<%=ReturnUrl%>">返 回</a>
        </li>
      </ul>

      <div class="basic">

        <div class="form-group form-row">
          <label class="col-1 col-form-label text-right text-truncate" title="标题">标题</label>
          <div class="col-6">
            <asp:TextBox ID="TbTitle" class="form-control" isvalidate="true" displayname="标题" isrequire="true" minnum="0" maxnum="0" validatetype="None" regexp="" errormessage="" runat="server" />
            <div id="TbTitle_msg" style="color: red; display: none">标题不能为空</div>
            <script>event_observe('TbTitle', 'blur', checkAttributeValue);</script>
          </div>
          <div class="col-5">
            <asp:Literal ID="LtlTitleHtml" runat="server" />
          </div>
        </div>

        <ctrl:AuxiliaryControl ID="AcAttributes" runat="server" />

      </div>

      <div class="advanced" style="display: none">
        <div class="form-group form-row">
          <label class="col-1 col-form-label text-right">属性</label>
          <div class="col-5">
            <asp:CheckBoxList class="checkbox checkbox-primary m-t-5" ID="CblContentAttributes" RepeatDirection="Horizontal" RepeatColumns="5"
              runat="server" />
          </div>
          <div class="col-6">

          </div>
        </div>
        <div class="form-group form-row">
          <label class="col-1 col-form-label text-right">内容组</label>
          <div class="col-6">
            <asp:CheckBoxList ID="CblContentGroups" RepeatDirection="Horizontal" class="checkbox checkbox-primary" RepeatColumns="5"
              runat="server" />
          </div>
          <div class="col-5">
            <asp:Button id="BtnContentGroupAdd" class="btn" text="新增内容组" runat="server" />
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-1 col-form-label text-right">标签</label>
          <div class="col-6">
            <asp:TextBox ID="TbTags" class="form-control" runat="server" />
          </div>
          <div class="col-5">
            <asp:Literal ID="LtlTags" runat="server"></asp:Literal>
            <small class="form-text text-muted">请用空格或英文逗号分隔</small>
          </div>
        </div>

        <asp:PlaceHolder ID="PhStatus" runat="server">
          <div class="form-group form-row">
            <label class="col-1 col-form-label text-right">状态</label>
            <div class="col-6">
              <asp:DropDownList ID="DdlContentLevel" class="form-control" runat="server" />
            </div>
            <div class="col-5">

            </div>
          </div>
        </asp:PlaceHolder>

        <div class="form-group form-row">
          <label class="col-1 col-form-label text-right">外部链接</label>
          <div class="col-6">
            <asp:TextBox ID="TbLinkUrl" class="form-control" runat="server" />
          </div>
          <div class="col-5">
            <small class="form-text text-muted">设置后链接将指向此地址</small>
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-1 col-form-label text-right">添加时间</label>
          <div class="col-6">
            <ctrl:DateTimeTextBox ID="TbAddDate" ShowTime="true" class="form-control" MaxLength="50" Width="180" runat="server" />
          </div>
          <div class="col-5">

          </div>
        </div>

        <asp:PlaceHolder ID="PhTranslate" runat="server">
          <div class="form-group form-row">
            <label class="col-1 col-form-label text-right">转移到</label>
            <div class="col-10">
              <span class="pull-left m-t-5" id="translateContainer"></span>
              <asp:Button id="BtnTranslate" class="btn pull-left" text="选择栏目" runat="server" />
            </div>
            <div class="col-1"></div>
          </div>
          <div class="form-group form-row" id="translateType" style="display: none">
            <label class="col-1 col-form-label text-right">转移方式</label>
            <div class="col-6">
              <input id="translateCollection" name="translateCollection" value="" type="hidden">
              <asp:DropDownList ID="DdlTranslateType" class="form-control" runat="server"></asp:DropDownList>
            </div>
            <div class="col-5"></div>
          </div>
        </asp:PlaceHolder>
      </div>

      <hr />

      <div class="text-center">
        <asp:Button class="btn btn-primary m-r-5" itemIndex="1" ID="BtnSubmit" Text="确 定" OnClick="Submit_OnClick" runat="server"
        />
        <input class="btn btn-success m-r-5" type="button" onClick="preview();" value="预 览" />
        <input class="btn" type="button" onclick="location.href='<%=ReturnUrl%>';return false;" value="返 回" />
        <small class="form-text text-muted m-t-5">
          提示：按CTRL+回车可以快速提交
        </small>
      </div>

    </div>


  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->
