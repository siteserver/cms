var $api = new utils.Api('/home/contentsLayerAttributes');

var data = {
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

var methods = {
  loadConfig: function () {
    this.pageLoad = true;
  },
  btnSubmitClick: function () {
    var $this = this;

    parent.utils.loading(true);
    $api.post({
        siteId: $this.siteId,
        channelId: $this.channelId,
        contentIds: $this.contentIds,
        pageType: $this.pageType,
        isRecommend: $this.isRecommend,
        isHot: $this.isHot,
        isColor: $this.isColor,
        isTop: $this.isTop,
        hits: $this.hits
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