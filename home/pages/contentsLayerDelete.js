var $api = new utils.Api('/home/contentsLayerDelete');

var data = {
  siteId: parseInt(utils.getQueryString('siteId')),
  channelId: parseInt(utils.getQueryString('channelId')),
  contentIds: utils.getQueryString('contentIds'),
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
      },
      function (err, res) {
        if (err || !res || !res.value) return;

        $this.contents = res.value;
        $this.pageAlert = {
          type: 'danger',
          html: '此操作将把以下 <strong>' +
            $this.contents.length +
            '</strong> 篇内容放入回收站，确定吗？'
        };
        $this.pageLoad = true;
      }
    );
  },
  btnSubmitClick: function () {
    var $this = this;

    parent.utils.loading(true);
    $api.post({
        siteId: $this.siteId,
        channelId: $this.channelId,
        contentIds: $this.contentIds,
        isRetainFiles: $this.isRetainFiles
      },
      function (err, res) {
        if (err || !res || !res.value) return;

        parent.location.reload(true);
      }
    );
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