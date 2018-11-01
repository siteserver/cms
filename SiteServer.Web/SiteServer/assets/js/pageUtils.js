if (window.swal && swal.mixin) {
  var alert = swal.mixin({
    confirmButtonClass: 'btn btn-primary',
    cancelButtonClass: 'btn btn-default ml-3',
    buttonsStyling: false,
  });
}

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

var pageUtils = {
  getQueryStringByName: function (name) {
    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (!result || result.length < 1) {
      return "";
    }
    return decodeURIComponent(result[1]);
  },

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

    // if (config.full) {
    //   layer.full(index);
    // }

    return false;
  },

  alertDelete: function (config) {
    if (!config) return false;

    alert({
        title: config.title,
        text: config.text,
        type: 'question',
        confirmButtonText: config.button,
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
  }
};