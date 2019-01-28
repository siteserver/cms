var $url = '/pages/cms/contentsLayerCheck';

var $data = {
  siteId: parseInt(utils.getQueryString('siteId')),
  channelId: parseInt(utils.getQueryString('channelId')),
  contentIds: utils.getQueryString('contentIds'),
  pageLoad: false,
  pageAlert: null,
  contents: null,
  description: '',
  checkedLevels: null,
  allChannels: null,
  channels: [],
  isChannelLoading: false,
  checkedLevel: null,
  isTranslate: false,
  translateChannel: null,
  reasons: null
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
      $this.checkedLevels = res.checkedLevels;
      $this.checkedLevel = res.checkedLevel;
      $this.channels = $this.allChannels = res.allChannels;
      $this.pageAlert = {
        type: 'warning',
        html: '此操作将审核以下 <strong>' + $this.contents.length + '</strong> 篇内容，确定吗？'
      };
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  asyncFind: function (query) {
    this.isChannelLoading = true;
    this.channels = [];
    for (var i = 0; i < this.allChannels.length; i++) {
      var channel = this.allChannels[i];
      if (channel.value.indexOf(query) !== -1) {
        this.channels.push(channel);
      }
    }
    this.isChannelLoading = false;
  },

  btnSubmitClick: function () {
    var $this = this;

    utils.loading(true);
    $api.post($url, {
      siteId: $this.siteId,
      channelId: $this.channelId,
      contentIds: $this.contentIds,
      checkedLevel: $this.checkedLevel,
      isTranslate: $this.isTranslate,
      translateChannelId: $this.translateChannel ? $this.translateChannel.key : 0,
      reasons: $this.reasons
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