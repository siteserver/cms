var $url = '/pages/cms/contents/contentsLayerCheck';

var data = utils.initData({
  page: utils.getQueryInt('page'),
  siteId: utils.getQueryInt('siteId'),
  channelId: utils.getQueryInt('channelId'),
  channelContentIds: utils.getQueryString('channelContentIds'),
  contents: null,
  checkedLevels: null,
  transSites: null,
  transChannels: null,
  form: {
    checkedLevel: null,
    reasons: null,
    isTranslate: false,
    transSiteId: null,
    transChannelIds: [],
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
      $this.checkedLevels = res.checkedLevels;
      $this.transSites = res.transSites;
      $this.form.checkedLevel = res.checkedLevel;
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
      checkedLevel: this.form.checkedLevel,
      reasons: this.form.reasons,
      isTranslate: this.form.isTranslate,
      transSiteId: this.form.transSiteId,
      transChannelId: this.form.transChannelIds.length === 0 ? 0 : this.form.transChannelIds[this.form.transChannelIds.length - 1],
    }).then(function (response) {
      var res = response.data;

      parent.$vue.apiList($this.channelId, $this.page, '内容审核成功!');
      utils.closeLayer();
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiGetOptions: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/options', {
      siteId: this.siteId,
      channelId: this.channelId,
      transSiteId: this.form.transSiteId
    }).then(function (response) {
      var res = response.data;

      $this.transChannels = [res.transChannels];
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getContentUrl: function (content) {
    if (content.checked) {
      return '../redirect.cshtml?siteId=' + content.siteId + '&channelId=' + content.channelId + '&contentId=' + content.id;
    }
    return apiUrl + '/preview/' + content.siteId + '/' + content.channelId + '/' + content.id;
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

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});