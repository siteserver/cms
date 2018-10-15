var $api = new utils.Api('/v1/users/actions/logout');

if (window.top != self) {
  window.top.location = self.location;
}

new Vue({
  el: '#main',
  data: {},
  methods: {
    logout: function () {
      $api.post(null, function () {
        utils.removeToken();
        location.href = utils.getQueryString('returnUrl') || 'login.html';
      });
    }
  },
  created: function () {
    this.logout();
  }
});