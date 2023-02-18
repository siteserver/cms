var $url = '/write/editor';
var $urlUpdate = $url + '/actions/update';
var $urlPreview = $url + '/actions/preview';

Date.prototype.Format = function (fmt) {
  var o = {
    "M+": this.getMonth() + 1,                   // 月份
    "d+": this.getDate(),                        // 日
    "h+": this.getHours(),                       // 时
    "m+": this.getMinutes(),                     // 分
    "s+": this.getSeconds(),                     // 秒
    "q+": Math.floor((this.getMonth() + 3) / 3), // 季度
    "S": this.getMilliseconds()                  // 毫秒
  };

  if (/(y+)/.test(fmt))
    fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));

  for (var k in o)
    if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));

  return fmt;
};

var data = utils.init({
  pageType: null,
  siteId: utils.getQueryInt('siteId'),
  channelId: utils.getQueryInt('channelId'),
  contentId: utils.getQueryInt('contentId'),
  page: utils.getQueryInt('page'),
  tabName: utils.getQueryString('tabName'),
  mainHeight: '',
  sideType: 'first',
  collapseSettings: ['checkedLevel', 'addDate'],
  isSettings: true,

  site: null,
  siteUrl: null,
  channel: null,
  groupNames: null,
  tagNames: null,
  checkedLevels: null,
  siteOptions: null,
  channelOptions: null,
  styles: null,
  relatedFields: null,
  settings: null,
  form: null,
  isPreviewSaving: false
});

var methods = {
  apiGet: function() {
    var $this = this;

    window.onresize = $this.winResize;
    window.onresize();

    $api.get($url, {
      params: {
        siteId: $this.siteId,
        channelId: $this.channelId,
        contentId: $this.contentId
      }
    })
    .then(function(response) {
      var res = response.data;

      if (res.unauthorized) {
        $this.pageType = 'Unauthorized';
        utils.loading($this, false);
        return;
      }

      $this.site = res.site;
      $this.siteUrl = res.siteUrl;
      $this.channel = res.channel;
      $this.groupNames = res.groupNames;
      $this.tagNames = res.tagNames;
      $this.checkedLevels = res.checkedLevels;

      $this.siteOptions = res.siteOptions;
      $this.channelOptions = res.channelOptions;

      $this.styles = res.styles;
      $this.relatedFields = res.relatedFields;
      $this.settings = res.settings;
      $this.form = _.assign({}, res.content);

      if (!$this.form.addDate) {
        $this.form.addDate = new Date().Format("yyyy-MM-dd hh:mm:ss");
      } else {
        $this.form.addDate = new Date($this.form.addDate).Format("yyyy-MM-dd hh:mm:ss");
      }

      if ($this.form.id === 0) {
        $this.form.checkedLevel = -99;
      }
      if ($this.form.top || $this.form.recommend || $this.form.hot || $this.form.color) {
        $this.collapseSettings.push('attributes');
      }
      if ($this.form.groupNames && $this.form.groupNames.length > 0) {
        $this.collapseSettings.push('groupNames');
      } else {
        $this.form.groupNames = [];
      }
      if ($this.form.tagNames && $this.form.tagNames.length > 0) {
        $this.collapseSettings.push('tagNames');
      } else {
        $this.form.tagNames = [];
      }
      if ($this.form.linkUrl) {
        $this.collapseSettings.push('linkUrl');
      }

      for (var i = 0; i < $this.styles.length; i++) {
        var style = $this.styles[i];
        if (style.inputType === 'CheckBox' || style.inputType === 'SelectMultiple') {
          var value = $this.form[utils.toCamelCase(style.attributeName)];
          if (!Array.isArray(value)) {
            if (!value) {
              $this.form[utils.toCamelCase(style.attributeName)] = [];
            } else {
              $this.form[utils.toCamelCase(style.attributeName)] = utils.toArray(value);
            }
          }
        } else if (style.inputType === 'Image' || style.inputType === 'File' || style.inputType === 'Video') {
          $this.form[utils.getCountName(style.attributeName)] = utils.toInt($this.form[utils.getCountName(style.attributeName)]);
        } else if (style.inputType === 'Text' || style.inputType === 'TextArea' || style.inputType === 'TextEditor') {
          if ($this.contentId === 0) {
            $this.form[utils.toCamelCase(style.attributeName)] = style.defaultValue;
          }
        }
      }

      setTimeout(function () {
        for (var i = 0; i < $this.styles.length; i++) {
          var style = $this.styles[i];
          if (style.inputType === 'TextEditor') {
            var editor = utils.getEditor(style.attributeName);
            editor.styleIndex = i;
            editor.ready(function () {
              this.addListener("contentChange", function () {
                var style = $this.styles[this.styleIndex];
                $this.form[utils.toCamelCase(style.attributeName)] = this.getContent();
              });
            });
          }
        }
      }, 100);
    })
    .catch(function(error) {
      utils.error(error);
    })
    .then(function() {
      utils.loading($this, false);
    });
  },

  apiInsert: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      siteId: this.siteId,
      channelId: this.channelId,
      contentId: this.contentId,
      content: this.form
    }).then(function(response) {
      var res = response.data;

      $this.closeAndRedirect();
    })
    .catch(function(error) {
      utils.error(error);
    })
    .then(function() {
      utils.loading($this, false);
    });
  },

  apiUpdate: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlUpdate, {
      siteId: this.siteId,
      channelId: this.channelId,
      contentId: this.contentId,
      content: this.form
    }).then(function(response) {
      var res = response.data;

      $this.closeAndRedirect();
    })
    .catch(function(error) {
      utils.error(error);
    })
    .then(function() {
      utils.loading($this, false);
    });
  },

  apiPreview: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlPreview, {
      siteId: this.siteId,
      channelId: this.channelId,
      contentId: this.contentId,
      content: this.form
    }).then(function(response) {
      var res = response.data;

      $this.isPreviewSaving = false;
      window.open(res.url);
    }).catch(function(error) {
      utils.error(error);
    }).then(function() {
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

  runFormLayerVideoUpload: function(attributeName, no, text, coverUrl) {
    this.insertText(attributeName, no, text);
    if (coverUrl) {
      this.runFormLayerImageUploadText("ImageUrl", no, coverUrl);
    }
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

  updateGroups: function(res, message) {
    this.groupNames = res.groupNames;
    utils.success(message);
  },

  closeAndRedirect: function(isEdit) {
    utils.success('内容保存成功！');
    if (this.tabName) {
      var tabVue = utils.getTabVue(this.tabName);
      if (tabVue) {
        tabVue.apiList(this.page);
      }
      utils.removeTab();
      utils.openTab(this.tabName);
    } else {
      utils.removeTab();
      utils.addTab('稿件管理', "/home/write/contents/");
    }
  },

  winResize: function () {
    this.mainHeight = ($(window).height() - 70) + 'px';
  },

  btnImageSelectClick: function(args) {
    var inputType = args.inputType;
    var attributeName = args.attributeName;
    var no = args.no;
    var type = args.type;

    if (type === 'uploadedImages') {
      this.btnLayerClick({
        title: '选择已上传图片',
        name: 'formLayerImageSelect',
        inputType: inputType,
        attributeName: attributeName,
        no: no,
        full: true
      });
    } else if (type === 'materialImages') {
      this.btnLayerClick({
        title: '选择素材库图片',
        name: 'materialLayerImageSelect',
        inputType: inputType,
        attributeName: attributeName,
        no: no,
        full: true
      });
    } else if (type === 'cloudImages') {
      utils.openLayer({
        title: '选择免版权图库',
        url: utils.getCloudsUrl('layerImagesSelect', {
          inputType: inputType,
          attributeName: args.attributeName,
          no: args.no,
        }),
      });
    }
  },

  btnLayerClick: function(options) {
    var query = {
      siteId: this.siteId,
      channelId: this.channelId,
      editorAttributeName: "Body",
    };

    if (options.attributeName) {
      query.attributeName = options.attributeName;
    }
    if (options.inputType) {
      query.inputType = options.inputType;
    }
    if (options.contentId) {
      query.contentId = options.contentId;
    }
    if (options.no) {
      query.no = options.no;
    }

    var args = {
      title: options.title,
      url: utils.getCommonUrl(options.name, query),
    };
    if (!options.full) {
      args.width = options.width ? options.width : 750;
      args.height = options.height ? options.height : 550;
    }

    utils.openLayer(args);
  },

  btnSaveClick: function() {
    if (UE) {
      $.each(UE.instants, function (index, editor) {
        editor.sync();
      });
    }

    var $this = this;
    this.$refs.form.validate(function (valid) {
      if (valid) {
        if ($this.contentId === 0) {
          $this.apiInsert();
        } else {
          $this.apiUpdate();
        }
      }
    });
  },

  syncEditors: function () {
    var $this = this;
    if (UE) {
      $.each(UE.instants, function (index, editor) {
        editor.sync();
        var style = $this.styles[editor.styleIndex];
        $this.form[utils.toCamelCase(style.attributeName)] = editor.getContent();
      });
    }
  },

  btnPreviewClick: function() {
    var $this = this;
    if (this.isPreviewSaving) return;
    this.syncEditors();
    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiPreview();
      } else {
        utils.error('预览失败，请检查表单值是否正确');
      }
    });
  },

  btnCloseClick: function() {
    utils.removeTab();
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
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function() {
    this.apiGet();
  }
});
