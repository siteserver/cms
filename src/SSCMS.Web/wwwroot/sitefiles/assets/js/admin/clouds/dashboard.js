var $url = "/clouds/dashboard"
var $urlDisconnect = $url + '/actions/disconnect';
var $urlCloud = "cms/dashboard";

var data = utils.init({
  isUpgrade: utils.getQueryBoolean('isUpgrade'),
  isConnect: false,
  iFrameUrl: '',
  cloudType: null,
  expirationDate: null,
  expirationDays: -1,
  upgradeAmount: null,
  cloudPriceBasic1Month: null,
  cloudPriceBasic1Year: null,
  cloudPriceBasic2Year: null,
  cloudPriceStandard1Month: null,
  cloudPriceStandard1Year: null,
  cloudPriceStandard2Year: null,
  features: null,
  counts: null,
  colors: [
    { color: '#5cb87a', percentage: 30 },
    { color: '#1989fa', percentage: 50 },
    { color: '#6f7ad3', percentage: 65 },
    { color: '#e6a23c', percentage: 80 },
    { color: '#f56c6c', percentage: 100 },
  ],
  active: 0,
  buyForm: {
    cloudType: '',
    periods: 'Y1',
    originalPrice: 0,
    save: 0,
    amount: 0,
    renewalTitle: '',
  }
});

var methods = {
  apiSubmit: function () {
    $api
      .post($url, {
        cloudType: this.cloudType,
        expirationDate: this.expirationDate,
      })
      .then(function (response) {
        var res = response.data;
      })
      .catch(function (error) {
        utils.error(error);
      });
  },

  apiCloudGet: function() {
    var $this = this;

    utils.loading(this, true);
    cloud.get($urlCloud).then(function (response) {
      var res = response.data;

      $this.cloudType = res.cloudType;
      $this.expirationDate = res.expirationDate;

      var diffInTime = (new Date(res.expirationDate)).getTime() - (new Date()).getTime();
      $this.expirationDays = parseInt(diffInTime / (1000 * 3600 * 24));


      $this.upgradeAmount = res.upgradeAmount;
      $this.cloudPriceBasic1Month = res.cloudPriceBasic1Month;
      $this.cloudPriceBasic1Year = res.cloudPriceBasic1Year;
      $this.cloudPriceBasic2Year = res.cloudPriceBasic2Year;
      $this.cloudPriceStandard1Month = res.cloudPriceStandard1Month;
      $this.cloudPriceStandard1Year = res.cloudPriceStandard1Year;
      $this.cloudPriceStandard2Year = res.cloudPriceStandard2Year;
      $this.features = res.features;
      $this.counts = res.counts;
      if ($this.isUpgrade) {
        setTimeout(function () {
          var url = location.href;
          location.href = '#upgrade';
          history.replaceState(null,null,url);
        }, 100);
      }

      $this.apiSubmit();
    }).catch(function (error) {
      utils.error(error, {
        ignoreAuth: true,
      });
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDisconnect: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlDisconnect).then(function (response) {
      var res = response.data;

      if (!res.value) return;
      cloud.logout();

      utils.alertSuccess({
        title: "云助手连接已断开!",
        button: "确 定",
        callback: function() {
          location.href = utils.getCloudsUrl('connect', {redirect: location.href});
        }
      });
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getUserName: function() {
    return $cloudUserName;
  },

  getExpirationTip: function() {
    if (this.expirationDays < 0) return '';
    if (this.expirationDays <= 10) return 'danger';
    if (this.expirationDays <= 20) return 'warning';
    return 'success';
  },

  getExpirationDate: function() {
    return utils.formatDate(this.expirationDate);
  },

  getCloudType: function() {
    if (this.cloudType == 'Basic') {
      return '基础版';
    } else if (this.cloudType == 'Standard') {
      return '标准版';
    }
    return '免费版';
  },

  btnBuyClick: function(cloudType) {
    this.active = 1;
    this.buyForm.cloudType = cloudType;
    this.buyForm.periods = 'Y1';
    this.btnChangeClick();
  },

  btnRenewalClick: function () {
    this.active = 2;
    this.buyForm.cloudType = this.cloudType;
    this.buyForm.periods = 'Y1';
    if (this.cloudType == 'Basic') {
      this.buyForm.renewalTitle = '基础版（¥' + this.cloudPriceBasic1Month + '/月）';
    } else if (this.cloudType == 'Standard') {
      this.buyForm.renewalTitle = '标准版（¥' + this.cloudPriceStandard1Month + '/月）';
    }
    this.btnChangeClick();
  },

  btnUpgradeClick: function () {
    this.active = 3;
    this.buyForm.cloudType = 'Standard';
    this.buyForm.amount = this.upgradeAmount;
  },

  btnPayClick: function() {
    var resourceType = 'CloudBuy';
    var title = '云助手购买';
    if (this.active == 2) {
      resourceType = 'CloudRenewal';
      title = '云助手续费';
    } else if (this.active == 3) {
      resourceType = 'CloudUpgrade';
      title = '云助手升级';
    }
    utils.openLayer({
      title: title,
      width: 600,
      height: 500,
      url: cloud.host + '/layer/pay.html?resourceType=' + resourceType + '&type=' + this.buyForm.cloudType + '&periods=' + this.buyForm.periods
    });

    window.addEventListener(
      'message',
      function(e) {
        if (e.origin !== cloud.host) return;
        location.href = utils.getCloudsUrl('dashboard', {r: Math.random()});
      },
      false,
    );
  },

  btnPreviousClick: function() {
    this.active = 0;
  },

  btnChangeClick: function() {
    this.buyForm.originalPrice = 0;
    var price = 0;
    if (this.buyForm.cloudType === 'Basic') {
      price = this.cloudPriceBasic1Month;
    } else if (this.buyForm.cloudType === 'Standard') {
      price = this.cloudPriceStandard1Month;
    }

    if (this.buyForm.periods === 'Y1') {
      this.buyForm.originalPrice = price * 12;
      if (this.buyForm.cloudType === 'Basic') {
        price = this.cloudPriceBasic1Year;
      } else if (this.buyForm.cloudType === 'Standard') {
        price = this.cloudPriceStandard1Year;
      }
      this.buyForm.save = Math.round((this.buyForm.originalPrice - price) / this.buyForm.originalPrice * 100);
    } else if (this.buyForm.periods === 'Y2') {
      this.buyForm.originalPrice = price * 24;
      if (this.buyForm.cloudType === 'Basic') {
        price = this.cloudPriceBasic2Year;
      } else if (this.buyForm.cloudType === 'Standard') {
        price = this.cloudPriceStandard2Year;
      }
      this.buyForm.save = Math.round((this.buyForm.originalPrice - price) / this.buyForm.originalPrice * 100);
    }

    this.buyForm.amount = price;
  },

  btnDisconnectClick: function() {
    var $this = this;

    utils.alertDelete({
      title: '断开云助手连接',
      text: '断开连接后云助手相关功能将无法使用，是否确定要断开连接？',
      button: '断开连接',
      callback: function () {
        $this.apiDisconnect();
      }
    });
  },

  btnTicketClick: function () {
    utils.addTab('工单技术支持', utils.getCloudsUrl('tickets', {
      isAdd: true,
      tabName: utils.getTabName()
    }));
  },

  btnDocsClick: function () {
    window.open('https://sscms.com/docs/v7/handbook/clouds/');
  },

  getOriginalAmount: function() {
    var amount = this.buyForm.cloudType == 'Basic' ? this.cloudPriceBasic1Month : this.cloudPriceStandard1Month;
    if (this.buyForm.periods == 'Y1') {
      amount *= 12;
    } else if (this.buyForm.periods == 'Y2') {
      amount *= 24;
    }
    return amount.toFixed(2);
  },

  btnCloseClick: function() {
    utils.removeTab();
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(null, this.btnCloseClick);
    var $this = this;
    cloud.checkAuth(function() {
      $this.apiCloudGet();
    });
  }
});
