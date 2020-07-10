var $url = '/cms/settings/settingsSite';

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  pageType: null,
  form: null,
  styles: null,
  files: []
});

var methods = {
  insertEditor: function(attributeName, html)
  {
    if (html)
    {
      UE.getEditor(attributeName, {allowDivTransToP: false, maximumWords:99999999}).execCommand('insertHTML', html);
    }
  },

  insertText: function(attributeName, no, text) {
    var count = this.form[utils.getCountName(attributeName)];
    if (count < no) {
      this.form[utils.getCountName(attributeName)] = no;
    }
    this.form[utils.getExtendName(attributeName, no)] = text;
    this.form = _.assign({}, this.form);
  },
  
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.loadEditor(res);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSiteStylesClick: function() {
    location.href = utils.getCmsUrl('settingsStyleSite', {
      siteId: this.siteId,
      returnUrl: location.href
    });
  },

  loadEditor: function(res) {
    // var values = {
    //   siteName: res.siteName,
    //   pageSize: res.pageSize,
    //   isCreateDoubleClick: res.isCreateDoubleClick,
    // };
    // this.styles = res.styles;
    // for(var i = 0; i < res.styles.length; i++) {
    //   var style = res.styles[i];
    //   values[style.attributeName] = res[style.attributeName];
    //   if (style.inputType === 'Image' || style.inputType === 'Video' || style.inputType === 'File') {
    //     var count = utils.toInt(res[style.attributeName + 'Count']);
        
    //   }
    // }
    
    this.styles = res.styles;
    this.form = _.assign({}, res.site);

    var $this = this;
    setTimeout(function () {
      for (var i = 0; i < $this.styles.length; i++) {
        var style = $this.styles[i];
        if (style.inputType === 'TextEditor') {
          var editor = UE.getEditor(style.attributeName, {
            allowDivTransToP: false,
            maximumWords: 99999999
          });
          editor.attributeName = style.attributeName;
          editor.ready(function () {
            editor.addListener("contentChange", function () {
              $this.form[this.attributeName] = this.getContent();
            });
          });
        }
      }

    }, 100);
  },

  btnExtendAddClick: function(style) {
    var no = this.form[utils.getCountName(style.attributeName)] + 1;
    this.form[utils.getCountName(style.attributeName)] = no;
    this.form[utils.getExtendName(style.attributeName, no)] = '';
  },

  btnExtendRemoveClick: function(style) {
    var no = this.form[utils.getCountName(style.attributeName)] - 1;
    this.form[utils.getCountName(style.attributeName)] = no;
    this.form[utils.getExtendName(style.attributeName, no)] = '';
  },

  btnLayerClick: function(options) {
    var query = {
      siteId: this.siteId,
      attributeName: options.attributeName
    };
    if (options.no) {
      query.no = options.no;
    }

    utils.openLayer({
      title: options.title,
      url: utils.getSharedUrl(options.name, query),
      width: options.full ? 0 : 700,
      height: options.full ? 0 : 500
    });
  },

  btnPreviewClick: function(attributeName, n) {
    var imageUrl = n ? this.form[utils.getExtendName(attributeName, n)] : this.form[attributeName];
    window.open(imageUrl);
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, _.assign({
      siteId: this.siteId
    }, this.form)).then(function (response) {
      var res = response.data;

      utils.success('站点设置保存成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});