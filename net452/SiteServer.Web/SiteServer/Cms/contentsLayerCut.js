var $url = '/pages/cms/contentsLayerCut';
var $urlGetChannels = '/pages/cms/contentsLayerCut/actions/getChannels';

var $data = {
  siteId: parseInt(utils.getQueryString('siteId')),
  channelId: parseInt(utils.getQueryString('channelId')),
  contentIds: utils.getQueryString('contentIds'),
  pageLoad: false,
  pageAlert: null,
  contents: null,
  sites: [],
  channels: [],
  site: {},
  channel: null,
  isSubmit: false
};

var $methods = {
  loadConfig: function () {
    var $this = this;

    $api.get($url, {
      params: {
        siteId: $this.siteId,
        channelId: $this.channelId,
        contentIds: $this.contentIds
      }
    }).then(function (response) {
      var res = response.data;

      $this.contents = res.value;
      $this.sites = res.sites;
      $this.channels = res.channels;
      $this.site = res.site;

      $this.pageAlert = {
        type: 'danger',
        html: '此操作将把以下 <strong>' +
          $this.contents.length +
          '</strong> 篇内容转移至指定栏目，确定吗？'
      };
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  onSiteSelect(site) {
    var $this = this;
    if (site.id === $this.site.id) return;
    $this.site = site;

    utils.loading(true);
    $api.get($urlGetChannels, {
      params: {
        siteId: $this.site.id
      }
    }).then(function (response) {
      var res = response.data;

      $this.channels = res.value;
      $this.channel = null;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  onChannelSelect(channel) {
    this.channel = channel;
  },

  btnSubmitClick: function () {
    var $this = this;
    $this.isSubmit = true;
    if (!$this.channel) return;

    utils.loading(true);
    $api.post($url, {
      siteId: $this.siteId,
      channelId: $this.channelId,
      contentIds: $this.contentIds,
      targetSiteId: $this.site.id,
      targetChannelId: $this.channel.id
    }).then(function (response) {
      var res = response.data;

      parent.location.reload(true);
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  }
};

Vue.component("multiselect", window.VueMultiselect.default);

new Vue({
  el: '#main',
  data: $data,
  methods: $methods,
  created: function () {
    this.loadConfig();
  }
});