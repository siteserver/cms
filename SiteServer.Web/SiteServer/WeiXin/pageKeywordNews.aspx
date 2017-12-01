<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageKeywordNews" %>
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
  <bairong:alerts text="设置关键词自动回复，可以通过添加规则，用户发送的消息内如果有您设置的关键字，即可把您设置在此规则名中回复的内容自动发送给用户。" runat="server" />

  <link rel="stylesheet" type="text/css" href="css/keywordList.css">

  <!-- start -->
<div class="appmsg_list" id="appmsgList">

  <asp:Repeater ID="RptContents" runat="server">
    <itemtemplate>
        <asp:PlaceHolder id="PhSingle" runat="server">

        <!-- single start -->
  <div class="appmsg_col">
    <div class="inner">
      <div>
        <div class="appmsg ">
          <div class="appmsg_content">
            <h4 class="appmsg_title">
              <asp:Literal id="LtlSingleTitle" runat="server" />
            </h4>
            <div class="appmsg_info">
              <code class="pull-right">
                <asp:Literal id="LtlSingleKeywords" runat="server" />
              </code>

              <em class="appmsg_date">
                <asp:Literal id="LtlSingleAddDate" runat="server" />
              </em>
            </div>
            <div class="appmsg_thumb_wrp">
              <asp:Literal id="LtlSingleImageUrl" runat="server" />
            </div>
            <p class="appmsg_desc">
              <asp:Literal id="LtlSingleSummary" runat="server" />
            </p>
          </div>
          <div class="appmsg_opr">
            <ul>
              <li class="appmsg_opr_item with2">
                <asp:Literal id="LtlSingleEditUrl" runat="server" />
              </li>
              <li class="appmsg_opr_item with2 no_extra">
                <asp:Literal id="LtlSingleDeleteUrl" runat="server" />
              </li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  </div>
        <!-- single end -->

        </asp:PlaceHolder>
        <asp:PlaceHolder id="PhMultiple" runat="server">

        <!-- multiple start -->
  <div class="appmsg_col">
    <div class="inner">
      <div>
        <div class="appmsg multi">
          <div class="appmsg_content">
            <div class="appmsg_info">
              <code class="pull-right">
                <asp:Literal id="LtlMultipleKeywords" runat="server" />
              </code>

              <em class="appmsg_date">
                <asp:Literal id="LtlMultipleAddDate" runat="server" />
              </em>
            </div>
            <div class="cover_appmsg_item">
              <h4 class="appmsg_title">
                <asp:Literal id="LtlMultipleTitle" runat="server" />
              </h4>
              <div class="appmsg_thumb_wrp">
                <asp:Literal id="LtlMultipleImageUrl" runat="server" />
              </div>
            </div>

            <asp:Repeater ID="RptMultipleContents" runat="server">
              <itemtemplate>
                  <div class="appmsg_item">
                    <asp:Literal id="LtlImageUrl" runat="server" />
                    <h4 class="appmsg_title">
                      <asp:Literal id="LtlTitle" runat="server" />
                    </h4>
                  </div>
              </itemtemplate>
            </asp:Repeater>
            
          </div>
            
          <div class="appmsg_opr">
            <ul>
              <li class="appmsg_opr_item with2">
                <asp:Literal id="LtlMultipleEditUrl" runat="server" />
              </li>
              <li class="appmsg_opr_item with2 no_extra">
                <asp:Literal id="LtlMultipleDeleteUrl" runat="server" />
              </li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  </div>
        <!-- multiple end -->

        </asp:PlaceHolder>
    </itemtemplate>
  </asp:Repeater>
 
  <!-- <div class="appmsg_col">
    <div class="inner" id="appmsgList1">
      <div id="appmsg200027427">
        <div class="appmsg " data-id="200027427" data-fileid="200027425">
          <div class="appmsg_content">
            <h4 class="appmsg_title">
              <a href="http://mp.weixin.qq.com/s?__biz=MzA3NTIzODQzNA==&amp;mid=200027427&amp;idx=1&amp;sn=a76a92c3109d9b047b5997e88e834447#rd"
              target="_blank">
                叮叮当当
              </a>
            </h4>
            <div class="appmsg_info">
              <code class="pull-right">关键词(包含)&nbsp;<a href="#">修改</a></code>

              <em class="appmsg_date">
                17:19
              </em>
            </div>
            <div class="appmsg_thumb_wrp">
              <img src="https://mmbiz.qlogo.cn/mmbiz/l8NgBpGRwrrb4ga9ALUQXusWQWEMU480UGDaLLef5oQvLbY7a85zE4MCCzEQJQNvkn3rmAtGzBrnkaYEF4gfZQ/0"
              alt="" class="appmsg_thumb" data-pinit="registered">
            </div>
            <p class="appmsg_desc">
              水电费地方水电费水电费水电费
            </p>
          </div>
          <div class="appmsg_opr">
            <ul>
              <li class="appmsg_opr_item with2">
                <a class="js_edit" href="/cgi-bin/appmsg?t=media/appmsg_edit&amp;action=edit&amp;lang=zh_CN&amp;token=276678508&amp;type=10&amp;appmsgid=200027427&amp;isMul=0">
                  <i class="icon18_common edit_gray">
                    编辑
                  </i>
                </a>
              </li>
              <li class="appmsg_opr_item with2 no_extra">
                <a class="js_del no_extra" data-id="200027427" href="javascript:void(0);">
                  <i class="icon18_common del_gray">
                    删除
                  </i>
                </a>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  </div> -->
  
</div>
  <!-- end -->

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn btn-success" id="BtnAddSingle" Text="添加单图文回复" runat="server" />
    <asp:Button class="btn btn-success" id="BtnAddMultiple" Text="添加多图文回复" runat="server" />
  </ul>

</form>
</body>
</html>
