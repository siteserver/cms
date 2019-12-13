var $url = "/pages/cms/editor";

var data = {
  siteId: utils.getQueryInt("siteId"),
  channelId: utils.getQueryInt("channelId"),
  contentId: utils.getQueryInt("contentId"),
  returnUrl: utils.getQueryString('returnUrl'),
  mainHeight: '',
  pageLoad: false,
  pageAlert: null,
  isSettings: true,
  sideType: "first",
  collapseType: "0",

  site: null,
  channel: null,
  groupNames: null,
  tagNames: null,
  styles: null,
  checkedLevels: null,
  content: null,
  siteOptions: null,
  channelOptions: null,

  transForm: null,
  translations: [],
};

var methods = {
  getConfig: function() {
    var $this = this;

    window.onresize = $this.winResize;
    window.onresize();

    $api
      .get($url, {
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
        $this.pageAlert = utils.getPageAlert(error);
      })
      .then(function() {
        $this.pageLoad = true;

        setTimeout(function () {
          for (var i = 0; i < $this.styles.length; i++) {
            var style = $this.styles[i];
            if (style.inputType === 'TextEditor') {
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
      });
  },

  loadEditor: function(res) {
    this.site = res.site;
    this.channel = res.channel;
    this.groupNames = res.groupNames;
    this.tagNames = res.tagNames;
    this.checkedLevels = res.checkedLevels;
    this.content = res.content;
    this.siteOptions = res.siteOptions;
    this.channelOptions = res.channelOptions;
    this.styles = [];

    for (let i = 0; i < res.styles.length; i++) {
      var style = res.styles[i];
      if (this.contentId) {
        style.value = this.content[_.camelCase(style.attributeName)];
      } else {
        style.value = style.defaultValue || '';
      }
      this.styles.push(style);
    }

    var $this = this;
    setTimeout(function () {
      for (var i = 0; i < $this.styles.length; i++) {
        var style = $this.styles[i];
        if (style.inputType === 'TextEditor') {
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
    this.mainHeight = ($(window).height() - 85) + 'px';
  },

  btnLayerClick: function(options, e) {
    e.stopPropagation();

    this.pageAlert = null;
    var url = "editorLayer" + options.name + ".cshtml?siteId=" + this.siteId;

    if (options.channelId) {
      url += "&channelId=" + options.channelId;
    } else {
      url += "&channelId=" + this.channelId;
    }
    if (options.contentId) {
      url += "&contentId=" + options.contentId;
    }

    if (options.withContents) {
      if (!this.isContentChecked) return;
      url += "&channelContentIds=" + this.channelContentIdsString;
    }

    if (options.withOptionalContents) {
      if (this.isContentChecked) {
        url += "&channelContentIds=" + this.channelContentIdsString;
      }
    }
    url += "&returnUrl=" + encodeURIComponent(location.href);

    utils.openLayer({
      title: options.title,
      url: url,
      full: options.full,
      width: options.width ? options.width : 700,
      height: options.height ? options.height : 500
    });
  },

  btnPreviewClick: function() {},

  btnSaveClick: function() {
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
  }
};

new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function() {
    this.getConfig();
  }
});
