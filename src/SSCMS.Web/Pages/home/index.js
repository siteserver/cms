var $url = '/home/index';

var getContentWindow = function () {
  return document.getElementById('frmMain').contentWindow;
};

var data = {
  user: null,
  config: null,
  menus: null,
  defaultPageUrl: null,

  pageUrl: null,
  elTopnav: null,
  elFrmMain: null,
  avatarUrl: null
};

var methods = {
  apiGet: function() {
    var $this = this;

    var token = sessionStorage.getItem(utils.USER_ACCESS_TOKEN_NAME) || localStorage.getItem(utils.USER_ACCESS_TOKEN_NAME);
    if (!token) {
      return this.redirectToLogin();
    }

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      if (!res.user) {
        return $this.redirectToLogin();
      }

      $this.user = res.user;
      $this.config = res.config;
      $this.menus = res.menus;
      $this.defaultPageUrl = res.defaultPageUrl;
  
      $this.avatarUrl = $this.user.avatarUrl || $this.config.homeDefaultAvatarUrl || '/sitefiles/assets/images/default_avatar.png';
      $this.pageUrl = $this.getPageUrl();
      window.onresize = $this.resize;
      setTimeout(function () {
        $this.ready();
        $this.resize();
      }, 100);

    }).catch(function (error) {
      if (error.response && (error.response.status === 401 || error.response.status === 403)) {
        $this.redirectToLogin();
      } else{
        utils.error($this, error);
      }
    }).then(function () {
      utils.loading($this, false);
    });
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
      return this.defaultPageUrl || 'dashboard.html';
    }
  },

  redirectToLogin: function () {
    location.href = 'login.html?returnUrl=' + encodeURIComponent(location.href);
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