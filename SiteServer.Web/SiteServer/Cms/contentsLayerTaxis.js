var $api = new apiUtils.Api(apiUrl + '/pages/cms/contentsLayerTaxis');

var data = {
  siteId: parseInt(pageUtils.getQueryStringByName('siteId')),
  channelId: parseInt(pageUtils.getQueryStringByName('channelId')),
  contentIds: pageUtils.getQueryStringByName('contentIds'),
  pageLoad: false,
  pageAlert: null,
  isUp: true,
  taxis: 1
};

var methods = {
  loadConfig: function () {
    this.pageLoad = true;
  },
  btnSubmitClick: function () {
    var $this = this;

    pageUtils.loading(true);
    $api.post({
      siteId: $this.siteId,
      channelId: $this.channelId,
      contentIds: $this.contentIds,
      isUp: $this.isUp,
      taxis: $this.taxis
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