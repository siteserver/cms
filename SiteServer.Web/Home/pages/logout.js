var $api = new apiUtils.Api(apiUrl + '/v1/users/actions/logout');

if (window.top != self) {
  window.top.location = self.location;
}

new Vue({
  el: '#main',
  data: {},
  methods: {
    logout: function () {
      $api.post(null, function () {
        location.href = pageUtils.getQueryString('returnUrl') || 'login.html';
      });
    }
  },
  created: function () {
    this.logout();
  }
});