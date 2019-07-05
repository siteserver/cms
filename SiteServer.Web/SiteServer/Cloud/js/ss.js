var $urlStatus = '/v1/users/status'
var $urlLogin = '/v1/users/actions/login';
var $urlCaptchaGet = '/v1/captcha/LOGIN-CAPTCHA';
var $urlCaptchaCheck = '/v1/captcha/LOGIN-CAPTCHA/actions/check';

var $ssApi = axios.create({
  baseURL: $urlCloud,
  withCredentials: true
});

var ssUtils = {
  getToken: function () {
    return Cookies.get('SS-USER-TOKEN-CLIENT');
  },

  setToken: function (accessToken, expiresAt) {
    Cookies.set('SS-USER-TOKEN-CLIENT', accessToken, {
      expires: new Date(expiresAt)
    });
  },

  removeToken: function () {
    Cookies.remove('SS-USER-TOKEN-CLIENT');
  },

  redirectLogin: function () {
    if (location.hash) {
      location.href = 'pages/login.html';
    } else {
      top.location.hash = 'pages/login.html';
    }
  }
};