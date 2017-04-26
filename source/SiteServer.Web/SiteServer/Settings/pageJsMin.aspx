<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageJsMin" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<!--#include file="../inc/header.aspx"-->
</head>

<body>
<!--#include file="../inc/openWindow.html"-->
<script>
String.prototype.has=function(c){return this.indexOf(c)>-1;};function jsmin(comment,input,level){if(input===undefined){input=comment;comment='';level=2;}else if(level===undefined||level<1||level>3){level=2;}if(comment.length>0){comment+='\n';}var a='',b='',EOF=-1,LETTERS='ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz',DIGITS='0123456789',ALNUM=LETTERS+DIGITS+'_$\\',theLookahead=EOF;function isAlphanum(c){return c!=EOF&&(ALNUM.has(c)||c.charCodeAt(0)>126);}function get(){var c=theLookahead;if(get.i==get.l){return EOF;}theLookahead=EOF;if(c==EOF){c=input.charAt(get.i);++get.i;}if(c>=' '||c=='\n'){return c;}if(c=='\r'){return'\n';}return' ';}get.i=0;get.l=input.length;function peek(){theLookahead=get();return theLookahead;}function next(){var c=get();if(c=='/'){switch(peek()){case'/':for(;;){c=get();if(c<='\n'){return c;}}break;case'*':get();for(;;){switch(get()){case'*':if(peek()=='/'){get();return' ';}break;case EOF:throw'Error: Unterminated comment.';}}break;default:return c;}}return c;}function action(d){var r=[];if(d==1){r.push(a);}if(d<3){a=b;if(a=='\''||a=='"'){for(;;){r.push(a);a=get();if(a==b){break;}if(a<='\n'){throw'Error: unterminated string literal: '+a;}if(a=='\\'){r.push(a);a=get();}}}}b=next();if(b=='/'&&'(,=:[!&|'.has(a)){r.push(a);r.push(b);for(;;){a=get();if(a=='/'){break;}else if(a=='\\'){r.push(a);a=get();}else if(a<='\n'){throw'Error: unterminated Regular Expression literal';}r.push(a);}b=next();}return r.join('');}function m(){var r=[];a='\n';r.push(action(3));while(a!=EOF){switch(a){case' ':if(isAlphanum(b)){r.push(action(1));}else{r.push(action(2));}break;case'\n':switch(b){case'{':case'[':case'(':case'+':case'-':r.push(action(1));break;case' ':r.push(action(3));break;default:if(isAlphanum(b)){r.push(action(1));}else{if(level==1&&b!='\n'){r.push(action(1));}else{r.push(action(2));}}}break;default:switch(b){case' ':if(isAlphanum(a)){r.push(action(1));break;}r.push(action(3));break;case'\n':if(level==1&&a!='\n'){r.push(action(1));}else{switch(a){case'}':case']':case')':case'+':case'-':case'"':case'\'':if(level==3){r.push(action(3));}else{r.push(action(1));}break;default:if(isAlphanum(a)){r.push(action(1));}else{r.push(action(3));}}}break;default:r.push(action(1));break;}}}return r.join('');}jsmin.oldSize=input.length;ret=m(input);jsmin.newSize=ret.length;return comment+ret;}
</script>
<script type="text/javascript">
function go() {
  document.getElementById('output').value = jsmin('', document.getElementById('input').value, document.getElementById('level').value);
  document.getElementById('output').style.display = document.getElementById('stats').style.display = 'block';
  $('#oldsize').html(jsmin.oldSize);
  $('#newsize').html(jsmin.newSize);
  $('#ratio').html((Math.round(jsmin.newSize / jsmin.oldSize * 1000) / 10) + '%');
}
</script>
<style type="text/css">
#comment {
  width:95%;
  height:4em;
}
#input {
  width:95%;
  height:10em;
}
#go {
  font-weight:bold;
}
#outputtitle, #statstitle, #stats {
  display:none;
}
#oldsize, #newsize, #ratio {
  text-align:right;
  width:4em;
}
#output {
  width:95%;
  height:10em;
  display:none;
}
h2 {
  margin-bottom:0;
}
</style>
<form class="form-inline" runat="server">
  <asp:Literal id="ltlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />

  <div class="popover popover-static">
  <h3 class="popover-title">JS脚本压缩</h3>
  <div class="popover-content">

    <table class="table noborder table-hover">
      <tr>
        <td width="160">需要压缩的代码：</td>
        <td><textarea name="input" rows="10" id="input"></textarea></td>
      </tr>
      <tr>
        <td>压缩级别：</td>
        <td>
          <select id="level" class="input-medium">
            <option value="1">最小压缩</option>
            <option value="2" selected="selected">普通压缩</option>
            <option value="3">超级压缩</option>
          </select>
        </td>
      </tr>
      <tr>
        <td>压缩后代码：</td>
        <td><textarea name="output" rows="10" id="output"></textarea></td>
      </tr>
      <tr>
        <td colspan="2" class="center">
          <p id="stats" class="lead">
            原来大小:
            <span id="oldsize" class="lead"></span>
            压缩后大小:
            <span id="newsize" class="lead"></span>
            压缩率:
            <span id="ratio" class="lead"></span>
          </p>
        </td>
      </tr>
    </table>

    <hr />
    <table class="table noborder">
      <tr>
        <td class="center">
          <input type="button" class="btn btn-primary" value="JS脚本压缩" onClick="go();return false;"/>
        </td>
      </tr>
    </table>

    </div>
  </div>

</form>
</body>
</html>
