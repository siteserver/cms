var $url = "/admin/cms/settings/settingsCreateRule"

var data = utils.initData({
  siteId: utils.getQueryInt("siteId"),
  channels: [],
  filterText: '',

  editPanel: false,
  editForm: null,
  editLinkTypes: [],
  channelFilePathRule: null,
  contentFilePathRule: null
});

var methods = {
  setRuleText: function(rule, isChannel) {
    if (isChannel) {
      this.editForm.channelFilePathRule = rule;
    } else {
      this.editForm.contentFilePathRule = rule;
    }
  },
  
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

  apiGet: function(channelId) {
    var $this = this;

    utils.loading(this, true);
    $api.get($url + '/' + this.siteId + '/' + channelId).then(function (response) {
      var res = response.data;

      $this.editForm = {
        siteId: $this.siteId,
        channelId: res.channel.id,
        channelName: res.channel.channelName,
        linkUrl: res.channel.linkUrl,
        linkType: res.channel.linkType,
        filePath: res.filePath,
        channelFilePathRule: res.channelFilePathRule,
        contentFilePathRule: res.contentFilePathRule,
      };
      $this.editLinkTypes = res.linkTypes;
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
      $this.apiList('页面路径设置成功!');
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getChannelUrl: function(data) {
    return '../redirect.cshtml?siteId=' + this.siteId + '&channelId=' + data.value;
  },

  filterNode: function(value, data) {
    if (!value || !value.filterText) return true;
    return utils.contains(data.label, value.filterText) || utils.contains(data.indexName, value.filterText) || utils.contains(data.filePath, value.filterText) || utils.contains(data.contentFilePathRule, value.filterText);
  },

  btnCancelClick: function() {
    this.editPanel = false;
  },

  btnEditClick: function(row) {
    this.editIsEditor = false;
    this.apiGet(row.value);
  },

  btnSetClick: function(channelId, isChannel, rule) {
    var url = 'settingsCreateRuleLayerSet.cshtml?siteId=' + this.siteId + '&isChannel=' + isChannel + '&channelId=' + channelId + '&rule=' + encodeURIComponent(rule);

    utils.openLayer({
      title: '构造',
      url: url,
      width: 800,
      height: 500
    });
  },

  btnSubmitClick: function() {
    var $this = this;
    this.$refs.editForm.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
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