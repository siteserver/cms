var pageUtils = {
  getQueryString: function (name) {
    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (!result || result.length < 1) {
      return "";
    }
    return decodeURIComponent(result[1]);
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

  getConfig: function (params, callback) {
    var api = new apiUtils.Api(apiUrl + '/home');
    if (typeof params === 'string') {
      params = {
        pageName: params
      };
    }
    api.get(params, function (err, res) {
      if (err) {
        api.get(params, function (err, res) {
          if (err) return pageUtils.alertError(err);
          if (res.value.isHomeClosed) {
            swal({
              title: "用户中心已关闭！",
              text: ' ',
              icon: 'error',
              button: false,
              closeOnClickOutside: false,
              closeOnEsc: false
            });
          }
          callback(res);
        });
      }
      if (res.value.isHomeClosed) {
        swal({
          title: "用户中心已关闭！",
          text: ' ',
          icon: 'error',
          button: false,
          closeOnClickOutside: false,
          closeOnEsc: false
        });
      }
      callback(res);
    });
  },

  alertError: function (err) {
    swal({
      title: "系统错误，请联系管理员协助解决！",
      text: err.message,
      icon: 'error',
      button: false,
      closeOnClickOutside: false,
      closeOnEsc: false
    });
  },

  alertDelete: function (config) {
    if (!config) return false;

    swal({
        title: config.title,
        text: config.text,
        icon: 'warning',
        buttons: {
          cancel: {
            text: '取 消',
            visible: true,
            className: 'btn'
          },
          confirm: {
            text: '确认删除',
            visible: true,
            className: 'btn btn-danger'
          }
        }
      })
      .then(function (isConfirm) {
        if (isConfirm) {
          config.callback();
        }
      });

    return false;
  }
};

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