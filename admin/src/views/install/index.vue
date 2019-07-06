<template>
  <div class="install-container">

    <el-card class="box-card">
      <div slot="header" class="clearfix">
        <span>SS CMS 安装向导</span>
        <lang-select style="float: right; padding: 3px 0" class="set-language" />
      </div>
      <div v-if="environment.isInstalled" class="text item">
        <el-alert
          title="系统已安装，向导被禁用"
          type="error"
          effect="dark"
          :closable="false"
        />
      </div>
      <div v-else v-loading="loading" class="text item">

        <el-steps :active="active" finish-status="success">
          <el-step title="许可协议" />
          <el-step title="环境检测" />
          <el-step title="数据库设置" />
          <el-step title="缓存设置" />
          <el-step title="系统设置" />
          <el-step title="安装系统" />
        </el-steps>

        <el-alert
          v-if="errorMessage"
          :title="errorMessage"
          type="error"
          effect="dark"
          :closable="false"
        />

        <hr>

        <template v-if="active === 0">
          <div class="form-group">
            <label class="col-form-label">
              <a href="https://github.com/siteserver/cms/blob/staging/LICENSE" target="_blank">SiteServer CMS 开源协议（GPL-3.0）</a>
            </label>
          </div>

          <iframe style="border-color:#F5F5F5; border-width:1px;" scrolling="yes" src="/agreement.html" height="320" width="100%" />

          <div class="text-center mt-3">
            <div class="checkbox checkbox-primary">
              <input id="autoLogin" v-model="isAgreement" type="checkbox">
              <label for="autoLogin">
                我已经阅读并同意此协议
              </label>
            </div>
          </div>
        </template>

        <template v-else-if="active === 1">
          <div class="form-group">
            <label class="col-form-label">
              服务器信息
            </label>
            <small class="form-text text-muted">下表显示当前服务器环境</small>
          </div>

          <div class="panel panel-default">
            <div class="panel-body p-0">
              <div class="table-responsive">
                <table class="tablesaw table table-hover m-b-0 tablesaw-stack">
                  <thead>
                    <tr>
                      <th>参数</th>
                      <th>值</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td>API 地址</td>
                      <td>
                        {{ environment.apiUrl }}
                      </td>
                    </tr>
                    <tr>
                      <td>SS CMS 版本</td>
                      <td>
                        {{ environment.productVersion }}
                      </td>
                    </tr>
                    <tr>
                      <td>系统根目录</td>
                      <td>
                        {{ environment.contentRootPath }}
                      </td>
                    </tr>
                    <tr>
                      <td>Web 根目录</td>
                      <td>
                        {{ environment.webRootPath }}
                      </td>
                    </tr>
                    <tr>
                      <td>运行环境</td>
                      <td>
                        {{ environment.targetFramework }}
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>

          <div class="form-group">
            <label class="col-form-label">
              目录权限检测
            </label>
            <small class="form-text text-muted">
              系统要求必须满足下列所有的目录权限全部可读写的需求才能使用，如果没有相关权限请添加。
            </small>
          </div>

          <div class="panel panel-default">
            <div class="panel-body p-0">
              <div class="table-responsive">
                <table class="tablesaw table table-hover m-b-0 tablesaw-stack">
                  <thead>
                    <tr>
                      <th>目录名</th>
                      <th>读写权限</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td>
                        {{ environment.contentRootPath }}
                      </td>
                      <td>
                        <font v-if="environment.isContentRootPathWritable" color="green">
                          [√]
                        </font>
                        <font v-else color="red">
                          [×]
                        </font>
                      </td>
                    </tr>
                    <tr>
                      <td>
                        {{ environment.webRootPath }}
                      </td>
                      <td>
                        <font v-if="environment.isWebRootPathWritable" color="green">
                          [√]
                        </font>
                        <font v-else color="red">
                          [×]
                        </font>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </template>

        <template v-else-if="active === 2">
          <form class="form-horizontal" @submit="btnNextClick">
            <div class="form-group">
              <label class="col-form-label">
                数据库类型
                <small v-show="errors.has('databaseType')" class="text-danger">
                  {{ errors.first('databaseType') }}
                </small>
              </label>
              <select
                key="databaseType"
                v-model="databaseType"
                v-validate="'required'"
                name="databaseType"
                data-vv-as="数据库类型"
                class="form-control"
                :class="{'is-invalid': errors.has('databaseType') }"
              >
                <option value="MySql">
                  MySQL
                </option>
                <option value="SqlServer">
                  SQL Server
                </option>
                <option value="PostgreSql">
                  PostgreSQL
                </option>
                <option value="SQLite">
                  SQLite
                </option>
                <option value="Oracle">
                  Oracle
                </option>
              </select>
              <small class="form-text text-muted">
                请选择需要安装的数据库类型。
              </small>
            </div>

            <template v-if="databaseType && databaseType !== 'SQLite'">

              <div class="form-group">
                <el-switch
                  v-model="databaseIsConnectionString"
                  active-text="自定义连接字符串"
                />
              </div>

              <template v-if="databaseIsConnectionString">
                <div class="form-group">
                  <label class="col-form-label">
                    数据库连接字符串
                    <small v-show="errors.has('databaseConnectionString')" class="text-danger">
                      {{ errors.first('databaseConnectionString') }}
                    </small>
                  </label>
                  <input
                    key="databaseConnectionString"
                    v-model="databaseConnectionString"
                    v-validate="'required'"
                    name="databaseConnectionString"
                    type="text"
                    data-vv-as="数据库连接字符串"
                    :class="{'is-invalid': errors.has('databaseConnectionString') }"
                    class="form-control"
                  >
                  <small class="form-text text-muted">
                    Connection String
                  </small>
                </div>
              </template>
              <template v-else>
                <div class="form-group">
                  <label class="col-form-label">
                    数据库主机
                    <small v-show="errors.has('databaseServer')" class="text-danger">
                      {{ errors.first('databaseServer') }}
                    </small>
                  </label>
                  <input
                    key="databaseServer"
                    v-model="databaseServer"
                    v-validate="'required'"
                    name="databaseServer"
                    type="text"
                    data-vv-as="数据库主机"
                    :class="{'is-invalid': errors.has('databaseServer') }"
                    class="form-control"
                  >
                  <small class="form-text text-muted">
                    IP地址或者服务器名
                  </small>
                </div>

                <div class="form-group">
                  <label class="col-form-label">
                    数据库端口
                    <small v-show="errors.has('databaseIsDefaultPort')" class="text-danger">
                      {{ errors.first('databaseIsDefaultPort') }}
                    </small>
                  </label>
                  <select
                    key="databaseIsDefaultPort"
                    v-model="databaseIsDefaultPort"
                    v-validate="'required'"
                    name="databaseIsDefaultPort"
                    data-vv-as="数据库端口"
                    class="form-control"
                    :class="{'is-invalid': errors.has('databaseIsDefaultPort') }"
                  >
                    <option :value="true">
                      默认数据库端口
                    </option>
                    <option :value="false">
                      自定义数据库端口
                    </option>
                  </select>
                </div>

                <div v-if="!databaseIsDefaultPort" class="form-group">
                  <label class="col-form-label">
                    自定义端口
                    <small v-show="errors.has('databasePort')" class="text-danger">
                      {{ errors.first('databasePort') }}
                    </small>
                  </label>
                  <input
                    key="databasePort"
                    v-model="databasePort"
                    v-validate="'required|numeric'"
                    name="databasePort"
                    type="text"
                    data-vv-as="自定义端口"
                    :class="{'is-invalid': errors.has('databasePort') }"
                    class="form-control"
                  >
                  <small class="form-text text-muted">
                    连接数据库的端口
                  </small>
                </div>

                <div class="form-group">
                  <label class="col-form-label">
                    数据库用户名
                    <small v-show="errors.has('databaseUid')" class="text-danger">
                      {{ errors.first('databaseUid') }}
                    </small>
                  </label>
                  <input
                    key="databaseUid"
                    v-model="databaseUid"
                    v-validate="'required'"
                    name="databaseUid"
                    type="text"
                    data-vv-as="数据库用户名"
                    :class="{'is-invalid': errors.has('databaseUid') }"
                    autocomplete="false"
                    class="form-control"
                  >
                  <small class="form-text text-muted">
                    连接数据库的用户名
                  </small>
                </div>

                <div class="form-group">
                  <label class="col-form-label">
                    数据库密码
                    <small v-show="errors.has('databasePwd')" class="text-danger">
                      {{ errors.first('databasePwd') }}
                    </small>
                  </label>
                  <input
                    key="databasePwd"
                    v-model="databasePwd"
                    v-validate="'required'"
                    name="databasePwd"
                    type="password"
                    data-vv-as="数据库密码"
                    :class="{'is-invalid': errors.has('databasePwd') }"
                    autocomplete="false"
                    class="form-control"
                  >
                  <small class="form-text text-muted">
                    连接数据库的密码
                  </small>
                </div>
                <div v-if="databaseNames" class="form-group">
                  <label class="col-form-label">
                    数据库
                    <small v-show="errors.has('databaseName')" class="text-danger">
                      {{ errors.first('databaseName') }}
                    </small>
                  </label>
                  <select
                    key="databaseName"
                    v-model="databaseName"
                    v-validate="'required'"
                    name="databaseName"
                    data-vv-as="数据库"
                    class="form-control"
                    :class="{'is-invalid': errors.has('databaseName') }"
                  >
                    <option v-for="name in databaseNames" :key="name" :value="name">
                      {{ name }}
                    </option>
                  </select>
                  <small class="form-text text-muted">
                    请选择需要安装的数据库。
                  </small>
                </div>
              </template>
            </template>
          </form>
        </template>

        <template v-else-if="active === 3">
          <form class="form-horizontal" @submit="btnNextClick">
            <div class="form-group">
              <label class="col-form-label">
                缓存方式
                <small v-show="errors.has('cacheType')" class="text-danger">
                  {{ errors.first('cacheType') }}
                </small>
              </label>
              <select
                key="cacheType"
                v-model="cacheType"
                v-validate="'required'"
                name="cacheType"
                data-vv-as="缓存方式"
                class="form-control"
                :class="{'is-invalid': errors.has('cacheType') }"
              >
                <option value="Memory">
                  Memory
                </option>
                <option value="SqlServer">
                  SQL Server
                </option>
                <option value="Redis">
                  Redis
                </option>
              </select>
              <small class="form-text text-muted">
                请选择系统使用的缓存类型。
              </small>
            </div>

            <template v-if="cacheType && cacheType !== 'Memory'">
              <div class="form-group">
                <el-switch
                  v-model="cacheIsConnectionString"
                  active-text="自定义连接字符串"
                />
              </div>

              <template v-if="cacheIsConnectionString">
                <div class="form-group">
                  <label class="col-form-label">
                    连接字符串
                    <small v-show="errors.has('cacheConnectionString')" class="text-danger">
                      {{ errors.first('cacheConnectionString') }}
                    </small>
                  </label>
                  <input
                    key="cacheConnectionString"
                    v-model="cacheConnectionString"
                    v-validate="'required'"
                    name="cacheConnectionString"
                    type="text"
                    data-vv-as="连接字符串"
                    :class="{'is-invalid': errors.has('cacheConnectionString') }"
                    class="form-control"
                  >
                  <small class="form-text text-muted">
                    Connection String
                  </small>
                </div>
              </template>
              <template v-else-if="cacheType === 'SqlServer'">
                <div class="form-group">
                  <label class="col-form-label">
                    数据库主机
                    <small v-show="errors.has('cacheServer')" class="text-danger">
                      {{ errors.first('cacheServer') }}
                    </small>
                  </label>
                  <input
                    key="cacheServer"
                    v-model="cacheServer"
                    v-validate="'required'"
                    name="cacheServer"
                    type="text"
                    data-vv-as="数据库主机"
                    :class="{'is-invalid': errors.has('cacheServer') }"
                    class="form-control"
                  >
                  <small class="form-text text-muted">
                    IP地址或者服务器名
                  </small>
                </div>

                <div class="form-group">
                  <label class="col-form-label">
                    数据库端口
                    <small v-show="errors.has('cacheIsDefaultPort')" class="text-danger">
                      {{ errors.first('cacheIsDefaultPort') }}
                    </small>
                  </label>
                  <select
                    key="cacheIsDefaultPort"
                    v-model="cacheIsDefaultPort"
                    v-validate="'required'"
                    name="cacheIsDefaultPort"
                    data-vv-as="数据库端口"
                    class="form-control"
                    :class="{'is-invalid': errors.has('cacheIsDefaultPort') }"
                  >
                    <option :value="true">
                      默认数据库端口
                    </option>
                    <option :value="false">
                      自定义数据库端口
                    </option>
                  </select>
                </div>

                <div v-if="!cacheIsDefaultPort" class="form-group">
                  <label class="col-form-label">
                    自定义端口
                    <small v-show="errors.has('cachePort')" class="text-danger">
                      {{ errors.first('cachePort') }}
                    </small>
                  </label>
                  <input
                    key="cachePort"
                    v-model="cachePort"
                    v-validate="'required|numeric'"
                    name="cachePort"
                    type="text"
                    data-vv-as="自定义端口"
                    :class="{'is-invalid': errors.has('cachePort') }"
                    class="form-control"
                  >
                  <small class="form-text text-muted">
                    连接数据库的端口
                  </small>
                </div>

                <div class="form-group">
                  <label class="col-form-label">
                    数据库用户名
                    <small v-show="errors.has('cacheUid')" class="text-danger">
                      {{ errors.first('cacheUid') }}
                    </small>
                  </label>
                  <input
                    key="cacheUid"
                    v-model="cacheUid"
                    v-validate="'required'"
                    name="cacheUid"
                    type="text"
                    data-vv-as="数据库用户名"
                    :class="{'is-invalid': errors.has('cacheUid') }"
                    autocomplete="false"
                    class="form-control"
                  >
                  <small class="form-text text-muted">
                    连接数据库的用户名
                  </small>
                </div>

                <div class="form-group">
                  <label class="col-form-label">
                    数据库密码
                    <small v-show="errors.has('cachePwd')" class="text-danger">
                      {{ errors.first('cachePwd') }}
                    </small>
                  </label>
                  <input
                    key="cachePwd"
                    v-model="cachePwd"
                    v-validate="'required'"
                    name="cachePwd"
                    type="password"
                    data-vv-as="数据库密码"
                    :class="{'is-invalid': errors.has('cachePwd') }"
                    autocomplete="false"
                    class="form-control"
                  >
                  <small class="form-text text-muted">
                    连接数据库的密码
                  </small>
                </div>
                <div v-if="cacheNames" class="form-group">
                  <label class="col-form-label">
                    数据库
                    <small v-show="errors.has('cacheName')" class="text-danger">
                      {{ errors.first('cacheName') }}
                    </small>
                  </label>
                  <select
                    key="cacheName"
                    v-model="cacheName"
                    v-validate="'required'"
                    name="cacheName"
                    data-vv-as="数据库"
                    class="form-control"
                    :class="{'is-invalid': errors.has('cacheName') }"
                  >
                    <option v-for="name in cacheNames" :key="name" :value="name">
                      {{ name }}
                    </option>
                  </select>
                  <small class="form-text text-muted">
                    请选择需要安装的数据库。
                  </small>
                </div>
              </template>
              <template v-else-if="cacheType === 'Redis'">
                <div class="form-group">
                  <label class="col-form-label">
                    Redis 主机
                    <small v-show="errors.has('cacheServer')" class="text-danger">
                      {{ errors.first('cacheServer') }}
                    </small>
                  </label>
                  <input
                    key="cacheServer"
                    v-model="cacheServer"
                    v-validate="'required'"
                    name="cacheServer"
                    type="text"
                    data-vv-as="Redis 主机"
                    :class="{'is-invalid': errors.has('cacheServer') }"
                    class="form-control"
                  >
                  <small class="form-text text-muted">
                    IP地址或者服务器名
                  </small>
                </div>

                <div class="form-group">
                  <label class="col-form-label">
                    Redis 端口
                    <small v-show="errors.has('cacheIsDefaultPort')" class="text-danger">
                      {{ errors.first('cacheIsDefaultPort') }}
                    </small>
                  </label>
                  <select
                    key="cacheIsDefaultPort"
                    v-model="cacheIsDefaultPort"
                    v-validate="'required'"
                    name="cacheIsDefaultPort"
                    data-vv-as="Redis 端口"
                    class="form-control"
                    :class="{'is-invalid': errors.has('cacheIsDefaultPort') }"
                  >
                    <option :value="true">
                      默认 Redis 端口
                    </option>
                    <option :value="false">
                      自定义 Redis 端口
                    </option>
                  </select>
                </div>

                <div v-if="!cacheIsDefaultPort" class="form-group">
                  <label class="col-form-label">
                    自定义端口
                    <small v-show="errors.has('cachePort')" class="text-danger">
                      {{ errors.first('cachePort') }}
                    </small>
                  </label>
                  <input
                    key="cachePort"
                    v-model="cachePort"
                    v-validate="'required|numeric'"
                    name="cachePort"
                    type="text"
                    data-vv-as="自定义端口"
                    :class="{'is-invalid': errors.has('cachePort') }"
                    class="form-control"
                  >
                  <small class="form-text text-muted">
                    连接数据库的端口
                  </small>
                </div>

                <div class="form-group">
                  <label class="col-form-label">
                    Redis 密码
                    <small v-show="errors.has('cachePwd')" class="text-danger">
                      {{ errors.first('cachePwd') }}
                    </small>
                  </label>
                  <input
                    key="cachePwd"
                    v-model="cachePwd"
                    v-validate="'required'"
                    name="cachePwd"
                    type="password"
                    data-vv-as="Redis 密码"
                    :class="{'is-invalid': errors.has('cachePwd') }"
                    autocomplete="false"
                    class="form-control"
                  >
                  <small class="form-text text-muted">
                    连接Redis 的密码
                  </small>
                </div>
              </template>

            </template>
          </form>
        </template>

        <template v-else-if="active === 4">
          <form class="form-horizontal" @submit="btnNextClick">
            <h4>管理员设置</h4>
            <hr>
            <div class="form-group">
              <label class="col-form-label">
                超级管理员用户名
                <small v-show="errors.has('adminName')" class="text-danger">
                  {{ errors.first('adminName') }}
                </small>
              </label>
              <input
                key="adminName"
                v-model="adminName"
                v-validate="'required'"
                name="adminName"
                type="text"
                data-vv-as="超级管理员用户名"
                :class="{'is-invalid': errors.has('adminName') }"
                autocomplete="false"
                class="form-control"
              >
              <small class="form-text text-muted">
                在此设置超级管理员的登录用户名
              </small>
            </div>

            <div class="form-group">
              <label class="col-form-label">
                超级管理员密码
                <small v-show="errors.has('adminPassword')" class="text-danger">
                  {{ errors.first('adminPassword') }}
                </small>
              </label>
              <input
                ref="adminPassword"
                key="adminPassword"
                v-model="adminPassword"
                v-validate="'required|min:6'"
                name="adminPassword"
                type="password"
                data-vv-as="超级管理员密码"
                :class="{'is-invalid': errors.has('adminPassword') }"
                autocomplete="false"
                class="form-control"
              >
              <small class="form-text text-muted">
                6-16个字符，支持大小写字母、数字和符号
              </small>

              <small v-if="passwordScore > 80" class="form-text text-primary">
                密码强度：极好
              </small>
              <small v-else-if="passwordScore > 60" class="form-text text-primary">
                密码强度：强
              </small>
              <small v-else-if="passwordScore > 0" class="form-text text-danger">
                密码强度：弱
              </small>
            </div>

            <div class="form-group">
              <label class="col-form-label">
                确认密码
                <small v-show="errors.has('adminPasswordConfirm')" class="text-danger">
                  {{ errors.first('adminPasswordConfirm') }}
                </small>
              </label>
              <input
                key="adminPasswordConfirm"
                v-model="adminPasswordConfirm"
                v-validate="'required|confirmed:adminPassword'"
                name="adminPasswordConfirm"
                type="password"
                data-vv-as="确认密码"
                :class="{'is-invalid': errors.has('adminPasswordConfirm') }"
                autocomplete="false"
                class="form-control"
              >
              <small class="form-text text-muted">
                6-16个字符，支持大小写字母、数字和符号
              </small>
            </div>

            <h4>安全设置</h4>
            <hr>

            <div class="form-group">
              <el-switch
                v-model="isProtectData"
                active-text="加密连接字符串"
              />
              <small class="form-text text-muted">
                设置是否加密 ss.json 中的数据库连接字符串
              </small>
            </div>

          </form>
        </template>

        <template v-else-if="active === 5">
          <div class="alert alert-success">
            <h4 class="alert-heading">
              安装完成！
            </h4>
            <p>
              恭喜，您已经完成了 SS CMS 的安装
              <router-link to="../">
                进入管理后台
              </router-link>
            </p>
            <hr>
            <p class="mb-0">
              获取更多使用帮助请访问
              <a href="https://www.sscms.com/docs/" target="_blank">SS CMS 文档中心</a>
            </p>
          </div>
        </template>

        <hr>

        <div class="text-center">
          <button :disabled="active === 0" class="btn btn-default btn-large btn-custom w-md mr-2" type="submit" @click="btnPrevClick">
            上一步
          </button>
          <button :disabled="!isAgreement || !environment.isContentRootPathWritable || !environment.isWebRootPathWritable" class="btn btn-primary btn-large btn-custom w-md" type="submit" @click="btnNextClick">
            下一步
          </button>
        </div>

      </div>
    </el-card>
  </div>
</template>

<script>
import _ from 'lodash'
import { getInfo, installTryDatabase, installTryCache, install } from '@/api/cms'
import LangSelect from '@/components/LangSelect'
import '@/assets/css/bootstrap.min.css'
import '@/assets/css/icons.min.css'
import '@/assets/css/siteserver.min.css'

export default {
  name: 'Install',
  components: { LangSelect },
  data() {
    return {
      loading: false,
      active: 0,
      isAgreement: true,
      environment: {},
      databaseType: null,
      databaseIsConnectionString: false,
      databaseServer: null,
      databaseIsDefaultPort: true,
      databasePort: null,
      databaseUid: null,
      databasePwd: null,
      databaseNames: null,
      databaseName: null,
      databaseConnectionString: null,
      cacheType: null,
      cacheIsConnectionString: false,
      cacheServer: null,
      cacheIsDefaultPort: true,
      cachePort: null,
      cacheUid: null,
      cachePwd: null,
      cacheNames: null,
      cacheName: null,
      cacheConnectionString: null,
      adminName: null,
      adminPassword: null,
      adminPasswordConfirm: null,
      isProtectData: null,
      errorMessage: null
    }
  },
  computed: {
    passwordScore() {
      return this.scorePassword(this.adminPassword)
    }
  },
  watch: {
    databaseType() {
      this.databaseNames = null
      this.databaseName = null
    },
    databaseServer() {
      this.databaseNames = null
      this.databaseName = null
    },
    databaseIsDefaultPort() {
      this.databaseNames = null
      this.databaseName = null
    },
    databasePort() {
      this.databaseNames = null
      this.databaseName = null
    },
    databaseUid() {
      this.databaseNames = null
      this.databaseName = null
    },
    databasePwd() {
      this.databaseNames = null
      this.databaseName = null
    },
    cacheType() {
      this.cacheNames = null
      this.cacheName = null
    },
    cacheServer() {
      this.cacheNames = null
      this.cacheName = null
    },
    cacheIsDefaultPort() {
      this.cacheNames = null
      this.cacheName = null
    },
    cachePort() {
      this.cacheNames = null
      this.cacheName = null
    },
    cacheUid() {
      this.cacheNames = null
      this.cacheName = null
    },
    cachePwd() {
      this.cacheNames = null
      this.cacheName = null
    }
  },
  mounted() {
    getInfo().then(response => {
      this.environment = response
    })
  },
  methods: {
    apiTryDatabase() {
      this.loading = true
      if (this.databaseIsConnectionString) {
        installTryDatabase({
          databaseType: this.databaseType,
          connectionString: this.databaseConnectionString
        }).then(response => {
          this.loading = false
          if (response.isSuccess) {
            this.active = 3
          } else {
            this.errorMessage = response.errorMessage
          }
        })
      } else {
        installTryDatabase({
          databaseType: this.databaseType,
          server: this.databaseServer,
          port: this.databaseIsDefaultPort ? null : this.databasePort,
          uid: this.databaseUid,
          pwd: this.databasePwd
        }).then(response => {
          this.loading = false
          if (response.isSuccess) {
            this.databaseNames = response.databaseNames
            this.databaseName = null
          } else {
            this.errorMessage = response.errorMessage
          }
        })
      }
    },

    apiTryCache() {
      this.loading = true
      if (this.cacheIsConnectionString) {
        installTryCache({
          cacheType: this.cacheType,
          connectionString: this.cacheConnectionString
        }).then(response => {
          this.loading = false
          if (response.isSuccess) {
            this.active = 4
          } else {
            this.errorMessage = response.errorMessage
          }
        })
      } else {
        installTryCache({
          cacheType: this.cacheType,
          server: this.cacheServer,
          port: this.cacheIsDefaultPort ? null : this.cachePort,
          uid: this.cacheUid,
          pwd: this.cachePwd
        }).then(response => {
          this.loading = false
          if (response.isSuccess) {
            if (this.cacheType === 'Redis') {
              this.active = 4
            } else {
              this.cacheNames = response.databaseNames
              this.cacheName = null
            }
          } else {
            this.errorMessage = response.errorMessage
          }
        })
      }
    },

    apiInstall() {
      const adminUrl = _.trimEnd(location.href.toLowerCase(), '/').replace('/install/database', '')
      const homeUrl = adminUrl.substring(0, adminUrl.lastIndexOf('/')) + '/home'

      this.loading = true
      install({
        databaseType: this.databaseType,
        databaseServer: this.databaseServer,
        databasePort: this.databaseIsDefaultPort ? null : this.databasePort,
        databaseUid: this.databaseUid,
        databasePwd: this.databasePwd,
        databaseName: this.databaseName,
        databaseConnectionString: this.databaseConnectionString,
        cacheType: this.cacheType,
        cacheServer: this.cacheServer,
        cachePort: this.cacheIsDefaultPort ? null : this.cachePort,
        cacheUid: this.cacheUid,
        cachePwd: this.cachePwd,
        cacheName: this.cacheName,
        cacheConnectionString: this.cacheConnectionString,
        adminUrl: adminUrl,
        homeUrl: homeUrl,
        adminName: this.adminName,
        adminPassword: this.adminPassword,
        isProtectData: this.isProtectData
      }).then(response => {
        this.loading = false
        if (response.isSuccess) {
          this.active = 5
        } else {
          this.errorMessage = response.errorMessage
        }
      })
    },

    scorePassword(pass) {
      let score = 0
      if (!pass) return 0

      // award every unique letter until 5 repetitions
      const letters = {}
      for (let i = 0; i < pass.length; i++) {
        letters[pass[i]] = (letters[pass[i]] || 0) + 1
        score += 5.0 / letters[pass[i]]
      }

      // bonus points for mixing it up
      const variations = {
        digits: /\d/.test(pass),
        lower: /[a-z]/.test(pass),
        upper: /[A-Z]/.test(pass),
        nonWords: /\W/.test(pass)
      }

      let variationCount = 0
      for (const check in variations) {
        variationCount += variations[check] ? 1 : 0
      }
      score += (variationCount - 1) * 10

      return score
    },

    btnPrevClick(e) {
      e.preventDefault()
      this.active -= 1
    },

    btnNextClick(e) {
      e.preventDefault()
      if (this.active === 0) {
        if (this.isAgreement) {
          this.active = 1
        }
      } else if (this.active === 1) {
        if (this.environment.isContentRootPathWritable && this.environment.isWebRootPathWritable) {
          this.active = 2
        }
      } else if (this.active === 2) {
        this.$validator.validateAll().then((result) => {
          if (result) {
            if (this.databaseType === 'SQLite' || this.databaseName) {
              this.active = 3
            } else {
              this.apiTryDatabase()
            }
          }
        })
      } else if (this.active === 3) {
        this.$validator.validateAll().then((result) => {
          if (result) {
            if (this.cacheType === 'Memory' || this.cacheName) {
              this.active = 4
            } else {
              this.apiTryCache()
            }
          }
        })
      } else if (this.active === 4) {
        this.$validator.validateAll().then((result) => {
          if (result) {
            if (this.redisIsEnabled) {
              this.apiTryRedis()
            } else {
              this.apiInstall()
            }
          }
        })
      }
    }
  }
}
</script>

<style lang="scss" scoped>
$bg:#32b09a;

.install-container {
  min-height: 100%;
  width: 100%;
  padding: 20px;
  overflow: hidden;
}
</style>
