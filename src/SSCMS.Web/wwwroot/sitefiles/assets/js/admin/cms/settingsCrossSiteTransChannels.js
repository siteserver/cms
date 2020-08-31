var $url = "/cms/settings/settingsCrossSiteTransChannels"

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  channels: [],
  filterText: '',

  editPanel: false,
  channelId: null,
  channelName: null,
  translates: [],
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

      if (message) {
        utils.success(message);
      }
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      siteId: this.siteId,
      channelId: this.channelId,
      translates: this.translates
    }).then(function (response) {
      var res = response.data;

      $this.editPanel = false;
      $this.apiList('跨站转发栏目设置成功!');
    }).catch(function (error) {
      utils.loading($this, false);
      utils.error(error);
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

  btnCancelClick: function() {
    this.editPanel = false;
  },

  btnEditClick: function(data) {
    this.channelId = data.value;
    this.channelName = data.label;
    this.translates = data.translates;
    this.editPanel = true;
    // this.apiGetOptions(data.value, data.transType, data.transSiteId);
  },

  handleTreeChanged: function() {
    this.editForm.transChannelIds = this.$refs.channelsTree.getCheckedKeys();
  },

  btnSubmitClick: function() {
    this.apiSubmit();
  },

  btnTranslateAddClick: function() {
    utils.openLayer({
      title: "选择转移栏目",
      url: utils.getCmsUrl('editorLayerTranslate', {
        siteId: this.siteId,
        channelId: this.channelId
      }),
      width: 620,
      height: 400
    });
  },

  addTranslation: function(transSiteId, transChannelId, transType, summary) {
    this.translates.push({
      siteId: this.siteId,
      channelId: this.channelId,
      targetSiteId: transSiteId,
      targetChannelId: transChannelId,
      translateType: transType,
      summary: summary
    });
  },

  handleTranslationClose: function(summary) {
    this.translates = _.remove(this.translates, function(n) {
      return summary !== n.summary;
    });
  },
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