if (window.navigator.userAgent.toLowerCase().indexOf('micromessenger') > 0) {
    axios.get(wxShare.apiUrl, {
        headers: {
            'Content-Type': 'application/json;charset=UTF-8',
        },
        params: {
            siteId: wxShare.siteId,
            url: location.href
        }
    }).then(function(resOrigin) {
        var res = resOrigin.data
        if (res.success) {
            wx.config({
                debug: false,
                appId: res.apiId,
                timestamp: res.timestamp,
                nonceStr: res.nonceStr,
                signature: res.signature,
                jsApiList: ['updateAppMessageShareData', 'updateTimelineShareData']
            });
            wx.ready(function() {
                var wxShareData = {
                    title: wxShare.title,
                    desc: wxShare.desc,
                    link: window.location.href,
                    imgUrl: wxShare.imgUrl
                };
                wx.updateAppMessageShareData(wxShareData);
                wx.updateTimelineShareData(wxShareData);
            });
        }
    }).catch(function(err) {
        console.error(err);
    });
}