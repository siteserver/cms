var $url = '/wx/users';
var $urlWx = '/wx/users/wx';
var $urlTenPay = '/wx/users/tenPay';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  success: false,
  errorMessage: null,
  tags: null,
  users: null,
  tagId: 0,
  form: {
    keyword: null,
  }
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

      $this.success = res.success;
      $this.errorMessage = res.errorMessage;
      $this.tags = res.tags;
      $this.users = res.users;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiWxSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlWx, {
      siteId: this.siteId,
      wxAppId: this.account.wxAppId,
      wxAppSecret: this.account.wxAppSecret,
      wxUrl: this.account.wxUrl,
      wxToken: this.account.wxToken,
      wxIsEncrypt: this.account.wxIsEncrypt,
      wxEncodingAESKey: this.account.wxEncodingAESKey
    }).then(function (response) {
      var res = response.data;

      utils.success('微信公众号设置保存成功！');
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
      tenPayAppId: this.account.tenPayAppId,
      tenPayAppSecret: this.account.tenPayAppSecret,
      tenPayMchId: this.account.tenPayMchId,
      tenPayKey: this.account.tenPayKey,
      tenPayAuthorizeUrl: this.account.tenPayAuthorizeUrl,
      tenPayNotifyUrl: this.account.tenPayNotifyUrl
    }).then(function (response) {
      var res = response.data;

      utils.success('微信支付设置保存成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSearchClick: function() {

  },

  btnEditClick: function (type) {
    if (type === 'wxUrl') {
      this.account.wxUrl = this.defaultWxUrl;
    } else if (type === 'tenPayAuthorizeUrl') {
      this.account.tenPayAuthorizeUrl = this.defaultTenPayAuthorizeUrl;
    } else if (type === 'tenPayNotifyUrl') {
      this.account.tenPayNotifyUrl = this.defaultTenPayNotifyUrl;
    }
  },
  
  btnDefaultClick: function () {
    var $this = this;

    this.$refs.account.validate(function(valid) {
      if (valid) {
        $this.apiWxSubmit();
      }
    });
  },

  btnDeleteClick: function () {
    var $this = this;

    this.$refs.account.validate(function(valid) {
      if (valid) {
        $this.apiTenPaySubmit();
      }
    });
  },

  btnAddClick: function () {

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