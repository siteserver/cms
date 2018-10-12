new Vue({
  el: '#main',
  data: {
    pageConfig: null,
    pageMenus: null,
    pageUser: null,
    pageUrl: "pages/dashboard.html",
    elTopnav: null,
    elFrmMain: null,
    avatarUrl: null
  },
  methods: {
    load: function (pageConfig, pageMenus) {
      var $this = this;
      this.pageConfig = pageConfig;
      this.pageMenus = pageMenus;
      this.pageUser = authUtils.getUser();
      this.avatarUrl = this.pageUser.avatarUrl || this.pageConfig.homeDefaultAvatarUrl || 'assets/images/default_avatar.png';
      this.pageUrl = this.getPageUrl();
      window.onresize = this.resize;
      setTimeout(function () {
        $this.ready();
        $this.resize();
      }, 100);
    },
    ready: function () {
      var $this = this;
      this.elTopnav = $('#topnav');
      this.elFrmMain = $('#frmMain');

      $('.navbar-toggle').on('click', function () {
        $(this).toggleClass('open');
        $('#navigation').slideToggle(400);
      });

      $('.navigation-menu>li').slice(-2).addClass('last-elements');

      $('.navigation-menu li.has-submenu a[href="#"]').on('click', function (e) {
        if ($(window).width() < 992) {
          e.preventDefault();
          $(this).parent('li').toggleClass('open').find('.submenu:first').toggleClass('open');
        }
      });

      $(window).on('hashchange', function () {
        $this.pageUrl = $this.getPageUrl();
      });
    },
    resize: function () {
      var topHeight = this.elTopnav.height();
      this.elFrmMain.css({
        top: topHeight + 'px',
        minHeight: ($(window).height() - topHeight) + 'px',
        display: 'block'
      }).iFrameResize({
        log: false
      });
    },
    getPageUrl: function () {
      if (location.hash && location.hash.length > 1) {
        return location.hash.substr(1, location.hash.length - 1);
      } else {
        return 'pages/dashboard.html';
      }
    }
  },
  created: function () {
    var $this = this;
    if (authUtils.isAuthenticated()) {
      pageUtils.getConfig('index', function (res) {
        if (res.isUserLoggin) {
          $this.load(res.value, res.menus);
        } else {
          location.href = 'pages/login.html';
        }
      });
    } else {
      location.href = 'pages/login.html';
    }
  }
});