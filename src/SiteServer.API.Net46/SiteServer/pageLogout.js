var $api = new apiUtils.Api(apiUrl + '/v1/administrators/actions/logout');

var $vue = new Vue({
  el: '#main',
  data: {
    pageLoad: false
  },
  methods: {
    logout: function () {
      var $this = this;

      $api.post(null, function (err, res) {
        $this.redirect();
      });
    },

    redirect: function () {
      window.top.location.href = 'pageLogin.cshtml';
    }
  }
});

$vue.logout();