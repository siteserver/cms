var $url = '/wx/account';
var $urlMp = '/wx/account/mp';
var $urlTenPay = '/wx/account/tenPay';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  mpUrl: null,
  defaultTenPayAuthorizeUrl: null,
  defaultTenPayNotifyUrl: null,
  account: null,
  mpForm: null,
  tenPayForm: null,
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
      $this.defaultTenPayAuthorizeUrl = res.defaultTenPayAuthorizeUrl;
      $this.defaultTenPayNotifyUrl = res.defaultTenPayNotifyUrl;
      $this.account = res.account;
      $this.mpForm = Object.assign({}, res.account);
      $this.tenPayForm = Object.assign({}, res.account);
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

  apiTenPaySubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlTenPay, {
      siteId: this.siteId,
      tenPayAppId: this.tenPayForm.tenPayAppId,
      tenPayAppSecret: this.tenPayForm.tenPayAppSecret,
      tenPayMchId: this.tenPayForm.tenPayMchId,
      tenPayKey: this.tenPayForm.tenPayKey,
      tenPayAuthorizeUrl: this.tenPayForm.tenPayAuthorizeUrl,
      tenPayNotifyUrl: this.tenPayForm.tenPayNotifyUrl
    }).then(function (response) {
      var res = response.data;

      utils.success('微信支付设置保存成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnResetClick: function (type) {
    if (type === 'mpUrl') {
      this.mpForm.mpUrl = this.mpUrl;
    } else if (type === 'tenPayAuthorizeUrl') {
      this.tenPayForm.tenPayAuthorizeUrl = this.defaultTenPayAuthorizeUrl;
    } else if (type === 'tenPayNotifyUrl') {
      this.tenPayForm.tenPayNotifyUrl = this.defaultTenPayNotifyUrl;
    }
  },
  
  btnMpSubmitClick: function () {
    var $this = this;

    this.$refs.mpForm.validate(function(valid) {
      if (valid) {
        $this.apiMpSubmit();
      }
    });
  },

  btnTenPaySubmitClick: function () {
    var $this = this;

    this.$refs.tenPayForm.validate(function(valid) {
      if (valid) {
        $this.apiTenPaySubmit();
      }
    });
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