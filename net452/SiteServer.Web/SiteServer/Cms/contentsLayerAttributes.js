var $url = '/pages/cms/contentsLayerAttributes';

var $data = {
  siteId: parseInt(utils.getQueryString('siteId')),
  channelId: parseInt(utils.getQueryString('channelId')),
  contentIds: utils.getQueryString('contentIds'),
  pageLoad: false,
  pageAlert: null,
  pageType: 'setAttributes',
  isRecommend: false,
  isHot: false,
  isColor: false,
  isTop: false,
  hits: 0
};

var $methods = {
  btnSubmitClick: function () {
    var $this = this;

    utils.loading(true);
    $api.post($url, {
      siteId: $this.siteId,
      channelId: $this.channelId,
      contentIds: $this.contentIds,
      pageType: $this.pageType,
      isRecommend: $this.isRecommend,
      isHot: $this.isHot,
      isColor: $this.isColor,
      isTop: $this.isTop,
      hits: $this.hits
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