var $url = '/pages/cms/contentsLayerColumns';

var $data = {
  siteId: parseInt(utils.getQueryString('siteId')),
  channelId: parseInt(utils.getQueryString('channelId')),
  pageLoad: false,
  pageAlert: null,
  attributes: null,
  attributeNames: []
};

var $methods = {
  loadConfig: function () {
    var $this = this;
    $api.get($url, {
      params: {
        siteId: $this.siteId,
        channelId: $this.channelId
      }
    }).then(function (response) {
      var res = response.data;

      $this.attributes = res.value;
      $this.attributeNames = [];
      for (var i = 0; i < $this.attributes.length; i++) {
        var attribute = $this.attributes[i];
        if (attribute.selected) {
          $this.attributeNames.push(attribute.value);
        }
      }
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  btnSubmitClick: function () {
    var $this = this;

    utils.loading(true);
    $api.post($url, {
      siteId: $this.siteId,
      channelId: $this.channelId,
      attributeNames: $this.attributeNames.join(',')
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
    this.loadConfig();
  }
});