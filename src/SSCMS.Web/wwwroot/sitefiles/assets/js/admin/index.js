if (window.top != self) {
  window.top.location = self.location;
}

var $url = '/index';
var $idSite = 'site';
var $sidebarWidth = 200;
var $collapseWidth = 60;

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  sessionId: localStorage.getItem('sessionId'),
  cmsVersion: null,
  adminLogoUrl: null,
  adminTitle: null,
  isSuperAdmin: null,
  culture: null,
  plugins: null,
  menus: [],
  siteType: null,
  siteUrl: null,
  previewUrl: null,
  local: null,
  menu: null,
  keyword: null,
  newCms: null,
  newPlugins: [],

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

    $api.get($url, {
      params: {
        siteId: this.siteId,
        sessionId: this.sessionId
      }
    }).then(function (response) {
      var res = response.data;

      if (res.value) {
        utils.addTab('首页', utils.getRootUrl('dashboard'));

        $this.cmsVersion = res.cmsVersion;
        $this.adminLogoUrl = res.adminLogoUrl || utils.getAssetsUrl('images/logo.png');
        $this.adminTitle = res.adminTitle || 'SS CMS';
        $this.isSuperAdmin = res.isSuperAdmin;
        $this.culture = res.culture;
        $this.plugins = res.plugins;
        $this.menus = res.menus;
        $this.siteType = res.siteType;
        $this.siteUrl = res.siteUrl;
        $this.previewUrl = res.previewUrl;
        $this.local = res.local;
        
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

        $this.btnTopMenuClick($this.menu);
        if (sideMenuIds.length > 0) {
          $this.btnSideMenuClick(sideMenuIds.join('/'));
        }

        document.title = $this.adminTitle;
        setTimeout($this.ready, 100);
      } else {
        location.href = res.redirectUrl;
      }
    }).catch(function (error) {
      utils.error(error);
    });
  },

  apiCache: function() {
    if (this.siteId === 0) return;
    $api.post($url + '/actions/cache', {
      siteId: this.siteId
    }).then(function (response) {
      var res = response.data;
      
    }).catch(function (error) {
      utils.error(error);
    });
  },

  ready: function () {
    window.onresize = this.winResize;
    window.onresize();

    this.apiCache();

    if (this.isSuperAdmin) {
      this.getUpdates();
    }

    utils.loading(this, false);
  },

  getUpdates: function () {
    var $this = this;

    var pluginIds = this.plugins.map(function (x){ return x.pluginId});
    cloud.getUpdates($this.cmsVersion, pluginIds).then(function (response) {
      var res = response.data;

      var cms = res.cms;
      var releases = res.releases;

      if (cms) {
        if (cloud.compareVersion($this.cmsVersion, cms.version) === -1) {
          $this.newCms = {
            current: $this.cmsVersion,
            version: cms.version,
            createdDate: cms.createdDate
          };
        }
      }

      for (var i = 0; i < releases.length; i++) {
        var release = releases[i];
        if (!release || !release.version) continue;
        
        var installedPlugins = $.grep($this.plugins, function (e) {
          return e.pluginId == release.userName + '.' + release.name;
        });
        if (installedPlugins.length == 1) {
          var installed = installedPlugins[0];
          if (installed.version) {
            if (cloud.compareVersion(installed.version, release.version) == -1) {
              $this.newPlugins.push({
                pluginId: release.userName + '.' + release.name,
                displayName: installed.displayName,
                current: installed.version,
                version: release.version,
                createdDate: release.createdDate
              });
            }
          } else {
            $this.newPlugins.push({
              pluginId: release.userName + '.' + release.name,
              displayName: installed.displayName,
              current: '0.0',
              version: release.version,
              createdDate: release.createdDate
            });
          }
        }
      }
    });
  },

  openContextMenu: function(e) {
    if (e.srcElement.id && _.startsWith(e.srcElement.id, 'tab-')) {
      this.contextTabName = _.trimStart(e.srcElement.id, 'tab-');
      this.contextMenuVisible = true;
      this.contextLeft = e.clientX;
      if (e.clientX + 130 > this.winWidth) {
        this.contextLeft = this.winWidth - 130;
      } else {
        this.contextLeft = e.clientX;
      }
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

  getHostUrl: function() {
    return cloud.host;
  },

  btnSearchClick: function() {
    if (!this.keyword) return;

    utils.addTab('内容搜索', utils.getCmsUrl('contentsSearch', {
      siteId: this.siteId,
      keyword: this.keyword
    }));
  },

  btnTopMenuClick: function (menu) {
    if (!menu) return;
    if (menu.children && menu.children.length > 0) {
      var first = menu.children[0];
      if (first.children) {
        this.defaultOpeneds = [first.id];
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
      if (menu) {
        menus = menu.children;
        defaultOpeneds.push(menu.id);
      }
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
    if (command === 'view') {
      utils.openLayer({
        title: '查看资料',
        url: utils.getSettingsUrl('administratorsLayerView', {pageType: 'user', userId: this.local.userId}),
        full: true
      });
    } else if (command === 'profile') {
      utils.openLayer({
        title: '修改资料',
        url: utils.getSettingsUrl('administratorsLayerProfile', {userName: this.local.userName}),
        full: true
      });
    } else if (command === 'password') {
      utils.openLayer({
        title: '更改密码',
        url: utils.getSettingsUrl('administratorsLayerPassword', {userName: this.local.userName}),
        full: true
      });
    } else if (command === 'logout') {
      location.href = utils.getRootUrl('logout')
    }
  },

  btnCultureClick: function(command) {
    $api.post($url + '/actions/setLanguage', {
      culture: command
    }).then(function (response) {
      var res = response.data;
      location.reload();

    }).catch(function (error) {
      utils.error(error);
    });
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
  },
  watch: {
    contextMenuVisible(value) {
      if (this.contextMenuVisible) {
        document.body.addEventListener("click", this.closeContextMenu);
      } else {
        document.body.removeEventListener("click", this.closeContextMenu);
      }
    }
  }
});