var $url = "/clouds/connect"

var data = utils.init({
  redirect: utils.getQueryString('redirect'),
  isConnect: false,
  iFrameUrl: '',
});

var methods = {
  btnConnectClick: function() {
    var $this = this;

    this.iFrameUrl = cloud.host + '/auth.html';
    this.isConnect = true;

    window.addEventListener(
      'message',
      function(e) {
        if (e.origin !== cloud.host) return;
        var userId = e.data.userId;
        var userName = e.data.userName;
        var mobile = e.data.mobile;
        var token = e.data.token;
        if (userId && userName && token) {
          $this.apiSubmit(userId, userName, mobile, token);
        }
      },
      false,
    );
  },

  apiGet: function() {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      if (res.userName && res.token) {
        cloud.login(res.userName, res.token);
        location.href = $this.redirect;
      }
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function(userId, userName, mobile, token) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      userId: userId,
      userName: userName,
      mobile: mobile,
      token: token,
    }).then(function (response) {
      var res = response.data;
      if (!res.value) return;

      cloud.login(userName, token);
      location.href = $this.redirect;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnCloseClick: function() {
    utils.removeTab();
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(null, this.btnCloseClick);
    // if (!$cloudToken || !$cloudUserName) {
    //   this.apiGet();
    // } else {
      utils.loading(this, false);
    // }
  }
});
