var data = {
  pageUser: null,
  pageConfig: null,
  pageAlert: null,
  avatarUrl: null,
  uploadUrl: null,
  files: [],
  editAvatar: false,
  cropper: false,
  styles: []
};

var methods = {
  load: function (pageUser, pageConfig, styles) {
    this.pageUser = pageUser;
    this.pageConfig = pageConfig;
    this.avatarUrl = this.pageUser.avatarUrl || this.pageConfig.homeDefaultAvatarUrl || '../assets/images/default_avatar.png';
    this.uploadUrl = utils.getApiUrl('/v1/users/' + this.pageUser.id + '/avatar?userToken=' + utils.getToken());

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

  submit: function () {
    var $this = this;

    var payload = {};
    for (var i = 0; i < this.styles.length; i++) {
      var style = this.styles[i];
      payload[style.attributeName] = style.value;
    }

    parent.utils.loading(true);
    new utils.Api('/v1/users/' + this.pageUser.id).put(payload, function (err, res) {
      parent.utils.loading(false);

      if (err) {
        $this.pageAlert = {
          type: 'danger',
          html: err.message
        };
        return;
      }

      $this.pageUser = res.value;
      $this.pageAlert = {
        type: 'success',
        html: '个人资料修改成功'
      };

      utils.scrollToTop();
    });
  },

  btnSubmitClick: function (e) {
    e.preventDefault();
    this.pageAlert = null;

    var $this = this;
    this.$validator.validate().then(function (result) {
      if (result) {
        $this.submit();
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