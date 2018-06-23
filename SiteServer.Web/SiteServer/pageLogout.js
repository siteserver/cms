var $api = new apiUtils.Api($apiConfig.apiUrl + '/v1/administrators/actions/logout');
var $innerApi = new apiUtils.Api($apiConfig.innerApiUrl + '/v1/administrators/actions/logout');

var $vue = new Vue({
  el: '#main',
  data: {
    pageLoad: false
  },
  methods: {
    logout: function () {
      var $this = this;

      $innerApi.post(null, function (err, res) {
        if ($apiConfig.isSeparatedApi) {
          $this.logoutSeparatedApi();
        } else {
          $this.redirect();
        }
      });
    },
    logoutSeparatedApi: function () {
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