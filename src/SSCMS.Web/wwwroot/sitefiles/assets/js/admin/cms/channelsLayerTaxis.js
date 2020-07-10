var $url = '/cms/channels/channelsLayerTaxis';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  channelIds: utils.getQueryIntList('channelIds'),
  form: {
    isUp: true,
    taxis: 1
  }
});

var methods = {
  apiSubmit: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      siteId: this.siteId,
      channelIds: this.channelIds,
      isUp: this.form.isUp,
      taxis: this.form.taxis
    }).then(function (response) {
      var res = response.data;

      parent.$vue.apiList('栏目排序成功!', res);
      utils.closeLayer();
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  },

  btnCancelClick: function () {
    utils.closeLayer();
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    utils.loading(this, false);
  }
});