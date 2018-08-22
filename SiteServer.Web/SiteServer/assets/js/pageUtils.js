var pageUtils = {
  getQueryStringByName: function (name) {
    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (!result || result.length < 1) {
      return "";
    }
    return decodeURIComponent(result[1]);
  },

  loading: function (isLoading) {
    if (isLoading) {
      return layer.load(1, {
        shade: [0.2, '#000'] //0.1透明度的白色背景
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

    layer.open({
      type: 2,
      btn: null,
      title: config.title,
      area: [config.width + 'px', config.height + 'px'],
      fixed: false,
      maxmin: true,
      shadeClose: true,
      content: config.url
    });

    return false;
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