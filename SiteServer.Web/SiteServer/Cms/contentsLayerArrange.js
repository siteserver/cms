var $api = new apiUtils.Api(apiUrl + '/pages/cms/contentsLayerArrange');

var data = {
  siteId: parseInt(pageUtils.getQueryString('siteId')),
  channelId: parseInt(pageUtils.getQueryString('channelId')),
  pageLoad: false,
  pageAlert: null,
  attributeName: 'Id',
  isDesc: true
};

var methods = {
  loadConfig: function () {
    this.pageLoad = true;
  },
  btnSubmitClick: function () {
    var $this = this;

    parent.pageUtils.loading(true);
    $api.post({
        siteId: $this.siteId,
        channelId: $this.channelId,
        attributeName: $this.attributeName,
        isDesc: $this.isDesc
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