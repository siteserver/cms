var $url = '/cms/contents/contentsLayerCopy';

var data = utils.init({
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
    copyType: 'Copy',
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
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    var transChannelIds = [];
    this.form.transChannelIds.forEach(function(arr) {
      var transChannelId = arr[arr.length - 1];
      transChannelIds.push(transChannelId);
    });

    var transSiteId = this.form.transSiteIds[this.form.transSiteIds.length - 1];

    utils.loading(this, true);
    $api.post($url, {
      siteId: this.siteId,
      channelId: this.channelId,
      channelContentIds: this.channelContentIds,
      transSiteId: transSiteId,
      transChannelIds: transChannelIds,
      copyType: this.form.copyType
    }).then(function (response) {
      var res = response.data;

      parent.$vue.apiList($this.channelId, $this.page, '内容复制成功!', true);
      utils.closeLayer();
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiGetOptions: function() {
    var $this = this;

    $api.post($url + '/actions/options', {
      siteId: this.siteId,
      channelId: this.channelId,
      transSiteId: this.getTransSiteId(),
    }).then(function (response) {
      var res = response.data;

      $this.transChannels = [res.transChannels];
      $this.form.transChannelIds = null;
    }).catch(function (error) {
      utils.loading($this, false);
      utils.error(error);
    });
  },

  getTransSiteId: function() {
    return this.form.transSiteIds[this.form.transSiteIds.length - 1];
  },

  getContentUrl: function (content) {
    if (content.checked) {
      return utils.getRootUrl('redirect', {
        siteId: content.siteId,
        channelId: content.channelId,
        contentId: content.id
      });
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
    utils.keyPress(this.btnSubmitClick, this.btnCancelClick);
    this.apiGet();
  }
});
