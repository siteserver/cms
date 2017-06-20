<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageKeywordResourceAdd" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <!--#include file="../inc/header.aspx"-->
</head>

<body>
    <!--#include file="../inc/openWindow.html"-->
    <form class="form-inline" runat="server">
        <bairong:Alerts runat="server" />

        <link rel="stylesheet" type="text/css" href="css/keywordListAdd.css">
        <bairong:Code Type="ajaxupload" runat="server" />
        <script type="text/javascript" src="../../sitefiles/bairong/scripts/swfUpload/swfupload.js"></script>
        <script type="text/javascript" src="../../sitefiles/bairong/scripts/swfUpload/handlers.js"></script>

        <div class="appmsg_editor" style="margin-top: 0px;">
            <div class="inner">
                <ul class="nav nav-tabs">
                    <asp:Literal ID="LtlNav" runat="server" />
                </ul>
                <input id="resourceType" name="resourceType" type="hidden" value="" />
                <div class="resource Site" style="margin: 20px 5px;">

                    <asp:Literal ID="LtlSite" runat="server" />

                    <div class="btn-group">
                        <asp:Button id="BtnContentSelect" class="btn btn-info" Text="选择内容页" runat="server" />
                        <asp:Button id="BtnChannelSelect" class="btn btn-info" Text="选择栏目页" runat="server" />
                    </div>

                    <script type="text/javascript">
                        var contentSelect = function (title, nodeID, contentID, pageUrl, imageUrl, imageSrc, summary) {
                            $('#titles').show().html('内容页：' + title + '&nbsp;<a href="' + pageUrl + '" target="blank">查看</a>');

                            if ($('#tbTitle').val() == '') {
                                $('#tbTitle').val(title);
                            }
                            if ($('#imageUrl').val() == '') {
                                $('.upload_preview img').attr('src', imageSrc);
                                $('#imageUrl').val(imageUrl);
                                $('.upload_preview').show();
                            }
                            if ($('#tbSummary').val() == '') {
                                $('#tbSummary').val(summary);
                            }

                            $('#channelID').val(nodeID);
                            $('#contentID').val(contentID);
                        };
                        var selectChannel = function (nodeNames, nodeID, pageUrl) {
                            $('#titles').show().html('栏目页：' + nodeNames + '&nbsp;<a href="' + pageUrl + '" target="blank">查看</a>');
                            $('#channelID').val(nodeID);
                            $('#contentID').val('0');
                        };
                    </script>
                </div>

                <div class="appmsg_edit_item">
                    <label for="" class="frm_label">
                        标题
                    </label>
                    <span style="padding-left: 5px;">
                        <asp:TextBox ID="TbTitle" Style="width: 95%" runat="server" />
                        <asp:RequiredFieldValidator ControlToValidate="TbTitle" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="TbTitle" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                    </span>
                </div>
                <div class="appmsg_edit_item">
                    <label for="" class="frm_label">
                        排序
                    </label>
                    <span style="padding-left: 5px;">
                        <asp:TextBox ID="TbTaxis" class="input-mini" runat="server" />
                    </span>
                </div>
                <div class="appmsg_edit_item">
                    <label for="" class="frm_label">
                        <strong class="title">封面
                        </strong>
                        <span class="js_cover_tip gray pull-right">大图片建议尺寸：360像素 * 200像素，小图片建议尺寸：200像素 * 200像素
                        </span>
                    </label>
                    <div class="frm_input_box" style="margin-left: 5px; width: 95%; padding: 0 5px;">
                        <div class="upload_box">
                            <div class="upload_area">
                                <a id="js_appmsg_upload_cover" href="javascript:void(0);" onclick="return false;" class="upload_access" width="50" height="22">
                                    <i class="icon18_common upload_gray"></i>
                                    上传
                                </a>
                                <ul class="upload_file_box" style="display: none;"></ul>
                            </div>
                        </div>
                        <p class="pull-right resource Content" style="margin-top: 5px; display: none">
                            <label for="" class="frm_checkbox_label js_show_cover_pic selected">
                                <i class="icon_checkbox"></i>
                                <input name="isShowCoverPic" type="hidden" value="true">
                                封面图片显示在正文中
                            </label>
                        </p>
                        <asp:Literal ID="LtlPreview" runat="server" />
                    </div>
                </div>

                <div class="appmsg_edit_item">
                    <label for="" class="frm_label">
                        <strong class="title">摘要
                        </strong>
                        <span class="gray">（选填）
                        </span>
                    </label>
                    <span style="padding-left: 5px;">
                        <asp:TextBox ID="TbSummary" TextMode="Multiline" class="textarea" Rows="4" Style="width: 95%; padding: 5px;" runat="server" />
                    </span>
                </div>

                <div id="js_ueditor" class="appmsg_edit_item content_edit resource Content" style="display: none">
                    <label for="" class="frm_label">
                        <strong class="title">正文
                        </strong>
                    </label>
                    <div>
                        <bairong:UEditor ID="BreContent" Width="100%" Height="350" runat="server">
                        </bairong:UEditor>
                    </div>
                </div>

                <div class="appmsg_edit_item resource Url" style="display: none;">
                    <label for="" class="frm_label">
                        链接到网址
                    </label>
                    <span style="padding-left: 5px;">
                        <asp:TextBox ID="TbNavigationUrl" Style="width: 95%" runat="server" />
                    </span>
                </div>

            </div>
            <asp:Literal ID="LtlArrow" runat="server" />
            <div class="mask" style="display: none;">
            </div>
        </div>

        <div class="tool_area">
            <div class="tool_bar">
                <asp:Button class="btn btn-primary" Text="保 存" OnClick="Submit_OnClick" runat="server" />
            </div>
        </div>

        <script type="text/javascript">
            function deleteImageUrl() {
                $('#imageUrl').val('');
                $('.upload_preview').hide();
            }
            new AjaxUpload('js_appmsg_upload_cover', {
                action: '<%=GetUploadUrl()%>',
      name: "Upload",
      data: {},
      onSubmit: function (file, ext) {
          var reg = /^(jpg|jpeg|png|gif)$/i;
          if (ext && reg.test(ext)) {
              //$('#img_upload_txt_').text('上传中... ');
          } else {
              //$('#img_upload_txt_').text('只允许上传JPG,PNG,GIF图片');
              alert('只允许上传JPG,PNG,GIF图片');
              return false;
          }
      },
      onComplete: function (file, response) {
          if (response) {
              response = eval("(" + response + ")");
              if (response.success == 'true') {
                  $('.upload_preview img').attr('src', response.url);
                  $('#imageUrl').val(response.virtualUrl);
                  $('.upload_preview').show();
              } else {
                  alert(response.message);
              }
          }
      }
  });
  $('.js_show_cover_pic').click(function () {
      $(this).toggleClass('selected');
      if ($('.js_show_cover_pic input').val() == 'true') {
          $('.js_show_cover_pic input').val('false');
      } else {
          $('.js_show_cover_pic input').val('true');
      }
  });
  $('.nav a').click(function () {
      $('.nav li').removeClass('active');
      $('#resourceType').val($(this).attr('resourceType'));
      $(this).parent().addClass('active');
      $('.resource').hide();
      $('.' + $(this).attr('resourceType')).show();
  });
        </script>

        <asp:Literal ID="LtlScript" runat="server" />

    </form>
</body>
</html>

