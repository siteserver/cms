if (window.top != self) {
  window.top.location = self.location;
}

var $url = '/index';
var $urlCreate = '/index/actions/create';
var $urlDownload = '/index/actions/download';
var $idSite = 'site';
var $sidebarWidth = 200;
var $collapseWidth = 60;

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  sessionId: localStorage.getItem('sessionId'),
  isNightly: null,
  version: null,
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
  searchWord: null,
  newCms: null,
  newPlugins: [],
  pendingCount: 0,
  lastExecuteTime: new Date(),
  timeoutId: null,

  defaultOpenedId: null,
  tabName: null,
  tabs: [],
  winHeight: 0,
  winWidth: 0,
  isCollapse: false,
  isDesktop: true,
  isMobileMenu: false
});

var methods = {
  openPageCreateStatus: function() {
    utils.openLayer({
      title: '生成进度查看',
      url: utils.getCmsUrl('createStatus', {siteId: this.siteId}),
      full: true
    });
    return false;
  },

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

        $this.isNightly = res.isNightly;
        $this.version = res.version;
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
        $this.menu = $this.menus[0];

        $this.btnTopMenuClick($this.menus[0]);

        document.title = $this.adminTitle;
        setTimeout($this.ready, 100);
      } else {
        location.href = res.redirectUrl;
      }
    }).catch(function (error) {
      if (error.response && error.response.status === 400) {
        utils.error($this, error, {redirect: true});
      } else if (error.response && (error.response.status === 401 || error.response.status === 403)) {
        location.href = utils.getRootUrl('login');
      } else if (error.response && error.response.status === 500) {
        utils.error($this, error);
      }
    });
  },

  apiCache: function() {
    var $this = this;

    $api.post($url + '/actions/cache', {
      siteId: this.siteId
    }).then(function (response) {
      var res = response.data;
      
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      $this.create();
    });
  },

  ready: function () {
    var $this = this;

    window.onresize = $this.winResize;
    window.onresize();

    $this.apiCache();

    if ($this.isSuperAdmin) {
      $this.getUpdates();
    }

    setInterval(function () {
      var dif = new Date().getTime() - $this.lastExecuteTime.getTime();
      var minutes = dif / 1000 / 60;
      if (minutes > 2) {
        $this.create();
      }
    }, 60000);

    utils.loading($this, false);
  },

  getUpdates: function () {
    var $this = this;

    var pluginIds = this.plugins.map(function (x){ return x.pluginId});
    $cloud.getReleases($this.isNightly, $this.version, pluginIds, function (cms, plugins) {
      if (cms) {
        $this.downloadSsCms(cms);
      }

      for (var i = 0; i < plugins.length; i++) {
        var plugin = plugins[i];
        if (!plugin || !plugin.version) continue;
        
        var installedPlugins = $.grep($this.plugins, function (e) {
          return e.pluginId == plugin.pluginId;
        });
        if (installedPlugins.length == 1) {
          var installed = installedPlugins[0];
          if (installed.version) {
            if (utils.compareVersion(installed.version, plugin.version) == -1) {
              $this.newPlugins.push({
                pluginId: plugin.pluginId,
                displayName: installed.displayName,
                current: installed.version,
                version: plugin.version,
                published: plugin.published
              });
            }
          } else {
            $this.newPlugins.push({
              pluginId: plugin.pluginId,
              displayName: installed.displayName,
              current: '0.0',
              version: plugin.version,
              published: plugin.published
            });
          }
        }
      }
    });
  },

  downloadSsCms: function (cms) {
    var $this = this;
    if (utils.compareVersion($this.version, cms.version) != -1) return;

    $api.post($urlDownload, {
      version: cms.version
    }).then(function (response) {
      var res = response.data;

      if (res.value) {
        $this.newCms = {
          current: $this.version,
          version: cms.version,
          published: cms.published
        };
      }
    });
  },

  create: function () {
    var $this = this;
    
    $this.lastExecuteTime = new Date();
    clearTimeout($this.timeoutId);
    $api.post($urlCreate, {
      sessionId: this.sessionId
    }).then(function (response) {
      var res = response.data;

      $this.pendingCount = res.value;
      if ($this.pendingCount === 0) {
        $this.timeoutId = setTimeout($this.create, 10000);
      } else {
        $this.timeoutId = setTimeout($this.create, 100);
      }
    }).catch(function (error) {
      if (error.response && error.response.status === 401) {
        location.href = utils.getRootUrl('login');
      }
      $this.timeoutId = setTimeout($this.create, 1000);
    });
  },

  winResize: function () {
    this.winHeight = $(window).height();
    this.winWidth = $(window).width();
    this.isDesktop = this.winWidth > 992;
  },

  getHref: function (menu) {
    var link = menu.target != '_layer' ? menu.link : '';
    return link || "javascript:;";
  },

  getTarget: function (menu) {
    return menu.target ? menu.target : "right";
  },

  btnTopMenuClick: function (menu) {
    if (menu.children) {
      for(var i = 0; i < menu.children.length; i++) {
        var child = menu.children[i];
        if (child.children) {
          this.defaultOpenedId = [child.id];
          break;
        }
      }
    }
    this.menu = menu;
  },

  btnMenuClick: function(key) {
    var menu = JSON.parse(key);
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

  btnCultureClick: function(command) {
    var $this = this;

    $api.post($url + '/actions/setLanguage', {
      culture: command
    }).then(function (response) {
      var res = response.data;
      location.reload();

    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      $this.create();
    });
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
        url: utils.getSettingsUrl('administratorsLayerProfile', {pageType: 'user', userId: this.local.userId}),
        full: true
      });
    } else if (command === 'password') {
      utils.openLayer({
        title: '更改密码',
        url: utils.getSettingsUrl('administratorsLayerPassword', {pageType: 'user', userId: this.local.userId}),
        full: true
      });
    } else if (command === 'logout') {
      location.href = utils.getRootUrl('logout')
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