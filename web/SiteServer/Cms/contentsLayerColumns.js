var $api = new apiUtils.Api(apiUrl + '/pages/cms/contentsLayerColumns');

var data = {
  siteId: parseInt(pageUtils.getQueryStringByName('siteId')),
  channelId: parseInt(pageUtils.getQueryStringByName('channelId')),
  pageLoad: false,
  pageAlert: null,
  attributes: null,
  attributeNames: []
};

var methods = {
  loadConfig: function () {
    var $this = this;
    $api.get({
      siteId: $this.siteId,
      channelId: $this.channelId
    }, function (err, res) {
      if (err || !res || !res.value) return;

      $this.attributes = res.value;
      $this.attributeNames = [];
      for (var i = 0; i < $this.attributes.length; i++) {
        var attribute = $this.attributes[i];
        if (attribute.selected) {
          $this.attributeNames.push(attribute.value);
        }
      }
      $this.pageLoad = true;
    });
  },
  btnSubmitClick: function () {
    var $this = this;

    pageUtils.loading(true);
    $api.post({
      siteId: $this.siteId,
      channelId: $this.channelId,
      attributeNames: $this.attributeNames.join(',')
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