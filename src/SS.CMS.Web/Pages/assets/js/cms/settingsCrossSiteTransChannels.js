var $url = "/admin/cms/settings/settingsCrossSiteTransChannels"

var data = utils.initData({
  siteId: utils.getQueryInt("siteId"),
  channels: [],
  transTypes: null,
  transDoneTypes: null,
  filterText: '',

  editPanel: false,
  editForm: null
});

var methods = {
  apiList: function(message) {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.channels = [res.channels];
      $this.transTypes = res.transTypes;
      $this.transDoneTypes = res.transDoneTypes;

      if (message) {
        $this.$message({
          type: 'success',
          message: message
        });
      }
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiGetOptions: function(channelId, transType, transSiteId) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/options', {
      siteId: this.siteId,
      channelId: channelId,
      transType: transType,
      transSiteId: transSiteId
    }).then(function (response) {
      var res = response.data;

      $this.editForm = {
        siteId: $this.siteId,
        channelId: channelId,
        channelName: res.channelName,
        transType: res.transType,
        isTransSiteId: res.isTransSiteId,
        transSiteId: res.transSiteId,
        isTransChannelIds: res.isTransChannelIds,
        transChannelIds: res.transChannelIds,
        isTransChannelNames: res.isTransChannelNames,
        transChannelNames: res.transChannelNames,
        isTransIsAutomatic: res.isTransIsAutomatic,
        transIsAutomatic: res.transIsAutomatic,
        transDoneType: res.transDoneType,
        transSites: res.transSites,
        transChannels: [res.transChannels]
      };
      $this.editPanel = true;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.put($url, this.editForm).then(function (response) {
      var res = response.data;

      $this.editPanel = false;
      $this.apiList('跨站转发栏目设置成功!');
    }).catch(function (error) {
      utils.loading($this, false);
      utils.error($this, error);
    });
  },

  getChannelUrl: function(data) {
    return utils.getRootUrl('redirect', {
      siteId: this.siteId,
      channelId: data.value
    });
  },

  filterNode: function(value, data) {
    if (!value || !value.filterText) return true;
    return utils.contains(data.label, value.filterText) || utils.contains(data.indexName, value.filterText);
  },

  handleTransTypeChange: function(transType) {
    this.apiGetOptions(this.editForm.channelId, transType, this.editForm.transSiteId);
  },

  handleTransSiteIdChange: function(transSiteId) {
    this.apiGetOptions(this.editForm.channelId, this.editForm.transType, transSiteId);
  },

  btnCancelClick: function() {
    this.editPanel = false;
  },

  btnEditClick: function(data) {
    this.apiGetOptions(data.value, data.transType, data.transSiteId);
  },

  handleTreeChanged: function() {
    this.editForm.transChannelIds = this.$refs.channelsTree.getCheckedKeys();
  },

  btnSubmitClick: function() {
    this.apiSubmit();
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  watch: {
    filterText: function(val) {
      this.$refs.tree.filter({
        filterText: val
      });
    }
  },
  created: function () {
    this.apiList();
  }
});