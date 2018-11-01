var $api = new utils.Api('/home/contentsLayerCopy');
var $apiChannels = new utils.Api('/home/contentsLayerCopy/actions/getChannels');

var data = {
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
  copyType: 'Copy',
  isSubmit: false
};

var methods = {
  loadConfig: function () {
    var $this = this;

    $api.get({
        siteId: $this.siteId,
        channelId: $this.channelId,
        contentIds: $this.contentIds
      },
      function (err, res) {
        if (err || !res || !res.value) return;

        $this.contents = res.value;
        $this.sites = res.sites;
        $this.channels = res.channels;
        $this.site = res.site;

        $this.pageLoad = true;
      }
    );
  },

  onSiteSelect(site) {
    if (site.id === this.site.id) return;
    this.site = site;
    var $this = this;

    parent.utils.loading(true);
    $apiChannels.get({
        siteId: this.site.id
      },
      function (err, res) {
        parent.utils.loading(false);
        if (err || !res || !res.value) return;

        $this.channels = res.value;
        $this.channel = null;
      }
    );
  },

  onChannelSelect(channel) {
    this.channel = channel;
  },

  btnSubmitClick: function () {
    var $this = this;
    this.isSubmit = true;
    if (!this.channel) return;

    parent.utils.loading(true);
    $api.post({
        siteId: $this.siteId,
        channelId: $this.channelId,
        contentIds: $this.contentIds,
        targetSiteId: $this.site.id,
        targetChannelId: $this.channel.id
      },
      function (err, res) {
        if (err || !res || !res.value) return;

        parent.location.reload(true);
      }
    );
  }
};

Vue.component("multiselect", window.VueMultiselect.default);

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.loadConfig();
  }
});