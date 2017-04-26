import * as React from 'react'
import './404.css'

export default class ChannelPage extends React.Component<{}, {}> {
  constructor() {
    super()
  }

  componentDidMount() {
    var emojiCount = 30;
    function getRandomInt(min, max) {
      return Math.floor(Math.random() * (max - min + 1)) + min;
    }

    function catchFeelings() {
      var emoji = getRandomInt(0, emojiCount - 1)
      if (window.innerWidth > 400) {
        var bgOffset = (emoji * 200 * -1) + "px 0px"
      } else {
        var bgOffset = (emoji * 100 * -1) + "px 0px"
      }
      document.getElementById('emoji').style.backgroundPosition = bgOffset;
    }

    catchFeelings();

    setTimeout(function() {
      setInterval(function() {
        catchFeelings()
      }, 3000)
    }, 3000)

    document.addEventListener('keydown', function(e: any) {
      e = e || window.event;
      switch (e.which || e.keyCode) {
        case 32:
          for (var i = 0; i < 20; i++) {
            setTimeout(function() {
              catchFeelings();
            }, 50 * i)
          }
          break;

        default:
          return;
      }
      e.preventDefault();
    });
  }

  render() {
    return (
      <div className="four_oh_four">
        <figure className="error">
          <h1>4<div id="emoji"></div>4</h1>
          <h2>对不起，此页面不存在！</h2>
        </figure>
        <ul className="nav">
          <li><a href="/">首页</a></li>
          <li><a href="/about">关于百容</a></li>
          <li><a href="/success">成功案例</a></li>
          <li><a href="/join">人才招聘</a></li>
          <li><a href="/new">发布项目</a></li>
        </ul>
      </div>
    );
  }
}
