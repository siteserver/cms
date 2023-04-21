var $url = '/logout';

var data = utils.init({
  returnUrl: utils.getQueryString('returnUrl')
});

var methods = {
  logout: function () {
    localStorage.removeItem(ACCESS_TOKEN_NAME);
    this.redirect();
  },

  redirect: function () {
    if (this.returnUrl) {
      window.top.location.href = this.returnUrl;
    } else {
      window.top.location.href = utils.getRootUrl('login');
    }
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.logout();
  }
});
