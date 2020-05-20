var $url = '/cms/contents/editorLayerTranslate';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  channelId: utils.getQueryInt('channelId'),
  transSites: null,
  transChannels: null,
  form: {
    transSiteIds: null,
    transChannelIds: null,
    transType: 'Copy',
  }
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId,
        channelId: this.channelId,
      }
    }).then(function (response) {
      var res = response.data;

      $this.transSites = res.transSites;
      $this.form.transSiteIds = [$this.siteId];
      $this.apiGetOptions();
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiGetOptions: function() {
    var $this = this;

    $api.post($url + '/actions/options', {
      siteId: this.siteId,
      channelId: this.channelId,
      transSiteId: this.form.transSiteIds[this.form.transSiteIds.length - 1],
    }).then(function (response) {
      var res = response.data;

      $this.transChannels = [res.transChannels];
      $this.form.transChannelIds = null;
    }).catch(function (error) {
      utils.loading($this, false);
      utils.error(error);
    });
  },

  apiSubmit: function() {
    var $this = this;

    var transSiteId = this.form.transSiteIds[this.form.transSiteIds.length - 1];
    var transChannelId = this.form.transChannelIds[this.form.transChannelIds.length - 1];

    utils.loading(this, true);
    $api.post($url, {
      siteId: this.siteId,
      channelId: this.channelId,
      transSiteId: transSiteId,
      transChannelId: transChannelId,
      transType: this.form.transType
    }).then(function (response) {
      var res = response.data;

      parent.$vue.addTranslation(
        transSiteId,
        transChannelId,
        $this.form.transType,
        res.value
      );
      utils.closeLayer();
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  handleTransSiteIdChange: function() {
    this.apiGetOptions();
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
    this.apiGet();
  }
});
