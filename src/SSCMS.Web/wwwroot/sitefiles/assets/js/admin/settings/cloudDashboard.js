var $url = "/settings/cloudDashboard"
var $urlDisconnect = $url + '/actions/disconnect';
var $urlCloud = "cms/dashboard";

var data = utils.init({
  isConnect: false,
  iFrameUrl: '',
  cloudType: null,
  expirationDate: null,
  upgradeAmount: null,
  cloudPriceStandard1Month: null,
  cloudPriceProfessional1Month: null,
  cloudPriceStandard1Year: null,
  cloudPriceProfessional1Year: null,
  cloudPriceStandard2Year: null,
  cloudPriceProfessional2Year: null,
  features: null,
  counts: null,
  colors: [
    { color: '#6f7ad3', percentage: 20 },
    { color: '#1989fa', percentage: 40 },
    { color: '#5cb87a', percentage: 60 },
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
  getUserName: function() {
    return $cloudUserName;
  },

  getExpirationDate: function() {
    return utils.formatDate(this.expirationDate);
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
    if (this.cloudType == 'Standard') {
      this.buyForm.renewalTitle = '标准版（¥' + this.cloudPriceStandard1Month + '/月）';
    } else if (this.cloudType == 'Professional') {
      this.buyForm.renewalTitle = '专业版（¥' + this.cloudPriceProfessional1Month + '/月）';
    }
    this.btnChangeClick();
  },

  btnUpgradeClick: function () {
    this.active = 3;
    this.buyForm.cloudType = 'Professional';
    this.buyForm.amount = this.upgradeAmount;
  },

  btnPayClick: function() {
    var resourceType = 'CloudBuy';
    if (this.active == 2) {
      resourceType = 'CloudRenewal';
    } else if (this.active == 3) {
      resourceType = 'CloudUpgrade';
    }
    utils.openLayer({
      title: '购买',
      width: 600,
      height: 500,
      url: cloud.host + '/layer/pay.html?resourceType=' + resourceType + '&type=' + this.buyForm.cloudType + '&periods=' + this.buyForm.periods
    });

    window.addEventListener(
      'message',
      function(e) {
        if (e.origin !== cloud.host) return;
        location.href = utils.getSettingsUrl('cloudDashboard', {r: Math.random()});
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
    if (this.buyForm.cloudType === 'Standard') {
      price = this.cloudPriceStandard1Month;
    } else if (this.buyForm.cloudType === 'Professional') {
      price = this.cloudPriceProfessional1Month;
    }

    if (this.buyForm.periods === 'Y1') {
      this.buyForm.originalPrice = price * 12;
      if (this.buyForm.cloudType === 'Standard') {
        price = this.cloudPriceStandard1Year;
      } else if (this.buyForm.cloudType === 'Professional') {
        price = this.cloudPriceProfessional1Year;
      }
      this.buyForm.save = Math.round((this.buyForm.originalPrice - price) / this.buyForm.originalPrice * 100);
    } else if (this.buyForm.periods === 'Y2') {
      this.buyForm.originalPrice = price * 24;
      if (this.buyForm.cloudType === 'Standard') {
        price = this.cloudPriceStandard2Year;
      } else if (this.buyForm.cloudType === 'Professional') {
        price = this.cloudPriceProfessional2Year;
      }
      this.buyForm.save = Math.round((this.buyForm.originalPrice - price) / this.buyForm.originalPrice * 100);
    }

    this.buyForm.amount = price;
  },

  apiCloudGet: function() {
    var $this = this;

    utils.loading(this, true);
    cloud.get($urlCloud).then(function (response) {
      var res = response.data;

      $this.cloudType = res.cloudType;
      $this.expirationDate = res.expirationDate;
      $this.upgradeAmount = res.upgradeAmount;
      $this.cloudPriceStandard1Month = res.cloudPriceStandard1Month;
      $this.cloudPriceProfessional1Month = res.cloudPriceProfessional1Month;
      $this.cloudPriceStandard1Year = res.cloudPriceStandard1Year;
      $this.cloudPriceProfessional1Year = res.cloudPriceProfessional1Year;
      $this.cloudPriceStandard2Year = res.cloudPriceStandard2Year;
      $this.cloudPriceProfessional2Year = res.cloudPriceProfessional2Year;
      $this.features = res.features;
      $this.counts = res.counts;
    }).catch(function (error) {
      utils.error(error);
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
          location.href = utils.getSettingsUrl('cloudConnect', {redirect: location.href});
        }
      });
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
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
    utils.addTab('工单技术支持', utils.getSettingsUrl('cloudTickets', {
      isAdd: true,
      tabName: utils.getTabName()
    }));
  },

  btnDocsClick: function () {
    window.open('https://sscms.com/docs/v7/');
  },

  getCloudProduct: function(product) {
    if (product === 'Censor') return '文字违规检测';
    if (product === 'Spell') return '错别字检查';
    if (product === 'Vod') return '云视频';
    if (product === 'Sms') return '短信';
    if (product === 'Mail') return '邮件';
    return '';
  },

  getOriginalAmount: function() {
    var amount = this.buyForm.cloudType == 'Standard' ? this.cloudPriceStandard1Month : this.cloudPriceProfessional1Month;
    if (this.buyForm.periods == 'Y1') {
      amount *= 12;
    } else if (this.buyForm.periods == 'Y2') {
      amount *= 24;
    }
    return amount.toFixed(2);
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    var $this = this;
    cloud.checkAuth(function() {
      $this.apiCloudGet();
    });
  }
});
