import * as React from 'react';
export default class IndexPage extends React.Component {
    constructor(props) {
        super(props);
    }
    render() {
        return (<div className="article">
        <div className="mod-3">
          <div className="art-mod">
            <div className="art-hd clearfix"><h2>投稿中心</h2></div>
            <div className="art-bd">
              <div className="mod-reslut-t2">
                <h3 className="f-s14">如何投稿</h3>
                <p>进入 <b className="green">发表稿件</b> 后选择对应的投稿 <b className="green"> 站点及栏目</b> 即可投稿，投稿后可能需要网站管理员审核通过后对应文章才可上线；</p>
                <br />
                <h3 className="f-s14">如何撤销</h3>
                <p>进入 <b className="green">稿件管理</b> 后选择对应的投稿 <b className="green"> 站点及栏目</b> ，找到已投稿件，点击删除即可撤销稿件。</p>
              </div>
            </div>
          </div>
        </div>
      </div>);
    }
}
//# sourceMappingURL=index.jsx.map