var $url = '/share';
var $urlWxShare = '/share/wxShare';
var $urlSettings = '/share/settings';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  siteUrl: null,
  ipAddress: null,
  settingsForm: null,
  wxShareForm: null,
  mpResult: null
});

var methods = {
  runFormLayerImageUploadText: function(attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  runMaterialLayerImageSelect: function(attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  insertText: function(attributeName, no, text) {
    this.settingsForm[attributeName] = text;
    this.settingsForm = _.assign({}, this.settingsForm);
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

      $this.siteUrl = res.siteUrl;
      $this.ipAddress = res.ipAddress;
      $this.settingsForm = Object.assign({}, res.settings);
      $this.wxShareForm = Object.assign({}, res.settings);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiWxShareSubmit: function () {
    this.mpResult = null;
    var $this = this;

    utils.loading(this, true);
    $api.post($urlWxShare, {
      siteId: this.siteId,
      isWxShare: this.wxShareForm.isWxShare,
      mpAppId: this.wxShareForm.mpAppId,
      mpAppSecret: this.wxShareForm.mpAppSecret
    }).then(function (response) {
      var res = response.data;

      $this.mpResult = {
        success: res.success,
        errorMessage: res.errorMessage
      };
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSettingsSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlSettings, {
      siteId: this.siteId,
      defaultTitle: this.settingsForm.defaultTitle,
      defaultImageUrl: this.settingsForm.defaultImageUrl,
      defaultDescription: this.settingsForm.defaultDescription
    }).then(function (response) {
      var res = response.data;

      utils.success('页面分享设置保存成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSettingsSubmitClick: function () {
    var $this = this;

    this.$refs.settingsForm.validate(function(valid) {
      if (valid) {
        $this.apiSettingsSubmit();
      }
    });
  },

  btnWxShareSubmitClick: function () {
    var $this = this;

    this.$refs.wxShareForm.validate(function(valid) {
      if (valid) {
        $this.apiWxShareSubmit();
      }
    });
  },

  btnLayerClick: function(options) {
    var query = {
      siteId: this.siteId,
      channelId: this.channelId
    };

    if (options.contentId) {
      query.contentId = options.contentId;
    }
    if (options.attributeName) {
      query.attributeName = options.attributeName;
    }
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

  btnPreviewClick: function(attributeName, no) {
    var data = [];
    var imageUrl = this.settingsForm.defaultImageUrl;
    imageUrl = utils.getUrl(this.siteUrl, imageUrl);
    data.push({
      "src": imageUrl
    });
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
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});
