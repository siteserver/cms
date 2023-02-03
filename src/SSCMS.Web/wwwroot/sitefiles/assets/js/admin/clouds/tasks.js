var $url = "/clouds/tasks"
var $urlDelete = $url + "/actions/delete";
var $urlEnable = $url + "/actions/enable";
var $urlDashboard = "/clouds/dashboard"
var $urlClouds = "cms/clouds";
var $defaultForm = {
  id: 0,
  title: '',
  description: '',
  taskType: '',
  taskInterval: '',
  every: 1,
  weeks: [],
  startDate: new Date(new Date().setTime(new Date().getTime() + 3600 * 1000)),
  isNoticeSuccess: false,
  isNoticeFailure: true,
  noticeFailureCount: 1,
  isNoticeMobile: false,
  noticeMobile: '',
  isNoticeMail: false,
  noticeMail: '',
  isDisabled: false,
  timeout: 10,
  createSiteIds: [],
  createType: 'Index',
  pingHost: '',
};

var data = utils.init({
  isAdd: false,
  task: null,
  active: 1,
  form: _.assign({}, $defaultForm),
  cloudType: null,
  taskTypes: null,
  taskIntervals: null,
  tasks: null,
  sites: [],
});

var methods = {
  apiGet: function(isCloudCheck) {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.cloudType = res.cloudType;
      $this.taskTypes = res.taskTypes;
      $this.taskIntervals = res.taskIntervals;
      $this.tasks = [];
      for (var task of res.tasks) {
        if (task.taskType == 'Create' || task.taskType == 'Ping') {
          $this.tasks.push(task);
        }
      }
      $this.sites = res.sites;
      if(isCloudCheck) {
        $this.apiCloudGet();
      }
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      if(!isCloudCheck) {
        utils.loading($this, false);
      }
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.form).then(function (response) {
      var res = response.data;

      utils.success('定时任务创建成功！');
      $this.isAdd = false;
      $this.apiGet(false);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDashboardSubmit: function (cloudType, expirationDate) {
    $api
      .post($urlDashboard, {
        cloudType: cloudType,
        expirationDate: expirationDate,
      })
      .then(function (response) {
        var res = response.data;
      })
      .catch(function (error) {
        utils.error(error);
      });
  },

  btnPreviousClick: function () {
    this.active--;
  },

  btnNextClick: function() {
    var $this = this;
    this.$refs.addForm.validate(function(valid) {
      if (valid) {
        if ($this.active == 3) {
          $this.apiSubmit();
        } else {
          $this.active++;
        }
      }
    });
  },

  btnCancelClick: function() {
    this.isAdd = false;
  },

  btnCloseClick: function() {
    utils.removeTab();
  },

  validateEmail: function (rule, value, callback) {
    if (!value) {
      callback(new Error('请输入通知邮箱'));
    } else {
      var re = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
      if (!re.test(value))
      {
        callback(new Error('通知邮箱格式不正确'));
      } else {
        callback();
      }
    }
  },

  validateMobile: function (rule, value, callback) {
    if (!value) {
      callback(new Error('请输入通知手机号码'));
    } else if (!/^1[3-9]\d{9}$/.test(value)) {
      callback(new Error('手机号码格式不正确'));
    } else {
      callback()
    }
  },

  apiCloudGet: function () {
    var $this = this;

    utils.loading(this, true);
    cloud.get($urlClouds)
    .then(function (response) {
      var res = response.data;

      if ($this.cloudType !== res.cloudType) {
        $this.cloudType == res.cloudType;
        $this.apiDashboardSubmit(res.cloudType, res.expirationDate);
      }
    })
    .catch(function (error) {
      utils.error(error, {
        ignoreAuth: true,
      });
    })
    .then(function () {
      utils.loading($this, false);
    });
  },

  apiDelete: function (id) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlDelete, {
        id: id,
      })
      .then(function (response) {
        var res = response.data;

        utils.success("定时任务删除成功！");
        $this.apiGet(false);
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  apiEnable: function (task) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlEnable, {
        id: task.id,
      })
      .then(function (response) {
        var res = response.data;

        var name = task.isDisabled ? '启用' : '禁用'
        utils.success("定时任务" + name + "成功！");
        $this.apiGet(false);
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  getTaskType: function (task) {
    for (var item of this.taskTypes) {
      if (item.value == task.taskType) {
        return item.label;
      }
    }
    return '';
  },

  getTaskInterval: function (taskInterval) {
    for (var item of this.taskIntervals) {
      if (item.value == taskInterval) {
        return item.label;
      }
    }
    return '';
  },

  btnUpgradeClick: function () {
    location.href = utils.getCloudsUrl('dashboard', {isUpgrade: true});
  },

  btnDeleteClick: function(task) {
    var $this = this;

    utils.alertDelete({
      title: '删除定时任务',
      text: '此操作将删除定时任务 “' + task.title + '”，确定吗？',
      callback: function () {
        $this.apiDelete(task.id);
      }
    });
  },

  btnEnableClick: function(task) {
    var $this = this;

    var name = task.isDisabled ? '启用' : '禁用';
    var button = task.isDisabled ? '启 用' : '禁 用';
    utils.alertDelete({
      title: name + '定时任务',
      text: '此操作将' + name + '定时任务 “' + task.title + '”，确定吗？',
      button: button,
      callback: function () {
        $this.apiEnable(task);
      }
    });
  },

  btnAddClick: function () {
    this.isAdd = true;
    this.task = null;
    this.active = 1;
    this.form = _.assign({}, $defaultForm);
  },

  btnEditClick: function(task) {
    this.isAdd = true;
    this.task = task;
    this.active = 1;
    this.form = _.assign({}, task);
    if (!this.form.createSiteIds) {
      this.form.createSiteIds = [];
    }
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    var $this = this;
    utils.keyPress(function () {
      if ($this.isAdd) {
        $this.btnNextClick();
      }
    }, function () {
      if ($this.isAdd) {
        $this.btnCancelClick();
      } else {
        $this.btnCloseClick();
      }
    });
    cloud.checkAuth(function() {
      $this.apiGet(true);
    });
  }
});
