import * as React from 'react'
import { IndexLink, Link } from 'react-router'

export default class IndexPage extends React.Component<{}, {}> {
  constructor(props) {
    super(props)
  }

  render() {
    return (
      <div id="doc3">
        <div id="bd">
          <div className="grid-3 clearfix">
            <div className="article mod-4">
              <div className="art-mod">
                <div className="art-hd clearfix"><h2>设置用户名</h2></div>
                <div className="art-bd">
                  <p className="mod-info">用户名可以用来登录您的帐号，<span className="orange">保存后不可修改</span></p>
                  <div className="set-username quc-wrapper quc-page">
                    <div className="quc-mod-set-username">
                      <form className="quc-form">
                        <p className="quc-input-wrapper quc-input-long">
                          <span className="quc-input-bg">
                            <input type="text" name="username" className="quc-input quc-input-username" maxLength={14} autoComplete="off" placeholder="请输入用户名：2-14个字符,支持中英文" />
                          </span>
                          <input type="submit" defaultValue="保存" className="quc-submit quc-button quc-button-save" />
                        </p>
                        <p className="quc-tip" />
                        <div className="quc-alternative-wrapper">
                          <p className="quc-tip-error">用户名已经被占用，我们为您推荐以下用户名：</p>
                          <div className="quc-alternatives" />
                        </div>
                      </form>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }
}
