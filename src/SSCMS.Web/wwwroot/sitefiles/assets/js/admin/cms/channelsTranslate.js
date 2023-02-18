var $url = '/cms/channels/channelsTranslate';

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  returnUrl: utils.getQueryString('returnUrl'),
  checkedChannelIds: utils.getQueryIntList("channelIds"),
  channels: null,
  expandedChannelIds: [],
  filterText: '',

  channelIds: utils.getQueryIntList("channelIds"),
  transSites: null,
  transChannels: null,
  translateTypes: null,

  form: {
    transSiteId: utils.getQueryInt("siteId"),
    transChannelIds: null,
    translateType: 'Content',
    isDeleteAfterTranslate: false,
  }
});

var methods = {
  apiConfig: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.channels = [res.channels];
      $this.transSites = res.transSites;
      $this.translateTypes = res.translateTypes;
      $this.expandedChannelIds = _.union([$this.siteId], $this.checkedChannelIds);
      $this.transChannels = [res.transChannels];
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiGetOptions: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/options', {
      siteId: this.siteId,
      transSiteId: this.form.transSiteId
    }).then(function (response) {
      var res = response.data;

      $this.transChannels = [res.transChannels];
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function (data) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, data).then(function (response) {
      var res = response.data;

      utils.success('批量复制成功！');
      location.href = $this.returnUrl;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getChannelUrl: function(data) {
    return utils.getRootUrl('redirect', {
      siteId: this.siteId,
      channelId: data.value
    });
  },

  filterNode: function(value, data) {
    if (!value) return true;
    return data.label.indexOf(value) !== -1 || data.value + '' === value;
  },

  handleTransSiteIdChange: function() {
    this.apiGetOptions();
  },

  handleCheckChange() {
    this.channelIds = this.$refs.tree.getCheckedKeys();
  },

  btnTranslateClick: function() {
    var transChannelIds = [];
    this.form.transChannelIds.forEach(function(arr) {
      var transChannelId = arr[arr.length - 1];
      transChannelIds.push(transChannelId);
    });

    var $this = this;
    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit({
          siteId: $this.siteId,
          channelIds: $this.channelIds,
          transSiteId: $this.form.transSiteId,
          transChannelIds: transChannelIds,
          translateType: $this.form.translateType,
          isDeleteAfterTranslate: $this.form.isDeleteAfterTranslate
        });
      }
    });
  },

  btnReturnClick: function() {
    location.href = this.returnUrl;
  },

  btnCloseClick: function() {
    utils.removeTab();
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  watch: {
    filterText: function(val) {
      this.$refs.tree.filter(val);
    }
  },
  created: function () {
    utils.keyPress(this.btnTranslateClick, this.btnCloseClick);
    this.apiConfig();
  }
});
