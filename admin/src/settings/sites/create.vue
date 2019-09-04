<template>
  <div class="app-container">
    <div class="page-container page-resource">
      <h3>{{ $t('route.settingsSitesCreate') }}</h3>
      <p style="margin-bottom:40px" v-html="getTips()" />
      <template v-if="pageType == 'selectType'">
        <div class="cards">
          <ul class="container">
            <li>
              <div class="card">
                <h3>创建空站点</h3>
                <p>点击按钮从零开始创建空站点</p>
                <button href="javascript:;" @click="btnCreateClick('empty')">创 建</button>
              </div>
            </li>
            <li v-if="siteTemplates && siteTemplates.length > 0">
              <div class="card">
                <h3>使用本地站点模板创建站点</h3>
                <p>点击按钮从本地站点模板中选择并创建站点</p>
                <button href="javascript:;" @click="btnLocalClick()">创 建</button>
              </div>
            </li>
            <li>
              <div class="card">
                <h3>使用在线站点模板创建站点</h3>
                <p>
                  点击按钮从
                  <el-link
                    type="success"
                    href="https://www.siteserver.cn/templates/"
                    target="_blank"
                  >官网模板中心</el-link>中选择并创建站点
                </p>
                <button href="javascript:;" @click="btnCloudClick()">创 建</button>
              </div>
            </li>
          </ul>
        </div>
      </template>
      <template v-else-if="pageType == 'selectLocal'">
        <el-form :model="form">
          <el-table
            :data="siteTemplates"
            border
            style="width: 100%"
          >
            <el-table-column
              fixed
              prop="displayName"
              label="站点模板名称"
              width="200"
            />
            <el-table-column
              prop="name"
              label="站点模板文件夹	"
              width="120"
            />
            <el-table-column
              prop="description"
              label="站点模板说明"
            />
            <el-table-column
              fixed="right"
              label="操作"
              width="80"
            >
              <template slot-scope="scope">
                <el-button
                  type="text"
                  size="small"
                  @click="btnCreateClick('local', scope.row.name)"
                >创建站点</el-button>
              </template>
            </el-table-column>
          </el-table>

          <el-divider />

          <el-form-item>
            <el-button @click="btnCancelClick">返 回</el-button>
          </el-form-item>
        </el-form>
      </template>
      <template v-else-if="pageType == 'selectCloud'">

        <el-form :model="form">
          <div class="cards">
            <ul class="container">
              <li
                v-for="templateInfo in templateInfoList"
                :key="templateInfo.id"
              >
                <div class="card" style="padding-top: 0">
                  <a
                    :href="getDisplayUrl(templateInfo.templateId)"
                    class="image-popup"
                    target="_blank"
                  >
                    <img
                      :src="getTemplateUrl(templateInfo.imageUrl)"
                      class="thumb-img"
                      style="max-height: 400px;"
                    >
                  </a>
                  <h3>{{ templateInfo.templateId }}</h3>
                  <p :title="templateInfo.description" style="height: 45px;overflow: hidden;">{{ templateInfo.description }}</p>
                  <a
                    :href="getPreviewUrl(templateInfo.templateId)"
                    target="_blank"
                    style="margin-right: 20px;"
                  >预览站点</a>
                  <button
                    href="javascript:;"
                    @click="btnCreateClick('cloud', templateInfo.templateId)"
                  >创建站点</button>
                </div>
              </li>
            </ul>
          </div>

          <el-divider />

          <el-button-group v-if="pages > 1">
            <a :href="getPageUrl(page - 1)" class="page-link">
              <span aria-hidden="true">上一页</span>
            </a>
            <el-button type="primary" icon="el-icon-arrow-left">上一页</el-button>
            <a v-for="i of pages" :key="i" :class="{'active': i === page}" :href="getPageUrl(i)" class="page-link" v-html="i" />
            <a :href="getPageUrl(page + 1)" class="page-link">
              <span aria-hidden="true">下一页</span>
            </a>
            <el-button type="primary">下一页<i class="el-icon-arrow-right el-icon--right" /></el-button>
          </el-button-group>
          <el-button @click="btnCancelClick">返 回</el-button>
        </el-form>
      </template>
      <template v-else-if="pageType == 'create'">
        <el-form ref="form" :model="form" :rules="rules" label-width="140px">
          <el-form-item label="站点名称" prop="siteName">
            <el-input v-model="form.siteName" />
          </el-form-item>

          <el-form-item v-if="sitesOptions && sitesOptions.length > 0" label="上级站点">
            <el-radio-group v-model="form.hasParent">
              <el-radio :label="false">无上级站点</el-radio>
              <el-radio :label="true">指定上级站点</el-radio>
            </el-radio-group>
            <el-cascader-panel
              v-if="form.hasParent"
              v-model="parentIds"
              :options="sitesOptions"
              :props="{ checkStrictly: true }"
              clearable
            />
          </el-form-item>

          <el-form-item label="站点文件" prop="siteDir">
            <el-radio-group v-if="!form.hasParent && !isRootExists" v-model="form.isRoot">
              <el-radio :label="false">存放在指定文件夹中</el-radio>
              <el-radio :label="true">存放在wwwroot根目录中</el-radio>
            </el-radio-group>
            <el-input
              v-if="form.hasParent || !form.isRoot"
              v-model="form.siteDir"
              placeholder="实际在服务器中保存此网站的文件夹名称，此路径必须以英文或拼音命名"
            />
          </el-form-item>

          <el-form-item label="内容表" prop="tableHandWrite">
            <el-radio-group v-model="form.tableRule">
              <el-radio v-if="tableNames && tableNames.length > 0" label="Choose">选择内容表</el-radio>
              <el-radio label="Create">创建新的内容表</el-radio>
              <el-radio label="HandWrite">指定内容表</el-radio>
            </el-radio-group>
            <template v-if="form.tableRule == 'Choose'">
              <el-select v-model="form.tableChoose" placeholder="请选择已存在的内容表">
                <el-option
                  v-for="tableName in tableNames"
                  :key="tableName"
                  :label="tableName"
                  :value="tableName"
                />
              </el-select>
            </template>
            <template v-else-if="form.tableRule == 'Create'">
              <p>
                系统将根据站点Id自动创建内容表，内容表名称为
                <strong>siteserver_Content_&lt;站点Id&gt;</strong>
              </p>
            </template>
            <template v-else-if="form.tableRule == 'HandWrite'">
              <el-input
                v-model="form.tableHandWrite"
                placeholder="请输入内容表名称，系统将检测数据库是否已存在指定的内容表，如果不存在系统将创建此内容表"
              />
            </template>
          </el-form-item>

          <el-form-item v-if="form.createType !== 'empty'" label="站点模板导入选项">
            <el-switch v-model="form.isImportContents" active-text="导入栏目及内容" />
            <el-switch v-model="form.isImportTableStyles" active-text="导入表虚拟字段" />
          </el-form-item>

          <el-form-item>
            <el-button type="primary" @click="btnSubmitClick">创建站点</el-button>
            <el-button @click="btnCancelClick">取消</el-button>
          </el-form-item>
        </el-form>
      </template>
    </div>
  </div>
</template>

<script>
import { getThemes } from '@/api/themes'
import { getSites, getTableNames, create } from '@/api/sites'
import { getTemplates } from '@/api/sscmscom'

export default {
  name: 'Create',
  data() {
    return {
      loading: true,
      pageAlert: null,
      pageType: this.$route.query.type || 'selectType',

      siteTemplates: null,
      isRootExists: null,
      sitesOptions: [],
      tableNames: null,

      page: this.$route.query.page || 1,
      word: this.$route.query.word,
      tag: this.$route.query.tag,
      price: this.$route.query.price,
      order: this.$route.query.order,
      templateInfoList: null,
      count: null,
      pages: null,
      allTagNames: [],
      form: {
        createType: this.$route.query.createType,
        createTemplateId: this.$route.query.createTemplateId,
        siteName: '',
        isRoot: false,
        hasParent: false,
        parentIds: [],
        parentId: 0,
        siteDir: '',
        tableRule: 'Create',
        tableChoose: '',
        tableHandWrite: 'foobar',
        isImportContents: true,
        isImportTableStyles: true
      },
      rules: {
        siteName: [
          { required: true, message: '请输入站点名称', trigger: 'blur' }
        ],
        siteDir: [
          { required: true, message: '请输入站点文件夹名称', trigger: 'blur' }
        ],
        tableHandWrite: [
          { required: true, message: '请输入内容表名称', trigger: 'blur' }
        ]
      }
    }
  },
  computed: {
    tableRule() {
      return this.form.tableRule
    }
  },
  watch: {
    tableRule(val) {
      if (val !== 'HandWrite') {
        this.form.tableHandWrite = this.form.tableHandWrite ? this.form.tableHandWrite : 'foobar'
      } else {
        this.form.tableHandWrite = this.form.tableHandWrite !== 'foobar' ? this.form.tableHandWrite : ''
      }
    }
  },
  async created() {
    this.siteTemplates = await getThemes()
    this.sitesOptions = this.getSitesOptions(await getSites())
    this.tableNames = await getTableNames()

    this.loading = false
  },
  methods: {
    getSitesOptions(sites) {
      if (!sites) return null

      var options = []
      for (const site of sites) {
        var option = {
          value: site.id
        }
        if (site.isRoot) {
          this.isRootExists = true
          option.label = site.siteName
        } else {
          option.label = `${site.siteName}(${site.siteDir})`
        }
        if (site.children && site.children.length > 0) {
          option.children = this.getSitesOptions(site.children)
        }

        options.push(option)
      }

      return options
    },
    getDisplayUrl(templateId) {
      return (
        'https://www.siteserver.cn/templates/template.html?id=' + templateId
      )
    },

    getTemplateUrl(relatedUrl) {
      return 'https://www.siteserver.cn/templates/' + relatedUrl
    },

    getPreviewUrl(templateId) {
      return 'https://demo.siteserver.cn/' + templateId
    },

    getPageUrl(page) {
      if (page < 1 || page > this.pages || page === this.page) { return 'javascript:;' }
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
        url += '&price=' + price
      }
      if (order) {
        url += '&order=' + order
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

    getTips() {
      if (this.pageType === 'create') {
        if (this.form.createType === 'cloud') {
          return (
            '使用在线站点模板创建站点，站点模板：<a href="' +
          this.getDisplayUrl(this.createTemplateId) +
          '" target="_blank">' +
          this.createTemplateId +
          '</a>'
          )
        } else if (this.form.createType === 'local') {
          return '使用本地站点模板创建站点，站点模板：' + this.createTemplateId
        } else if (this.form.createType === 'empty') {
          return '创建空站点（不使用站点模板）'
        }
      } else if (this.pageType === 'selectType') {
        return '欢迎使用创建新站点向导，请选择创建站点的方式'
      } else if (this.pageType === 'selectLocal') {
        return '选中站点模板后点击创建站点按钮开始创建站点'
      } else if (this.pageType === 'selectCloud') {
        return '选中站点模板后点击创建站点按钮开始创建站点'
      }
    },

    btnLocalClick() {
      this.pageType = 'selectLocal'
      this.form.createType = 'local'
    },

    btnCloudClick() {
      this.pageType = 'selectCloud'
      this.form.createType = 'cloud'
      this.load()
    },

    btnCreateClick(createType, templateId) {
      this.form.createType = createType
      this.createTemplateId = templateId
      this.pageType = 'create'
    },

    btnCancelClick() {
      this.pageType = 'selectType'
    },

    btnSubmitClick() {
      this.$refs['form'].validate(valid => {
        if (valid) {
          alert('submit!')
          this.loading = true
          this.form.parentId = 0
          if (this.hasParent && this.parentIds && this.parentIds.length > 0) {
            this.form.parentId = this.parentIds[this.parentIds.length - 1]
          }
          create(this.form).then(siteId => {
            this.$router.push(`/sites/${siteId}`)
          })
        } else {
          console.log('error submit!!')
          return false
        }
      })
    }
  }
}
</script>

<style>
.el-cascader-node > .el-radio {
  margin-top: 8px;
}
.page-resource {
  box-sizing: border-box;
}
.page-resource .resource-placeholder {
  margin: 50px auto 100px;
  text-align: center;
}
.page-resource .resource-placeholder img {
  width: 150px;
}
.page-resource .resource-placeholder h4 {
  margin: 20px 0 16px;
  font-size: 16px;
  color: #1f2f3d;
  line-height: 1;
}
.page-resource .resource-placeholder p {
  margin: 0;
  font-size: 14px;
  color: #99a9bf;
  line-height: 1;
}
.cards {
  margin: 35px auto 110px;
}
.cards .container {
  padding: 0;
  margin: 0 -11px;
  width: auto;
}
.cards .container:after,
.cards .container:before {
  display: table;
  content: "";
}
.cards .container:after {
  clear: both;
}
.cards li {
  margin-bottom: 20px;
  width: 33.33333%;
  padding: 0 11px;
  box-sizing: border-box;
  float: left;
  list-style: none;
}
h2 {
  font-size: 28px;
  margin: 0;
}
p {
  font-size: 14px;
  color: #5e6d82;
}
.card {
  padding-top: 40px;
  padding-bottom: 40px;
  width: 100%;
  background: #fff;
  border: 1px solid #eaeefb;
  border-radius: 5px;
  box-sizing: border-box;
  text-align: center;
  position: relative;
  transition: bottom 0.3s;
  bottom: 0;
}
.card img {
  margin: 35px auto 35px;
  height: 260px;
}
.card h3 {
  margin: 0 0 10px;
  font-size: 18px;
  color: #1f2f3d;
  font-weight: 400;
  height: 22px;
}
.card p {
  font-size: 14px;
  color: #99a9bf;
  padding: 0 30px;
  margin: 0;
  word-break: break-all;
  line-height: 1.8;
}
.card button {
  height: 42px;
  width: 190px;
  display: inline-block;
  line-height: 42px;
  font-size: 14px;
  background-color: #409eff;
  color: #fff;
  text-align: center;
  border: 0;
  padding: 0;
  cursor: pointer;
  border-radius: 2px;
  transition: all 0.3s;
  text-decoration: none;
  margin-top: 20px;
}
@media (max-width: 850px) {
  .cards li {
    max-width: 500px;
    float: none;
    margin: 10px auto 30px;
    width: 80%;
  }
  .cards li .card {
    height: auto;
    padding-bottom: 20px;
  }
  .cards h3 {
    height: auto;
  }
}
</style>
