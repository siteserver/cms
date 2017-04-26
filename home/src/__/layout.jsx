import * as React from 'react';
import { hashHistory } from 'react-router';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { Header } from './components';
import { Footer } from '../components';
import { loggedOut } from '../lib/actions/account';
class LayoutPage extends React.Component {
    constructor(props) {
        super(props);
    }
    componentDidMount() {
        if (!this.props.account || !this.props.account.user) {
            hashHistory.push('/login');
        }
    }
    render() {
        if (!this.props.account || !this.props.account.user) {
            return null;
        }
        return (<div>
        <Header location={this.props.location} config={this.props.config} account={this.props.account} loggedOut={this.props.loggedOut}/>
        {this.props.children}
        <Footer config={this.props.config}/>
      </div>);
    }
}
function mapStateToProps(state) {
    return {
        config: state.config,
        account: state.account,
    };
}
function mapDispatchToProps(dispatch) {
    return {
        loggedOut: bindActionCreators(loggedOut, dispatch),
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(LayoutPage);
//# sourceMappingURL=layout.jsx.map