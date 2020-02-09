var $url = "/pages/cms/editor/editor";

var data = utils.initData({
  page: utils.getQueryInt("page"),
  siteId: utils.getQueryInt("siteId"),
  channelId: utils.getQueryInt("channelId"),
  contentId: utils.getQueryInt("contentId"),
  mainHeight: '',
  isSettings: true,
  sideType: "first",
  collapseType: "0",

  site: null,
  channel: null,
  groupNames: null,
  tagNames: null,
  checkedLevels: null,
  siteOptions: null,
  channelOptions: null,
  styles: null,
  form: null,

  transForm: null,
  translations: [],
  isPreviewSaving: false
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

  getConfig: function() {
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

      $this.loadEditor(res);
    })
    .catch(function(error) {
      utils.error($this, error);
    })
    .then(function() {
      utils.loading($this, false);
    });
  },

  loadEditor: function(res) {
    this.site = res.site;
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

    var $this = this;
    setTimeout(function () {
      for (var i = 0; i < $this.styles.length; i++) {
        var style = $this.styles[i];
        if (style.inputType === 'TextEditor') {
          // var editor = new FroalaEditor('textarea#' + style.attributeName, {
          //   language: 'zh_cn',
          //   heightMin: 350
          // });
          var editor = UE.getEditor(style.attributeName, {
            allowDivTransToP: false,
            maximumWords: 99999999
          });
          editor.styleIndex = i;
          editor.ready(function () {
            editor.addListener("contentChange", function () {
              $this.styles[this.styleIndex].value = this.getContent();
            });
          });
        }
      }

    }, 100);
  },

  winResize: function () {
    this.mainHeight = ($(window).height() - 100) + 'px';
  },

  btnLayerClick: function(options) {
    var url = "../Shared/" + options.name + ".cshtml?siteId=" + this.siteId + "&channelId=" + this.channelId;

    if (options.attributeName) {
      url += "&attributeName=" + options.attributeName;
    }
    if (options.contentId) {
      url += "&contentId=" + options.contentId;
    }

    utils.openLayer({
      title: options.title,
      url: url,
      full: options.full,
      width: options.width ? options.width : 700,
      height: options.height ? options.height : 500
    });
  },

  btnCancelClick: function() {
    utils.closeLayer();
  },

  btnSaveClick: function() {
    console.log(parent.$vue);
    if (UE) {
      $.each(UE.instants, function (index, editor) {
        editor.sync();
      });
    }

    var $this = this;
    utils.loading(this, true);
    $api.put($url, {
      siteId: this.siteId,
      channelId: this.channelId,
      contentId: this.contentId,
      content: this.form
    }).then(function(response) {
      var res = response.data;

      parent.$vue.apiList($this.channelId, $this.page, '内容保存成功！');
      utils.closeLayer();
    })
    .catch(function(error) {
      utils.error($this, error);
    })
    .then(function() {
      utils.loading($this, false);
    });
  },

  btnTransAddClick: function() {
    this.transForm = {
      siteId: this.siteId,
      channelId: this.channelId,
      transType: 'Copy'
    };
  },

  btnTransSaveClick: function() {
    this.translations.push({
      siteId: this.transForm.siteId,
      channelId: this.transForm.channelId,
      transType: this.transForm.transType
    });
    this.transForm = null;
  },

  btnTransCancelClick: function() {
    this.transForm = null;
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
      utils.error($this, error);
    })
    .then(function() {
      utils.loading($this, false);
    });
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
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function() {
    this.getConfig();
  }
});
