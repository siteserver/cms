var $url = '/settings/users';
var $urlExport = $url + '/actions/export';
var $urlUpload = $apiUrl + '/settings/users/actions/import';

var data = utils.init({
  items: null,
  count: null,
  groups: null,
  formInline: {
    state: '',
    groupId: -1,
    order: '',
    lastActivityDate: 0,
    keyword: '',
    currentPage: 1,
    offset: 0,
    limit: 30
  },
  uploadPanel: false,
  uploadLoading: false,
  uploadList: []
});

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get($url, {
      params: this.formInline
    }).then(function (response) {
      var res = response.data;

      $this.items = res.users;
      $this.count = res.count;
      $this.groups = res.groups;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getGroupName: function (groupId) {
    var group = _.find(this.groups, function (x) { return x.id === groupId; });
    return group ? group.groupName :'默认用户组';
  },

  btnViewClick: function(userId) {
    utils.openLayer({
      title: '查看资料',
      url: utils.getCommonUrl('userLayerView', {userId: userId})
    });
  },

  btnAddClick: function() {
    utils.openLayer({
      title: '添加用户',
      url: utils.getSettingsUrl('usersLayerProfile')
    });
  },

  btnEditClick: function(row) {
    utils.openLayer({
      title: '编辑用户',
      url: utils.getSettingsUrl('usersLayerProfile', {userId: row.id})
    });
  },

  btnPasswordClick: function(row) {
    utils.openLayer({
      title: '更改密码',
      url: utils.getSettingsUrl('usersLayerPassword', {userId: row.id})
    });
  },

  btnExportClick: function() {
    var $this = this;
    
    utils.loading(this, true);
    $api.post($urlExport).then(function (response) {
      var res = response.data;

      window.open(res.value);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDelete: function(item) {
    var $this = this;

    utils.loading(this, true);
    $api.delete($url, {
      data: {
        id: item.id
      }
    }).then(function (response) {
      var res = response.data;

      $this.items.splice($this.items.indexOf(item), 1);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnDeleteClick: function (item) {
    var $this = this;

    utils.alertDelete({
      title: '删除用户',
      text: '此操作将删除用户 ' + item.userName + '，确定吗？',
      callback: function () {
        $this.apiDelete(item);
      }
    });
  },

  apiCheck: function (item) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/check', {
      id: item.id
    }).then(function (response) {
      var res = response.data;

      item.checked = true;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnCheckClick: function(item) {
    var $this = this;

    utils.alertWarning({
      title: '审核用户',
      text: '此操作将设置用户 ' + item.userName + ' 的状态为审核通过，确定吗？',
      callback: function () {
        $this.apiCheck(item);
      }
    });
  },

  apiLock: function (item) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/lock', {
      id: item.id
    }).then(function (response) {
      var res = response.data;

      item.locked = true;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnLockClick: function(item) {
    var $this = this;

    utils.alertWarning({
      title: '锁定用户',
      text: '此操作将锁定用户 ' + item.userName + '，确定吗？',
      callback: function () {
        $this.apiLock(item);
      }
    });
  },

  apiUnLock: function (item) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/unLock', {
      id: item.id
    }).then(function (response) {
      var res = response.data;

      item.locked = false;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnUnLockClick: function(item) {
    var $this = this;

    utils.alertWarning({
      title: '解锁用户',
      text: '此操作将解锁用户 ' + item.userName + '，确定吗？',
      callback: function () {
        $this.apiUnLock(item);
      }
    });
  },

  btnSearchClick() {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: this.formInline
    }).then(function (response) {
      var res = response.data;

      $this.items = res.users;
      $this.count = res.count;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  handleCurrentChange: function(val) {
    this.formInline.currentValue = val;
    this.formInline.offset = this.formInline.limit * (val - 1);

    this.btnSearchClick();
  },

  btnImportClick: function() {
    this.uploadPanel = true;
  },

  uploadBefore(file) {
    var isExcel = file.name.indexOf('.xlsx', file.name.length - '.xlsx'.length) !== -1;
    if (!isExcel) {
      utils.error('用户导入文件只能是 Excel 格式!');
    }
    return isExcel;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadSuccess: function(res, file) {
    this.uploadPanel = false;

    var success = res.success;
    var failure = res.failure;
    var errorMessage = res.errorMessage;

    var $this = this;

    $api.get($url, {
      params: this.formInline
    }).then(function (response) {
      var res = response.data;

      $this.items = res.users;
      $this.count = res.count;
      $this.groups = res.groups;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      if (success) {
        utils.success('成功导入 ' + success + ' 名用户！');
      }
      if (errorMessage) {
        utils.error(failure + ' 名用户导入失败：' + errorMessage);
      }
      utils.loading($this, false);
    });
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});