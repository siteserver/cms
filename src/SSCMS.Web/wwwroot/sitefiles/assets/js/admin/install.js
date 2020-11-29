var $url = "/install";
var $routeUnCheckedList = "unCheckedList";
var $sqlite = 'SQLite';

var data = utils.init({
  forbidden: false,
  version: null,
  frameworkDescription: null,
  osDescription: null,
  contentRootPath: null,
  webRootPath: null,
  rootWritable: null,
  siteFilesWritable: null,
  databaseTypes: null,
  adminUrl: null,
  containerized: false,
  databaseType: null,
  databaseConnectionString: null,
  redisConnectionString: null,
  databaseNames: null,
  pageIndex: 0,
  agreement: false,
  passwordLevel: null,
  errorMessage: null,

  databaseForm: {
    databaseType: 'MySql',
    databaseHost: null,
    isDatabaseDefaultPort: true,
    databasePort: null,
    databaseUserName: null,
    databasePassword: null,
    databaseName: null
  },

  redisForm: {
    isRedis: false,
    redisHost: 'localhost',
    isRedisDefaultPort: true,
    redisPort: 6379,
    isSsl: false,
    redisPassword: null
  },

  adminForm: {
    userName: null,
    adminPassword: null,
    email: null,
    mobile: null,
    confirmPassword: null,
    isProtectData: false
  }
});

var methods = {
  apiGet: function () {
    var $this = this;

    this.errorMessage = null;
    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.forbidden = res.forbidden;
      $this.version = res.version;
      $this.frameworkDescription = res.frameworkDescription;
      $this.osDescription = res.osDescription;
      $this.contentRootPath = res.contentRootPath;
      $this.webRootPath = res.webRootPath;
      $this.rootWritable = res.rootWritable;
      $this.siteFilesWritable = res.siteFilesWritable;
      $this.databaseTypes = res.databaseTypes;
      $this.adminUrl = res.adminUrl;
      $this.containerized = res.containerized;
      $this.databaseType = res.databaseType;
      $this.databaseConnectionString = res.databaseConnectionString;
      $this.redisConnectionString = res.redisConnectionString;
    }).catch(function (error) {
      $this.errorMessage = utils.getErrorMessage(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDatabaseConnect: function() {
    var $this = this;

    this.errorMessage = null;
    utils.loading(this, true);
    $api.post($url + '/actions/databaseConnect', this.databaseForm).then(function (response) {
      var res = response.data;

      if ($this.containerized) {
        $this.pageIndex++;
        return;
      }

      $this.databaseNames = res.databaseNames;
    }).catch(function (error) {
      $this.errorMessage = utils.getErrorMessage(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiRedisConnect: function() {
    var $this = this;

    this.errorMessage = null;
    utils.loading(this, true);
    $api.post($url + '/actions/redisConnect', this.redisForm).then(function (response) {
      var res = response.data;

      $this.pageIndex++;
    }).catch(function (error) {
      $this.errorMessage = utils.getErrorMessage(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiPrepare: function () {
    var $this = this;

    this.errorMessage = null;
    utils.loading(this, true);
    $api.post($url + '/actions/prepare', _.assign({}, this.databaseForm, this.redisForm, this.adminForm)).then(function (response) {
      var res = response.data;

      setTimeout(function () {
        $this.apiInstall(res.value);
      }, 2000);
    }).catch(function (error) {
      utils.loading($this, false);
      $this.errorMessage = utils.getErrorMessage(error);
    });
  },

  apiInstall: function (securityKey) {
    var $this = this;

    this.errorMessage = null;
    utils.loading(this, true);
    $api.post($url + '/actions/install', _.assign({securityKey: securityKey}, this.databaseForm, this.redisForm, this.adminForm)).then(function (response) {
      var res = response.data;

      $this.pageIndex++;
    }).catch(function (error) {
      $this.errorMessage = utils.getErrorMessage(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getDocsUrl: function() {
    return cloud.getDocsUrl('');
  },

  validatePass: function(rule, value, callback) {
    if (value === '') {
      callback(new Error('请再次输入密码'));
    } else if (value !== this.adminForm.adminPassword) {
      callback(new Error('两次输入密码不一致!'));
    } else {
      callback();
    }
  },

  checkPasswordLevel: function (strPassword) {
    //check length
    var result = 0;
    if (strPassword.length == 0)
      result += 0;
    else if (strPassword.length < 8 && strPassword.length > 0)
      result += 5;
    else if (strPassword.length > 10)
      result += 25;
    else
      result += 10;
    //alert("检查长度:"+strPassword.length+"-"+result);
  
    //check letter
    var bHave = false;
    var bAll = false;
    var capital = strPassword.match(/[A-Z]{1}/); //找大写字母
    var small = strPassword.match(/[a-z]{1}/); //找小写字母
    if (capital == null && small == null) {
      result += 0; //没有字母
      bHave = false;
    } else if (capital != null && small != null) {
      result += 20;
      bAll = true;
    } else {
      result += 10;
      bAll = true;
    }
    //alert("检查字母："+result);
  
    //检查数字
    var bDigi = false;
    var digitalLen = 0;
    for (var i = 0; i < strPassword.length; i++) {
  
      if (strPassword.charAt(i) <= '9' && strPassword.charAt(i) >= '0') {
        bDigi = true;
        digitalLen += 1;
        //alert(strPassword[i]);
      }
  
    }
    if (digitalLen == 0) //没有数字
    {
      result += 0;
      bDigi = false;
    } else if (digitalLen > 2) //2个数字以上
    {
      result += 20;
      bDigi = true;
    } else {
      result += 10;
      bDigi = true;
    }
    //alert("数字个数：" + digitalLen);
    //alert("检查数字："+result);
  
    //检查非单词字符
    var bOther = false;
    var otherLen = 0;
    for (var i = 0; i < strPassword.length; i++) {
      if ((strPassword.charAt(i) >= '0' && strPassword.charAt(i) <= '9') ||
        (strPassword.charAt(i) >= 'A' && strPassword.charAt(i) <= 'Z') ||
        (strPassword.charAt(i) >= 'a' && strPassword.charAt(i) <= 'z'))
        continue;
      otherLen += 1;
      bOther = true;
    }
    if (otherLen == 0) //没有非单词字符
    {
      result += 0;
      bOther = false;
    } else if (otherLen > 1) //1个以上非单词字符
    {
      result += 25;
      bOther = true;
    } else {
      result += 10;
      bOther = true;
    }
    //alert("检查非单词："+result);
  
    //检查额外奖励
    if (bAll && bDigi && bOther)
      result += 5;
    else if (bHave && bDigi && bOther)
      result += 3;
    else if (bHave && bDigi)
      result += 2;
    //alert("检查额外奖励："+result);
  
    this.passwordLevel = (result / 100) * 5;
  },

  btnNextClick: function () {
    this.databaseNames = null;
    this.pageIndex++;
  },

  btnPreviousClick: function () {
    this.pageIndex--;
  },

  btnDatabaseConnectClick: function () {
    var $this = this;

    if (this.containerized) {
      if (this.databaseType === $sqlite) {
        this.pageIndex++;
        return;
      } else {
        $this.apiDatabaseConnect();
        return;
      }
    } else {
      if (this.databaseNames && this.databaseForm.databaseName || this.databaseForm.databaseType === $sqlite) {
        this.pageIndex++;
        return;
      }
  
      this.$refs.databaseForm.validate(function(valid) {
        if (valid) {
          $this.apiDatabaseConnect();
        }
      });
    }
  },

  btnRedisConnectClick: function () {
    var $this = this;

    if (this.containerized) {
      if (!this.redisConnectionString) {
        this.pageIndex++;
        return;
      } else {
        $this.apiRedisConnect();
      }
    } else {
      if (!this.redisForm.isRedis) {
        this.pageIndex++;
        return;
      }
  
      this.$refs.redisForm.validate(function(valid) {
        if (valid) {
          $this.apiRedisConnect();
        }
      });
    }
  },

  btnInstallClick: function() {
    var $this = this;

    this.$refs.adminForm.validate(function(valid) {
      if (valid) {
        $this.apiPrepare();
      }
    });
  },

  btnEnterClick: function() {
    location.href = utils.getIndexUrl();
  }
}

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});