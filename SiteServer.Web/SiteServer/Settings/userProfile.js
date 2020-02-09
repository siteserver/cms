var $url = '/pages/settings/userProfile';

var data = utils.initData({
  userId: utils.getQueryInt('userId'),
  user: null,
  password: null,
  confirmPassword: null,
  uploadUrl: null,
  files: []
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        userId: this.userId
      }
    }).then(function (response) {
      var res = response.data;

      $this.user = res.value;
      $this.uploadUrl = apiUrl + $url + '/upload?adminToken=' + res.adminToken + '&userId=' + $this.userId;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  inputAvatar(newFile, oldFile) {
    if (Boolean(newFile) !== Boolean(oldFile) || oldFile.error !== newFile.error) {
      if (!this.$refs.avatar.active) {
        this.$refs.avatar.active = true
      }
    }

    if (newFile && oldFile && newFile.xhr && newFile.success !== oldFile.success) {
      this.user.avatarUrl = newFile.response.value;
    }
  },

  submit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '?userId=' + this.userId, _.assign({}, this.user, {
      password: this.password
    })).then(function (response) {
      var res = response.data;

      swal({
        toast: true,
        type: 'success',
        title: "用户资料保存成功！",
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
  components: {
    FileUpload: VueUploadComponent
  },
  methods: methods,
  created: function () {
    this.apiGet();
  }
});