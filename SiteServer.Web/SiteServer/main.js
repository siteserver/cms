if (window.top != self) {
  window.top.location = self.location;
}

var $url = '/pages/main';
var $urlCreate = '/pages/main/actions/create';
var $urlDownload = '/pages/main/actions/download';
var $packageIdSsCms = 'SS.CMS';
var $siteId = parseInt(utils.getQueryString('siteId') || '0');

var data = {
  pageLoad: false,
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
  topMenus: null,
  siteMenus: null,
  activeParentMenu: null,
  activeChildMenu: null,
  pluginMenus: null,
  local: null,

  newVersion: null,
  updatePackages: 0,
  pendingCount: 0,
  lastExecuteTime: new Date(),
  timeoutId: null,

  winHeight: 0,
  winWidth: 0,
  isDesktop: true,
  isMobileMenu: false
};

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get($url + '?siteId=' + $siteId).then(function (response) {
      var res = response.data;
      if (res.value) {
        $this.defaultPageUrl = res.defaultPageUrl;
        $this.isNightly = res.isNightly;
        $this.pluginVersion = res.pluginVersion;
        $this.productVersion = res.productVersion;
        $this.targetFramework = res.targetFramework;
        $this.environmentVersion = res.environmentVersion;
        $this.adminLogoUrl = res.adminLogoUrl || './assets/icons/logo.png';
        $this.adminTitle = res.adminTitle || 'SiteServer CMS';
        $this.isSuperAdmin = res.isSuperAdmin;
        $this.packageList = res.packageList;
        $this.packageIds = res.packageIds;
        $this.topMenus = res.topMenus;
        $this.siteMenus = res.siteMenus;
        $this.pluginMenus = res.pluginMenus;
        $this.local = res.local;
        $this.activeParentMenu = $this.siteMenus[0];

        document.title = $this.adminTitle;
      } else {
        location.href = res.redirectUrl;
      }
    }).catch(function (error) {
      if (error.response && error.response.status === 401) {
        location.href = 'pageLogin.cshtml';
      } else if (error.response && error.response.status === 500) {
        $this.pageAlert = utils.getPageAlert(error);
      }
    }).then(function () {
      setTimeout($this.ready, 100);
    });
  },

  apiCache: function() {
    var $this = this;

    $api.post($url + '/actions/cache', {
      siteId: this.siteId
    }).then(function (response) {
      var res = response.data;
      
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
      var releases = response.data;
      for (var i = 0; i < releases.length; i++) {
        var releaseInfo = releases[i];
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
              if (compareversion(installedPackage.version, releaseInfo.version) == -1) {
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
    if (compareversion($this.productVersion, releaseInfo.version) != -1) return;

    $api.post($urlDownload, {
      packageId: $packageIdSsCms,
      version: releaseInfo.version
    }).then(function (response) {
      var res = response.data;

      if (res.value) {
        $this.newVersion = {
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
    $api.post($urlCreate).then(function (response) {
      var res = response.data;

      $this.pendingCount = res.value;
      if ($this.pendingCount === 0) {
        $this.timeoutId = setTimeout($this.create, 10000);
      } else {
        $this.timeoutId = setTimeout($this.create, 100);
      }
      $this.pageLoad = true;
    }).catch(function (error) {
      if (error.response && error.response.status === 401) {
        location.href = 'pageLogin.cshtml';
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
    return menu.href && menu.target != '_layer' ? menu.href : "javascript:;";
  },

  getTarget: function (menu) {
    return menu.target ? menu.target : "right";
  },

  btnTopMenuClick: function (menu, e) {
    if (menu.target == '_layer') {
      utils.openLayer({
        title: menu.text,
        url: menu.href,
        full: true
      });
      e.stopPropagation();
      e.preventDefault();
    }
    return false;
  },

  btnLeftMenuClick: function (menu) {
    if (menu.hasChildren) {
      this.activeParentMenu = this.activeParentMenu === menu ? null : menu;
    } else {
      this.activeChildMenu = menu;
      this.isMobileMenu = false;
      if (menu.target == '_layer') {
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

new Vue({
  el: "#wrapper",
  data: data,
  methods: methods,
  created: function () {
    this.getConfig();
  },
  computed: {
    leftMenuWidth: function () {
      if (this.isDesktop) return '200px';
      return this.isMobileMenu ? '100%' : '200px'
    }
  }
});

function redirect(url) {
  $('#right').src = url;
}

function openPageCreateStatus() {
  utils.openLayer({
    title: '生成进度查看',
    url: "cms/createStatus.cshtml?siteId=" + $siteId,
    full: true
  });
  return false;
}

function reloadPage() {
  document.getElementById('frmMain').contentWindow.location.reload(true);
}
