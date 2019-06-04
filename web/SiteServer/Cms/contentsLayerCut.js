var $api = new apiUtils.Api(apiUrl + '/pages/cms/contentsLayerCut');
var $apiChannels = new apiUtils.Api(apiUrl + '/pages/cms/contentsLayerCut/actions/getChannels');

var data = {
  siteId: parseInt(pageUtils.getQueryString('siteId')),
  channelId: parseInt(pageUtils.getQueryString('channelId')),
  contentIds: pageUtils.getQueryString('contentIds'),
  pageLoad: false,
  pageAlert: null,
  contents: null,
  sites: [],
  channels: [],
  site: {},
  channel: null,
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

        $this.pageAlert = {
          type: 'danger',
          html: '此操作将把以下 <strong>' +
            $this.contents.length +
            '</strong> 篇内容转移至指定栏目，确定吗？'
        };
        $this.pageLoad = true;
      }
    );
  },

  onSiteSelect(site) {
    if (site.id === this.site.id) return;
    this.site = site;
    var $this = this;

    parent.pageUtils.loading(true);
    $apiChannels.get({
        siteId: this.site.id
      },
      function (err, res) {
        parent.pageUtils.loading(false);
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

    parent.pageUtils.loading(true);
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