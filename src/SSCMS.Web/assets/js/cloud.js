var CLOUD_ACCESS_TOKEN_NAME = "ss_cloud_access_token";

var $apiCloud = axios.create({
  baseURL: 'https://localhost:6001/v7/cms',
  headers: {
    Authorization: "Bearer " + localStorage.getItem(CLOUD_ACCESS_TOKEN_NAME),
  },
});

var $cloud = {
  getPlugins: function(word, callback) {
    $apiCloud.post('plugins', {
      word: word
    }).then(function (response) {
      var plugins = response.data;

      callback(plugins);
    });
  },

  getReleases: function(isNightly, version, pluginIds, callback) {
    $apiCloud.post('releases', {
      isNightly: isNightly,
      version: version,
      pluginIds: pluginIds
    }).then(function (response) {
      var res = response.data;

      callback(res.cms, res.plugins);
    });
  }
};
