var $url = '/cms/settings/settingsSite';

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  siteUrl: null,
  pageType: null,
  form: null,
  styles: null,
  relatedFields: null,
  settings: null,
  files: []
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.siteUrl = res.siteUrl;
      $this.styles = res.styles;
      $this.relatedFields = res.relatedFields;
      $this.form = res.entity;
      $this.settings = res.settings;

      $this.loadEditor(res);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
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

  runFormLayerImageUploadText: function(attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  runFormLayerImageUploadEditor: function(attributeName, html) {
    this.insertEditor(attributeName, html);
  },

  runMaterialLayerImageSelect: function(attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  runFormLayerFileUpload: function(attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  runMaterialLayerFileSelect: function(attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  runFormLayerVideoUpload: function(attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  runMaterialLayerVideoSelect: function(attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  runEditorLayerImage: function(attributeName, html) {
    this.insertEditor(attributeName, html);
  },

  insertText: function(attributeName, no, text) {
    var count = this.form[utils.getCountName(attributeName)] || 0;
    if (count <= no) {
      this.form[utils.getCountName(attributeName)] = no;
    }
    this.form[utils.getExtendName(attributeName, no)] = text;
    this.form = _.assign({}, this.form);
  },

  insertEditor: function(attributeName, html) {
    if (!attributeName) attributeName = 'Body';
    if (!html) return;
    utils.getEditor(attributeName).execCommand('insertHTML', html);
  },

  btnImageSelectClick: function(args) {
    var attributeName = args.attributeName;
    var no = args.no;
    var type = args.type;

    if (type === 'uploadedImages') {
      this.btnLayerClick({
        title: '选择已上传图片',
        name: 'formLayerImageSelect',
        attributeName: attributeName,
        no: no,
        full: true
      });
    } else if (type === 'materialImages') {
      this.btnLayerClick({
        title: '选择素材库图片',
        name: 'materialLayerImageSelect',
        attributeName: attributeName,
        no: no,
        full: true
      });
    } else if (type === 'cloudImages') {
      utils.openLayer({
        title: '选择免版权图库',
        url: utils.getCloudsUrl('layerImagesSelect', {
          attributeName: args.attributeName,
          no: args.no,
        }),
      });
    }
  },

  btnSiteStylesClick: function() {
    location.href = utils.getCmsUrl('settingsStyleSite', {
      siteId: this.siteId,
      returnUrl: location.href
    });
  },

  loadEditor: function(res) {
    var $this = this;

    setTimeout(function () {
      for (var i = 0; i < $this.styles.length; i++) {
        var style = $this.styles[i];
        var attributeName = utils.toCamelCase(style.attributeName);
        if (style.inputType === 'TextEditor') {
          var editor = utils.getEditor(style.attributeName);
          editor.attributeName = style.attributeName;
          editor.ready(function () {
            this.addListener("contentChange", function () {
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
    this.form = _.assign({}, this.form);
  },

  btnExtendRemoveClick: function(style) {
    var no = this.form[utils.getCountName(style.attributeName)];
    this.form[utils.getCountName(style.attributeName)] = no - 1;
    this.form[utils.getExtendName(style.attributeName, no)] = '';
    this.form = _.assign({}, this.form);
  },

  btnExtendPreviewClick: function(attributeName, no) {
    var count = this.form[utils.getCountName(attributeName)];
    var data = [];
    for (var i = 0; i <= count; i++) {
      var imageUrl = this.form[utils.getExtendName(attributeName, i)];
      imageUrl = utils.getUrl(this.siteUrl, imageUrl);
      data.push({
        "src": imageUrl
      });
    }
    layer.photos({
      photos: {
        "start": no,
        "data": data
      }
      ,anim: 5
    });
  },

  btnLayerClick: function(options) {
    var query = {
      siteId: this.siteId,
      attributeName: options.attributeName
    };
    if (options.no) {
      query.no = options.no;
    }

    var args = {
      title: options.title,
      url: utils.getCommonUrl(options.name, query)
    };
    if (!options.full) {
      args.width = options.width ? options.width : 700;
      args.height = options.height ? options.height : 500;
    }
    utils.openLayer(args);
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  },

  btnCloseClick: function() {
    utils.removeTab();
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(this.btnSubmitClick, this.btnCloseClick);
    this.apiGet();
  }
});
