var $api = new apiUtils.Api(apiUrl + '/pages/settings/user');

var data = {
  pageLoad: false,
  pageAlert: null,
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
  }
};

var methods = {
  apiGetConfig: function () {
    var $this = this;

    $api.get(this.formInline, function (err, res) {
      if (err || !res || !res.value) return;

      $this.items = res.value;
      $this.count = res.count;
      $this.groups = res.groups;
      $this.pageLoad = true;
    });
  },

  btnEditClick: function(row) {
    location.href = 'userProfile.cshtml?userId=' + row.id;
  },

  btnPasswordClick: function(row) {
    location.href = 'userPassword.cshtml?userId=' + row.id;
  },

  btnExportClick: function() {
    utils.loading(true);
    $api.postAt('actions/export', null, function (err, res) {
      utils.loading(false);
      if (err || !res || !res.value) return;

      window.open(res.value);
    });
  },

  btnDeleteClick: function (item) {
    var $this = this;

    utils.alertDelete({
      title: '删除用户',
      text: '此操作将删除用户 ' + item.userName + '，确定吗？',
      callback: function () {
        utils.loading(true);
        $api.delete({
          id: item.id
        }, function (err, res) {
          utils.loading(false);
          if (err || !res || !res.value) return;

          $this.items.splice($this.items.indexOf(item), 1);
        });
      }
    });
  },

  btnCheckClick: function(item) {
    utils.alertWarning({
      title: '审核用户',
      text: '此操作将设置用户 ' + item.userName + ' 的状态为审核通过，确定吗？',
      callback: function () {
        utils.loading(true);
        $api.postAt('actions/check', {
          id: item.id
        }, function (err, res) {
          utils.loading(false);
          if (err || !res || !res.value) return;

          item.locked = true;
        });
      }
    });
  },

  btnLockClick: function(item) {
    utils.alertWarning({
      title: '锁定用户',
      text: '此操作将锁定用户 ' + item.userName + '，确定吗？',
      callback: function () {
        utils.loading(true);
        $api.postAt('actions/lock', {
          id: item.id
        }, function (err, res) {
          utils.loading(false);
          if (err || !res || !res.value) return;

          item.locked = true;
        });
      }
    });
  },

  btnUnLockClick: function(item) {
    utils.alertWarning({
      title: '解锁用户',
      text: '此操作将解锁用户 ' + item.userName + '，确定吗？',
      callback: function () {
        utils.loading(true);
        $api.postAt('actions/unLock', {
          id: item.id
        }, function (err, res) {
          utils.loading(false);
          if (err || !res || !res.value) return;

          item.locked = false;
        });
      }
    });
  },

  btnSearchClick() {
    var $this = this;

    utils.loading(true);
    $api.get(this.formInline, function (err, res) {
      utils.loading(false);
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