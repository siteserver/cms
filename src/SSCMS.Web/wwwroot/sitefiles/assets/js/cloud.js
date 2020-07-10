var cloud = _.extend(axios.create({
  baseURL: 'https://api.sscms.com/v7',
  // baseURL: 'http://localhost:81/v7',
  headers: {
    Authorization: "Bearer " + localStorage.getItem('ss_cloud_access_token'),
  },
}), {
  host: 'https://sscms.com',
  hostDl: 'https://dl.sscms.com',
  hostDemo: 'https://demo.sscms.com',
  hostStorage: 'https://storage.sscms.com',

  getPluginIconUrl: function (plugin) {
    if (!plugin.icon) return utils.getAssetsUrl('images/favicon.png');
    if (plugin.success && !plugin.disabled) {
      return plugin.icon;
    }
    return this.hostStorage + '/plugins/' + plugin.pluginId + '/logo' + plugin.icon.substring(plugin.icon.lastIndexOf('.'));
  },

  getTemplatesUrl: function(relatedUrl) {
    return this.host + '/templates/' + relatedUrl;
  },

  getPluginsUrl: function(relatedUrl) {
    return this.host + '/plugins/' + relatedUrl;
  },

  getDocsUrl: function(relatedUrl) {
    return this.host + '/docs/v7/' + relatedUrl;
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