var $url = "/cms/settings/settingsCreateTrigger"

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  channels: [],
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

      $this.channels = [res.channel];

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
    $api.put($url, this.editForm).then(function (response) {
      var res = response.data;

      $this.editPanel = false;
      $this.apiList('页面生成触发器设置成功!');
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
    var $this = this;

    this.editForm = {
      siteId: this.siteId,
      channelId: data.value,
      channelName: data.label,
      isCreateChannelIfContentChanged: data.isCreateChannelIfContentChanged,
      createChannelIdsIfContentChanged: data.createChannelIdsIfContentChanged,
    };
    this.editPanel = true;
    setTimeout(function () {
      $this.$refs.channelsTree.setCheckedKeys($this.editForm.createChannelIdsIfContentChanged);
    }, 100);
  },

  handleTreeChanged: function() {
    this.editForm.createChannelIdsIfContentChanged = this.$refs.channelsTree.getCheckedKeys();
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