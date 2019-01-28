var $url = '/pages/cms/contentsLayerTaxis';

var $data = {
  siteId: parseInt(utils.getQueryString('siteId')),
  channelId: parseInt(utils.getQueryString('channelId')),
  contentIds: utils.getQueryString('contentIds'),
  pageLoad: false,
  pageAlert: null,
  isUp: true,
  taxis: 1
};

var $methods = {
  btnSubmitClick: function () {
    var $this = this;

    utils.loading(true);
    $api.post($url, {
      siteId: $this.siteId,
      channelId: $this.channelId,
      contentIds: $this.contentIds,
      isUp: $this.isUp,
      taxis: $this.taxis
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

new Vue({
  el: '#main',
  data: $data,
  methods: $methods,
  created: function () {
    this.pageLoad = true;
  }
});