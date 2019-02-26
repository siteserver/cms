var $guid = utils.getQueryString('guid');

var $urlLogin = '/pages/connect/index/actions/login';
var $urlConnect = '/pages/connect/index/actions/connect';
var $urlGetCaptcha = '/v1/captcha/LOGIN-CAPTCHA';
var $urlCheckCaptcha = '/v1/captcha/LOGIN-CAPTCHA/actions/check';

var $data = {
  pageLoad: false,
  pageAlert: null,
  pageSubmit: false,
  account: null,
  password: null,
  captcha: null,
  captchaUrl: null,
  isNightlyUpdate: null,
  apiPrefix: null,
  adminDirectory: null,
  homeDirectory: null,
  secretKey: null,
  cmsVersion: null,
  pluginVersion: null,
  apiVersion: null,
  adminName: null,
  adminToken: null,
  repositoryOwner: null,
  repositoryName: null,
  repositoryToken: null
};

var $methods = {
  reload: function () {
    this.pageLoad = true;
    this.captcha = '';
    this.pageSubmit = false;
    this.captchaUrl = $apiUrl + $urlGetCaptcha + '?r=' + new Date().getTime();
  },

  apiCheckCaptcha: function () {
    var $this = this;

    utils.loading(true);
    $api.post($urlCheckCaptcha, {
      captcha: $this.captcha
    }).then(function (response) {
      $this.apiLogin();
    }).catch(function (error) {
      utils.loading(false);
      $this.reload();
      $this.pageAlert = utils.getPageAlert(error);
    });
  },

  apiLogin: function () {
    var $this = this;

    $api.post($urlLogin, {
      account: $this.account,
      password: md5($this.password)
    }).then(function (response) {
      var res = response.data;

      $this.isNightlyUpdate = res.isNightlyUpdate;
      $this.apiPrefix = res.apiPrefix;
      $this.adminDirectory = res.adminDirectory;
      $this.homeDirectory = res.homeDirectory;
      $this.secretKey = res.secretKey;
      $this.cmsVersion = res.cmsVersion;
      $this.pluginVersion = res.pluginVersion;
      $this.apiVersion = res.apiVersion;
      $this.adminName = res.adminName;
      $this.adminToken = res.adminToken;

      $this.ssApiConnect();
    }).catch(function (error) {
      utils.loading(false);
      $this.pageAlert = utils.getPageAlert(error);
    });
  },

  ssApiConnect: function () {
    var $this = this;

    $ssApi.post($ssUrlConnections, {
      guid: $guid,
      isNightlyUpdate: $this.isNightlyUpdate,
      apiPrefix: $this.apiPrefix,
      adminDirectory: $this.adminDirectory,
      homeDirectory: $this.homeDirectory,
      secretKey: $this.secretKey,
      cmsVersion: $this.cmsVersion,
      pluginVersion: $this.pluginVersion,
      apiVersion: $this.apiVersion,
      adminName: $this.adminName,
      adminToken: $this.adminToken
    }).then(function (response) {
      var res = response.data;

      $this.repositoryOwner = res.repositoryOwner;
      $this.repositoryName = res.repositoryName;
      $this.repositoryToken = res.repositoryToken;

      $this.apiConnect();
    }).catch(function (error) {
      utils.loading(false);
      $this.pageAlert = utils.getPageAlert(error);
    });
  },

  apiConnect: function () {
    var $this = this;

    $api.post($urlConnect, {
      repositoryOwner: $this.repositoryOwner,
      repositoryName: $this.repositoryName,
      repositoryToken: $this.repositoryToken
    }).then(function (response) {
      var res = response.data;

      location.href = ssUtils.getConnectUrl($guid);
    }).catch(function (error) {
      utils.loading(false);
      $this.pageAlert = utils.getPageAlert(error);
    });
  },

  btnConnectClick: function (e) {
    e.preventDefault();

    this.pageSubmit = true;
    this.pageAlert = null;
    if (!this.account || !this.password || !this.captcha) return;
    this.apiCheckCaptcha();
  }
}

new Vue({
  el: '#main',
  data: $data,
  directives: {
    focus: {
      inserted: function (el) {
        el.focus()
      }
    }
  },
  methods: $methods,
  created: function () {
    this.reload();
  }
});