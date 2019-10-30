var $api = new apiUtils.Api(apiUrl + '/pages/settings/admin');

var data = {
  pageLoad: false,
  pageAlert: null,
  drawer: false,
  items: null,
  count: null,
  roles: null,
  isSuperAdmin: null,
  adminId: null,
  formInline: {
    role: '',
    order: '',
    lastActivityDate: 0,
    keyword: '',
    currentPage: 1,
    offset: 0,
    limit: 30
  },
  permissionInfo: {}
};

var methods = {
  apiGetConfig: function () {
    var $this = this;

    $api.get(this.formInline, function (err, res) {
      if (err || !res || !res.value) return;

      $this.items = res.value;
      $this.count = res.count;
      $this.roles = res.roles;
      $this.isSuperAdmin = res.isSuperAdmin;
      $this.adminId = res.adminId;
      $this.pageLoad = true;
    });
  },

  btnEditClick: function(row) {
    location.href = 'adminProfile.cshtml?pageType=admin&userId=' + row.id;
  },

  btnPasswordClick: function(row) {
    location.href = 'adminPassword.cshtml?pageType=admin&userId=' + row.id;
  },

  btnPermissionsClick: function(row) {
    var $this = this;

    pageUtils.loading(true);
    $api.getAt('permissions/' + row.id, null, function (err, res) {
      pageUtils.loading(false);
      if (err || !res || !res.value) return;

      var allRoles = [];
      for (var i = 0; i < res.roles.length; i++) {
        allRoles.push({ 
          key: res.roles[i], 
          label: res.roles[i], 
          disabled: false
        });
      }

      $this.permissionInfo = {
        adminId: row.id,
        allRoles: allRoles,
        allSites: res.allSites,
        
        adminLevel: res.adminLevel,
        checkedSites: res.checkedSites,
        checkedRoles: res.checkedRoles,
        loading: false
      };

      $this.drawer = true;
    });
  },

  btnPermissionSubmitClick: function() {
    var $this = this;
    this.permissionInfo.loading = true;

    $api.postAt('permissions/' + this.permissionInfo.adminId, {
      adminLevel: this.permissionInfo.adminLevel,
      checkedSites: this.permissionInfo.checkedSites,
      checkedRoles: this.permissionInfo.checkedRoles,
    }, function (err, res) {
      if (err || !res || !res.value) return;

      for (var i = 0; i < $this.items.length; i++) {
        var adminInfo = $this.items[i];
        if (adminInfo.id === $this.permissionInfo.adminId) {
          adminInfo.roles = res.roles;
        }
      }

      $this.drawer = false;
    });
  },

  btnDeleteClick: function (item) {
    var $this = this;

    pageUtils.alertDelete({
      title: '删除管理员',
      text: '此操作将删除管理员 ' + item.userName + '，确定吗？',
      callback: function () {
        pageUtils.loading(true);
        $api.delete({
          id: item.id
        }, function (err, res) {
          pageUtils.loading(false);
          if (err || !res || !res.value) return;

          $this.items.splice($this.items.indexOf(item), 1);
        });
      }
    });
  },

  btnLockClick: function(item) {
    var $this = this;

    pageUtils.alertWarning({
      title: '锁定管理员',
      text: '此操作将锁定管理员 ' + item.userName + '，确定吗？',
      callback: function () {
        pageUtils.loading(true);
        $api.postAt('actions/lock', {
          id: item.id
        }, function (err, res) {
          pageUtils.loading(false);
          if (err || !res || !res.value) return;

          item.locked = true;
        });
      }
    });
  },

  btnUnLockClick: function(item) {
    var $this = this;

    pageUtils.alertWarning({
      title: '解锁管理员',
      text: '此操作将解锁管理员 ' + item.userName + '，确定吗？',
      callback: function () {
        pageUtils.loading(true);
        $api.postAt('actions/unLock', {
          id: item.id
        }, function (err, res) {
          pageUtils.loading(false);
          if (err || !res || !res.value) return;

          item.locked = false;
        });
      }
    });
  },

  btnSearchClick() {
    var $this = this;

    pageUtils.loading(true);
    $api.get(this.formInline, function (err, res) {
      pageUtils.loading(false);
      if (err || !res || !res.value) return;

      $this.items = res.value;
      $this.count = res.count;
    });
  },

  handleCurrentChange: function(val) {
    this.formInline.currentValue = val;
    this.formInline.offset = this.formInline.limit * (val - 1);

    this.btnSearchClick();
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGetConfig();
  }
});