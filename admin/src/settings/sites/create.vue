<template>
  <div class="app-container">

    <div class="card-box">
      <div class="m-t-0 header-title">
        创建新站点
      </div>

      <template v-if="pageType == 'selectType'">
        <p class="text-muted m-b-30 font-14">
          欢迎使用创建新站点向导，请选择创建站点的方式
        </p>

        <button type="button" class="btn btn-block btn-md btn-outline-primary" @click="btnCreateClick('empty')">创建空站点</button>
        <p class="text-muted mt-2 mb-4 font-13">
          点击按钮创建空站点，站点的栏目、内容及模板需要手动添加。
        </p>
        <button v-if="siteTemplates && siteTemplates.length > 0" type="button" class="btn btn-block btn-md btn-outline-primary" @click="btnLocalClick()">使用本地站点模板创建站点</button>
        <p v-if="siteTemplates && siteTemplates.length > 0" class="text-muted mt-2 mb-4 font-13">
          点击按钮从本地站点模板中选择并创建站点。
        </p>
        <button type="button" class="btn btn-block btn-md btn-outline-primary" @click="btnCloudClick()">使用在线站点模板创建站点</button>
        <p class="text-muted mt-2 mb-4 font-13">
          点击按钮从<a href="https://www.siteserver.cn/templates/" target="_blank">官网模板中心</a>中选择并创建站点。
        </p>
      </template>

      <template v-else-if="pageType == 'selectLocal'">
        <p class="text-muted m-b-30 font-14">
          选中站点模板后点击创建站点按钮开始创建站点。
        </p>

        <div class="panel panel-default">
          <div class="panel-body p-0">
            <div class="table-responsive">
              <table class="table tablesaw table-hover m-0">
                <thead>
                  <tr class="thead">
                    <th>站点模板名称</th>
                    <th>站点模板文件夹</th>
                    <th>站点模板介绍</th>
                    <th class="text-center" />
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="siteTemplate in siteTemplates" :key="siteTemplate.siteTemplateName">
                    <td>
                      {{ siteTemplate.siteTemplateName }}
                    </td>
                    <td>
                      {{ siteTemplate.directoryName }}
                    </td>
                    <td>
                      {{ siteTemplate.description }}
                    </td>
                    <td>
                      <a href="javascript:;" class="btn btn-primary" @click="btnCreateClick('local', siteTemplate.directoryName)">创建站点</a>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </template>

      <template v-else-if="pageType == 'selectCloud'">
        <p class="text-muted m-b-30 font-14">
          选中站点模板后点击创建站点按钮开始创建站点。
        </p>

        <div class="row">
          <div class="col-8">
            <div class="form-inline">
              <input v-model="word" type="text" style="width: 80%" class="form-control" placeholder="请输入关键字...">
              <a :href="getUrl(page, word, tag, price, order)" class="btn btn-primary ml-1">搜索</a>
            </div>
          </div>
          <div class="col-4 text-right">
            <a class="text-primary mr-2" target="_blank" href="https://www.siteserver.cn/templates/">官网模板中心</a>
          </div>
        </div>

        <div class="row mt-4">
          <div class="col-6">

            <div class="portfolioFilter">
              <a :href="getTagUrl('')" :class="{'current' : !tag}">全部</a>
              <a v-for="tagName in allTagNames" :key="tagName" :href="getTagUrl(tagName)" :class="{'current' : tag == tagName}">
                {{ tagName }}
              </a>
            </div>

          </div>
          <div class="col-6">
            <form class="form-inline float-right">
              <div class="form-group ml-3">
                价格：
                <select v-model="price" class="form-control form-control-sm" @change="priceChanged">
                  <option value="">全部</option>
                  <option value="false">免费模板</option>
                  <option value="true">收费模板</option>
                </select>
              </div>
              <div class="form-group ml-3">
                排序：
                <select v-model="order" class="form-control form-control-sm" @change="orderChanged">
                  <option value="">默认</option>
                  <option value="UpdatedDate">更新时间</option>
                  <option value="AddDate">上传时间</option>
                  <option value="Price">价格</option>
                </select>
              </div>
            </form>
          </div>

        </div>

        <div class="row mt-4" />

        <div class="port mb-4">
          <div class="portfolioContainer row">

            <div v-for="templateInfo in templateInfoList" :key="templateInfo.id" class="col-sm-6 col-lg-3 col-md-4">
              <div class="gal-detail thumb">
                <a :href="getDisplayUrl(templateInfo.templateId)" class="image-popup" target="_blank">
                  <img :src="getTemplateUrl(templateInfo.imageUrl)" class="thumb-img" style="max-height: 400px;">
                </a>
                <h4 class="text-center">{{ templateInfo.templateId }}</h4>
                <div class="ga-border" />
                <p class="text-muted text-center" style="max-height: 45px;overflow: hidden;">
                  <small>{{ templateInfo.description }}</small>
                </p>
                <div class="text-center">
                  <a :href="getPreviewUrl(templateInfo.templateId)" target="_blank" class="btn btn-info mr-2">预览站点</a>
                  <a href="javascript:;" class="btn btn-primary" @click="btnCreateClick('cloud', templateInfo.templateId)">创建站点</a>
                </div>
              </div>
            </div>

          </div>
        </div>

        <hr>

        <nav v-if="pages > 1">
          <ul class="pagination justify-content-center">
            <li class="page-item">
              <a :href="getPageUrl(page - 1)" class="page-link">
                <span aria-hidden="true">上一页</span>
              </a>
            </li>
            <li v-for="i of pages" :key="i" :class="{'active': i === page}" class="page-item">
              <a :href="getPageUrl(i)" class="page-link" v-html="i" />
            </li>
            <li class="page-item">
              <a :href="getPageUrl(page + 1)" class="page-link">
                <span aria-hidden="true">下一页</span>
              </a>
            </li>
          </ul>
        </nav>
      </template>

      <template v-else-if="pageType == 'create'">
        <p class="text-muted font-13 m-b-25" v-html="getCreateType()" />

        <div class="form-group">
          <label>
            站点名称
            <small v-show="errors.has('siteName')" class="text-danger">{{ errors.first('siteName') }}</small>
          </label>
          <input v-model="siteName" v-validate="'required'" name="siteName" data-vv-as="站点名称" :class="{'is-invalid': errors.has('siteName') }" type="text" class="form-control">
        </div>

        <div v-if="!isRootExists" class="form-group">
          <label>
            站点级别
          </label>
          <div class="m-2">
            <div class="radio radio-primary form-check-inline">
              <input id="isRoot_true" v-model="isRoot" type="radio" :value="true" name="isRoot">
              <label for="isRoot_true"> 主站 </label>
            </div>
            <div class="radio radio-primary form-check-inline">
              <input id="isRoot_false" v-model="isRoot" type="radio" :value="false" name="isRoot">
              <label for="isRoot_false"> 子站 </label>
            </div>
          </div>
        </div>

        <div v-if="!isRootExists && !isRoot" class="form-group">
          <label>
            上级站点
          </label>
          <select v-model="parentId" class="form-control">
            <option v-for="site in siteList" :key="site.key" :value="site.key">{{ site.value }}</option>
          </select>
        </div>

        <div v-if="!isRoot" class="form-group">
          <label>
            文件夹名称
            <small v-show="errors.has('siteDir')" class="text-danger">{{ errors.first('siteDir') }}</small>
          </label>
          <input v-model="siteDir" v-validate="'required'" name="siteDir" data-vv-as="文件夹名称" :class="{'is-invalid': errors.has('siteDir') }" type="text" class="form-control">
          <small class="form-text text-muted">实际在服务器中保存此网站的文件夹名称，此路径必须以英文或拼音命名</small>
        </div>

        <div class="form-group">
          <label>
            内容表
            <small v-show="errors.has('tableChoose')" class="text-danger">{{ errors.first('tableChoose') }}</small>
            <small v-show="errors.has('tableHandWrite')" class="text-danger">{{ errors.first('tableHandWrite') }}</small>
          </label>
          <div class="m-2">
            <div class="radio radio-primary form-check-inline">
              <input id="tableRule_Choose" v-model="tableRule" type="radio" value="Choose" name="tableRule">
              <label for="tableRule_Choose"> 选择内容表 </label>
            </div>
            <div class="radio radio-primary form-check-inline">
              <input id="tableRule_Create" v-model="tableRule" type="radio" value="Create" name="tableRule">
              <label for="tableRule_Create"> 创建新的内容表 </label>
            </div>
            <div class="radio radio-primary form-check-inline">
              <input id="tableRule_HandWrite" v-model="tableRule" type="radio" value="HandWrite" name="tableRule">
              <label for="tableRule_HandWrite"> 指定内容表 </label>
            </div>
          </div>
          <template v-if="tableRule == 'Choose'">
            <select v-model="tableChoose" v-validate="'required'" name="tableChoose" data-vv-as="内容表" :class="{'is-invalid': errors.has('tableChoose') }" class="form-control">
              <option v-for="tableName in tableNameList" :key="tableName" :value="tableName">{{ tableName }}</option>
            </select>
            <small class="form-text text-muted">请选择已存在的内容表。</small>
          </template>
          <template v-else-if="tableRule == 'Create'">
            <small class="form-text text-muted">系统将创建内容表，表名称为<code>siteserver_Content_&lt;站点Id&gt;</code>。</small>
          </template>
          <template v-else-if="tableRule == 'HandWrite'">
            <input v-model="tableHandWrite" v-validate="'required'" name="tableHandWrite" data-vv-as="内容表" :class="{'is-invalid': errors.has('tableHandWrite') }" type="text" class="form-control">
            <small class="form-text text-muted">请输入内容表名称，系统将检测数据库是否已存在指定的内容表，如果不存在系统将创建此内容表。</small>
          </template>
        </div>

        <div v-if="createType == 'local' || createType == 'cloud'" class="form-group">
          <label>
            站点模板导入选项
          </label>
          <div class="m-2">
            <div class="checkbox checkbox-primary form-check-inline">
              <input id="isImportContents" v-model="isImportContents" type="checkbox" name="isImportContents">
              <label for="isImportContents"> 导入栏目及内容 </label>
            </div>
            <div class="checkbox checkbox-primary form-check-inline">
              <input id="isImportTableStyles" v-model="isImportTableStyles" type="checkbox" name="isImportTableStyles">
              <label for="isImportTableStyles"> 导入表虚拟字段 </label>
            </div>
          </div>
        </div>

        <hr>

        <div class="text-center">
          <button type="button" class="btn btn-primary" @click="btnSubmitClick">创建站点</button>
        </div>
      </template>

    </div>

  </div>
</template>

<script>
import { getThemes } from '@/api/themes'
import { getSites, getTableNames, create } from '@/api/sites'
import { getTemplates } from '@/api/sscmscom'
import '@/assets/css/bootstrap.min.css'
import '@/assets/css/siteserver.min.css'

export default {
  name: 'Info',
  data() {
    return {
      loading: true,
      pageAlert: null,
      pageType: this.$route.query.type || 'selectType',

      siteTemplates: null,
      isRootExists: null,
      siteList: null,
      tableNameList: null,

      page: this.$route.query.page || 1,
      word: this.$route.query.word,
      tag: this.$route.query.tag,
      price: this.$route.query.price,
      order: this.$route.query.order,
      templateInfoList: null,
      count: null,
      pages: null,
      allTagNames: [],

      createType: this.$route.query.createType,
      createTemplateId: this.$route.query.createTemplateId,
      siteName: '',
      isRoot: false,
      parentId: 0,
      siteDir: '',
      tableRule: 'Choose',
      tableChoose: '',
      tableHandWrite: '',
      isImportContents: true,
      isImportTableStyles: true
    }
  },
  created() {
    getThemes().then(siteTemplates => {
      this.siteTemplates = siteTemplates
      getSites().then(siteList => {
        this.siteList = siteList
        for (const site of this.siteList) {
          if (site.isRoot) {
            this.isRootExists = true
          }
        }
        getTableNames().then(tableNames => {
          this.tableNameList = tableNames
          this.loading = false
        })
      })
    })
  },
  methods: {
    getDisplayUrl(templateId) {
      return 'https://www.siteserver.cn/templates/template.html?id=' + templateId
    },

    getTemplateUrl(relatedUrl) {
      return 'https://www.siteserver.cn/templates/' + relatedUrl
    },

    getPreviewUrl(templateId) {
      return 'https://demo.siteserver.cn/' + templateId
    },

    getPageUrl(page) {
      if (page < 1 || page > this.pages || page === this.page) return 'javascript:;'
      return this.getUrl(page, this.word, this.tag, this.price, this.order)
    },

    getTagUrl(tag) {
      return this.getUrl(this.page, this.word, tag, this.price, this.order)
    },

    getPriceUrl(price) {
      return this.getUrl(this.page, this.word, this.tag, price, this.order)
    },

    getOrderUrl(order) {
      return this.getUrl(this.page, this.word, this.tag, this.price, order)
    },

    getUrl(page, word, tag, price, order) {
      var url = '?type=selectCloud&page=' + page
      if (word) {
        url += '&word=' + word
      }
      if (tag) {
        url += '&tag=' + tag
      }
      if (price) {
        url += '&price=' + (price)
      }
      if (order) {
        url += '&order=' + (order)
      }
      return url
    },

    priceChanged() {
      this.load()
    },

    orderChanged() {
      this.load()
    },

    load() {
      getTemplates({
        page: this.page,
        word: this.word,
        tag: this.tag,
        price: this.price,
        order: this.order
      }).then(response => {
        this.templateInfoList = response.value
        this.count = response.count
        this.pages = response.pages
        this.allTagNames = response.allTagNames
      })
    },

    getCreateType() {
      if (this.createType === 'cloud') {
        return '使用在线站点模板创建站点，站点模板：<a href="' + this.getDisplayUrl(this.createTemplateId) + '" target="_blank">' + this.createTemplateId + '</a>'
      } else if (this.createType === 'local') {
        return '使用本地站点模板创建站点，站点模板：' + this.createTemplateId
      } else {
        return '创建空站点（不使用站点模板）'
      }
    },

    btnLocalClick() {
      this.pageType = 'selectLocal'
      this.createType = 'local'
    },

    btnCloudClick() {
      this.pageType = 'selectCloud'
      this.createType = 'cloud'
      this.load()
    },

    btnCreateClick(createType, templateId) {
      this.createType = createType
      this.createTemplateId = templateId
      this.pageType = 'create'
    },

    btnSubmitClick: function() {
      this.$validator.validate().then(result => {
        if (result) {
          this.loading = true
          create({
            createType: this.createType,
            createTemplateId: this.createTemplateId,
            siteName: this.siteName,
            isRoot: this.isRoot,
            parentId: this.parentId,
            siteDir: this.siteDir,
            tableRule: this.tableRule,
            tableChoose: this.tableChoose,
            tableHandWrite: this.tableHandWrite,
            isImportContents: this.isImportContents,
            isImportTableStyles: this.isImportTableStyles
          }).then(siteId => {
            this.$router.push(`/sites/${siteId}`)
          })
        }
      })
    }
  }
}
</script>
