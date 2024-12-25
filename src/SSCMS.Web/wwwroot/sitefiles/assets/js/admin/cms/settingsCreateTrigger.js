var $url = "/cms/settings/settingsCreateTrigger"
var $urlEdit = "/cms/settings/settingsCreateTrigger/actions/edit"
var $urlEditSelected = "/cms/settings/settingsCreateTrigger/actions/editSelected"

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  channels: [],
  allChannelIds: [],
  filterText: '',
  expandedChannelIds: [],

  channelIds: [],
  editPanel: false,
  editForm: null,
  editSelectedPanel: false,
  editSelectedForm: null,
});

var methods = {
  apiGet: function(expandedChannelIds, message) {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.channels = [res.channel];
      $this.allChannelIds = res.allChannelIds;
      $this.expandedChannelIds = expandedChannelIds ? expandedChannelIds : [$this.siteId];

      if (message) {
        utils.success(message);
      }
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiEdit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlEdit, this.editForm).then(function (response) {
      var res = response.data;

      $this.editPanel = false;
      $this.apiGet(res, '页面生成触发器设置成功!');
    }).catch(function (error) {
      utils.loading($this, false);
      utils.error(error);
    });
  },

  apiEditSelected: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlEditSelected, this.editSelectedForm).then(function (response) {
      var res = response.data;

      $this.editSelectedPanel = false;
      $this.apiGet(null, '页面生成触发器设置成功!');
    }).catch(function (error) {
      utils.loading($this, false);
      utils.error(error);
    });
  },

  handleCheckChange() {
    this.channelIds = this.$refs.tree.getCheckedKeys();
  },

  btnCheckClick: function(row) {
    if (this.channelIds.indexOf(row.value) !== -1) {
      this.channelIds.splice(this.channelIds.indexOf(row.value), 1);
    } else {
      this.channelIds.push(row.value);
    }
    this.$refs.tree.setCheckedKeys(this.channelIds);
  },

  btnSelectAllClick: function() {
    this.channelIds = this.allChannelIds;
    this.$refs.tree.setCheckedKeys(this.channelIds);
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
    this.editSelectedPanel = false;
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

  btnEditSelectedClick: function() {
    this.editSelectedForm = {
      siteId: this.siteId,
      channelIds: this.channelIds,
      createChannelIdsIfContentChanged: [],
    };
    this.editSelectedPanel = true;
  },

  handleTreeChanged: function() {
    this.editForm.createChannelIdsIfContentChanged = this.$refs.channelsTree.getCheckedKeys();
  },

  handleTreeSelectedChanged: function() {
    this.editSelectedForm.createChannelIdsIfContentChanged = this.$refs.channelsTreeSelected.getCheckedKeys();
  },

  btnEditSubmitClick: function() {
    this.apiEdit();
  },

  btnEditSelectedSubmitClick: function() {
    this.apiEditSelected();
  },

  btnCloseClick: function() {
    utils.removeTab();
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
    var $this = this;
    utils.keyPress(function () {
      if ($this.editPanel) {
        $this.btnSubmitClick();
      }
    }, function() {
      if ($this.editPanel) {
        $this.btnCancelClick();
      } else {
        $this.btnCloseClick();
      }
    });
    this.apiGet();
  }
});
