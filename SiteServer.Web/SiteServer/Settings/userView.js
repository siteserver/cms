var $url = '/pages/settings/userView';

var data = {
  pageLoad: false,
  pageAlert: null,
  userId: utils.getQueryInt('userId'),
  user: null,
  groupName: null
};

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get($url + '?userId=' + $this.userId).then(function (response) {
      var res = response.data;

      $this.user = res.value;
      $this.groupName = res.groupName;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  btnReturnClick: function () {
    location.href = 'user.cshtml';
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getConfig();
  }
});