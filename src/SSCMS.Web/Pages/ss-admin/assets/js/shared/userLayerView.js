var $url = '/admin/shared/userLayerView';

var data = utils.initData({
  userId: utils.getQueryInt('userId'),
  userName: utils.getQueryString('userName'),
  user: null,
  groupName: null
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        userId: this.userId,
        userName: this.userName
      }
    }).then(function (response) {
      var res = response.data;

      $this.user = res.user;
      $this.groupName = res.groupName;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnCancelClick: function () {
    utils.closeLayer();
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