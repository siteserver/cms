var $url = '/write/editor';

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
  form: null,
  isPreviewSaving: false
});

var methods = {
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
    var count = this.form[utils.getCountName(attributeName)];
    if (count && count < no) {
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

      $this.loadEditor(res);
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
    $api.put($url, {
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

  closeAndRedirect: function(isEdit) {
    var tabVue = utils.getTabVue(this.tabName);
    if (tabVue) {
      utils.success('内容保存成功！');
      tabVue.apiList(this.page);
    }
    utils.removeTab();
    utils.openTab(this.tabName);
  },

  loadEditor: function(res) {
    this.site = res.site;
    this.siteUrl = res.siteUrl;
    this.channel = res.channel;
    this.groupNames = res.groupNames;
    this.tagNames = res.tagNames;
    this.checkedLevels = res.checkedLevels;
    
    this.siteOptions = res.siteOptions;
    this.channelOptions = res.channelOptions;
    // this.styles = [];
    // this.content = res.content;
    // for (let i = 0; i < res.styles.length; i++) {
    //   var style = res.styles[i];
    //   if (this.contentId) {
    //     style.value = this.content[_.camelCase(style.attributeName)];
    //   } else {
    //     style.value = style.defaultValue || '';
    //   }
    //   this.styles.push(style);
    // }

    this.styles = res.styles;
    this.form = _.assign({}, res.content);
    
    if (this.form.id === 0) {
      this.form.checkedLevel = -99;
    }
    if (this.form.top || this.form.recommend || this.form.hot || this.form.color) {
      this.collapseSettings.push('attributes');
    }
    if (this.form.groupNames && this.form.groupNames.length > 0) {
      this.collapseSettings.push('groupNames');
    } else {
      this.form.groupNames = [];
    }
    if (this.form.tagNames && this.form.tagNames.length > 0) {
      this.collapseSettings.push('tagNames');
    } else {
      this.form.tagNames = [];
    }
    if (this.form.linkUrl) {
      this.collapseSettings.push('linkUrl');
    }

    for (var i = 0; i < this.styles.length; i++) {
      var style = this.styles[i];
      if (style.inputType !== 'Image' && style.inputType !== 'File' && style.inputType !== 'Video') continue;
      
      var count = this.form[utils.getCountName(style.attributeName)];
      if (!count){
        this.form[utils.getCountName(style.attributeName)] = 0;
      }
    }

    var $this = this;
    setTimeout(function () {
      for (var i = 0; i < $this.styles.length; i++) {
        var style = $this.styles[i];
        if (style.inputType === 'TextEditor') {
          // var editor = new FroalaEditor('textarea#' + style.attributeName, {
          //   language: 'zh_cn',
          //   heightMin: 350
          // });
          var editor = utils.getEditor(style.attributeName);
          editor.styleIndex = i;
          editor.ready(function () {
            this.addListener("contentChange", function () {
              var style = $this.styles[this.styleIndex];
              $this.form[style.attributeName] = this.getContent();
            });
          });
        }
      }
    }, 100);
  },

  winResize: function () {
    this.mainHeight = ($(window).height() - 70) + 'px';
  },

  btnLayerClick: function(options) {
    var query = {
      siteId: this.siteId,
      channelId: this.channelId
    };

    if (options.attributeName) {
      query.attributeName = options.attributeName;
    }
    if (options.contentId) {
      query.contentId = options.contentId;
    }
    if (options.no) {
      query.no = options.no;
    }

    utils.openLayer({
      title: options.title,
      url: utils.getCommonUrl(options.name, query),
      full: options.full,
      width: options.width ? options.width : 700,
      height: options.height ? options.height : 500
    });
  },

  btnSaveClick: function() {
    if (UE) {
      $.each(UE.instants, function (index, editor) {
        editor.sync();
      });
    }

    if (this.contentId === 0) {
      this.apiInsert();
    } else {
      this.apiUpdate();
    }
  },

  btnPreviewClick: function() {
    if (!this.styles[0].value) return;
    if (this.isPreviewSaving) return;

    if (UE) {
      $.each(UE.instants, function (index, editor) {
        editor.sync();
      });
    }

    var $this = this;
    utils.loading(this, true);
    $api.post($url + '/actions/preview', {
      siteId: this.siteId,
      channelId: this.channelId,
      contentId: this.contentId,
      content: this.form
    }).then(function(response) {
      var res = response.data;

      $this.isPreviewSaving = false;
      window.open(res.url);
    })
    .catch(function(error) {
      utils.error(error);
    })
    .then(function() {
      utils.loading($this, false);
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
