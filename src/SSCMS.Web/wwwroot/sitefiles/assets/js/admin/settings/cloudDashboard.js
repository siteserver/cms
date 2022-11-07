var $url = "/settings/cloudDashboard"
var $urlDisconnect = $url + '/actions/disconnect';

var data = utils.init({
  isConnect: false,
  iFrameUrl: '',
});

var methods = {
  getUserName: function() {
    return $cloudUserName;
  },

  apiDisconnect: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlDisconnect).then(function (response) {
      var res = response.data;

      if (!res.value) return;
      cloud.logout();

      utils.alertSuccess({
        title: "云助手连接已断开!",
        button: "确 定",
        callback: function() {
          location.href = utils.getSettingsUrl('cloudConnect', {redirect: location.href});
        }
      });
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnDisconnectClick: function() {
    var $this = this;

    utils.alertDelete({
      title: '断开云助手连接',
      text: '断开连接后云助手相关功能将无法使用，是否确定要断开连接？',
      button: '断开连接',
      callback: function () {
        $this.apiDisconnect();
      }
    });
  },

  btnTicketClick: function () {
    window.open('https://sscms.com/home/#/my/tickets');
  },

  btnDocsClick: function () {
    window.open('https://sscms.com/docs/v7/');
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    var $this = this;
    cloud.checkAuth(function() {
      utils.loading($this, false);
    });
  }
});
