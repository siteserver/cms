const fs = require('fs-extra');
const del = require('del');
const gulp = require('gulp');
const through2 = require('through2');
const minifier = require('gulp-minifier');
const minify = require('gulp-minify');
const rename = require('gulp-rename');
const replace = require('gulp-string-replace');
const filter = require('gulp-filter');
const runSequence = require('gulp4-run-sequence');
const ALY = require('aliyun-sdk');

let os = '';
const version = process.env.PRODUCTVERSION || '7.0.0';
const timestamp = (new Date()).getTime();
let publishDir = '';
let htmlDict = {};
fs.readdirSync('./src/SSCMS.Web/Pages/shared/').forEach(fileName => {
  let html = fs.readFileSync('./src/SSCMS.Web/Pages/shared/' + fileName, {
    encoding: "utf8",
  });
  htmlDict[fileName] = html;
  htmlDict[fileName.replace('.cshtml', '')] = html;
});

function transform(file, html) {
  let content = new String(file.contents);
  let result = html || '';

  let matches = [...content.matchAll(/@await Html.PartialAsync\("([\s\S]+?)"\)/gi)];
  if (matches) {
    for (let i = 0; i < matches.length; i++) {
      var match = matches[i];
      content = content.replace(match[0], htmlDict[match[1]]);
    }
  }

  let styles = '';
  matches = [...content.matchAll(/<style>([\s\S]+?)<\/style>/gi)];
  if (matches && matches[0]){
    content = content.replace(matches[0][0], '');
    styles = matches[0][0];
  }
  matches = [...content.matchAll(/@section Styles{([\s\S]+?)}/gi)];
  if (matches && matches[0]){
    content = content.replace(matches[0][0], '');
    styles = matches[0][1] + styles;
  }
  let scripts = '';
  matches = [...content.matchAll(/@section Scripts{([\s\S]+?)}/gi)];
  if (matches && matches[0]){
    content = content.replace(matches[0][0], '');
    scripts = matches[0][1];
  }
  
  result = result.replace('@RenderSection("Styles", required: false)', styles);
  result = result.replace('@RenderBody()', content);
  result = result.replace('@RenderSection("Scripts", required: false)', scripts);
  result = result.replace('@page', '');
  result = result.replace('@{ Layout = "_Layout"; }', '');
  result = result.replace('@{ Layout = "_LayoutHome"; }', '');
  result = result.replace(/\.css"/g, ".css?v=" + timestamp + '"');
  result = result.replace(/\.js"/g, ".js?v=" + timestamp + '"');

  file.contents = Buffer.from(result, 'utf8');
  return file;
}

function writeOss(bucket, key, fileName) {
  var ossStream = require('aliyun-oss-upload-stream')(new ALY.OSS({
    accessKeyId: process.env.OSS_ACCESS_KEY_ID,
    secretAccessKey: process.env.OSS_SECRET_ACCESS_KEY,
    endpoint: 'http://oss-cn-beijing.aliyuncs.com',
    apiVersion: '2013-10-15'
  }));

  var upload = ossStream.upload({
    Bucket: bucket,
    Key: key
  });
  
  // upload.minPartSize(1048576);
  
  var read = fs.createReadStream(`./publish/dist/${fileName}`);
  read.pipe(upload);
}

// build tasks

gulp.task("build-src", function () {
  return gulp.src("./src/**/*").pipe(gulp.dest(`./build-${os}/src`));
});

gulp.task("build-sln", function () {
  return gulp.src("./build.sln").pipe(gulp.dest(`./build-${os}`));
});


gulp.task("build-ss-admin", function () {
  return gulp
    .src("./src/SSCMS.Web/Pages/ss-admin/**/*.cshtml")
    .pipe(through2.obj((file, enc, cb) => {
      cb(null, transform(file, htmlDict['_Layout']))
    }))
    .pipe(rename(function (path) {
      if (path.basename != 'index'){
        path.dirname += "/" + path.basename;
        path.basename = "index";
      }
      path.extname = ".html";
    }))
    .pipe(
      minifier({
        minify: true,
        minifyHTML: {
          collapseWhitespace: true,
          conservativeCollapse: true,
        },
      })
    )
    .pipe(gulp.dest(`./build-${os}/src/SSCMS.Web/wwwroot/ss-admin`));
});

gulp.task("build-home", function () {
  return gulp
    .src("./src/SSCMS.Web/Pages/home/**/*.cshtml")
    .pipe(through2.obj((file, enc, cb) => {
      cb(null, transform(file, htmlDict['_LayoutHome']))
    }))
    .pipe(rename(function (path) {
      if (path.basename != 'index'){
        path.dirname += "/" + path.basename;
        path.basename = "index";
      }
      path.extname = ".html";
    }))
    .pipe(
      minifier({
        minify: true,
        minifyHTML: {
          collapseWhitespace: true,
          conservativeCollapse: true,
        },
      })
    )
    .pipe(gulp.dest(`./build-${os}/src/SSCMS.Web/wwwroot/home`));
});

gulp.task('build-clean', function(){
  return del([`./build-${os}/src/SSCMS.Web/Pages/ss-admin/**`, `./build-${os}/src/SSCMS.Web/Pages/home/**`], {force:true});
});

gulp.task("build-linux-x64", async function () {
  os = 'linux-x64';
  return runSequence(
      "build-src",
      "build-sln",
      "build-ss-admin",
      "build-home",
      "build-clean"
  );
});

gulp.task("build-win-x64", async function () {
  os = 'win-x64';
  return runSequence(
      "build-src",
      "build-sln",
      "build-ss-admin",
      "build-home",
      "build-clean"
  );
});

gulp.task("build-win-x86", async function () {
  os = 'win-x86';
  return runSequence(
      "build-src",
      "build-sln",
      "build-ss-admin",
      "build-home",
      "build-clean"
  );
});

// copy tasks

gulp.task("copy-files", async function () {
  fs.copySync('./appsettings.json', publishDir + '/appsettings.json');
  fs.copySync('./web.config', publishDir + '/web.config');
  fs.copySync('./404.html', publishDir + '/wwwroot/404.html');
  fs.copySync('./favicon.ico', publishDir + '/wwwroot/favicon.ico');
  fs.copySync('./index.html', publishDir + '/wwwroot/index.html');
  fs.removeSync(publishDir + '/appsettings.Development.json');
});

gulp.task("copy-sscms-linux", async function () {
  fs.copySync(publishDir + '/SSCMS.Cli', publishDir + '/sscms');
  fs.removeSync(publishDir + '/SSCMS.Cli.pdb');
  fs.removeSync(publishDir + '/SSCMS.Cli');
  fs.removeSync(publishDir + '/web.config');
});

gulp.task("copy-sscms-win", async function () {
  fs.copySync(publishDir + '/SSCMS.Cli.exe', publishDir + '/sscms.exe');
  fs.removeSync(publishDir + '/SSCMS.Cli.pdb');
  fs.removeSync(publishDir + '/SSCMS.Cli.exe');
});

gulp.task("copy-css", function () {
  return gulp
    .src(["./src/SSCMS.Web/wwwroot/sitefiles/**/*.css"])
    .pipe(
      minifier({
        minify: true,
        collapseWhitespace: true,
        conservativeCollapse: true,
        minifyJS: false,
        minifyCSS: true,
        minifyHTML: false,
        ignoreFiles: ['.min.css']
      })
    )
    .pipe(gulp.dest(publishDir + "/wwwroot/sitefiles"));
});

gulp.task("copy-js", function () {
  const f = filter(['**/*-min.js']);
  return gulp
    .src(["./src/SSCMS.Web/wwwroot/sitefiles/**/*.js"])
    .pipe(minify())
    .pipe(f)
    .pipe(rename(function (path) {
      path.basename = path.basename.substring(0, path.basename.length - 4);
    }))
    .pipe(gulp.dest(publishDir + "/wwwroot/sitefiles"));
});

gulp.task("replace-localhost", function () {
  return gulp
    .src("./src/SSCMS.Web/wwwroot/sitefiles/assets/js/cloud.js")
    .pipe(replace('http://localhost:6060/', 'https://api.sscms.com/'))
    .pipe(gulp.dest(publishDir + "/wwwroot/sitefiles/assets/js"));
});

gulp.task("copy-linux-x64", async function (callback) {
  os = 'linux-x64';
  publishDir = `./publish/sscms-${version}-${os}`;

  return runSequence(
    "copy-files",
    "copy-sscms-linux",
    "copy-css",
    "copy-js",
    "replace-localhost"
  );
});

gulp.task("copy-win-x64", async function (callback) {
  os = 'win-x64';
  publishDir = `./publish/sscms-${version}-${os}`;

  return runSequence(
    "copy-files",
    "copy-sscms-win",
    "copy-css",
    "copy-js",
    "replace-localhost"
  );
});

gulp.task("copy-win-x86", async function (callback) {
  os = 'win-x86';
  publishDir = `./publish/sscms-${version}-${os}`;

  return runSequence(
    "copy-files",
    "copy-sscms-win",
    "copy-css",
    "copy-js",
    "replace-localhost"
  );
});

gulp.task("publish-linux-x64-tgz", async function () {
  writeOss(process.env.OSS_BUCKET_DL, `cms/${version}/sscms-${version}-linux-x64.tar.gz`, `sscms-${version}-linux-x64.tar.gz`);
});

gulp.task("publish-linux-x64-zip", async function () {
  writeOss(process.env.OSS_BUCKET_DL, `cms/${version}/sscms-${version}-linux-x64.zip`, `sscms-${version}-linux-x64.zip`);
});

gulp.task("publish-win-x64-zip", async function () {
  writeOss(process.env.OSS_BUCKET_DL, `cms/${version}/sscms-${version}-win-x64.zip`, `sscms-${version}-win-x64.zip`);
});

gulp.task("publish-win-x86-zip", async function () {
  writeOss(process.env.OSS_BUCKET_DL, `cms/${version}/sscms-${version}-win-x86.zip`, `sscms-${version}-win-x86.zip`);

//   var fileName = 'ci.js';
//   var date = new Date();
//   date.setHours(date.getHours() + 8);
//   var json = `var ci = {
//   version: '${version}',
//   releaseDate: '${date.getFullYear()}年${date.getMonth() + 1}月${date.getDate()}日'
// };`;
  
//   fs.writeFileSync(`./publish/dist/${fileName}`, json);
//   writeOss(process.env.OSS_BUCKET_WWW, `assets/js/${fileName}`, fileName);
});