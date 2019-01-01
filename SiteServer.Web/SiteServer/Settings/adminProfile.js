var $url = '/pages/settings/adminProfile';
var $pageTypeAdmin = 'admin';
var $pageTypeUser = 'user';

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: utils.getQueryString('pageType'),
  userId: parseInt(utils.getQueryString('userId') || '0'),
  adminInfo: null,
  departments: null,
  areas: null,
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

      $this.adminInfo = res.value;
      $this.departments = res.departments;
      $this.areas = res.areas;
      $this.uploadUrl = apiUrl + '/pages/settings/adminProfile/upload?adminToken=' + res.adminToken + '&userId=' + $this.userId;
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
      this.adminInfo.avatarUrl = newFile.response.value;
    }
  },

  submit: function () {
    var $this = this;

    pageUtils.loading(true);
    $api.post($url + '?userId=' + $this.userId, _.assign({}, $this.adminInfo, {
      password: $this.password
    })).then(function (response) {
      var res = response.data;

      swal({
        toast: true,
        type: 'success',
        title: "管理员资料保存成功！",
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
  components: {
    FileUpload: VueUploadComponent
  },
  methods: methods,
  created: function () {
    this.getConfig();
  }
});