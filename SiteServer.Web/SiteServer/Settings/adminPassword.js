var $url = '/pages/settings/adminPassword';
var $pageTypeAdmin = 'admin';
var $pageTypeUser = 'user';

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: utils.getQueryString('pageType'),
  userId: parseInt(utils.getQueryString('userId') || '0'),
  adminInfo: null,
  password: null,
  confirmPassword: null
};

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get($url + '?userId=' + $this.userId).then(function (response) {
      var res = response.data;

      $this.adminInfo = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  submit: function () {
    var $this = this;

    pageUtils.loading(true);
    $api.post($url + '?userId=' + $this.userId, {
      password: $this.password
    }).then(function (response) {
      var res = response.data;

      swal({
        toast: true,
        type: 'success',
        title: "密码更改成功！",
        showConfirmButton: false,
        timer: 3000
      }).then(function () {
        if ($this.pageType == $pageTypeAdmin) {
          $this.btnReturnClick();
        } else {
          top.location.reload(true);
        }
      });
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      pageUtils.loading(false);
    });
  },

  btnSubmitClick: function () {
    this.pageAlert = null;

    var $this = this;
    if (this.adminInfo.id > 0 && this.password != this.confirmPassword) {
      return;
    }
    this.$validator.validate().then(function (result) {
      if (result) {
        $this.submit();
      }
    });
  },

  btnReturnClick: function () {
    location.href = 'pageAdministrator.aspx';
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getConfig();
  }
});