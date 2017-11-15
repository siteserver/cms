<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageSearchAdd" %>
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
  <asp:Literal id="LtlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />

  <script type="text/javascript" src="background_searchAdd.js"></script>
  <style type="text/css">
  div.step {
  font-weight: bold;
  font-size: 16px;
  margin-bottom: 10px;
  }

  span.activate_title {
  line-height: 34px;
  font-size: 16px;
  color: #333;
  }

  p.activate_desc {
  width: 100%;
  margin-left: 32px;
  font-size: 13px;
  font-weight: bold;
  }

  div.step_one, div.step_two, div.step_three {
    display: inline-block;
    margin-left: 30px;
    width: 280px;
    height: 190px;
    background: transparent url("images/weixin-activate.png") no-repeat;
  }
  div.step_two, div.step_three {
    margin-top: 20px;
  }
  div.step_one {
    background-position: -40px -48px;
  }
  div.step_two {
    background-position: -395px -48px;
  }
  div.step_three {
    background-position: -760px -48px;
  }
  </style>
  <script type="text/javascript">
  var selectChannel = function(itemIndex, nodeNames, nodeID){
    if (itemIndex == 1){
      $('#imageChannelTitle').show().html(nodeNames);
      $('#imageChannelID').val(nodeID);
    }else{
      $('#textChannelTitle').show().html(nodeNames);
      $('#textChannelID').val(nodeID);
    }
  };
  <asp:Literal id="LtlSearchNavs" runat="server" />
</script>

  <bairong:Code type="ajaxupload" runat="server" />
  <link rel="stylesheet" href="../../SiteFiles/Services/WeiXin/components/lib/fontawesome/css/font-awesome.min.css"  />
  <script type="text/javascript" src="../../sitefiles/bairong/scripts/swfUpload/swfupload.js"></script>
  <script type="text/javascript" src="../../sitefiles/bairong/scripts/swfUpload/handlers.js"></script>

  <script src="../../SiteFiles/bairong/JQuery/jscolor/jscolor.js"></script>

  <div class="popover popover-static operation-area">
    <h3 class="popover-title">
      <asp:Literal id="LtlPageTitle" runat="server" />
    </h3>
    <div class="popover-content">
      <div class="container-fluid" id="weixinactivate">

      <asp:PlaceHolder id="PhStep1" runat="server">
        <div class="row-fluid">

          <div class="Span6">
            <div class="step">第一步：配置微搜索开始属性</div>
            <table class="table noborder table-hover">
              <tr>
                <td width="120">主题：</td>
                <td>
                  <asp:TextBox class="input-xlarge" id="TbTitle" runat="server"/>
                  <asp:RequiredFieldValidator
                    ControlToValidate="TbTitle"
                    errorMessage=" *" foreColor="red" 
                    Display="Dynamic"
                    runat="server"/>
                  <asp:RegularExpressionValidator
                    runat="server"
                    ControlToValidate="TbTitle"
                    ValidationExpression="[^']+"
                    errorMessage=" *" foreColor="red" 
                    Display="Dynamic" />
                </td>
              </tr>
              <tr>
                <td>摘要：</td>
                <td>
                  <asp:TextBox id="TbSummary" textMode="Multiline" class="textarea" rows="4" style="width:95%; padding:5px;" runat="server" />
                </td>
              </tr>
              <tr>
                <td>微搜索状态：</td>
                <td class="checkbox">
                  <asp:CheckBox id="CbIsEnabled" runat="server" checked="true" text="启用微搜索" />
                </td>
              </tr>
              <tr>
                <td>触发关键词：</td>
                <td>
                  <asp:TextBox id="TbKeywords" runat="server" />
                  <asp:RegularExpressionValidator
                    runat="server"
                    ControlToValidate="TbKeywords"
                    ValidationExpression="[^']+"
                    errorMessage=" *" foreColor="red" 
                    Display="Dynamic" />
                    <br>
                  <span class="gray">多个关键词请用空格格开：例如: 微信 腾讯 阁下</span> 
                </td>
              </tr>
            </table>
          </div>

          <div class="Span6">
            <div class="intro-grid">
              <p><strong>微搜索进行中显示图片：</strong></p>
              <asp:Literal id="LtlImageUrl" runat="server" />
              <a id="js_imageUrl" href="javascript:;" onclick="return false;" class="btn btn-success">上传</a>
            </div>
          </div>
        
        </div>

        <script type="text/javascript">
        new AjaxUpload('js_imageUrl', {
         action: '<%=GetUploadUrl()%>',
         name: "Upload",
         data: {},
         onSubmit: function(file, ext) {
           var reg = /^(jpg|jpeg|png|gif)$/i;
           if (ext && reg.test(ext)) {
             //$('#img_upload_txt_').text('上传中... ');
           } else {
             //$('#img_upload_txt_').text('只允许上传JPG,PNG,GIF图片');
             alert('只允许上传JPG,PNG,GIF图片');
             return false;
           }
         },
         onComplete: function(file, response) {
           if (response) {
             response = eval("(" + response + ")");
             if (response.success == 'true') {
               $('#preview_imageUrl').attr('src', response.url);
               $('#imageUrl').val(response.virtualUrl);
             } else {
               alert(response.message);
             }
           }
         }
        });
        </script>
      </asp:PlaceHolder>

      <asp:PlaceHolder id="PhStep2" visible="false" runat="server">
        <div class="row-fluid">

          <div class="Span6">
            <div class="step">第二步：配置微搜索详情页</div>
            <table class="table noborder table-hover">
              <tr>
                <td width="120">站外搜索状态：</td>
                <td class="checkbox">
                  <asp:CheckBox id="CbIsOutsiteSearch" runat="server" checked="true" text="启用站外搜索" />
                </td>
              </tr>
              <tr>
                <td width="120">导航栏状态：</td>
                <td class="checkbox">
                  <asp:CheckBox id="CbIsNavigation" runat="server" checked="true" text="启用导航栏" />
                </td>
              </tr>
              <tr class="isNavigation">
                <td width="120">导航标题颜色值：</td>
                <td>
                    <asp:TextBox class="input-xlarge color" id="TbNavTitleColor" Width="220px" runat="server"/>
                </td>
              </tr>  
              <tr class="isNavigation">
                <td width="120">导航图标颜色值：</td>
                <td>
                    <asp:TextBox class="input-xlarge color" id="TbNavImageColor" Width="220px" runat="server"/>
                </td>
              </tr>
            </table>
          </div>

          <div class="Span6">
            <div class="intro-grid">
              <p><strong>微搜索详情页背景图片：</strong></p>
              <asp:Literal id="LtlContentImageUrl" runat="server" />
              <a id="js_contentImageUrl" href="javascript:;" onclick="return false;" class="btn btn-success">上传</a>
            </div>
          </div>

          <div class="clearfix"></div>
          <hr />

          <script type="text/javascript">
          </script>
          <script type="text/html" class="itemController">
<input name="itemCount" type="hidden" value="{{items.length}}" />
<div class="isNavigation">
  <div class="step">导航设置</div>
  <table class="table noborder">
    <tr>
      <td>
        <table class="table table-bordered table-hover">
            <tr class="info thead">
              <td width="120">链接名称</td>
              <td>链接地址</td>
              <td width="160">链接图标</td>
              <td width="60"></td>
            </tr>
            {{each items}}
            <tr>
              <td>
                <input type="hidden" name="itemID" class="itemID" value="{{$value.id ? $value.id + '' : '0'}}">

                <input type="hidden" name="itemImageCssClass" class="itemImageCssClass" value="{{$value.imageCssClass}}">
                <input  type="hidden" name="itemKeywordType" class="itemKeywordType" value="{{$value.keywordType}}">
                <input  type="hidden" name="itemFunctionID" class="itemFunctionID" value="{{$value.functionID}}">
                <input  type="hidden" name="itemChannelID" class="itemChannelID" value="{{$value.channelID}}">
                <input  type="hidden" name="itemContentID" class="itemContentID" value="{{$value.contentID}}">

                <input type="text" name="itemTitle" value="{{$value.title}}" class="input-medium itemTitle">
              </td>
              <td>
                <select name="itemNavigationType" class="itemNavigationType" style="width:110px" onchange="itemController.changeItem(this, {{$index}});return false;">
                  <option value="Url" {{if ($value.navigationType == 'Url')}}selected="selected"{{/if}}>指定网址</option>
                  <option value="Site" {{if ($value.navigationType == 'Site')}}selected="selected"{{/if}}>微网站页面</option>
                  <option value="Function" {{if ($value.navigationType == 'Function')}}selected="selected"{{/if}}>微功能页面</option>
                </select>

                <input type="text" id="itemUrl_{{$index}}" name="itemUrl" value="{{$value.url}}" class="itemUrl input-xlarge" style="display:{{(!$value.navigationType || $value.navigationType == 'Url') ? '' : 'none'}}">

                {{if ($value.navigationType != 'Url')}}
                  {{if $value.pageTitle}}
                  <code>{{$value.pageTitle}}</code>
                  {{/if}}
                {{/if}}

                {{if ($value.navigationType == 'Site')}}
                  <a href="javascript:;" onclick="itemController.openChannelSelect({{$index}});" class="btn">选择栏目页</a>
                  <a href="javascript:;" onclick="itemController.openContentSelect({{$index}});" class="btn">选择内容页</a>
                {{/if}}

                {{if ($value.navigationType == 'Function')}}
                  <a href="javascript:;" onclick="itemController.openFunctionSelect({{$index}});" class="btn">选择微功能页</a>
                {{/if}}
              </td>
              <td class="center">
                <i class="{{$value.imageCssClass}}" style="font-size:20px; "></i>
                <a href="javascript:;" onclick="itemController.openImageCssClassSelect({{$index}});" class="btn" style="float:right;">选择图标</a>
              </td>
              <td class="center">
                {{if $index > 0}}
                <a href="javascript:;" onclick="itemController.removeItem({{$index}});">删除</a>
                {{/if}}
              </td>
            </tr>
            {{/each}}
            <tr>
              <td colspan="6">
                <a href="javascript:;" onclick="itemController.addItem({})" class="btn btn-success">再加一项</a>
                <span>至少设置一项</span>
              </td>
            </tr>
        </table>
      </td>
    </tr>
  </table>
</div>
          </script>
        </div>

        <script type="text/javascript">
        new AjaxUpload('js_contentImageUrl', {
         action: '<%=GetUploadUrl()%>',
         name: "Upload",
         data: {},
         onSubmit: function(file, ext) {
           var reg = /^(jpg|jpeg|png|gif)$/i;
           if (ext && reg.test(ext)) {
             //$('#img_upload_txt_').text('上传中... ');
           } else {
             //$('#img_upload_txt_').text('只允许上传JPG,PNG,GIF图片');
             alert('只允许上传JPG,PNG,GIF图片');
             return false;
           }
         },
         onComplete: function(file, response) {
           if (response) {
             response = eval("(" + response + ")");
             if (response.success == 'true') {
               $('#preview_contentImageUrl').attr('src', response.url);
               $('#contentImageUrl').val(response.virtualUrl);
             } else {
               alert(response.message);
             }
           }
         }
        });
        </script>
      </asp:PlaceHolder>

      <asp:PlaceHolder id="PhStep3" visible="false" runat="server">
        <div class="row-fluid">
          <div class="Span6">
            <div class="step">第三步：配置搜索首页显示内容</div>
            <table class="table noborder table-hover">
              <tr>
                <td width="120">图文列表标题：</td>
                <td>
                 <asp:TextBox class="input-xlarge" id="TbImageAreaTitle" runat="server"/>
                  <asp:RegularExpressionValidator
                    runat="server"
                    ControlToValidate="TbImageAreaTitle"
                    ValidationExpression="[^']+"
                    errorMessage=" *" foreColor="red" 
                    Display="Dynamic" />
                </td>
              </tr>
               <tr>
                <td width="120">选择图文栏目：</td>
                <td>
                    <code id="imageChannelTitle" style="display:none"></code>
                    <asp:Button id="BtnImageChannelSelect" class="btn btn-info" text="选择图文栏目" runat="server" />
                    <input id="imageChannelID" name="imageChannelID" type="hidden" value="" />
              </td>
              </tr>
            </table> 
          </div>
          <div class="Span6">
             <div class="step">&nbsp;</div>
             <table class="table noborder table-hover">
               <tr>
                <td width="120">文本列表标题：</td>
                <td>
                  <asp:TextBox class="input-xlarge" id="TbTextAreaTitle" runat="server"/>
                  <asp:RegularExpressionValidator
                    runat="server"
                    ControlToValidate="TbTextAreaTitle"
                    ValidationExpression="[^']+"
                    errorMessage=" *" foreColor="red" 
                    Display="Dynamic" />
                </td>
              </tr>
               <tr>
                <td width="120">选择文本栏目：</td>
                <td>
                   <code id="textChannelTitle" style="display:none"></code>
                    <asp:Button id="BtnTextChannelSelect" class="btn btn-info" text="选择文本栏目" runat="server" />
                    <input id="textChannelID" name="textChannelID" type="hidden" value="" />
              </td>
              </tr>
            </table> 
           </div>
         </div>
 
      </asp:PlaceHolder>
 
      <input id="imageUrl" name="imageUrl" type="hidden" runat="server" />
      <input id="contentImageUrl" name="contentImageUrl" type="hidden" runat="server" />
    
      <hr />
      <table class="table table-noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-primary" id="BtnSubmit" text="下一步" OnClick="Submit_OnClick" runat="server"/>
            <asp:Button class="btn" id="BtnReturn" text="返 回" runat="server"/>
          </td>
        </tr>
      </table>
    </div>
    </div>
  </div>

</form>
</body>
</html>
