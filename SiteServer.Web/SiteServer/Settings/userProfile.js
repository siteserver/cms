var $url = '/pages/settings/userProfile';

var data = {
  pageLoad: false,
  pageAlert: null,
  userId: utils.getQueryInt('userId'),
  user: null,
  password: null,
  confirmPassword: null,
  uploadUrl: null,
  files: []
};

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get($url + '?userId=' + $this.userId).then(function (response) {
      var res = response.data;

      $this.user = res.value;
      $this.uploadUrl = apiUrl + '/pages/settings/userProfile/upload?adminToken=' + res.adminToken + '&userId=' + $this.userId;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
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

    utils.loading(true);
    $api.post($url + '?userId=' + $this.userId, _.assign({}, $this.user, {
      password: $this.password
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
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
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
    this.getConfig();
  }
});