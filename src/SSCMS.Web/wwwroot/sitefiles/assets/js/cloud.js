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

  getPlugin: function(pluginId, version) {
    return this.get('cms/plugins/' + pluginId, {
      params: {
        version: version
      }
    }).catch(function (error) {
      if (error.response && error.response.status === 404) {
        utils.error('找不到资源，请重试或者检查计算机是否能够连接外网');
      } else {
        throw error;
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

  getUpdates: function(version, pluginIds) {
    return this.post('cms/updates', {
      version: version,
      pluginIds: pluginIds
    });
  },

  compareVersion: function (currentVersion, newVersion, options) {
    var v1 = (currentVersion || "").split("-")[0];
    var v2 = (newVersion || "").split("-")[0];
    // var v1 = (currentVersion || '').replace(/[^0-9\.]+/g, ".");
    // var v2 = (newVersion || '').replace(/[^0-9\.]+/g, ".");

    var lexicographical = options && options.lexicographical,
      zeroExtend = options && options.zeroExtend,
      v1parts = v1.split("."),
      v2parts = v2.split(".");

    function isValidPart(x) {
      return (lexicographical ? /^\d+[A-Za-z]*$/ : /^\d+$/).test(x);
    }

    if (!v1parts.every(isValidPart) || !v2parts.every(isValidPart)) {
      return NaN;
    }

    if (zeroExtend) {
      while (v1parts.length < v2parts.length) v1parts.push("0");
      while (v2parts.length < v1parts.length) v2parts.push("0");
    }

    if (!lexicographical) {
      v1parts = v1parts.map(Number);
      v2parts = v2parts.map(Number);
    }

    for (var i = 0; i < v1parts.length; ++i) {
      if (v2parts.length == i) {
        return 1;
      }

      if (v1parts[i] == v2parts[i]) {
        continue;
      } else if (v1parts[i] > v2parts[i]) {
        return 1;
      } else {
        return -1;
      }
    }

    if (v1parts.length != v2parts.length) {
      return -1;
    }

    //1 >, -1 <, 0 ==
    return 0;
  },
});