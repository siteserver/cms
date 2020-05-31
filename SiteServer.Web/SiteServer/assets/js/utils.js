var $api = axios.create({
  baseURL: window.apiUrl || '../api',
  withCredentials: true
});

var $urlCloud = 'https://sscms.com';
var $urlCloudDl = 'https://dl.sscms.com';
var $urlCloudDemo = 'https://demo.sscms.com';
var $urlCloudApi = 'https://api.sscms.com';
var $apiCloud = axios.create({
  baseURL: $urlCloudApi + '/v6',
  withCredentials: true
});

var utils = {
  alertDelete: function (config) {
    if (!config) return false;

    alert({
        title: config.title,
        text: config.text,
        type: 'warning',
        confirmButtonText: config.button || '删 除',
        confirmButtonClass: 'btn btn-danger',
        showCancelButton: true,
        cancelButtonText: '取 消'
      })
      .then(function (result) {
        if (result.value) {
          config.callback();
        }
      });

    return false;
  },
  
  getQueryString: function (name) {
    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (!result || result.length < 1) {
      return "";
    }
    return decodeURIComponent(result[1]);
  },

  notifyError: function (app, error) {
    var message = error.message;
    if (error.response && error.response.data) {
      if (error.response.data.exceptionMessage) {
        message = error.response.data.exceptionMessage;
      } else if (error.response.data.message) {
        message = error.response.data.message;
      }
    }

    app.$notify.error({
      title: '错误',
      message: message
    });
  },

  getQueryBoolean: function (name) {
    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (!result || result.length < 1) {
      return false;
    }
    return result[1] === 'true' || result[1] === 'True';
  },

  getQueryInt: function (name) {
    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (!result || result.length < 1) {
      return 0;
    }
    return parseInt(result[1]);
  },

  getPageAlert: function (error) {
    var message = error.message;
    if (error.response && error.response.data) {
      if (error.response.data.exceptionMessage) {
        message = error.response.data.exceptionMessage;
      } else if (error.response.data.message) {
        message = error.response.data.message;
      }
    }

    return {
      type: "danger",
      html: message
    };
  },

  loading: function (isLoading) {
    if (isLoading) {
      return layer.load(1, {
        shade: [0.2, '#000']
      });
    } else {
      layer.close(layer.index);
    }
  },

  closeLayer: function () {
    parent.layer.closeAll();
    return false;
  },

  openLayer: function (config) {
    if (!config || !config.url) return false;

    if (!config.width) {
      config.width = $(window).width() - 50;
    }
    if (!config.height) {
      config.height = $(window).height() - 50;
    }

    if (config.full) {
      config.width = $(window).width() - 50;
      config.height = $(window).height() - 50;
    }

    var index = layer.open({
      type: 2,
      btn: null,
      title: config.title,
      area: [config.width + 'px', config.height + 'px'],
      maxmin: true,
      resize: true,
      shadeClose: true,
      content: config.url
    });

    if (config.max) {
      layer.full(index);
    }

    return false;
  }
};