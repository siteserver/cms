var $url = '/pages/settings/adminAccessTokensViewLayer';

var data = utils.initData({
  id: utils.getQueryInt('id'),
  tokenInfo: null,
  accessToken: null
});

var methods = {
  getAccessToken: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url + '/accessTokens/' + this.id).then(function (response) {
      var res = response.data;

      $this.tokenInfo = res.tokenInfo;
      $this.accessToken = res.accessToken;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  regenerate: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/regenerate/' + this.id).then(function (response) {
      var res = response.data;

      $this.accessToken = res.value;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnCancelClick: function () {
    utils.closeLayer();
  },

  btnRegenerateClick: function () {
    this.regenerate();
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getAccessToken();
  }
});