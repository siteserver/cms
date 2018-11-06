var $api = new utils.Api('/home/contentsLayerCheck');

var data = {
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
        $this.checkedLevels = res.checkedLevels;
        $this.checkedLevel = res.checkedLevel;
        $this.channels = $this.allChannels = res.allChannels;
        $this.pageAlert = {
          type: 'warning',
          html: '此操作将审核以下 <strong>' +
            $this.contents.length +
            '</strong> 篇内容，确定吗？'
        };
        $this.pageLoad = true;
      }
    );
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

    parent.utils.loading(true);
    $api.post({
        siteId: $this.siteId,
        channelId: $this.channelId,
        contentIds: $this.contentIds,
        checkedLevel: $this.checkedLevel,
        isTranslate: $this.isTranslate,
        translateChannelId: $this.translateChannel ?
          $this.translateChannel.key : 0,
        reasons: $this.reasons
      },
      function (err, res) {
        if (err || !res || !res.value) return;

        parent.location.reload(true);
      }
    );
  }
};

Vue.component('multiselect', window.VueMultiselect.default);

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.loadConfig();
  }
});