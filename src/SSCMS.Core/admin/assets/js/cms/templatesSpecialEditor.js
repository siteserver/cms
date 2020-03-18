var $url = '/admin/cms/templates/templatesSpecialEditor';

var data = utils.initData({
  siteId: utils.getQueryInt('siteId'),
  specialId: utils.getQueryInt('specialId'),
  items: null,
  specialInfo: null,
  specialUrl: null,
  headHtml: null,
  bodyHtml: null
});

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

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId,
        specialId: this.specialId
      }
    }).then(function (response) {
      var res = response.data;

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

      utils.loading($this, false);
      setTimeout(function () {
        $this.load();
      }, 100);
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  load: function() {
    new FroalaEditor('textarea');
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getConfig();
  }
});