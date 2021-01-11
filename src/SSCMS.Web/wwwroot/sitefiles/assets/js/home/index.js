var $url = '/index';

var $sidebarWidth = 200;
var $collapseWidth = 60;

var data = utils.init({
  homeLogoUrl: null,
  homeTitle: null,
  menus: [],
  user: null,

  menu: null,

  defaultOpeneds: [],
  defaultActive: null,
  tabName: null,
  tabs: [],
  winHeight: 0,
  winWidth: 0,
  isCollapse: false,
  isDesktop: true,
  isMobileMenu: false,

  contextMenuVisible: false,
  contextTabName: null,
  contextLeft: 0,
  contextTop: 0
});

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;
      if (res.user) {

        $this.user = res.user;

        $this.homeLogoUrl = res.homeLogoUrl || utils.getAssetsUrl('images/logo.png');
        $this.homeTitle = res.homeTitle || '用户中心';
        $this.menus = res.menus;
        
        var sideMenuIds = [];
        if (location.hash) {
          var ids = location.hash.substr(1).split('/');
          var topMenuId = ids[0];
          $this.menu = _.find($this.menus, function (x) {
            return x.id == topMenuId;
          });
          for (var i = 1; i < ids.length; i++) {
            if (!ids[i]) break;
            sideMenuIds.push(ids[i]);
          }
        }
        if (!$this.menu && $this.menus.length > 0) {
          $this.menu = $this.menus[0];
        }

        if ($this.menu) {
          $this.btnTopMenuClick($this.menu);
          if (sideMenuIds.length > 0) {
            $this.btnSideMenuClick(sideMenuIds.join('/'));
          }
        }

        document.title = $this.homeTitle;

        setTimeout($this.ready, 100);
      } else {
        location.href = utils.getRootUrl('login');
      }
    }).catch(function (error) {
      utils.error(error);
    });
  },

  ready: function () {
    window.onresize = this.winResize;
    window.onresize();

    utils.loading(this, false);
  },

  openContextMenu: function(e) {
    if (e.srcElement.id && _.startsWith(e.srcElement.id, 'tab-')) {
      this.contextTabName = _.trimStart(e.srcElement.id, 'tab-');
      this.contextMenuVisible = true;
      this.contextLeft = e.clientX;
      this.contextTop = e.clientY;
    }
  },

  closeContextMenu: function() {
    this.contextMenuVisible = false;
  },

  btnContextClick: function(command) {
    var $this = this;
    if (command === 'this') {
      this.tabs = this.tabs.filter(function(tab) {
        return tab.name !== $this.contextTabName;
      });
    } else if (command === 'others') {
      this.tabs = this.tabs.filter(function(tab) {
        return tab.name === $this.contextTabName;
      });
      utils.openTab($this.contextTabName);
    } else if (command === 'left') {
      var isTab = false;
      this.tabs = this.tabs.filter(function(tab) {
        if (tab.name === $this.contextTabName) {
          isTab = true;
        }
        return tab.name === $this.contextTabName || isTab;
      });
    } else if (command === 'right') {
      var isTab = false;
      this.tabs = this.tabs.filter(function(tab) {
        if (tab.name === $this.contextTabName) {
          isTab = true;
        }
        return tab.name === $this.contextTabName || !isTab;
      });
    } else if (command === 'all') {
      this.tabs = [];
    }
    this.closeContextMenu();
  },

  winResize: function () {
    this.winHeight = $(window).height();
    this.winWidth = $(window).width();
    this.isDesktop = this.winWidth > 992;
  },

  getIndex: function (level1, level2, level3) {
    if (level3) return level1.id + '/' + level2.id + '/' + level3.id;
    else if (level2) return level1.id + '/' + level2.id;
    else if (level1) return level1.id;
    return '';
  },

  btnTopMenuClick: function (menu) {
    if (menu.children && menu.children.length > 0) {
      for(var i = 0; i < menu.children.length; i++) {
        var child = menu.children[i];
        if (child.children) {
          this.defaultOpeneds = [child.id];
          break;
        }
      }
    } else {
      this.btnMenuClick(menu);
    }
    this.menu = menu;
  },

  btnSideMenuClick: function(sideMenuIds) {
    var ids = sideMenuIds.split('/');
    var menus = this.menu.children;
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
    location.hash = this.menu.id + '/' + sideMenuIds;
  },

  btnMenuClick: function(menu) {
    this.defaultActive = this.defaultOpeneds.join('/');
    this.isMobileMenu = false;
    if (menu.target == '_layer') {
      utils.openLayer({
        title: menu.text,
        url: menu.link,
        full: true
      });
    } else if (menu.target == '_self') {
      location.href = menu.link;
    } else if (menu.target == '_parent') {
      parent.location.href = menu.link;
    }  else if (menu.target == '_top') {
      top.location.href = menu.link;
    } else if (menu.target == '_blank') {
      window.open(menu.link);
    } else {
      utils.addTab(menu.text, menu.link);
    }
  },

  btnMobileMenuClick: function () {
    this.isCollapse = false;
    this.isMobileMenu = !this.isMobileMenu;
  },

  btnUserMenuClick: function (command) {
    if (command === 'profile') {
      utils.addTab('修改资料', utils.getPageUrl(null, 'profile'));
    } else if (command === 'password') {
      utils.addTab('更改密码', utils.getPageUrl(null, 'password'));
    } else if (command === 'logout') {
      location.href = utils.getRootUrl('logout');
    }
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  },
  computed: {
    leftWidth: function () {
      if (this.isDesktop) {
        return this.isCollapse ? $collapseWidth : $sidebarWidth;
      }
      return this.isMobileMenu ? this.winWidth : 0;
    }
  }
});