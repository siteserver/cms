var $url = '/settings/users';
var $urlTree = $url + '/actions/tree'
var $urlDelete = $url + '/actions/delete';
var $urlExport = $url + '/actions/export';
var $urlSet = $url + '/actions/set';
var $urlUpload = $apiUrl + '/settings/users/actions/import';

var data = utils.init({
  departmentId: utils.getQueryInt("departmentId"),
  root: null,
  expendedNodeIds: [],
  filterText: '',
  total: null,
  pageSize: null,
  page: 1,
  asideHeight: 0,
  tableMaxHeight: 0,
  multipleSelection: [],
  users: [],
  groups: [],
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
  runReload: function(message) {
    this.apiGet(this.departmentId, this.page, message, true);
  },

  apiGet: function(departmentId, page, message, reload) {
    if (typeof departmentId === "undefined") {
      departmentId = this.departmentId;
      page = this.page;
      message = '';
      reload = true;
    }

    var $this = this;
    this.asideHeight = $(window).height() - 4;
    this.tableMaxHeight = $(window).height() - 155;

    utils.loading(this, true);
    $api.post($url, {
      state: this.formInline.state,
      groupId: this.formInline.groupId,
      order: this.formInline.order,
      lastActivityDate: this.formInline.lastActivityDate,
      keyword: this.formInline.keyword,
      departmentId: departmentId,
      page: page,
      pageSize: 30
    }).then(function(response) {
      var res = response.data;

      $this.total = res.total;
      $this.pageSize = res.pageSize;
      $this.page = page;
      $this.expendedNodeIds = [departmentId];
      $this.users = res.users;
      $this.groups = res.groups;

      if (message) {
        utils.success(message);
      }
      utils.loading($this, false);

      if (reload) {
        $this.apiTree();
      }
    }).catch(function(error) {
      utils.error(error);
    }).then(function() {
      $this.scrollToTop();
    });
  },

  apiTree: function() {
    var $this = this;

    $api.post($urlTree).then(function(response) {
      var res = response.data;

      $this.root = [{
        children: res.departments,
        count: res.count,
        disabled: false,
        label : "全体成员",
        department: {imageUrl: null, linkUrl: null, departmentName: "全体成员", departmentType: "Department", parentId: 0},
        value: 0,
      }];
    }).catch(function(error) {
      utils.error(error);
    });
  },

  apiDelete: function(user) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlDelete, {
      id: user.id
    }).then(function (response) {
      var res = response.data;

      $this.users.splice($this.users.indexOf(user), 1);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSet: function (userId, type, value) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlSet, {
      userId: userId,
      type: type,
      value: value
    }).then(function (response) {
      var res = response.data;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnExportClick: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlExport, {
      departmentId: this.departmentId
    }).then(function (response) {
      var res = response.data;

      window.open(res.value);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getNodeUrl: function(data) {
    return utils.getRootUrl('redirect', {
      departmentId: data.value
    });
  },

  btnSearchClick: function() {
    this.apiGet(this.departmentId, 1);
  },

  btnViewClick: function(user) {
    utils.openLayer({
      title: '查看资料',
      url: utils.getCommonUrl('userLayerView', { guid: user.guid })
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
      url: utils.getSettingsUrl('usersLayerProfile', { userId: row.id })
    });
  },

  btnMoreClick: function(args) {
    var $this = this;

    if (args.type === 'Edit') {
      utils.openLayer({
        title: '编辑用户',
        url: utils.getSettingsUrl('usersLayerProfile', {userId: args.user.id})
      });
    } else if (args.type === 'Password') {
      utils.openLayer({
        title: '更改密码',
        url: utils.getSettingsUrl('usersLayerPassword', {userId: args.user.id})
      });
    } else if (args.type === 'Locked') {
      utils.alertDelete({
        title: '锁定用户',
        text: '此操作将锁定用户 ' + args.user.userName + '，确定吗？',
        button: '确 定',
        callback: function () {
          args.user.locked = true;
          $this.apiSet(args.user.id, 'Locked', true);
        }
      });
    } else if (args.type === 'UnLocked') {
      utils.alertDelete({
        title: '解锁用户',
        text: '此操作将解锁用户 ' + args.user.userName + '，确定吗？',
        button: '确 定',
        callback: function () {
          args.user.locked = false;
          $this.apiSet(args.user.id, 'Locked', false);
        }
      });
    } else if (args.type === 'Manager') {
      utils.alertDelete({
        title: '设置为主管',
        text: '此操作将把用户 ' + args.user.userName + ' 设置为主管，确定吗？',
        button: '确 定',
        callback: function () {
          args.user.manager = true;
          $this.apiSet(args.user.id, 'Manager', true);
        }
      });
    } else if (args.type === 'UnManager') {
      utils.alertDelete({
        title: '取消设为主管',
        text: '此操作将把用户 ' + args.user.userName + ' 取消设为主管，确定吗？',
        button: '确 定',
        callback: function () {
          args.user.manager = false;
          $this.apiSet(args.user.id, 'Manager', false);
        }
      });
    } else if (args.type === 'Delete') {
      utils.alertDelete({
        title: '删除用户',
        text: '此操作将删除用户 ' + args.user.userName + '，确定吗？',
        callback: function () {
          $this.apiDelete(args.user);
        }
      });
    }
  },

  btnImportClick: function() {
    this.uploadPanel = true;
  },

  btnLayerClick: function(options) {
    var query = {
      page: this.page
    };

    if (options.departmentId) {
      query.departmentId = options.departmentId;
    } else {
      query.departmentId = this.departmentId;
    }
    if (options.userId) {
      query.userId = options.userId;
    }

    if (options.withUsers) {
      if (!this.isUserCheck) return;
      query.userIds = this.userIdsString;
    }

    options.url = utils.getSettingsUrl('usersLayer' + options.name, query);
    utils.openLayer(options);
  },

  scrollToTop: function() {
    document.documentElement.scrollTop = document.body.scrollTop = 0;
  },

  tableRowClassName: function(scope) {
    if (this.multipleSelection.indexOf(scope.row) !== -1) {
      return 'current-row';
    }
    return '';
  },

  getUploadUrl: function() {
    return $urlUpload + '?departmentId=' + this.departmentId;
  },

  btnNodeClick: function(data) {
    if (data.disabled) return;
    this.departmentId = data.value;
    this.apiGet(data.value, 1);
  },

  filterNode: function(value, data) {
    if (!value) return true;
    return data.label.indexOf(value) !== -1 || data.value + '' === value;
  },

  handleSelectionChange: function(val) {
    this.multipleSelection = val;
  },

  toggleSelection: function(row) {
    this.$refs.multipleTable.toggleRowSelection(row);
  },

  handleCurrentChange: function(val) {
    this.apiGet(this.departmentId, val);
  },

  btnCloseClick: function() {
    utils.removeTab();
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
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  computed: {
    isUserCheck: function() {
      return this.multipleSelection.length > 0;
    },

    userIds: function() {
      var retVal = [];
      for (var i = 0; i < this.multipleSelection.length; i++) {
        var user = this.multipleSelection[i];
        retVal.push({
          id: user.id
        });
      }
      return retVal;
    },

    userIdsString: function() {
      var retVal = [];
      for (var i = 0; i < this.multipleSelection.length; i++) {
        var user = this.multipleSelection[i];
        retVal.push(user.id);
      }
      return retVal.join(",");
    }
  },
  watch: {
    filterText: function(val) {
      this.$refs.tree.filter(val);
    }
  },
  created: function() {
    utils.keyPress(null, this.btnCloseClick);
    this.apiGet();
  }
});
