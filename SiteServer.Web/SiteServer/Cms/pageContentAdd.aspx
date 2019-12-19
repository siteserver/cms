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
  <link rel="stylesheet" href="../assets/ueditor/third-party/xiumi/xiumi-ue-v5.css">
  <link href="../assets/element-ui/theme-chalk/index.css" rel="stylesheet" type="text/css" />
</head>

<body>
  <form id="myForm" class="m-l-15 m-r-15" enctype="multipart/form-data" runat="server">
    <ctrl:alerts runat="server" />

    <div class="card-box">
      <div class="m-t-0 header-title">
        <asp:Literal ID="LtlPageTitle" runat="server" />
      </div>
      <p class="text-muted font-13 m-b-25"></p>

      <div class="form-group form-row">
        <label class="col-1 col-form-label text-right text-truncate text-nowrap" title="标题">标题</label>
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

      <div class="form-group form-row">
        <label class="col-1 col-form-label text-right text-truncate text-nowrap">属性</label>
        <div class="col-5">
          <asp:CheckBoxList class="checkbox checkbox-primary m-t-5" ID="CblContentAttributes" RepeatDirection="Horizontal" RepeatColumns="5"
            runat="server" />
        </div>
        <div class="col-6">

        </div>
      </div>
      <div class="form-group form-row">
        <label class="col-1 col-form-label text-right text-truncate text-nowrap">内容组</label>
        <div class="col-6">
          <asp:CheckBoxList ID="CblContentGroups" RepeatDirection="Horizontal" class="checkbox checkbox-primary" RepeatColumns="5"
            runat="server" />
          <input type="button" onClick="utils.openLayer({title: '添加内容组', url: 'modalContentGroupAdd.aspx?siteId=<%=SiteId%>', width: 600, height: 300});return false;" class="btn btn-sm" value="新增内容组" />
        </div>
        <div class="col-5">
          
        </div>
      </div>

      <div class="form-group form-row" style="display: none">
        <label class="col-1 col-form-label text-right text-truncate text-nowrap">标签</label>
        <div class="col-6">
          <asp:TextBox ID="TbTags" class="form-control" runat="server" />
        </div>
        <div class="col-5">
          <asp:Literal ID="LtlTags" runat="server"></asp:Literal>
          <small class="form-text text-muted">请用空格或英文逗号分隔</small>
        </div>
      </div>

      <div class="form-group form-row">
        <label class="col-1 col-form-label text-right text-truncate text-nowrap"><a name="tagNames"></a>标签</label>
        <div class="col-6">
          <el-select
            v-model="tagNames"
            style="width: 100%;"
            size="medium"
            multiple
            filterable
            allow-create
            default-first-option
            placeholder="请选择内容标签">
            <el-option
              v-for="tagName in allTagNames"
              :key="tagName"
              :label="tagName"
              :value="tagName">
            </el-option>
          </el-select>
          <small class="form-text text-muted">输入文字后回车可以创建并选中选项中不存在的条目</small>
        </div>
        <div class="col-5">

        </div>
      </div>

      <asp:PlaceHolder ID="PhStatus" runat="server">
        <div class="form-group form-row">
          <label class="col-1 col-form-label text-right text-truncate text-nowrap">状态</label>
          <div class="col-6">
            <asp:RadioButtonList ID="RblContentLevel" RepeatDirection="Horizontal" class="radio radio-primary" RepeatColumns="5"
            runat="server" />
          </div>
          <div class="col-5">

          </div>
        </div>
      </asp:PlaceHolder>

      <div class="form-group form-row">
        <label class="col-1 col-form-label text-right text-truncate text-nowrap">外部链接</label>
        <div class="col-6">
          <asp:TextBox ID="TbLinkUrl" class="form-control" runat="server" />
        </div>
        <div class="col-5">
          <small class="form-text text-muted">设置后链接将指向此地址</small>
        </div>
      </div>

      <div class="form-group form-row">
        <label class="col-1 col-form-label text-right text-truncate text-nowrap">添加时间</label>
        <div class="col-6">
          <ctrl:DateTimeTextBox ID="TbAddDate" ShowTime="true" class="form-control" MaxLength="50" Width="180" runat="server" />
        </div>
        <div class="col-5">

        </div>
      </div>

      <asp:PlaceHolder ID="PhTranslate" runat="server">
        <div class="form-group form-row">
          <label class="col-1 col-form-label text-right text-truncate text-nowrap">转移到</label>
          <div class="col-10">
            <span class="pull-left m-t-5" id="translateContainer"></span>
            <button onClick="utils.openLayer({title: '选择目标栏目', url: 'modalChannelMultipleSelect.aspx?siteId=<%=SiteId%>&isSiteSelect=True&jsMethod=translateNodeAdd', width: 650, height: 580});return false;" class="btn btn-sm pull-left">选择栏目</button>
          </div>
          <div class="col-1"></div>
        </div>
        <div class="form-group form-row" id="translateType" style="display: none">
          <label class="col-1 col-form-label text-right text-truncate text-nowrap">转移方式</label>
          <div class="col-6">
            <input id="translateCollection" name="translateCollection" value="" type="hidden">
            <asp:DropDownList ID="DdlTranslateType" class="form-control" runat="server"></asp:DropDownList>
          </div>
          <div class="col-5"></div>
        </div>
      </asp:PlaceHolder>

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
<script type="text/javascript" charset="utf-8" src="../assets/ueditor/third-party/xiumi/xiumi-ue-dialog-v5.js"></script>
<script src="../assets/js/es6-promise.auto.min.js" type="text/javascript"></script>
<script src="../assets/js/lodash-4.17.10.min.js" type="text/javascript"></script>
<script src="../assets/js/sweetalert2-7.28.4.all.min.js" type="text/javascript"></script>
<script src="../assets/js/vue-2.5.16.min.js" type="text/javascript"></script>
<script src="../assets/js/vee-validate-2.1.0.js" type="text/javascript"></script>
<script src="../assets/js/vee-validate-locale-zh_CN-2.1.0.js" type="text/javascript"></script>
<script src="../assets/element-ui/index.js"></script>
<script src="../assets/js/apiUtils.js" type="text/javascript"></script>
<script type="text/javascript">
  var apiUrl = '@Sys.InnerApiUrl';
</script>
<script src="pageContentAdd.js"></script>
