var CLOUD_ACCESS_TOKEN_NAME = "ss_cloud_access_token";

var $cloudApi = _.extend(axios.create({
  baseURL: $urlCloudApi,
  headers: {
    Authorization: "Bearer " + localStorage.getItem(CLOUD_ACCESS_TOKEN_NAME),
  },
}), {
  getPluginIconUrl: function (plugin) {
    if (!plugin.icon) return utils.getAssetsUrl('images/favicon.png');
    if (plugin.success && !plugin.disabled) {
      return plugin.icon;
    }
    return $urlCloudStorage + '/plugins/' + plugin.pluginId + '/logo' + plugin.icon.substring(plugin.icon.lastIndexOf('.'));
  },

  getPlugins: function(word) {
    return this.post('cms/plugins', {
      word: word
    });
  },

  getPlugin: function(pluginId, isNightly, version) {
    return this.get('cms/plugins/' + pluginId, {
      params: {
        isNightly: isNightly,
        version: version
      }
    }).catch(function (error) {
      if (error.response && error.response.status === 404) {
        utils.error('找不到资源，请重试或者检查计算机是否能够连接外网');
      }
    });
  },

  getTemplates: function(page, word, tag, price, order) {
    return this.get('cms/templates', {
      params: {
        page: page,
        word: word,
        tag: tag,
        price: price,
        order: order
      }
    });
  },

  getTemplate: function(templateId) {
    return this.get('cms/templates/' + templateId);
  },

  getUpdates: function(isNightly, version, pluginIds) {
    return this.post('cms/updates', {
      isNightly: isNightly,
      version: version,
      pluginIds: pluginIds
    });
  },
});