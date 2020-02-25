var $url = '/admin/cms/contents/contentsLayerTranslate';

var data = utils.initData({
  page: utils.getQueryInt('page'),
  siteId: utils.getQueryInt('siteId'),
  channelId: utils.getQueryInt('channelId'),
  channelContentIds: utils.getQueryString('channelContentIds'),
  contents: null,
  transSites: null,
  transChannels: null,
  form: {
    transSiteIds: null,
    transChannelIds: null,
  }
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId,
        channelId: this.channelId,
        channelContentIds: this.channelContentIds
      }
    }).then(function (response) {
      var res = response.data;

      $this.contents = res.contents;
      $this.transSites = res.transSites;
      $this.form.transSiteIds = [$this.siteId];
      $this.apiGetOptions();
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      siteId: this.siteId,
      channelId: this.channelId,
      channelContentIds: this.channelContentIds,
      transSiteId: this.form.transSiteIds[this.form.transSiteIds.length - 1],
      transChannelId: this.form.transChannelIds[this.form.transChannelIds.length - 1],
    }).then(function (response) {
      var res = response.data;

      parent.$vue.apiList($this.channelId, $this.page, '内容转移成功!', true);
      utils.closeLayer();
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiGetOptions: function() {
    var $this = this;

    $api.post($url + '/actions/options', {
      siteId: this.siteId,
      channelId: this.channelId,
      transSiteId: this.form.transSiteIds[this.form.transSiteIds.length - 1],
    }).then(function (response) {
      var res = response.data;

      $this.transChannels = [res.transChannels];
      $this.form.transChannelIds = null;
    }).catch(function (error) {
      utils.loading($this, false);
      utils.error($this, error);
    });
  },

  getContentUrl: function (content) {
    if (content.checked) {
      return '../redirect.cshtml?siteId=' + content.siteId + '&channelId=' + content.channelId + '&contentId=' + content.id;
    }
    return $apiUrl + '/preview/' + content.siteId + '/' + content.channelId + '/' + content.id;
  },

  handleTransSiteIdChange: function() {
    this.apiGetOptions();
  },

  btnSubmitClick: function () {
    var $this = this;
      this.$refs.form.validate(function(valid) {
        if (valid) {
          $this.apiSubmit();
        }
      });
  },

  btnCancelClick: function () {
    utils.closeLayer();
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});