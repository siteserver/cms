var $url = '/logout';

var data = utils.init({});

var methods = {
  logout: function () {
    sessionStorage.removeItem(ACCESS_TOKEN_NAME);
    localStorage.removeItem(ACCESS_TOKEN_NAME);
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