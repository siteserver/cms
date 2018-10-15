var $api = new apiUtils.Api(apiUrl + '/pages/cms/contentsLayerDelete');

var data = {
  siteId: parseInt(pageUtils.getQueryStringByName('siteId')),
  channelId: parseInt(pageUtils.getQueryStringByName('channelId')),
  contentIds: pageUtils.getQueryStringByName('contentIds'),
  pageLoad: false,
  pageAlert: null,
  contents: null,
  isRetainFiles: false
};

var methods = {
  loadConfig: function () {
    var $this = this;

    $api.get({
      siteId: $this.siteId,
      channelId: $this.channelId,
      contentIds: $this.contentIds
    }, function (err, res) {
      if (err || !res || !res.value) return;

      $this.contents = res.value;
      $this.pageAlert = {
        type: 'danger',
        html: '此操作将把以下 <strong>' + $this.contents.length + '</strong> 篇内容放入回收站，确定吗？'
      };
      $this.pageLoad = true;
    });
  },
  btnSubmitClick: function () {
    var $this = this;

    pageUtils.loading(true);
    $api.post({
      siteId: $this.siteId,
      channelId: $this.channelId,
      contentIds: $this.contentIds,
      isRetainFiles: $this.isRetainFiles,
    }, function (err, res) {
      if (err || !res || !res.value) return;

      parent.location.reload(true);
    });
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.loadConfig();
  }
});