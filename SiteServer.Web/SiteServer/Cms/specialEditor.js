var $api = new apiUtils.Api(apiUrl + '/pages/cms/specialEditor');

var data = {
  siteId: parseInt(pageUtils.getQueryString('siteId')),
  specialId: parseInt(pageUtils.getQueryString('specialId')),
  pageLoad: false,
  pageAlert: null,
  items: null,
  specialInfo: null,
  specialUrl: null,
  headHtml: null,
  bodyHtml: null
};

var methods = {
  clear: function(html) {
    var re = /<script\b[^>]*>([\s\S]*?)<\/script>/gm;

    var match;
    while (match = re.exec(html)) {
      html = html.replace(match[0], '');
    }

    return html;
  },

  getStyleSheets: function(html) {
    var list = [];
    var pattern = /<link.+?href=\"(.+\.css)\".*?>/;
    var match;
    while (match = pattern.exec(html)) {
      var href = this.specialUrl + '/' + match[1];
      list.push(href);
      html = html.replace(match[0], '');
    }
    return list;
  },

  getConfig: function () {
    var $this = this;

    $api.get({
      siteId: this.siteId,
      specialId: this.specialId
    }, function (err, res) {
      if (err || !res || !res.value) return;

      $this.specialInfo = res.value;
      $this.specialUrl = res.specialUrl;

      if (res.html) {
        var pattern = /<body[^>]*>((.|[\n\r])*)<\/body>/im;
        $this.bodyHtml = $this.clear($this.clear($this.clear($this.clear(pattern.exec(res.html)[1]))));

        var links = $this.getStyleSheets(res.html);
        var h = document.getElementsByTagName('head').item(0);
        for(var i = 0; i < links.length; i++) {
          var el = document.createElement('link');
          el.setAttribute('rel', 'stylesheet');
          el.setAttribute('type', 'text/css');
          el.setAttribute('href', links[i]);
          h.appendChild(el);
        }
      }

      $this.pageLoad = true;
      setTimeout(function () {
        $this.load();
      }, 100);
    });
  },

  load: function() {
    var $this = this;

    setTimeout(function () {
      var editor = UE.getEditor('content', {
        allowDivTransToP: false,
        maximumWords: 99999999
      });
      editor.ready(function () {
        editor.addListener("contentChange", function () {
          $this.bodyHtml = this.getContent();
        });
      });
    }, 100);
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getConfig();
  }
});