import * as React from 'react';
import { Link, hashHistory } from 'react-router';
import cx from 'classnames';
import { Loading } from '../../../../components';
import client from '../../../../lib/client';
require('moment/locale/zh-cn');
import Datetime from 'react-datetime';
import * as utils from '../../../../lib/utils';
import { UEditor, ImageUpload } from '../../../components';
export default class Attributes extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            content: utils.assign({}, props.content),
            tableStyles: null,
            errors: {}
        };
    }
    componentDidMount() {
        this.load(this.props.publishmentSystemId, this.props.nodeId);
    }
    componentWillReceiveProps(nextProps, nextState, nextContext) {
        if (nextProps.publishmentSystemId !== this.props.publishmentSystemId || nextProps.nodeId !== this.props.nodeId) {
            this.load(nextProps.publishmentSystemId, nextProps.nodeId);
        }
    }
    load(publishmentSystemId, nodeId) {
        client.writing.getTableStyles(publishmentSystemId, nodeId, (err, res) => {
            this.state.tableStyles = res;
            this.setState(this.state);
        });
    }
    isAttribute(tableStyle) {
        if (!tableStyle.isVisible)
            return false;
        return !utils.findIgnoreCase(tableStyle.attributeName, [
            utils.ContentAttributeName.IsRecommend,
            utils.ContentAttributeName.IsHot,
            utils.ContentAttributeName.IsColor,
            utils.ContentAttributeName.IsTop,
            utils.ContentAttributeName.LinkUrl,
            utils.ContentAttributeName.VideoUrl,
            utils.ContentAttributeName.FileUrl,
            utils.ContentAttributeName.AddDate
        ]);
    }
    submit() {
        const content = this.state.content;
        let errors = {};
        for (let tableStyle of this.state.tableStyles) {
            if (this.isAttribute(tableStyle)) {
                if (tableStyle.inputType === utils.InputType.TextEditor) {
                    content[utils.lowerFirst(tableStyle.attributeName)] = UE.getEditor(tableStyle.attributeName).getContent();
                }
                if (tableStyle.additional.isRequired && !content[utils.lowerFirst(tableStyle.attributeName)]) {
                    errors[tableStyle.attributeName] = true;
                }
            }
        }
        if (!content.content) {
            errors['Content'] = true;
        }
        this.state.errors = errors;
        this.setState(this.state);
        if (utils.keys(errors).length > 0)
            return;
        if (!this.props.content) {
            client.writing.createContent(this.props.publishmentSystemId, this.props.nodeId, content, (err, res) => {
                if (err) {
                    return utils.Swal.error(err);
                }
                return utils.Swal.success('稿件发表成功', () => {
                    hashHistory.push('/writing/list');
                });
            });
        }
        else {
            client.writing.editContent(this.props.publishmentSystemId, this.props.nodeId, this.props.id, content, (err, res) => {
                if (err) {
                    return utils.Swal.error(err);
                }
                return utils.Swal.success('稿件编辑成功', () => {
                    hashHistory.push('/writing/list');
                });
            });
        }
    }
    getField(tableStyle) {
        if (!this.isAttribute(tableStyle))
            return null;
        let value = '';
        if (this.props.id) {
            value = this.state.content[utils.lowerFirst(tableStyle.attributeName)] || '';
        }
        else {
            value = this.state.content[utils.lowerFirst(tableStyle.attributeName)] || tableStyle.defaultValue || '';
        }
        const isError = this.state.errors[tableStyle.attributeName];
        let inputEl = null;
        if (tableStyle.inputType === utils.InputType.Text || tableStyle.inputType === utils.InputType.TextEditor) {
            inputEl = <input value={value} onChange={(e) => {
                this.state.content[utils.lowerFirst(tableStyle.attributeName)] = e.target.value;
                this.setState(this.state);
            }} type="text" className={cx('text input-xxl', { 'error': isError })}/>;
        }
        else if (tableStyle.inputType === utils.InputType.TextArea) {
            inputEl = <textarea value={value} onChange={(e) => {
                this.state.content[utils.lowerFirst(tableStyle.attributeName)] = e.target.value;
                this.setState(this.state);
            }} className={cx('textarea input-xxl', { 'error': isError })}/>;
        }
        else if (tableStyle.inputType === utils.InputType.DateTime) {
            if (value === '{Current}') {
                value = new Date();
            }
            inputEl = <Datetime value={value} onChange={(date) => {
                this.state.content[utils.lowerFirst(tableStyle.attributeName)] = date.format('YYYY-MM-DD hh:mm:ss');
                this.setState(this.state);
            }} className={cx('text input-xl', { 'error': isError })} timeFormat="HH:mm:ss"/>;
        }
        else if (tableStyle.inputType === utils.InputType.Image) {
            var extendAttributeName = utils.ContentAttributeName.getExtendAttributeName(utils.ContentAttributeName.ImageUrl);
            var imageUrls = [];
            if (this.state.content.imageUrl) {
                imageUrls.push(this.state.content.imageUrl);
            }
            if (this.state.content[extendAttributeName]) {
                var urls = utils.trim(this.state.content[extendAttributeName], ',').split(',');
                for (var url of urls) {
                    imageUrls.push(url);
                }
            }
            inputEl = <ImageUpload publishmentSystemId={this.props.publishmentSystemId} imageUrls={imageUrls} onChange={(imageUrls) => {
                this.state.content.imageUrl = '';
                this.state.content[extendAttributeName] = '';
                if (imageUrls && imageUrls.length > 0) {
                    var i = 0;
                    for (var imageUrl of imageUrls) {
                        if (i === 0) {
                            this.state.content.imageUrl = imageUrl;
                        }
                        else {
                            this.state.content[extendAttributeName] += imageUrl + ',';
                        }
                        i++;
                    }
                }
                this.setState(this.state);
            }}/>;
        }
        if (!inputEl)
            return null;
        let errorEl = null;
        if (this.state.errors[tableStyle.attributeName]) {
            errorEl = <span className="text-error"><i className="ico ico-err-2"/><em className="error-message">请填写{tableStyle.displayName}</em></span>;
        }
        if (tableStyle.inputType === 'TextEditor') {
            return (<li key={tableStyle.attributeName} className="fm-item" style={{ height: 'auto', marginLeft: 40 }}>
          <UEditor id={tableStyle.attributeName} height={350} value={value}/>
          <div style={{ textAlign: 'right', marginRight: 50 }}>{errorEl}</div>
        </li>);
        }
        else {
            return (<li key={tableStyle.attributeName} className="fm-item">
          <label htmlFor="#" className="k">{tableStyle.displayName}：</label>
          <span className="v">
            {inputEl}
            {errorEl}
          </span>
        </li>);
        }
    }
    render() {
        if (!this.state.tableStyles)
            return <Loading />;
        const fields = this.state.tableStyles.map((tableStyle) => this.getField(tableStyle));
        return (<div className="form form-1">
        <ul>
          {fields}
        </ul>
        <div className="btns" style={{ margin: 20 }}>
          <span onClick={this.submit.bind(this)} href="javascript:;" className="btn btn-3">保存修改</span>
          <Link to="/writing/list" className="btn btn-4" style={{ marginLeft: 20 }}>返回列表</Link>
        </div>
      </div>);
    }
}
//# sourceMappingURL=index.jsx.map