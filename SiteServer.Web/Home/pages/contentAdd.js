var $api = new apiUtils.Api(apiUrl + "/home/contentAdd");
var $createApi = new apiUtils.Api(apiUrl + "/home/contents/actions/create");

Object.defineProperty(Object.prototype, "getProp", {
  value: function (prop) {
    var key, self = this;
    for (key in self) {
      if (key.toLowerCase() == prop.toLowerCase()) {
        return self[key];
      }
    }
  }
});

var data = {
  pageConfig: null,
  pageLoad: false,
  pageAlert: null,
  pageType: '',

  sites: [],
  channels: [],
  site: {},
  channel: {},
  groupNames: [],

  content: {},
  styles: [],
};

var methods = {
  loadSite: function (res) {
    this.pageConfig = res.value;
    this.sites = res.sites;
    this.channels = res.channels;
    this.site = res.site;
    this.channel = res.channel;
    this.groupNames = res.groupNames;
    this.loadContent(res.styles, res.content);
  },

  loadChannel: function (res) {
    this.loadContent(res.styles, res.value);
  },

  loadContent: function (styles, content) {
    var $this = this;

    this.styles = [];
    for (let i = 0; i < styles.length; i++) {
      var style = styles[i];
      style.value = style.defaultValue || '';
      this.styles.push(style);
    }

    content.groupNames = [];
    if (content.groupNameCollection) {
      content.groupNames = content.groupNameCollection.split(',');
    }
    this.content = content;

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

  onSiteSelect: function (site) {
    if (site.id === this.site.id) return;
    var $this = this;
    this.pageLoad = false;
    pageUtils.getConfig({
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
    pageUtils.getConfig({
      pageName: 'contentAdd',
      siteId: this.site.id,
      channelId: channel.id
    }, function (res) {
      $this.pageLoad = true;
      $this.loadChannel(res);
    });
  },

  submit: function () {
    var $this = this;

    var payload = {
      id: this.content.id,
      groupNameCollection: this.groupNames.join(','),
      isColor: this.content.isColor,
      isHot: this.content.isHot,
      isRecommend: this.content.isRecommend,
      isTop: this.content.isTop,
      tags: this.content.tags
    };
    for (var i = 0; i < this.styles.length; i++) {
      var style = this.styles[i];
      payload[style.attributeName] = style.value;
    }

    pageUtils.loading(true);
    if (payload.id) {
      new apiUtils.Api(apiUrl + '/v1/contents/' + this.site.id + '/' + this.channel.id + '/' + payload.id + '?sourceId=-1')
        .put(payload, function (err, res) {
          pageUtils.loading(false);

          if (err) {
            $this.pageAlert = {
              type: 'danger',
              html: err.message
            };
            return;
          }
          $this.pageType = 'success';
        });
    } else {
      new apiUtils.Api(apiUrl + '/v1/contents/' + this.site.id + '/' + this.channel.id + '?sourceId=-1')
        .post(payload, function (err, res) {
          pageUtils.loading(false);

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

  btnSubmitClick: function () {
    var $this = this;
    this.pageAlert = null;

    this.$validator.validate().then(function (result) {
      if (result) {
        $this.submit();
      }
    });
  },

  btnContinueAddClick: function () {
    location.reload();
  }
};

Vue.component("multiselect", window.VueMultiselect.default);

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    var $this = this;
    if (authUtils.isAuthenticated()) {
      pageUtils.getConfig('contentAdd', function (res) {
        if (res.isUserLoggin) {
          $this.loadSite(res);
        } else {
          authUtils.redirectLogin();
        }
      });
    } else {
      authUtils.redirectLogin();
    }
  }
});