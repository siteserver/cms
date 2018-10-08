var authUtils = {
  parseJson: function (json) {
    try {
      return JSON.parse(json);
    } catch (e) {}
    return null;
  },

  getUser: function () {
    return this.parseJson(Cookies.get('SS-USER'));
  },

  getToken: function () {
    return Cookies.get('SS-USER-TOKEN');
  },

  getExpiresAt: function () {
    return parseInt(Cookies.get('SS-USER-EXPIRES-AT'));
  },

  isAuthenticated: function () {
    return this.getUser() && this.getToken() && new Date().getTime() < this.getExpiresAt();
  },

  redirectLogin: function () {
    if (location.hash) {
      location.href = 'pages/login.html';
    } else {
      top.location.hash = 'pages/login.html';
    }
  },

  // addAuth: function (authData, redirectUrl) {
  //   this.state.user = authData.value
  //   this.state.accessToken = authData.accessToken
  //   this.state.expiresAt = authData.expiresAt

  //   localStorage.setItem('SS-USER', JSON.stringify(this.state.user))
  //   localStorage.setItem('SS-USER-TOKEN', this.state.accessToken)
  //   localStorage.setItem('SS-USER-EXPIRES-AT', this.state.expiresAt.toString())

  //   var returnUrl = pageUtils.getQueryString("returnUrl") || redirectUrl
  //   location.href = returnUrl
  // },

  saveUser: function (user) {
    Cookies.set('SS-USER', JSON.stringify(user), {
      expires: 7,
      path: '/'
    })
  },

  // clearAuth: function (redirectUrl) {
  //   this.state.user = null
  //   this.state.accessToken = null
  //   this.state.expiresAt = null

  //   localStorage.removeItem('SS-USER')
  //   localStorage.removeItem('SS-USER-TOKEN')
  //   localStorage.removeItem('SS-USER-EXPIRES-AT')

  //   let returnUrl = pageUtils.getQueryString("returnUrl") || redirectUrl
  //   location.href = returnUrl
  // }
};