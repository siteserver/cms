var $url = '/pages/cms/contents/contentsLayerView';

Object.defineProperty(Object.prototype, "getProp", {
  value: function (prop) {
    var key, self = this;
    for (key in self) {
      if (key.toLowerCase() == prop.toLowerCase()) {
        return self[key];
      }
    }
  }
});

var data = utils.initData({
  siteId: utils.getQueryInt('siteId'),
  channelId: utils.getQueryInt('channelId'),
  contentId: utils.getQueryInt('contentId'),
  content: null,
  channelName: null,
  attributes: null
});

var methods = {
  loadConfig: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId,
        channelId: this.channelId,
        contentId: this.contentId
      }
    }).then(function (response) {
      var res = response.data;

      $this.content = res.value;
      $this.channelName = res.channelName;
      $this.attributes = res.attributes;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
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