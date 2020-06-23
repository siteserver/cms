var $url = '/settings/administratorsAccessTokensLayerView';

var data = utils.init({
  id: utils.getQueryInt('id'),
  token: null,
  accessToken: null
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        id: this.id
      }
    }).then(function (response) {
      var res = response.data;

      $this.token = res.token;
      $this.accessToken = res.accessToken;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiRegenerate: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/regenerate', {
      id: this.id
    }).then(function (response) {
      var res = response.data;

      $this.accessToken = res.accessToken;
      utils.success('API密钥重设成功，请将原密码替换为新的密钥');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnCancelClick: function () {
    utils.closeLayer();
  },

  btnRegenerateClick: function () {
    this.apiRegenerate();
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});