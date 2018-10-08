var data = {
  pageConfig: null,
  pageUser: null,
  pageAlert: null,
  avatarUrl: null,
  uploadUrl: null,
  files: [],
  editAvatar: false,
  cropper: false,
  accountAlertType: "",
  accountAlertMessage: "",
  mobile: "",
  code: "",
  isAccountSubmit: false,
  passwordAlertType: "",
  passwordAlertMessage: "",
  isPasswordSubmit: false,
  oldPassword: "",
  newPassword: "",
  confirmPassword: "",
  deleteAlertMessage: ""
};

var methods = {
  load: function (pageConfig, styles) {
    this.pageConfig = pageConfig;
    this.pageUser = authUtils.getUser();
    this.avatarUrl = this.pageUser.avatarUrl || this.pageConfig.homeDefaultAvatarUrl || '../assets/images/default_avatar.png';
    this.uploadUrl = apiUrl + '/v1/users/' + this.pageUser.id + '/avatar?userToken=' + authUtils.getToken();

    this.styles = styles;
    for (var i = 0; i < this.styles.length; i++) {
      var style = this.styles[i];
      style.value = this.pageUser[_.camelCase(style.attributeName)];
    }
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
      authUtils.saveUser(newFile.response.value);
      this.avatarUrl = newFile.response.value.avatarUrl;
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
    if (this.errors.any("account")) return;
    this.isAccountSubmit = true;
    if (
      !this.user.userName ||
      !this.user.mobile ||
      (this.mobile !== this.user.mobile && !this.code)
    )
      return;

    this.isAccountLoading = true;
    this.accountAlertMessage = "";

    Users.update(this.user)
      .then(response => {
        this.isAccountLoading = false;
        this.mobile = response.data.mobile;
        this.$store.dispatch("update", response.data);
        this.accountAlertType = "success";
        this.accountAlertMessage = "个人资料设置成功";
      })
      .catch(error => {
        this.isAccountLoading = false;
        this.accountAlertType = "danger";
        this.accountAlertMessage = error.response.data.message;
      });
  },

  updatePassword: function () {
    this.isPasswordSubmit = true;
    if (
      !this.oldPassword ||
      !this.newPassword ||
      !this.confirmPassword ||
      this.newPassword !== this.confirmPassword
    )
      return;

    this.isPasswordLoading = true;
    this.passwordAlertMessage = "";

    Users.resetPassword(
        this.user.userName,
        this.oldPassword,
        this.newPassword
      )
      .then(() => {
        this.isPasswordLoading = false;
        this.passwordAlertType = "success";
        this.passwordAlertMessage = "新密码设置成功";
      })
      .catch(error => {
        this.isPasswordLoading = false;
        this.passwordAlertType = "danger";
        this.passwordAlertMessage = error.response.data.message;
      });
  },

  deleteAccount: function () {
    var $this = this;

    swal({
      title: "永久关闭账户",
      text: "帐户关闭操作无法撤销，此操作会删除您的帐户以及帐户中的所有数据",
      icon: "warning",
      buttons: {
        cancel: {
          text: "取 消",
          value: null,
          visible: true,
          className: "",
          closeModal: true
        },
        confirm: {
          text: "永久关闭账户",
          value: true,
          visible: true,
          className: "",
          closeModal: true
        }
      },
      dangerMode: true
    }).then(function (willDelete) {
      if (willDelete) {
        pageUtils.loading(true);
        $this.deleteAlertMessage = "";

        new apiUtils.Api(apiUrl + '/v1/users/' + $this.pageUser.id).delete(null, function (err, res) {
          pageUtils.loading(false);

          if (err) {
            $this.deleteAlertMessage = error.response.data.message;
            return;
          }

          swal({
            title: "账户已关闭",
            icon: "success"
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
    if (authUtils.isAuthenticated()) {
      pageUtils.getConfig('profile', function (res) {
        if (res.isUserLoggin) {
          $this.load(res.value, res.styles);
        } else {
          authUtils.redirectLogin();
        }
      });
    } else {
      authUtils.redirectLogin();
    }
  }
});