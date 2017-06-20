<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageScratchAdd" %>
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

  <script type="text/javascript" src="background_scratchAdd.js"></script>
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

  <bairong:Code type="ajaxupload" runat="server" />
  <script type="text/javascript" src="../../sitefiles/bairong/scripts/swfUpload/swfupload.js"></script>
  <script type="text/javascript" src="../../sitefiles/bairong/scripts/swfUpload/handlers.js"></script>

  <div class="popover popover-static operation-area">
    <h3 class="popover-title">
      <asp:Literal id="LtlPageTitle" runat="server" />
    </h3>
    <div class="popover-content">
      <div class="container-fluid" id="weixinactivate">

      <asp:PlaceHolder id="PhStep1" runat="server">
        <div class="row-fluid">

          <div class="Span6">
            <div class="step">第一步：配置刮刮卡开始属性</div>
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
                <td>开始时间：</td>
                <td>
                  <bairong:DateTimeTextBox id="dtbStartDate" now="true" showTime="true" Columns="20" runat="server" />
                </td>
              </tr>
              <tr>
                <td>结束时间：</td>
                <td>
                  <bairong:DateTimeTextBox id="dtbEndDate" showTime="true" Columns="20" runat="server" />
                </td>
              </tr>
              <tr>
                <td>刮刮卡状态：</td>
                <td class="checkbox">
                  <asp:CheckBox id="CbIsEnabled" runat="server" checked="true" text="启用刮刮卡" />
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
                  <span class="gray">多个关键词请用空格格开：例如: 微信 腾讯</span> 
                </td>
              </tr>
            </table>
          </div>

          <div class="Span6">
            <div class="intro-grid">
              <p><strong>刮刮卡进行中显示图片：</strong></p>
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
            <div class="step">第二步：配置刮刮卡详情页</div>
            <table class="table noborder table-hover">
              <tr>
                <td width="120">刮刮卡活动规则：</td>
                <td>
                  <asp:TextBox id="TbContentUsage" textMode="Multiline" class="textarea" rows="4" style="width:95%; padding:5px;" runat="server" />
                </td>
              </tr>
            </table>
          </div>

          <div class="Span6">
            <div class="intro-grid">
              <p><strong>刮刮卡详情页头部图片：</strong></p>
              <asp:Literal id="LtlContentImageUrl" runat="server" />
              <a id="js_contentImageUrl" href="javascript:;" onclick="return false;" class="btn btn-success">上传</a>
            </div>
          </div>

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
            <div class="step">第三步：配置奖项设置</div>
            <table class="table noborder table-hover">
              <tr>
                <td width="190">中奖凭证使用说明：</td>
                <td>
                  <asp:TextBox id="TbAwardUsage" textMode="Multiline" class="textarea" rows="4" style="width:95%; padding:5px;" runat="server" text="1.本活动凭票兑奖，一人一票一次性
2.请下载至手机或打印携带，保持条码清晰完整
3.本活动组织方拥有最终解释权" />
                </td>
              </tr>
              <tr>
                <td>是否显示奖品数量：</td>
                <td>
                  <asp:CheckBox id="CbIsAwardTotalNum" class="checkbox" runat="server" text="显示" />
                </td>
              </tr>
              <tr>
                <td>每人最多允许抽奖次数：</td>
                <td>
                  <asp:TextBox id="TbAwardMaxCount" class="input-mini" runat="server" text="0" />
                  <span>0代表不限制</span>
                </td>
              </tr>
              <tr>
                <td>每人每天最多允许抽奖次数：</td>
                <td>
                  <asp:TextBox id="TbAwardMaxDailyCount" class="input-mini" runat="server" text="0" />
                  <span>0代表不限制，必须小于最多抽奖次数</span>
                </td>
              </tr>
              <tr>
                <td>活动方兑奖密码：</td>
                <td>
                  <asp:TextBox id="TbAwardCode" runat="server" />
                  <br>
                  <span>若不设置密码，中奖确认页面的活动方兑奖区域将不显示</span>
                </td>
              </tr>
            </table>
          </div>

          <div class="Span6">
            <div class="intro-grid">
              <p><strong>获奖页头部图片：</strong></p>
              <asp:Literal id="LtlAwardImageUrl" runat="server" />
              <a id="js_awardImageUrl" href="javascript:;" onclick="return false;" class="btn btn-success">上传</a>
            </div>
          </div>

          <div class="clearfix"></div>
          <hr />

          <script type="text/javascript"><asp:Literal id="LtlAwardItems" runat="server" /></script>
          <script type="text/html" class="itemController">
          <input id="itemCount" name="itemCount" type="hidden" value="{{itemCount}}" />
          <div>
            <div class="step">奖项设置</div>
            <table class="table noborder">
              <tr>
                <td>
                  <table class="table table-bordered table-hover">
                      <tr class="info thead">
                        <td width="120">奖项名称</td>
                        <td>奖品名</td>
                        <td width="120">奖品总数</td>
                        <td width="120">中奖概率</td>
                        <td width="60"></td>
                      </tr>
                      {{each items}}
                      <tr>
                        <td>
                          <input type="hidden" name="itemID" class="itemID" value="{{$value.id ? $value.id + '' : '0'}}">
                          <input type="text" name="itemAwardName" value="{{$value.awardName}}" class="itemAwardName input-medium">
                        </td>
                        <td>
                          <input type="text" name="itemTitle" value="{{$value.title}}" class="itemTitle input-xlarge">
                        </td>
                        <td>
                          <input type="text" name="itemTotalNum" value="{{$value.totalNum ? $value.totalNum + '' : '0'}}" class="itemTotalNum input-mini">
                        </td>
                        <td>
                          <input type="text" name="itemProbability" value="{{$value.probability ? $value.probability + '' : '0'}}" class="itemProbability input-mini"> <span>%</span>
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
                          <span>至少设置一项，各项中奖概率总和不能超过100%</span>
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
        new AjaxUpload('js_awardImageUrl', {
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
               $('#preview_awardImageUrl').attr('src', response.url);
               $('#awardImageUrl').val(response.virtualUrl);
             } else {
               alert(response.message);
             }
           }
         }
        });
        </script>
      </asp:PlaceHolder>

      <asp:PlaceHolder id="PhStep4" visible="false" runat="server">
        <div class="row-fluid">

          <div class="step">第四步：配置领奖提交表单</div>
            <table class="table noborder table-hover">
              <tr>
                <td width="160">是否显示姓名字段：</td>
                <td class="checkbox">
                  <asp:CheckBox id="CbIsFormRealName" runat="server" checked="true" text="显示" />
                </td>
              </tr>
              <tr>
                <td>姓名重命名：</td>
                <td>
                  <asp:TextBox class="input-xlarge" id="TbFormRealNameTitle" text="姓名" runat="server"/>
                  <asp:RequiredFieldValidator
                    ControlToValidate="TbFormRealNameTitle"
                    errorMessage=" *" foreColor="red" 
                    Display="Dynamic"
                    runat="server"/>
                  <asp:RegularExpressionValidator
                    runat="server"
                    ControlToValidate="TbFormRealNameTitle"
                    ValidationExpression="[^']+"
                    errorMessage=" *" foreColor="red" 
                    Display="Dynamic" />
                </td>
              </tr>
              <tr>
                <td>是否显示手机字段：</td>
                <td class="checkbox">
                  <asp:CheckBox id="CbIsFormMobile" runat="server" checked="true" text="显示" />
                </td>
              </tr>
              <tr>
                <td>手机重命名：</td>
                <td>
                  <asp:TextBox class="input-xlarge" id="TbFormMobileTitle" text="手机" runat="server"/>
                  <asp:RequiredFieldValidator
                    ControlToValidate="TbFormMobileTitle"
                    errorMessage=" *" foreColor="red" 
                    Display="Dynamic"
                    runat="server"/>
                  <asp:RegularExpressionValidator
                    runat="server"
                    ControlToValidate="TbFormMobileTitle"
                    ValidationExpression="[^']+"
                    errorMessage=" *" foreColor="red" 
                    Display="Dynamic" />
                </td>
              </tr>
              <tr>
                <td>是否显示邮箱字段：</td>
                <td class="checkbox">
                  <asp:CheckBox id="CbIsFormEmail" runat="server" checked="true" text="显示" />
                </td>
              </tr>
              <tr>
                <td>邮箱重命名：</td>
                <td>
                  <asp:TextBox class="input-xlarge" id="TbFormEmailTitle" text="邮箱" runat="server"/>
                  <asp:RequiredFieldValidator
                    ControlToValidate="TbFormEmailTitle"
                    errorMessage=" *" foreColor="red" 
                    Display="Dynamic"
                    runat="server"/>
                  <asp:RegularExpressionValidator
                    runat="server"
                    ControlToValidate="TbFormEmailTitle"
                    ValidationExpression="[^']+"
                    errorMessage=" *" foreColor="red" 
                    Display="Dynamic" />
                </td>
              </tr>
              <tr>
                <td>是否显示地址字段：</td>
                <td class="checkbox">
                  <asp:CheckBox id="CbIsFormAddress" runat="server" checked="true" text="显示" />
                </td>
              </tr>
              <tr>
                <td>地址重命名：</td>
                <td>
                  <asp:TextBox class="input-xlarge" id="TbFormAddressTitle" text="地址" runat="server"/>
                  <asp:RequiredFieldValidator
                    ControlToValidate="TbFormAddressTitle"
                    errorMessage=" *" foreColor="red" 
                    Display="Dynamic"
                    runat="server"/>
                  <asp:RegularExpressionValidator
                    runat="server"
                    ControlToValidate="TbFormAddressTitle"
                    ValidationExpression="[^']+"
                    errorMessage=" *" foreColor="red" 
                    Display="Dynamic" />
                </td>
              </tr>
            </table>
        
        </div>

      </asp:PlaceHolder>

      <asp:PlaceHolder id="PhStep5" visible="false" runat="server">
        <div class="row-fluid">

          <div class="Span6">
            <div class="step">第五步：配置刮刮卡结束属性</div>
            <table class="table noborder table-hover">
              <tr>
                <td width="120">刮刮卡结束主题：</td>
                <td>
                  <asp:TextBox class="input-large" id="TbEndTitle" runat="server"/>
                  <asp:RequiredFieldValidator
                    ControlToValidate="TbEndTitle"
                    errorMessage=" *" foreColor="red" 
                    Display="Dynamic"
                    runat="server"/>
                  <asp:RegularExpressionValidator
                    runat="server"
                    ControlToValidate="TbEndTitle"
                    ValidationExpression="[^']+"
                    errorMessage=" *" foreColor="red" 
                    Display="Dynamic" />
                </td>
              </tr>
              <tr>
                <td>刮刮卡结束摘要：</td>
                <td>
                  <asp:TextBox id="TbEndSummary" textMode="Multiline" class="textarea" rows="4" style="width:95%; padding:5px;" runat="server" />
                </td>
              </tr>
            </table>
          </div>

          <div class="Span6">
            <div class="intro-grid">
              <p><strong>刮刮卡已结束显示图片：</strong></p>
              <asp:Literal id="LtlEndImageUrl" runat="server" />
              <a id="js_endImageUrl" href="javascript:;" onclick="return false;" class="btn btn-success">上传</a>
            </div>
          </div>

        </div>

        <script type="text/javascript">
        new AjaxUpload('js_endImageUrl', {
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
               $('#preview_endImageUrl').attr('src', response.url);
               $('#endImageUrl').val(response.virtualUrl);
             } else {
               alert(response.message);
             }
           }
         }
        });
        </script>
      </asp:PlaceHolder>

      </div>

      <input id="imageUrl" name="imageUrl" type="hidden" runat="server" />
      <input id="contentImageUrl" name="contentImageUrl" type="hidden" runat="server" />
      <input id="awardImageUrl" name="awardImageUrl" type="hidden" runat="server" />
      <input id="endImageUrl" name="endImageUrl" type="hidden" runat="server" />
  
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

</form>
</body>
</html>
