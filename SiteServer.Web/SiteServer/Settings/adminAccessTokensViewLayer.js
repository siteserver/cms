var $url = '/pages/settings/adminAccessTokensViewLayer';
var $id = utils.getQueryString('id');

var $data = {
  pageLoad: false,
  pageAlert: {
    type: 'warning',
    html: 'API密钥属于敏感信息，请妥善保管不要泄露；如果怀疑信息泄露，请重设密钥。'
  },
  tokenInfo: null,
  accessToken: null
};

var $methods = {
  getAccessToken: function () {
    var $this = this;

    $api.get($url, {
      params: {
        id: $id
      }
    }).then(function (response) {
      var res = response.data;

      $this.tokenInfo = res.tokenInfo;
      $this.accessToken = res.accessToken;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  regenerate: function () {
    var $this = this;

    utils.loading(true);
    $api.post($url, {
      id: $id
    }).then(function (response) {
      var res = response.data;

      $this.accessToken = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnCancelClick: function () {
    utils.closeLayer();
  },

  btnRegenerateClick: function () {
    this.regenerate();
  }
};

new Vue({
  el: '#main',
  data: $data,
  methods: $methods,
  created: function () {
    this.getAccessToken();
  }
});