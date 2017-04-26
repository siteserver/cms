import React from 'react';
export default class Footer extends React.Component {
    constructor(props) {
        super(props);
    }
    render() {
        return (<div id="ft">
        <p>{this.props.config.copyright}</p>
        <p><a target="_blank" href="http://www.miibeian.gov.cn">{this.props.config.beianNo}</a></p>
      </div>);
    }
}
//# sourceMappingURL=index.jsx.map