var $url = '/plugins/config';
var $urlActionsGetChannels = '/plugins/config/actions/getChannels';
var $urlActionsSubmitChannels = '/plugins/config/actions/submitChannels';

var data = utils.init({
  pluginId: utils.getQueryString('pluginId'),
  siteId: utils.getQueryInt('siteId'),
  form: {
    taxis: 0,
    isAllSites: true,
    siteIds: null,
  },
  sites: null,
  siteConfigs: null,
  siteName: null,
  treeData: [],
  defaultExpandedKeys: [],
  pageType: null,
  channelsForm: null,
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        pluginId: this.pluginId
      }
    }).then(function (response) {
      var res = response.data;

      $this.plugin = res.plugin;
      $this.sites = res.sites;
      $this.siteConfigs = res.plugin.siteConfigs || [];
      $this.form = {
        taxis: res.plugin.taxis,
        isAllSites: res.plugin.isAllSites,
        siteIds: res.plugin.siteIds || []
      }
      $this.pageType = 'sites';
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
      pluginId: this.pluginId,
      taxis: this.form.taxis,
      isAllSites: this.form.isAllSites,
      siteIds: this.siteIds
    }).then(function (response) {
      var res = response.data;

      utils.alertSuccess({
        title: '插件配置保存成功',
        text: '插件配置保存成功，系统需要重新加载',
        callback: function() {
          window.top.location.reload(true);
        }
      });
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiGetChannels: function (siteId) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlActionsGetChannels, {
      pluginId: this.pluginId,
      siteId: siteId
    }).then(function (response) {
      var res = response.data;

      $this.siteName = res.siteName;
      $this.channel = res.channel;
      $this.channelsForm = {
        siteId: res.siteConfig.siteId,
        isAllChannels: res.siteConfig.isAllChannels,
        channelIds: res.siteConfig.channelIds
      };
      $this.treeData = [res.channel];
      $this.defaultExpandedKeys = [res.channel.id];
      $this.pageType = 'channels';
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmitChannels: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlActionsSubmitChannels, {
      pluginId: this.pluginId,
      siteId: this.channelsForm.siteId,
      isAllChannels: this.channelsForm.isAllChannels,
      channelIds: this.channelsForm.channelIds
    }).then(function (response) {
      var res = response.data;

      utils.alertSuccess({
        title: '插件配置保存成功',
        text: '插件配置保存成功，系统需要重新加载',
        callback: function() {
          window.top.location.reload(true);
        }
      });
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getSiteConfig: function(site) {
    var siteConfig = this.siteConfigs.find(function (x) {
      return x.siteId === site.value;
    });
    if (!siteConfig) {
      siteConfig = {
        siteId: site.value,
        isAllChannels: false,
        channelIds: []
      };
    }
    return siteConfig;
  },

  handleTreeChanged: function() {
    this.channelsForm.channelIds = this.$refs.tree.getCheckedKeys();
  },

  btnChannelsClick: function(site) {
    this.apiGetChannels(site.value);
  },

  btnSubmitClick: function () {
    var $this = this;

    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  },

  btnCloseClick: function () {
    utils.removeTab();
  },

  btnChannelsSubmitClick: function () {
    this.apiSubmitChannels();
  },

  btnCancelClick: function () {
    this.pageType = 'sites';
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  computed: {
    siteIds: function() {
      var siteIds =  [];
      if (!this.form.isAllSites && this.form.siteIds && this.form.siteIds.length > 0) {
        siteIds = this.form.siteIds.map(function (x) { 
          return typeof x === 'number' ? x : x[x.length - 1];
        });
      }
      return siteIds;
    }
  },
  created: function () {
    if (this.siteId > 0) {
      this.apiGetChannels(this.siteId);
    } else {
      this.apiGet();
    }
  }
});