var $url = '/common/editor/layerPreviewVideo';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  videoUrl: utils.getQueryString('videoUrl'),
  rootUrl: null,
  siteUrl: null,
});

var methods = {
  apiGet: function() {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function(response) {
      var res = response.data;

      $this.rootUrl = res.rootUrl;
      $this.siteUrl = res.siteUrl;

      $this.videoUrl = utils.getUrl($this.siteUrl, $this.videoUrl);
    })
    .catch(function(error) {
      utils.error(error);
    })
    .then(function() {
      utils.loading($this, false);
    });
  },

  btnCancelClick: function () {
    utils.closeLayer();
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(null, this.btnCancelClick);
    if (this.siteId) {
      this.apiGet();
    } else {
      utils.loading(this, false);
    }
  }
});
