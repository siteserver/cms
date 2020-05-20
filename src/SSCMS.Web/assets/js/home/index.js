var $url = '/index';

var data = utils.init({
  defaultPageUrl: null,
  homeLogoUrl: null,
  homeTitle: null,
  menus: [],
  user: null,

  menu: null,
  activeParentMenu: null,
  activeChildMenu: null,

  winHeight: 0,
  winWidth: 0,
  isDesktop: true,
  isMobileMenu: false
});

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;
      if (res.user) {

        $this.user = res.user;
        $this.defaultPageUrl = res.defaultPageUrl || utils.getRootUrl('dashboard');

        $this.homeLogoUrl = res.homeLogoUrl || utils.getAssetsUrl('images/logo.png');
        $this.homeTitle = res.homeTitle || '用户中心';
        $this.menus = res.menus;
        
        $this.menu = $this.menus[0];
        $this.activeParentMenu = $this.menus[0].children[0];

        document.title = $this.homeTitle;

        setTimeout($this.ready, 100);
      } else {
        location.href = utils.getRootUrl('login');
      }
    }).catch(function (error) {
      if (error.response && error.response.status === 400) {
        utils.error(error, {redirect: true});
      } else if (error.response && (error.response.status === 401 || error.response.status === 403)) {
        location.href = utils.getRootUrl('login');
      } else if (error.response && error.response.status === 500) {
        utils.error(error);
      }
    });
  },

  ready: function () {
    var $this = this;

    window.onresize = $this.winResize;
    window.onresize();

    utils.loading($this, false);
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
          this.activeParentMenu = child;
          break;
        }
      }
    }
    this.menu = menu;
  },

  btnLeftMenuClick: function (menu, e) {
    if (menu.children) {
      this.activeParentMenu = this.activeParentMenu === menu ? null : menu;
    } else {
      this.activeChildMenu = menu;
      this.isMobileMenu = false;
      if (menu.target == '_layer') {
        e.stopPropagation();
        e.preventDefault();
        utils.openLayer({
          title: menu.text,
          url: menu.link,
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
  el: "#main",
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