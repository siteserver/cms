var $api = new apiUtils.Api(apiUrl + '/pages/settings/userMenu');
var $apiReset = new apiUtils.Api(apiUrl + '/pages/settings/userMenu/actions/reset');

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: 'list',
  items: null,
  groups: null,
  item: null
};

var methods = {
  getItems: function (menus) {
    var items = [];
    for (var i = 0; i < menus.length; i++) {
      var menu = menus[i];
      menu.isGroup = false;
      menu.groupIds = [];
      if (menu.groupIdCollection) {
        menu.isGroup = true;
        menu.groupIds = menu.groupIdCollection.split(',');
      }
      if (menu.parentId === 0) {
        menu.children = [];
        items.push(menu);
      }
    }
    for (var i = 0; i < menus.length; i++) {
      var menu = menus[i];
      if (menu.parentId > 0) {
        var parent = _.find(items, function (x) {
          return x.id === menu.parentId
        })
        if (parent) {
          parent.children.push(menu);
        }
      }
    }

    return items;
  },
  getList: function () {
    var $this = this;

    $api.get(null, function (err, res) {
      if (err || !res || !res.value) return;

      $this.items = $this.getItems(res.value);
      $this.groups = res.groups;

      $this.pageLoad = true;
    });
  },
  getUserGroups: function (item) {
    if (item.isGroup) {
      var str = '';
      _.forEach(this.groups, function (group) {
        if (item.groupIds.indexOf(group.id + '') !== -1) {
          str += ', ' + group.groupName;
        }
      });
      return str ? str.substr(2) : '';
    }
    return '所有用户组';
  },
  delete: function (id) {
    var $this = this;

    pageUtils.loading(true);
    $api.delete({
      id: id
    }, function (err, res) {
      pageUtils.loading(false);
      if (err || !res || !res.value) return;

      $this.items = $this.getItems(res.value);
    });
  },
  reset: function () {
    var $this = this;

    pageUtils.loading(true);
    $apiReset.post(null, function (err, res) {
      pageUtils.loading(false);
      if (err || !res || !res.value) return;

      $this.items = $this.getItems(res.value);
    });
  },
  submit: function (item) {
    var $this = this;
    item.groupIdCollection = item.isGroup ? item.groupIds.join(',') : '';
    pageUtils.loading(true);
    $api.post(item, function (err, res) {
      pageUtils.loading(false);
      if (err) {
        $this.pageAlert = {
          type: 'danger',
          html: err.message
        };
        return;
      }

      $this.pageAlert = {
        type: 'success',
        html: item.id === -1 ? '用户菜单添加成功！' : '用户菜单修改成功！'
      };
      $this.item = null;
      $this.items = $this.getItems(res.value);
      $this.pageType = 'list';
    });
  },

  btnAddClick: function (parentId) {
    var taxis = 0;
    var parent = null;
    if (parentId > 0) {
      parent = this.items.find(function (x) {
        return x.id === parentId;
      })
    }
    if (parent) {
      _.forEach(parent.children, function (value) {
        if (value.taxis > taxis) {
          taxis = value.taxis;
        }
      });
    } else {
      _.forEach(this.items, function (value) {
        if (value.taxis > taxis) {
          taxis = value.taxis;
        }
      });
    }

    this.item = {
      id: 0,
      systemId: '',
      groupIdCollection: '',
      isDisabled: false,
      parentId: parentId,
      taxis: taxis + 1,
      text: '',
      href: '',
      iconClass: '',
      target: '',
      isGroup: false,
      groupIds: []
    };
    this.pageType = 'add';
  },

  btnResetClick: function () {
    var $this = this;

    pageUtils.alertDelete({
      title: '重置用户菜单',
      text: '此操作将把用户菜单恢复为系统默认值，确定吗？',
      button: '确认重置',
      callback: function () {
        $this.reset();
      }
    });
  },

  btnEditClick: function (item) {
    this.pageType = 'add';
    this.item = item;
  },

  btnDeleteClick: function (item) {
    var $this = this;

    pageUtils.alertDelete({
      title: '删除用户菜单',
      text: '此操作将删除用户菜单 ' + item.text + '，确定吗？',
      callback: function () {
        $this.delete(item.id);
      }
    });
  },
  btnSubmitClick: function () {
    var $this = this;
    this.$validator.validate().then(function (result) {
      if (result) {
        $this.submit($this.item);
      }
    });
  },
  btnCancelClick: function () {
    this.pageType = 'list';
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getList();
  }
});