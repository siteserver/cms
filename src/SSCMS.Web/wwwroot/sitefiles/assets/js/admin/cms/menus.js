var $url = '/cms/open/menus';
var $urlActionsPull = '/cms/open/menus/actions/pull';
var $urlActionsPush = '/cms/open/menus/actions/push';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  menuTypes: null,
  openMenus: null,
  firstMenuIds: null,
  expandRowKeys: [],
  openMenu: null,
  defaultOpeneds: [],
  defaultActive: null
});

var methods = {
  btnSideMenuOpen: function(index) {
    var openMenu = _.find(this.openMenus, function(x){
      return x.id == index;
    });
    this.openMenu = _.assign({}, openMenu);
    this.defaultActive = index;
  },

  btnSideMenuClick: function(val) {
    var sideMenuIds = val + '';
    var ids = sideMenuIds.split('/');
    var menus = this.openMenus;
    var menu = null;
    var defaultOpeneds = [];

    for (var i = 0; i < ids.length; i++) {
      menu = _.find(menus, function(x){
        return x.id == ids[i];
      });
      menus = menu.children;
      defaultOpeneds.push(menu.id);
    }
    this.defaultOpeneds = defaultOpeneds;
    
    if (menu) {
      this.btnMenuClick(menu);
    }
  },

  btnMenuClick: function(menu) {
    this.defaultActive = this.defaultOpeneds.join('/');
    this.openMenu = _.assign({}, menu);
  },

  getIndex: function (level1, level2, level3) {
    if (level3) return level1.id + '/' + level2.id + '/' + level3.id;
    else if (level2) return level1.id + '/' + level2.id;
    else if (level1) return level1.id;
    return '';
  },

  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.menuTypes = res.menuTypes;
      $this.openMenus = $this.getItems(res.openMenus);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDelete: function (id) {
    var $this = this;

    utils.loading(this, true);
    $api.delete($url, {
      data: {
        siteId: this.siteId,
        id: id
      }
    }).then(function (response) {
      var res = response.data;

      $this.openMenus = $this.getItems(res.openMenus);
      $this.openMenu = null;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function (openMenu) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      siteId: this.siteId,
      id: openMenu.id, 
      parentId: openMenu.parentId, 
      taxis: openMenu.taxis, 
      text: openMenu.text, 
      menuType: openMenu.menuType, 
      key: openMenu.key, 
      url: openMenu.url,
      appId: openMenu.appId,
      pagePath: openMenu.pagePath,
      mediaId: openMenu.mediaId
    }).then(function (response) {
      var res = response.data;

      $this.openMenu = null;
      $this.openMenus = $this.getItems(res.openMenus);
      utils.success(openMenu.id === 0 ? '用户菜单新增成功！' : '用户菜单修改成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiPull: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlActionsPull, {
      siteId: this.siteId
    }).then(function (response) {
      var res = response.data;

      utils.success('微信菜单拉取成功！');
      $this.openMenu = null;
      $this.openMenus = $this.getItems(res.openMenus);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiPush: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlActionsPush, {
      siteId: this.siteId
    }).then(function (response) {
      var res = response.data;

      utils.success('微信菜单推送成功，需要24小时后或取消关注后重新关注才能看到效果！');
      $this.openMenu = null;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getItems: function (menus) {
    var openMenus = [];
    for (var i = 0; i < menus.length; i++) {
      var menu = menus[i];
      if (menu.parentId === 0) {
        menu.children = [];
        openMenus.push(menu);
      }
    }
    for (var i = 0; i < menus.length; i++) {
      var menu = menus[i];
      if (menu.parentId > 0) {
        var parent = _.find(openMenus, function (x) {
          return x.id === menu.parentId
        })
        if (parent) {
          parent.children.push(menu);
        }
      }
    }

    this.firstMenuIds = _.map(openMenus, function(x){
      return x.id;
    });

    return openMenus;
  },

  getCardTitle: function(){
    if (this.openMenu.id !== 0) return '修改菜单';
    return this.openMenu.parentId > 0 ? '新增下级菜单' : '新增一级菜单';
  },

  btnAddChildClick: function () {
    var parentId = this.openMenu ? this.openMenu.id : 0;

    var taxis = 0;
    var parent = null;
    if (parentId > 0) {
      parent = this.openMenus.find(function (x) {
        return x.id === parentId;
      })
    }
    if (parent) {
      if (parent.children.length >= 5) {
        return utils.error('子菜单最多创建5个');
      }

      _.forEach(parent.children, function (value) {
        if (value.taxis > taxis) {
          taxis = value.taxis;
        }
      });
    } else if (parentId === 0) {
      _.forEach(this.openMenus, function (value) {
        if (value.taxis > taxis) {
          taxis = value.taxis;
        }
      });
    }

    this.openMenu = {
      id: 0,
      parentId: parentId,
      taxis: taxis + 1,
      text: '',
      menuType: 'click',
      key: '',
      url: '',
      appId: '',
      pagePath: '',
      mediaId: ''
    };

    this.defaultActive = null;
  },

  btnAddFirstClick: function () {
    if (this.openMenus.length >= 3) {
      return utils.error('主菜单最多创建3个');
    }
    var taxis = 0;
    _.forEach(this.openMenus, function (value) {
      if (value.taxis > taxis) {
        taxis = value.taxis;
      }
    });

    this.openMenu = {
      id: 0,
      siteId: this.siteId,
      parentId: 0,
      taxis: taxis + 1,
      text: '',
      menuType: 'click',
      key: '',
      url: '',
      appId: '',
      pagePath: '',
      mediaId: ''
    };

    this.defaultActive = null;
  },

  btnPullClick: function() {
    var $this = this;

    utils.alertDelete({
      title: '拉取微信菜单',
      text: '此操作将获取微信公众号的菜单，当前设置的菜单将被覆盖，确定吗？',
      button: '拉 取',
      callback: function () {
        $this.apiPull();
      }
    });
  },

  btnPushClick: function() {
    var $this = this;

    utils.alertDelete({
      title: '推送微信菜单',
      text: '此操作将推送当前菜单至微信公众号，确定吗？',
      button: '推 送',
      callback: function () {
        $this.apiPush();
      }
    });
  },

  btnDeleteClick: function () {
    var $this = this;

    utils.alertDelete({
      title: '删除用户菜单',
      text: '此操作将删除用户菜单及其下级菜单，确定吗？',
      callback: function () {
        $this.apiDelete($this.openMenu.id);
      }
    });
  },

  btnSubmitClick: function () {
    var $this = this;

    this.$refs.openMenu.validate(function(valid) {
      if (valid) {
        $this.apiSubmit($this.openMenu);
      }
    });
  },

  btnCancelClick: function() {
    this.openMenu = null;
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