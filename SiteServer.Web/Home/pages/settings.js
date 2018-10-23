var data = {
  pageUser: null,
  pageConfig: null,
  pageAlert: null,
  avatarUrl: null,
  uploadUrl: null,
  files: [],
  editAvatar: false,
  cropper: false,
  accountAlertType: "",
  accountAlertMessage: "",
  mobile: "",
  email: "",
  passwordAlertType: "",
  passwordAlertMessage: "",
  oldPassword: "",
  newPassword: "",
  confirmPassword: "",
  deleteAlertMessage: ""
};

var methods = {
  load: function (pageUser, pageConfig, styles) {
    this.pageUser = pageUser;
    this.pageConfig = pageConfig;
    this.avatarUrl = this.pageUser.avatarUrl || this.pageConfig.homeDefaultAvatarUrl || '../assets/images/default_avatar.png';
    this.uploadUrl = utils.getApiUrl('/v1/users/' + this.pageUser.id + '/avatar?userToken=' + utils.getToken());

    this.mobile = this.pageUser.mobile;
    this.email = this.pageUser.email;
  },

  editSave: function () {
    this.pageAlert = null;
    this.editAvatar = false;
    var oldFile = this.files[0];
    var binStr = atob(this.cropper.getCroppedCanvas().toDataURL(oldFile.type).split(',')[1]);
    var arr = new Uint8Array(binStr.length);
    for (var i = 0; i < binStr.length; i++) {
      arr[i] = binStr.charCodeAt(i);
    }
    var file = new File([arr], oldFile.name, {
      type: oldFile.type
    });
    this.$refs.upload.update(oldFile.id, {
      file,
      type: file.type,
      size: file.size,
      active: true
    });
  },

  inputFile: function (newFile, oldFile, prevent) {
    if (newFile && !oldFile) {
      this.$nextTick(function () {
        this.editAvatar = true;
      });
    }

    if (!newFile && oldFile) {
      this.editAvatar = false;
    }

    if (newFile && oldFile && newFile.xhr && newFile.success !== oldFile.success) {
      this.pageUser = newFile.response.value;
      this.avatarUrl = this.pageUser.avatarUrl;
    }
  },

  inputFilter: function (newFile, oldFile, prevent) {
    if (newFile && !oldFile) {
      if (!/\.(gif|jpg|jpeg|png|webp)$/i.test(newFile.name)) {
        return prevent();
      }
    }
    if (newFile && (!oldFile || newFile.file !== oldFile.file)) {
      newFile.url = '';
      var URL = window.URL || window.webkitURL;
      if (URL && URL.createObjectURL) {
        newFile.url = URL.createObjectURL(newFile.file);
      }
    }
  },

  updateAccount: function () {
    var $this = this;
    this.accountAlertMessage = "";

    this.$validator.validate("account.*").then(function (result) {
      if (result) {
        parent.utils.loading(true);
        new utils.Api('/v1/users/' + $this.pageUser.id).put({
          mobile: $this.mobile,
          email: $this.email
        }, function (err, res) {
          parent.utils.loading(false);

          if (err) {
            $this.accountAlertType = "danger";
            $this.accountAlertMessage = err.message;
            return;
          }

          $this.pageUser = res.value;
          $this.accountAlertType = "success";
          $this.accountAlertMessage = '登录账号设置修改成功';
        });
      }
    });
  },

  updatePassword: function () {
    var $this = this;
    this.passwordAlertMessage = "";

    this.$validator.validate("password.*").then(function (result) {
      if (result) {
        parent.utils.loading(true);
        new utils.Api('/v1/users/' + $this.pageUser.id + '/actions/resetPassword').post({
          password: $this.oldPassword,
          newPassword: $this.newPassword
        }, function (err, res) {
          parent.utils.loading(false);

          if (err) {
            $this.passwordAlertType = "danger";
            $this.passwordAlertMessage = err.message;
            return;
          }

          $this.pageUser = res.value;
          $this.passwordAlertType = "success";
          $this.passwordAlertMessage = '新密码设置成功';
        });
      }
    });
  },

  deleteAccount: function () {
    var $this = this;

    alert({
      title: "永久删除账户",
      text: "帐户删除操作无法撤销，此操作会删除您的帐户以及帐户中的所有数据",
      type: "warning",
      confirmButtonClass: 'btn btn-danger',
      confirmButtonText: '永久删除账户',
      showCancelButton: true,
      cancelButtonText: '取 消'
    }).then(function (result) {
      if (result.value) {
        parent.utils.loading(true);
        $this.deleteAlertMessage = "";

        new utils.Api('/v1/users/' + $this.pageUser.id).delete(null, function (err, res) {
          parent.utils.loading(false);

          if (err) {
            $this.deleteAlertMessage = error.response.data.message;
            return;
          }

          alert({
            title: "账户已关闭",
            type: "success",
            showConfirmButton: false
          }).then(function () {
            location.href = 'login.html';
          });
        });
      }
    });
  }
}

new Vue({
  el: '#main',
  data: data,
  components: {
    FileUpload: VueUploadComponent
  },
  watch: {
    editAvatar: function (value) {
      if (value) {
        this.$nextTick(function () {
          if (!this.$refs.editImage) {
            return;
          }
          var cropper = new Cropper(this.$refs.editImage, {
            aspectRatio: 1 / 1,
            viewMode: 1,
          });
          this.cropper = cropper;
        })
      } else {
        if (this.cropper) {
          this.cropper.destroy();
          this.cropper = false;
        }
      }
    }
  },
  methods: methods,
  created: function () {
    var $this = this;
    utils.getConfig('profile', function (res) {
      if (res.value) {
        $this.load(res.value, res.config, res.styles);
      } else {
        utils.redirectLogin();
      }
    });
  }
});