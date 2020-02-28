if (window.top != self) {
  window.top.location = self.location;
}

var $url = '/admin/index';
var $urlCreate = '/admin/index/actions/create';
var $urlDownload = '/admin/index/actions/download';
var $packageIdSsCms = 'SS.CMS';

var data = utils.initData({
  siteId: utils.getQueryInt('siteId'),
  sessionId: localStorage.getItem('sessionId'),
  pageAlert: null,
  defaultPageUrl: null,
  isNightly: null,
  pluginVersion: null,
  productVersion: null,
  targetFramework: null,
  environmentVersion: null,
  adminLogoUrl: null,
  adminTitle: null,
  isSuperAdmin: null,
  packageList: null,
  packageIds: null,
  menus: [],
  siteUrl: null,
  previewUrl: null,
  local: null,

  menu: null,
  activeParentMenu: null,
  activeChildMenu: null,

  newVersion: null,
  updatePackages: 0,
  pendingCount: 0,
  lastExecuteTime: new Date(),
  timeoutId: null,

  winHeight: 0,
  winWidth: 0,
  isDesktop: true,
  isMobileMenu: false
});

var methods = {
  openPageCreateStatus() {
    utils.openLayer({
      title: '生成进度查看',
      url: utils.pageCmsUrl('createStatus', {siteId: this.siteId}),
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
        $this.defaultPageUrl = res.defaultPageUrl;
        $this.isNightly = res.isNightly;
        $this.pluginVersion = res.pluginVersion;
        $this.productVersion = res.productVersion;
        $this.targetFramework = res.targetFramework;
        $this.environmentVersion = res.environmentVersion;
        $this.adminLogoUrl = res.adminLogoUrl || utils.getAssetsUrl('images/logo.png');
        $this.adminTitle = res.adminTitle || 'SiteServer CMS';
        $this.isSuperAdmin = res.isSuperAdmin;
        $this.packageList = res.packageList;
        $this.packageIds = res.packageIds;
        $this.menus = res.menus;
        $this.siteUrl = res.siteUrl;
        $this.previewUrl = res.previewUrl;
        $this.local = res.local;
        $this.menu = $this.menus[0];
        $this.activeParentMenu = $this.menus[0].children[0];

        document.title = $this.adminTitle;

        setTimeout($this.ready, 100);
      } else {
        location.href = res.redirectUrl;
      }
    }).catch(function (error) {
      if (error.response && error.response.status === 400) {
        utils.error($this, error, {redirect: true});
      } else if (error.response && error.response.status === 401) {
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
    $apiCloud.get('updates', {
      params: {
        isNightly: $this.isNightly,
        pluginVersion: $this.pluginVersion,
        targetFramework: $this.targetFramework,
        environmentVersion: $this.environmentVersion,
        packageIds: $this.packageIds.join(',')
      }
    }).then(function (response) {
      var res = response.data;
      for (var i = 0; i < res.value.length; i++) {
        var releaseInfo = res.value[i];
        if (!releaseInfo || !releaseInfo.version) continue;
        if (releaseInfo.pluginId == $packageIdSsCms) {
          $this.downloadSsCms(releaseInfo);
        } else {
          var installedPackages = $.grep($this.packageList, function (e) {
            return e.id == releaseInfo.pluginId;
          });
          if (installedPackages.length == 1) {
            var installedPackage = installedPackages[0];
            if (installedPackage.version) {
              if (utils.compareVersion(installedPackage.version, releaseInfo.version) == -1) {
                $this.updatePackages++;
              }
            } else {
              $this.updatePackages++;
            }
          }
        }
      }
    });
  },

  downloadSsCms: function (releaseInfo) {
    var $this = this;
    if (utils.compareVersion($this.productVersion, releaseInfo.version) != -1) return;
    var major = releaseInfo.version.split('.')[0];
    var minor = releaseInfo.version.split('.')[1];

    $api.post($urlDownload, {
      packageId: $packageIdSsCms,
      version: releaseInfo.version
    }).then(function (response) {
      var res = response.data;

      if (res.value) {
        $this.newVersion = {
          updatesUrl: 'https://www.siteserver.cn/updates/v' + major + '_' + minor + '/index.html',
          version: releaseInfo.version,
          published: releaseInfo.published,
          releaseNotes: releaseInfo.releaseNotes
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
    var href = menu.target != '_layer' ? menu.href : '';
    return href || "javascript:;";
  },

  getTarget: function (menu) {
    return menu.target ? menu.target : "right";
  },

  btnTopMenuClick: function (menu) {
    if (menu.hasChildren) {
      for(var i = 0; i < menu.children.length; i++) {
        var child = menu.children[i];
        if (child.hasChildren) {
          this.activeParentMenu = child;
          break;
        }
      }
    }
    this.menu = menu;
  },

  btnLeftMenuClick: function (menu, e) {
    if (menu.hasChildren) {
      this.activeParentMenu = this.activeParentMenu === menu ? null : menu;
    } else {
      this.activeChildMenu = menu;
      this.isMobileMenu = false;
      if (menu.target == '_layer') {
        e.stopPropagation();
        e.preventDefault();
        utils.openLayer({
          title: menu.text,
          url: menu.href,
          full: true
        });
      }
    }
  },

  btnMobileMenuClick: function () {
    this.isMobileMenu = !this.isMobileMenu;
  }
};

var $vue = new Vue({
  el: "#wrapper",
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  },
  computed: {
    leftMenuWidth: function () {
      if (this.isDesktop) return '200px';
      return this.isMobileMenu ? '100%' : '200px'
    }
  }
});