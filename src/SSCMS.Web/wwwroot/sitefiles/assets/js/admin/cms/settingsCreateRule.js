var $url = "/cms/settings/settingsCreateRule"

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  channels: [],
  filterText: '',
  expandedChannelIds: [],

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

  apiList: function(expandedChannelIds, message) {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.channels = [res.channel];
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

  apiGet: function(channelId) {
    var $this = this;

    utils.loading(this, true);
    $api.get($url + '/' + this.siteId + '/' + channelId).then(function (response) {
      var res = response.data;

      $this.editForm = _.assign({}, res.channel, res.linkTo);

      $this.editForm.filePath = res.filePath;
      $this.editForm.channelFilePathRule = res.channelFilePathRule;
      $this.editForm.contentFilePathRule = res.contentFilePathRule;

      $this.editLinkTypes = res.linkTypes;
      $this.editPanel = true;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.editForm).then(function (response) {
      var res = response.data;

      $this.editPanel = false;
      $this.apiList(res, '页面路径设置成功!');
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

  getContentUrl: function() {
    return utils.getRootUrl('redirect', {
      siteId: this.siteId,
      channelId:  this.editForm.channelIds[this.editForm.channelIds.length - 1],
      contentId: this.editForm.contentId
    });
  },

  runLayerContentSelect: function (content) {
    this.editForm.contentId = content.id;
    this.editForm.contentTitle = content.title;
  },

  btnLinkToContentClick: function () {
    var channelId = this.editForm.channelIds[this.editForm.channelIds.length - 1];
    utils.openLayer({
      title: "选择指定内容",
      url: utils.getCmsUrl("layerContentSelect", {
        siteId: this.siteId,
        channelId: channelId,
        contentId: 0,
      }),
    });
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
    var url = utils.getCmsUrl('settingsCreateRuleLayerSet', {
      siteId: this.siteId,
      isChannel: isChannel,
      channelId: channelId,
      rule: rule || ''
    });
    utils.openLayer({
      title: '构造',
      url: url,
      width: 800,
      height: 550
    });
  },

  btnSubmitClick: function() {
    var $this = this;
    this.$refs.editForm.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
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
    this.apiList();
  }
});
