<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageUtilityJsMin" %>
<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>

<head>
  <meta charset="utf-8">
  <!--#include file="../inc/head.html"-->
  <script>
    String.prototype.has = function (c) {
      return this.indexOf(c) > -1;
    };

    function jsmin(comment, input, level) {
      if (input === undefined) {
        input = comment;
        comment = '';
        level = 2;
      } else if (level === undefined || level < 1 || level > 3) {
        level = 2;
      }
      if (comment.length > 0) {
        comment += '\n';
      }
      var a = '',
        b = '',
        EOF = -1,
        LETTERS = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz',
        DIGITS = '0123456789',
        ALNUM = LETTERS + DIGITS + '_$\\',
        theLookahead = EOF;

      function isAlphanum(c) {
        return c != EOF && (ALNUM.has(c) || c.charCodeAt(0) > 126);
      }

      function get() {
        var c = theLookahead;
        if (get.i == get.l) {
          return EOF;
        }
        theLookahead = EOF;
        if (c == EOF) {
          c = input.charAt(get.i);
          ++get.i;
        }
        if (c >= ' ' || c == '\n') {
          return c;
        }
        if (c == '\r') {
          return '\n';
        }
        return ' ';
      }
      get.i = 0;
      get.l = input.length;

      function peek() {
        theLookahead = get();
        return theLookahead;
      }

      function next() {
        var c = get();
        if (c == '/') {
          switch (peek()) {
            case '/':
              for (;;) {
                c = get();
                if (c <= '\n') {
                  return c;
                }
              }
              break;
            case '*':
              get();
              for (;;) {
                switch (get()) {
                  case '*':
                    if (peek() == '/') {
                      get();
                      return ' ';
                    }
                    break;
                  case EOF:
                    throw 'Error: Unterminated comment.';
                }
              }
              break;
            default:
              return c;
          }
        }
        return c;
      }

      function action(d) {
        var r = [];
        if (d == 1) {
          r.push(a);
        }
        if (d < 3) {
          a = b;
          if (a == '\'' || a == '"') {
            for (;;) {
              r.push(a);
              a = get();
              if (a == b) {
                break;
              }
              if (a <= '\n') {
                throw 'Error: unterminated string literal: ' + a;
              }
              if (a == '\\') {
                r.push(a);
                a = get();
              }
            }
          }
        }
        b = next();
        if (b == '/' && '(,=:[!&|'.has(a)) {
          r.push(a);
          r.push(b);
          for (;;) {
            a = get();
            if (a == '/') {
              break;
            } else if (a == '\\') {
              r.push(a);
              a = get();
            } else if (a <= '\n') {
              throw 'Error: unterminated Regular Expression literal';
            }
            r.push(a);
          }
          b = next();
        }
        return r.join('');
      }

      function m() {
        var r = [];
        a = '\n';
        r.push(action(3));
        while (a != EOF) {
          switch (a) {
            case ' ':
              if (isAlphanum(b)) {
                r.push(action(1));
              } else {
                r.push(action(2));
              }
              break;
            case '\n':
              switch (b) {
                case '{':
                case '[':
                case '(':
                case '+':
                case '-':
                  r.push(action(1));
                  break;
                case ' ':
                  r.push(action(3));
                  break;
                default:
                  if (isAlphanum(b)) {
                    r.push(action(1));
                  } else {
                    if (level == 1 && b != '\n') {
                      r.push(action(1));
                    } else {
                      r.push(action(2));
                    }
                  }
              }
              break;
            default:
              switch (b) {
                case ' ':
                  if (isAlphanum(a)) {
                    r.push(action(1));
                    break;
                  }
                  r.push(action(3));
                  break;
                case '\n':
                  if (level == 1 && a != '\n') {
                    r.push(action(1));
                  } else {
                    switch (a) {
                      case '}':
                      case ']':
                      case ')':
                      case '+':
                      case '-':
                      case '"':
                      case '\'':
                        if (level == 3) {
                          r.push(action(3));
                        } else {
                          r.push(action(1));
                        }
                        break;
                      default:
                        if (isAlphanum(a)) {
                          r.push(action(1));
                        } else {
                          r.push(action(3));
                        }
                    }
                  }
                  break;
                default:
                  r.push(action(1));
                  break;
              }
          }
        }
        return r.join('');
      }
      jsmin.oldSize = input.length;
      ret = m(input);
      jsmin.newSize = ret.length;
      return comment + ret;
    }
  </script>
  <script type="text/javascript">
    function go() {
      var val = jsmin('', document.getElementById('input').value, document.getElementById('level').value);
      document.getElementById('output').value = val.trim();
      document.getElementById('output').style.display = document.getElementById('stats').style.display = 'block';
      $('#oldsize').html(jsmin.oldSize);
      $('#newsize').html(jsmin.newSize);
      $('#ratio').html((Math.round(jsmin.newSize / jsmin.oldSize * 1000) / 10) + '%');
    }
  </script>
  <style type="text/css">
    #comment {
      width: 95%;
      height: 4em;
    }

    #input {
      width: 95%;
      height: 10em;
    }

    #go {
      font-weight: bold;
    }

    #outputtitle,
    #statstitle,
    #stats {
      display: none;
    }

    #ratio {
      text-align: right;
      width: 4em;
    }

    #output {
      width: 95%;
      height: 10em;
      display: none;
    }

    h2 {
      margin-bottom: 0;
    }
  </style>
</head>

<body>
  <form class="m-l-15 m-r-15" runat="server">

    <div class="card-box" style="padding: 10px; margin-bottom: 10px;">
      <ul class="nav nav-pills nav-justified">
        <li class="nav-item">
          <a class="nav-link" href="pageUtilityCache.aspx">系统缓存</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageUtilityParameter.aspx">系统参数查看</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageUtilityEncrypt.aspx">加密字符串</a>
        </li>
        <li class="nav-item active">
          <a class="nav-link" href="pageUtilityJsMin.aspx">JS脚本压缩</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageUtilityDbLogDelete.aspx">清空数据库日志</a>
        </li>
      </ul>
    </div>

    <ctrl:alerts runat="server" />

    <div class="card-box">
      <div class="form-group">
        <label class="col-form-label">需要压缩的代码</label>
        <textarea name="input" rows="10" id="input" class="form-control"></textarea>
      </div>

      <div class="form-group">
        <label class="col-form-label">压缩级别</label>
        <select id="level" class="form-control">
          <option value="1">最小压缩</option>
          <option value="2" selected="selected">普通压缩</option>
          <option value="3">超级压缩</option>
        </select>
      </div>

      <div class="form-group">
        <label class="col-form-label">压缩后代码</label>
        <textarea name="output" rows="10" id="output" class="form-control"></textarea>
        <small id="stats" class="form-text text-muted">
          原来大小:
          <span id="oldsize" class="badge badge-secondary"></span>
          压缩后大小:
          <span id="newsize" class="badge badge-secondary"></span>
          压缩率:
          <span id="ratio" class="badge badge-secondary"></span>
        </small>
      </div>

      <hr />

      <input type="button" class="btn btn-primary" value="JS脚本压缩" onClick="go();return false;" />

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->