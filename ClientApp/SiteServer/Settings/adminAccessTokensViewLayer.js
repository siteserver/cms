var $api = new apiUtils.Api(apiUrl + '/pages/settings/adminAccessTokensViewLayer');
var $id = pageUtils.getQueryStringByName('id');

var $vue = new Vue({
  el: '#main',
  data: {
    pageLoad: false,
    pageAlert: {
      type: 'warning',
      html: 'API密钥属于敏感信息，请妥善保管不要泄露；如果怀疑信息泄露，请重设密钥。'
    },
    tokenInfo: null,
    accessToken: null
  },
  methods: {
    getAccessToken: function () {
      var $this = this;

      $api.get(null, function (err, res) {
        $this.pageLoad = true;
        if (err || !res) return;

        $this.tokenInfo = res.tokenInfo;
        $this.accessToken = res.accessToken;
      }, 'accessTokens', $id);
    },
    regenerate: function () {
      var $this = this;

      pageUtils.loading(true);
      $api.post(null, function (err, res) {
        pageUtils.loading(false);
        if (err || !res || !res.value) return;

        $this.accessToken = res.value;
      }, 'regenerate', $id);
    },
    btnCancelClick: function () {
      pageUtils.closeLayer();
    },
    btnRegenerateClick: function () {
      this.regenerate();
    }
  }
});

$vue.getAccessToken();