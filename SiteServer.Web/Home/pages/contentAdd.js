var sourceIdUser = -1;
var sourceIdPreview = -99;

var data = {
  pageConfig: null,
  pageLoad: false,
  pageAlert: null,
  pageType: '',
  sites: [],
  channels: [],
  site: {},
  channel: {},
  allGroupNames: [],
  allTagNames: [],
  styles: [],
  allCheckedLevels: [],
  returnUrl: utils.getQueryString('returnUrl'),

  contentId: 0,
  isColor: false,
  isHot: false,
  isRecommend: false,
  isTop: false,
  groupNames: [],
  tagNames: [],
  linkUrl: '',
  addDate: new Date(),
  checkedLevel: 0,
};

var methods = {
  loadSite: function (res) {
    this.pageConfig = res.config;
    this.sites = res.sites;
    this.channels = res.channels;
    this.site = res.site;
    this.channel = res.channel;
    this.allGroupNames = res.allGroupNames;
    this.allTagNames = res.allTagNames;
    if (this.site && this.channel) {
      this.loadContent(res.styles, res.checkedLevels, res.checkedLevel, res.content);
    } else {
      this.pageType = 'Unauthorized';
    }
  },

  loadChannel: function (res) {
    this.loadContent(res.styles, res.checkedLevels, res.checkedLevel, res.value);
  },

  loadContent: function (styles, checkedLevels, checkedLevel, content) {
    var $this = this;

    this.styles = [];
    for (let i = 0; i < styles.length; i++) {
      var style = styles[i];
      if (content.id) {
        style.value = content[_.camelCase(style.attributeName)];
      } else {
        style.value = style.defaultValue || '';
      }
      this.styles.push(style);
    }
    this.allCheckedLevels = checkedLevels;

    this.contentId = content.id;
    this.isTop = content.isTop;
    this.isRecommend = content.isRecommend;
    this.isHot = content.isHot;
    this.isColor = content.isColor;
    this.groupNames = [];
    if (content.groupNameCollection) {
      this.groupNames = content.groupNameCollection.split(',');
    }
    this.tagNames = [];
    if (content.tags) {
      this.tagNames = content.tags.split(',');
    }
    this.linkUrl = content.linkUrl;
    this.addDate = content.addDate;
    this.checkedLevel = checkedLevel;

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

          $('#' + style.attributeName).show();
        }
      }

    }, 100);
  },

  getValue: function (attributeName) {
    for (var i = 0; i < this.styles.length; i++) {
      var style = this.styles[i];
      if (style.attributeName === attributeName) {
        return style.value;
      }
    }
    return '';
  },

  setValue: function (attributeName, value) {
    for (var i = 0; i < this.styles.length; i++) {
      var style = this.styles[i];
      if (style.attributeName === attributeName) {
        style.value = value;
      }
    }
  },

  onSiteSelect: function (site) {
    if (site.id === this.site.id) return;
    var $this = this;
    this.pageLoad = false;
    utils.getConfig({
      pageName: 'contentAdd',
      siteId: site.id
    }, function (res) {
      $this.pageLoad = true;
      $this.loadSite(res);
    });
  },

  onChannelSelect: function (channel) {
    if (channel.id === this.channel.id) return;
    var $this = this;
    this.pageLoad = false;
    utils.getConfig({
      pageName: 'contentAdd',
      siteId: this.site.id,
      channelId: channel.id
    }, function (res) {
      $this.pageLoad = true;
      $this.loadChannel(res);
    });
  },

  addTag: function (newTag) {
    this.allTagNames.push(newTag);
    this.tagNames.push(newTag);
  },

  submit: function (sourceId) {
    var $this = this;

    var payload = {
      id: this.contentId,
      isTop: this.isTop,
      isRecommend: this.isRecommend,
      isHot: this.isHot,
      isColor: this.isColor,
      linkUrl: this.linkUrl,
      addDate: this.addDate,
      groupNameCollection: this.groupNames.join(','),
      tags: this.tagNames.join(','),
      checkedLevel: this.checkedLevel,
      sourceId: sourceId
    };
    for (var i = 0; i < this.styles.length; i++) {
      var style = this.styles[i];
      payload[style.attributeName] = style.value;
    }

    parent.utils.loading(true);
    if (sourceId == sourceIdPreview) {
      new utils.Api('/v1/contents/' + this.site.id + '/' + this.channel.id)
        .post(payload, function (err, res) {
          parent.utils.loading(false);

          if (err) {
            $this.pageAlert = {
              type: 'danger',
              html: err.message
            };
            return;
          }

          var contentId = $this.contentId ? $this.contentId : res.value.id;
          window.open(utils.getApiUrl('/preview/' + $this.site.id + '/' + $this.channel.id + '/' + contentId + '?isPreview=true&previewId=' + res.value.id));
        });
    } else if (payload.id) {
      new utils.Api('/v1/contents/' + this.site.id + '/' + this.channel.id + '/' + payload.id)
        .put(payload, function (err, res) {
          parent.utils.loading(false);

          if (err) {
            $this.pageAlert = {
              type: 'danger',
              html: err.message
            };
            return;
          }

          parent.alert({
            toast: true,
            type: 'success',
            title: "稿件修改成功",
            showConfirmButton: false,
            timer: 3000
          }).then(function () {
            parent.location.hash = $this.returnUrl;
          });
        });
    } else {
      new utils.Api('/v1/contents/' + this.site.id + '/' + this.channel.id)
        .post(payload, function (err, res) {
          parent.utils.loading(false);

          if (err) {
            $this.pageAlert = {
              type: 'danger',
              html: err.message
            };
            return;
          }

          $this.pageType = 'success';
        });
    }
  },

  btnLayerClick: function (options) {
    this.pageAlert = null;
    var url = "pages/contentAddLayer" +
      options.name +
      ".html?siteId=" +
      this.site.id +
      "&channelId=" +
      this.channel.id;

    if (options.contentId) {
      url += "&contentId=" + options.contentId
    }

    if (options.args) {
      _.forIn(options.args, function (value, key) {
        url += "&" + key + "=" + encodeURIComponent(value);
      });
    }

    parent.utils.openLayer({
      title: options.title,
      url: url,
      full: options.full,
      width: options.width ? options.width : 700,
      height: options.height ? options.height : 500
    });
  },

  btnImageClick: function (imageUrl) {
    top.utils.openImagesLayer([imageUrl]);
  },

  btnContinueAddClick: function () {
    location.reload();
  },

  btnSubmitClick: function () {
    var $this = this;
    this.pageAlert = null;

    this.$validator.validate().then(function (result) {
      if (result) {
        $this.submit(sourceIdUser);
      }
    });
  },

  btnPreviewClick: function () {
    var $this = this;
    this.pageAlert = null;

    this.$validator.validate().then(function (result) {
      if (result) {
        $this.submit(sourceIdPreview);
      }
    });
  }
};

Vue.component("multiselect", window.VueMultiselect.default);
Vue.component("date-picker", window.DatePicker.default);

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    var $this = this;
    var siteId = 0;
    var channelId = 0;
    var contentId = parseInt(utils.getQueryString('contentId') || 0);
    if (contentId == 0) {
      siteId = parseInt(Cookies.get('SS-USER-SITE-ID') || 0);
      channelId = parseInt(Cookies.get('SS-USER-CHANNEL-ID') || 0);
    } else {
      siteId = parseInt(utils.getQueryString('siteId') || 0);
      channelId = parseInt(utils.getQueryString('channelId') || 0);
    }

    utils.getConfig({
        pageName: 'contentAdd',
        siteId: siteId,
        channelId: channelId,
        contentId: contentId
      },
      function (res) {
        if (res.value) {
          $this.loadSite(res);
        } else {
          utils.redirectLogin();
        }
      });
  }
});

var getValue = function (attributeName) {
  return $vue.getValue(attributeName);
}

var setValue = function (attributeName, value) {
  $vue.setValue(attributeName, value);
}