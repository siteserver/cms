<%@ Page Language="C#" Trace="false" EnableViewState="false" Inherits="SiteServer.BackgroundPages.Cms.PageTemplateLeft" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html style="background-color: #fff;">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
      <style>
        .scroll {
          overflow-x: scroll !important
        }

        .list-group {
          margin-bottom: 0;
        }

        .table>tbody>tr>td,
        .table>tbody>tr>th,
        .table>tfoot>tr>td,
        .table>tfoot>tr>th,
        .table>thead>tr>td,
        .table>thead>tr>th {
          border-top: none;
        }

        .table a,
        .table span,
        .table div {
          font-size: 12px !important;
        }
      </style>
      <script type="text/javascript">
        $(document).ready(function () {
          $('body').height($(window).height());
          $('body').addClass('scroll');
        });
      </script>
    </head>

    <body style="margin: 0; padding: 0; background-color: #fff;">
      <form class="m-0" runat="server">
        <div class="list-group mail-list">
          <div onclick="location.reload(true);" style="cursor: pointer;" class="list-group-item b-0 active">
            模板列表
          </div>
        </div>
        <table class="table table-sm table-hover">
          <tbody>
            <tr treeitemlevel="1">
              <td align="left" nowrap>
                <img align="absmiddle" style="cursor: pointer" onclick="displayChildren(this);" isajax="false" isopen="true" src="../assets/icons/tree/minus.png"
                />
                <img align="absmiddle" border="0" src="../assets/Icons/tree/folder.gif" />
                <a href='pageTemplate.aspx?SiteID=<%=base.Request.QueryString["SiteID"]%>' islink='true' onclick='fontWeightLink(this)'
                  target='management'>所有模板</a>
                </a>
                <span style="font-size: 8pt; font-family: arial" class="gray">
                  <asp:Literal id="LtlTotalCount" runat="server" />
                </span>
              </td>
            </tr>

            <tr treeitemlevel="2">
              <td align="left" nowrap>
                <img align="absmiddle" src="../assets/icons/tree/empty.gif" />
                <img align="absmiddle" style="cursor: pointer" onclick="displayChildren(this);" isajax="true" isopen="false" templatetype="IndexPageTemplate"
                  src="../assets/icons/tree/plus.png" />
                <img align="absmiddle" border="0" src="../assets/icons/tree/folder.gif" />
                <a href='pageTemplate.aspx?SiteID=<%=base.Request.QueryString["SiteID"]%>&TemplateType=IndexPageTemplate'
                  islink='true' onclick='fontWeightLink(this)' target='management'>首页模板</a>
                <span style="font-size: 8pt; font-family: arial" class="gray">
                  <asp:Literal id="LtlIndexPageCount" runat="server" />
                </span>
              </td>
            </tr>

            <tr treeitemlevel="2">
              <td align="left" nowrap>
                <img align="absmiddle" src="../assets/icons/tree/empty.gif" />
                <img align="absmiddle" style="cursor: pointer" onclick="displayChildren(this);" isajax="true" isopen="false" templatetype="ChannelTemplate"
                  src="../assets/icons/tree/plus.png" />
                <img align="absmiddle" border="0" src="../assets/icons/tree/folder.gif" />
                <a href='pageTemplate.aspx?SiteID=<%=base.Request.QueryString["SiteID"]%>&TemplateType=ChannelTemplate'
                  islink='true' onclick='fontWeightLink(this)' target='management'>栏目模板</a>
                <span style="font-size: 8pt; font-family: arial" class="gray">
                  <asp:Literal id="LtlChannelCount" runat="server" />
                </span>
              </td>
            </tr>

            <tr treeitemlevel="2">
              <td align="left" nowrap>
                <img align="absmiddle" src="../assets/icons/tree/empty.gif" />
                <img align="absmiddle" style="cursor: pointer" onclick="displayChildren(this);" isajax="true" isopen="false" templatetype="ContentTemplate"
                  src="../assets/icons/tree/plus.png" />
                <img align="absmiddle" border="0" src="../assets/icons/tree/folder.gif" />
                <a href='pageTemplate.aspx?SiteID=<%=base.Request.QueryString["SiteID"]%>&TemplateType=ContentTemplate'
                  islink='true' onclick='fontWeightLink(this)' target='management'>内容模板</a>
                <span style="font-size: 8pt; font-family: arial" class="gray">
                  <asp:Literal id="LtlContentCount" runat="server" />
                </span>
              </td>
            </tr>

            <tr treeitemlevel="2">
              <td align="left" nowrap>
                <img align="absmiddle" src="../assets/icons/tree/empty.gif" />
                <img align="absmiddle" style="cursor: pointer" onclick="displayChildren(this);" isajax="true" isopen="false" templatetype="FileTemplate"
                  src="../assets/icons/tree/plus.png" />
                <img align="absmiddle" border="0" src="../assets/icons/tree/folder.gif" />
                <a href='pageTemplate.aspx?SiteID=<%=base.Request.QueryString["SiteID"]%>&TemplateType=FileTemplate'
                  islink='true' onclick='fontWeightLink(this)' target='management'>单页模板</a>
                <span style="font-size: 8pt; font-family: arial" class="gray">
                  <asp:Literal id="LtlFileCount" runat="server" />
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </form>
    </body>

    </html>
    <script language="JavaScript">
      function getTreeLevel(e) {
        var length = 0;
        if (!isNull(e)) {
          if (e.tagName == 'TR') {
            length = parseInt(e.getAttribute('treeItemLevel'));
          }
        }
        return length;
      }

      function getTrElement(element) {
        if (isNull(element)) return;
        for (element = element.parentNode;;) {
          if (element != null && element.tagName == 'TR') {
            break;
          } else {
            element = element.parentNode;
          }
        }
        return element;
      }

      function getImgClickableElementByTr(element) {
        if (isNull(element) || element.tagName != 'TR') return;
        var img = null;
        if (!isNull(element.childNodes)) {
          var imgCol = element.getElementsByTagName('IMG');
          if (!isNull(imgCol)) {
            for (x = 0; x < imgCol.length; x++) {
              if (!isNull(imgCol.item(x).getAttribute('isOpen'))) {
                img = imgCol.item(x);
                break;
              }
            }
          }
        }
        return img;
      }

      var weightedLink = null;

      function fontWeightLink(element) {
        if (weightedLink != null) {
          weightedLink.style.fontWeight = 'normal';
        }
        element.style.fontWeight = 'bold';
        weightedLink = element;
      }

      function displayChildren(img) {
        if (isNull(img)) return;

        var tr = getTrElement(img);

        var isToOpen = img.getAttribute('isOpen') == 'false';
        var isByAjax = img.getAttribute('isAjax') == 'true';
        var templateType = img.getAttribute('templateType');

        if (!isNull(img) && img.getAttribute('isOpen') != null) {
          if (img.getAttribute('isOpen') == 'false') {
            img.setAttribute('isOpen', 'true');
            img.setAttribute('src', '../assets/icons/tree/minus.png');
          } else {
            img.setAttribute('isOpen', 'false');
            img.setAttribute('src', '../assets/icons/tree/plus.png');
          }
        }

        if (isToOpen && isByAjax) {
          var div = document.createElement('div');
          div.innerHTML = "<img align='absmiddle' border='0' src='../assets/icons/loading.gif' /> 栏目加载中，请稍候...";
          img.parentNode.appendChild(div);
          $(div).addClass('loading');
          loadingChannels(tr, img, div, templateType);
        } else {
          var level = getTreeLevel(tr);

          var collection = new Array();
          var index = 0;

          for (var e = tr.nextSibling; !isNull(e); e = e.nextSibling) {
            if (!isNull(e) && !isNull(e.tagName) && e.tagName == 'TR') {
              var currentLevel = getTreeLevel(e);
              if (currentLevel <= level) break;
              if (e.style.display == '') {
                e.style.display = 'none';
              } else {
                if (currentLevel != level + 1) continue;
                e.style.display = '';
                var imgClickable = getImgClickableElementByTr(e);
                if (!isNull(imgClickable)) {
                  if (!isNull(imgClickable.getAttribute('isOpen')) && imgClickable.getAttribute('isOpen') == 'true') {
                    imgClickable.setAttribute('isOpen', 'false');
                    imgClickable.setAttribute('src', '../assets/icons/tree/plus.png');
                    collection[index] = imgClickable;
                    index++;
                  }
                }
              }
            }
          }

          if (index > 0) {
            for (i = 0; i <= index; i++) {
              displayChildren(collection[i]);
            }
          }
        }
      }

      function loadingChannels(tr, img, div, templateType) {
        var url = '<%=GetServiceUrl()%>';
        var pars = '<%=GetServiceParams()%>' + templateType;

        jQuery.post(url, pars, function (data, textStatus) {
          $($.parseHTML(data)).insertAfter($(tr));
          img.setAttribute('isAjax', 'false');
          img.parentNode.removeChild(div);
        });
      }
    </script>
    <!--#include file="../inc/foot.html"-->