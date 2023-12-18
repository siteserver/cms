var $url = '/wx/account';
var $urlMp = '/wx/account/mp';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  mpUrl: null,
  account: null,
  mpTypes: null,
  mpForm: null,
  mpResult: null
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.mpUrl = res.mpUrl;
      $this.account = res.account;
      $this.mpForm = Object.assign({}, res.account);
      $this.mpTypes = res.mpTypes;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiMpSubmit: function () {
    this.mpResult = null;
    var $this = this;

    utils.loading(this, true);
    $api.post($urlMp, {
      siteId: this.siteId,
      isEnabled: this.mpForm.isEnabled,
      mpName: this.mpForm.mpName,
      mpType: this.mpForm.mpType,
      mpAppId: this.mpForm.mpAppId,
      mpAppSecret: this.mpForm.mpAppSecret
    }).then(function (response) {
      var res = response.data;

      $this.mpResult = {
        success: res.success,
        errorMessage: res.errorMessage
      };
      $this.account = res.account;

    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnMpSubmitClick: function () {
    var $this = this;

    this.$refs.mpForm.validate(function(valid) {
      if (valid) {
        $this.apiMpSubmit();
      }
    });
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});
