import * as React from 'react';
import { connect } from 'react-redux';
import Select from 'react-select';
import { Loading } from '../../../components';
import { Attributes } from '../components';
import client from '../../../lib/client';
import * as utils from '../../../lib/utils';
class IndexPage extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            publishmentSystem: null,
            nodes: null,
            node: null
        };
    }
    componentDidMount() {
        let publishmentSystem = null;
        for (let ps of this.props.publishmentSystems) {
            if (this.props.account.user.additional.lastWritingPublishmentSystemId === ps.publishmentSystemId) {
                publishmentSystem = ps;
            }
        }
        if (!publishmentSystem)
            publishmentSystem = this.props.publishmentSystems[0];
        this.loadPublishmentSystem(publishmentSystem);
    }
    loadPublishmentSystem(publishmentSystem) {
        if (this.state.publishmentSystem && publishmentSystem.publishmentSystemId === this.state.publishmentSystem.publishmentSystemId)
            return;
        client.writing.getChannels(publishmentSystem.publishmentSystemId, (err, res) => {
            this.state.publishmentSystem = publishmentSystem;
            this.state.nodes = res;
            if (res.length > 0) {
                this.state.node = res[0];
            }
            this.setState(this.state);
        });
    }
    render() {
        if (!this.state.nodes)
            return <Loading />;
        let publishmentSystemsEl = null;
        if (this.props.publishmentSystems.length > 0) {
            const options = this.props.publishmentSystems.map((publishmentSystem) => {
                return { value: publishmentSystem.publishmentSystemId, label: publishmentSystem.publishmentSystemName };
            });
            publishmentSystemsEl = (<Select clearable={false} value={this.state.publishmentSystem.publishmentSystemId} options={options} onChange={(item) => {
                const publishmentSystem = utils.find(this.props.publishmentSystems, (publishmentSystem) => {
                    return publishmentSystem.publishmentSystemId === item.value;
                });
                this.loadPublishmentSystem(publishmentSystem);
            }}/>);
        }
        let nodesEl = null;
        if (this.state.nodes.length > 0) {
            const options = this.state.nodes.map((node) => {
                return { value: node.nodeId, label: node.nodeName };
            });
            nodesEl = (<Select clearable={false} value={this.state.node.nodeId} options={options} onChange={(item) => {
                this.state.node = {
                    nodeId: item.value,
                    nodeName: item.label
                };
                this.setState(this.state);
            }}/>);
        }
        let attributesEl = null;
        if (this.state.publishmentSystem && this.state.node) {
            attributesEl = <Attributes publishmentSystemId={this.state.publishmentSystem.publishmentSystemId} nodeId={this.state.node.nodeId}/>;
        }
        return (<div className="article">
        <div className="mod-3">
          <div className="art-mod">
            <div className="art-hd clearfix"><h2>发表稿件</h2></div>
            <div className="form form-1" style={{ marginTop: 25 }}>
              <ul>
                <li className="fm-item">
                  <label htmlFor="#" className="k">投稿站点：</label>
                  <span className="v" style={{ width: 365 }}>
                    {publishmentSystemsEl}
                  </span>
                </li>
                <li className="fm-item">
                  <label htmlFor="#" className="k">投稿栏目：</label>
                  <span className="v" style={{ width: 365 }}>
                    {nodesEl}
                  </span>
                </li>
                <li className="fm-item">
                  <hr className="hr"/>
                </li>
              </ul>
            </div>
            {attributesEl}
          </div>
        </div>
      </div>);
    }
}
function mapStateToProps(state) {
    return {
        config: state.config,
        account: state.account
    };
}
export default connect(mapStateToProps, null)(IndexPage);
//# sourceMappingURL=index.jsx.map