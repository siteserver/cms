var $api = axios.create({
  baseURL: window.$apiUrl || '../api',
  withCredentials: true
});

var swal2 = swal.mixin({
  confirmButtonClass: 'btn btn-primary',
  cancelButtonClass: 'btn btn-default ml-2',
  buttonsStyling: false,
});

if (window.Vue && window.VeeValidate) {
  VeeValidate.Validator.localize('zh_CN');
  Vue.use(VeeValidate);
  VeeValidate.Validator.localize({
    zh_CN: {
      messages: {
        required: function (name) {
          return name + '不能为空'
        },
      }
    }
  });
  VeeValidate.Validator.extend('mobile', {
    getMessage: function () {
      return " 请输入正确的手机号码"
    },
    validate: function (value, args) {
      return value.length == 11 && /^((13|14|15|16|17|18|19)[0-9]{1}\d{8})$/.test(value)
    }
  });
}

var nameUtils = {
  logout: "logout.cshtml"
};

var utils = {
  getQueryString: function (name) {
    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (!result || result.length < 1) {
      return "";
    }
    return decodeURIComponent(decodeURIComponent(result[1]));
  },

  isInnerPage: function () {
    if (window.top == self) {
      window.top.location.href = '../main.cshtml?pageUrl=' + encodeURIComponent(location.href);
      return false;
    }
    return true;
  },

  getPageAlert: function (error) {
    var message = error.message;
    if (error.response) {
      if (error.response.status === 401) {
        message = '检测到用户未登录或者登录已超时，请重新登录';
      } else if (error.response.data) {
        if (error.response.data.exceptionMessage) {
          message = error.response.data.exceptionMessage;
        } else if (error.response.data.message) {
          message = error.response.data.message;
        }
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

    layer.open({
      type: 2,
      btn: null,
      title: config.title,
      area: [config.width + 'px', config.height + 'px'],
      maxmin: true,
      resize: true,
      shadeClose: true,
      content: config.url
    });

    return false;
  },

  getRedirectUrl: function (obj) {
    if (typeof (obj) == 'object') {
      var url = $apiUrl + '/pages/loading?siteId=' + obj.siteId;
      if (obj.channelId) {
        url += '&channelId=' + obj.channelId;
      }
      if (obj.contentId) {
        url += '&contentId=' + obj.contentId;
      }
      if (obj.fileTemplateId) {
        url += '&fileTemplateId=' + obj.fileTemplateId;
      }
      if (obj.specialId) {
        url += '&specialId=' + obj.specialId;
      }
      return url;
    } else if (typeof (obj) == 'string') {
      return $apiUrl + '/pages/loading?redirectUrl=' + encodeURIComponent(obj);
    }
    return 'javascript:;';
  }
};