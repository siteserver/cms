import * as React from 'react';
import { Menu } from './components';
export default class IndexPage extends React.Component {
    constructor(props) {
        super(props);
    }
    render() {
        return (<div>
        <div id="doc3">
          <div id="bd">
            <div className="grid-2 clearfix">
              <Menu location={this.props.location}/>
              {this.props.children}
            </div>
          </div>
        </div>
      </div>);
    }
}
//# sourceMappingURL=layout.jsx.map