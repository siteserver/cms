var $url = '/admin/cms/channels/channelsTranslate';

var data = utils.initData({
  siteId: utils.getQueryInt("siteId"),
  returnUrl: utils.getQueryString('returnUrl'),
  checkedChannelIds: utils.getQueryIntList("channelIds"),
  channels: null,
  transSites: null,
  translateTypes: null,

  expandedChannelIds: [],
  filterText: '',

  channelIds: [],
  transSiteId: null,
  transChannelIds: null,
  transChannels: null,
  translateType: 'Content',
  isDeleteAfterTranslate: false,
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
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiGetOptions: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/options', {
      siteId: this.siteId,
      transSiteId: this.transSiteId
    }).then(function (response) {
      var res = response.data;

      $this.transChannels = [res.transChannels];
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function (data) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, data).then(function (response) {
      var res = response.data;

      $this.$message.success('批量转移成功！');
    }).catch(function (error) {
      utils.error($this, error);
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
    if (value.channelName) {
      return data.label.indexOf(value.channelName) !== -1;
    }
    return true;
  },

  handleTransSiteIdChange: function() {
    this.apiGetOptions();
  },

  handleCheckChange() {
    this.channelIds = this.$refs.tree.getCheckedKeys();
  },

  btnTranslateClick: function() {
    this.apiSubmit({
      siteId: this.siteId,
      channelIds: this.channelIds,
      transSiteId: this.transSiteId,
      transChannelId: this.transChannelIds[this.transChannelIds.length - 1],
      translateType: this.translateType,
      isDeleteAfterTranslate: this.isDeleteAfterTranslate
    });
  },

  btnReturnClick: function() {
    location.href = this.returnUrl;
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  watch: {
    filterText: function(val) {
      this.$refs.tree.filter({
        channelName: val
      });
    }
  },
  created: function () {
    this.apiConfig();
  }
});