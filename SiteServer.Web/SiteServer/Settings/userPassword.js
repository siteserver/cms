var $url = '/pages/settings/userPassword';

var data = utils.initData({
  userId: utils.getQueryInt('userId'),
  user: null,
  password: null,
  confirmPassword: null
});

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get($url + '?userId=' + $this.userId).then(function (response) {
      var res = response.data;

      $this.user = res.value;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  submit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '?userId=' + this.userId, {
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
        $this.btnReturnClick();
      });
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function () {
    this.pageAlert = null;

    var $this = this;
    if (this.user.id > 0 && this.password != this.confirmPassword) {
      return;
    }
    this.$validator.validate().then(function (result) {
      if (result) {
        $this.submit();
      }
    });
  },

  btnReturnClick: function () {
    location.href = 'user.cshtml';
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