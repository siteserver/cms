if (window.top != self) {
  window.top.location = self.location;
}

function redirect(url) {
  $("#right").src = url;
}

function openPageCreateStatus() {
  utils.openLayer({
    title: "生成进度查看",
    url: "cms/createStatus.cshtml?siteId=" + $siteId,
    full: true
  });
  return false;
}

function reloadPage() {
  document.getElementById("frmMain").contentWindow.location.reload(true);
}

var $url = "/pages/main";
var $urlCreate = "/pages/main/actions/create";
var $urlDownload = "/pages/main/actions/download";
var $packageIdSsCms = "SS.CMS";
var $siteId = parseInt(utils.getQueryString("siteId") || "0");
var $pageUrl = utils.getQueryString("pageUrl");

var $data = {
  pageLoad: false,
  pageAlert: null,
  defaultPageUrl: null,
  isNightlyUpdate: null,
  pluginVersion: null,
  isSuperAdmin: null,
  packageList: null,
  packageIds: null,
  cmsVersion: null,
  apiPrefix: null,
  adminDirectory: null,
  homeDirectory: null,
  topMenus: null,
  siteMenus: null,
  activeParentMenu: null,
  activeChildMenu: null,
  pluginMenus: null,
  adminInfo: null,
  newVersion: null,
  updatePackages: [],
  status: {},
  pendingCount: 0,
  lastExecuteTime: new Date(),
  timeoutId: null,
  winHeight: 0,
  winWidth: 0,
  isDesktop: true,
  isMobileMenu: false
};

var $methods = {
  getConfig: function () {
    var $this = this;

    $api.post($url, {
      siteId: $siteId,
      pageUrl: $pageUrl
    }).then(function (response) {
      var res = response.data;
      if (res.value) {
        $this.defaultPageUrl = res.defaultPageUrl;
        $this.isNightlyUpdate = res.isNightlyUpdate;
        $this.pluginVersion = res.pluginVersion;
        $this.isSuperAdmin = res.isSuperAdmin;
        $this.packageList = res.packageList;
        $this.packageIds = res.packageIds;
        $this.cmsVersion = res.cmsVersion;
        $this.apiPrefix = res.apiPrefix;
        $this.adminDirectory = res.adminDirectory;
        $this.homeDirectory = res.homeDirectory;
        $this.topMenus = res.topMenus;
        $this.siteMenus = res.siteMenus;
        $this.pluginMenus = res.pluginMenus;
        $this.adminInfo = res.adminInfo;
        $this.activeParentMenu = $this.siteMenus[0];
        $this.pageLoad = true;
        setTimeout($this.ready, 100);
      } else {
        location.href = res.redirectUrl;
      }
    });
  },

  ready: function () {
    var $this = this;

    window.onresize = $this.winResize;
    window.onresize();

    $this.create();

    setInterval(function () {
      var dif = new Date().getTime() - $this.lastExecuteTime.getTime();
      var minutes = dif / 1000 / 60;
      if (minutes > 2) {
        $this.create();
      }
    }, 60000);

    $("#sidebar").slimScroll({
      height: "auto",
      position: "right",
      size: "5px",
      color: "#495057",
      wheelStep: 5
    });

    $this.connect();
  },

  connect: function () {
    var $this = this;
    $ssApi
      .post($ssUrlConnect, {
        url: location.href,
        apiPrefix: $this.apiPrefix,
        isNightlyUpdate: $this.isNightlyUpdate,
        pluginVersion: $this.pluginVersion,
        packageIds: $this.isSuperAdmin ? $this.packageIds.join(",") : ''
      })
      .then(function (response) {
        var res = response.data;

        $this.status = res.value;

        if (res.updates && res.updates.length > 0) {
          for (var i = 0; i < res.updates.length; i++) {
            var releaseInfo = res.updates[i];
            if (!releaseInfo || !releaseInfo.version) continue;
            if (releaseInfo.pluginId == $packageIdSsCms) {
              $this.downloadSsCms(releaseInfo);
            } else {
              var installedPackages = _.filter($this.packageList, function (o) {
                return o.id == releaseInfo.pluginId;
              });
              if (installedPackages.length == 1) {
                var installedPackage = installedPackages[0];
                if (installedPackage.version) {
                  if (
                    compareversion(
                      installedPackage.version,
                      releaseInfo.version
                    ) == -1
                  ) {
                    $this.updatePackages.push(
                      _.assign({}, {
                          cmsVersion: installedPackage.version
                        },
                        releaseInfo
                      )
                    );
                  }
                } else {
                  $this.updatePackages.push(
                    _.assign({}, {
                        cmsVersion: installedPackage.version
                      },
                      releaseInfo
                    )
                  );
                }
              }
            }
          }
        }
      });
  },

  downloadSsCms: function (releaseInfo) {
    var $this = this;
    if (compareversion($this.cmsVersion, releaseInfo.version) != -1) return;

    $api
      .post($urlDownload, {
        packageId: $packageIdSsCms,
        version: releaseInfo.version
      })
      .then(function (response) {
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
    clearTimeout($this.timeoutId);
    $api
      .post($urlCreate)
      .then(function (response) {
        var res = response.data;

        $this.pendingCount = res.value;
        if ($this.pendingCount === 0) {
          $this.timeoutId = setTimeout($this.create, 10000);
        } else {
          $this.timeoutId = setTimeout($this.create, 100);
        }
      })
      .catch(function (error) {
        if (error.response && error.response.status === 401) {
          location.href = "login.cshtml";
        }
        $this.timeoutId = setTimeout($this.create, 1000);
      })
      .then(function () {
        $this.lastExecuteTime = new Date();
      });
  },

  winResize: function () {
    this.winHeight = $(window).height();
    this.winWidth = $(window).width();
    this.isDesktop = this.winWidth > 992;
  },

  getHref: function (menu) {
    return menu.href && menu.target != "_layer" ? menu.href : "javascript:;";
  },

  getTarget: function (menu) {
    return menu.target ? menu.target : "right";
  },

  btnCloudLoginClick: function () {
    utils.openLayer({
      title: "云服务登录",
      url: "cloud/layerLogin.cshtml",
      width: 550,
      height: 560
    });
  },

  btnTopMenuClick: function (menu) {
    if (menu.target == "_layer") {
      utils.openLayer({
        title: menu.text,
        url: menu.href,
        full: true
      });
    }
  },

  btnLeftMenuClick: function (menu) {
    if (menu.hasChildren) {
      this.activeParentMenu = this.activeParentMenu === menu ? null : menu;
    } else {
      this.activeChildMenu = menu;
      this.isMobileMenu = false;
      if (menu.target == "_layer") {
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
  data: $data,
  methods: $methods,
  created: function () {
    this.getConfig();
  },
  computed: {
    leftMenuWidth: function () {
      if (this.isDesktop) return "200px";
      return this.isMobileMenu ? "100%" : "200px";
    }
  }
});