var $url = '/admin/logout';

var data = utils.initData({});

var methods = {
  logout: function () {
    sessionStorage.removeItem(utils.ACCESS_TOKEN_NAME);
    localStorage.removeItem(utils.ACCESS_TOKEN_NAME);
    this.redirect();
  },

  redirect: function () {
    window.top.location.href = utils.getRootUrl('login');
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