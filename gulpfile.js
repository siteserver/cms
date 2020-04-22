const fs = require('fs-extra')
const gulp = require("gulp");
const through2 = require('through2');
const minifier = require("gulp-minifier");
const minify = require("gulp-minify");
const rename = require("gulp-rename");
const replace = require('gulp-string-replace');
const filter = require("gulp-filter");
const runSequence = require("gulp4-run-sequence");

const version = process.env.PRODUCTVERSION;
const timestamp = (new Date()).getTime();
let publishDir = '';

function transform(file, html) {
  let content = new String(file.contents);
  let result = html;

  let styles = '';
  let matches = [...content.matchAll(/@section Styles{([\s\S]+?)}/gi)];
  if (matches && matches[0]){
    content = content.replace(matches[0][0], '');
    styles = matches[0][1];
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
  result = result.replace(/\.css"/g, ".css?v=" + timestamp + '"');
  result = result.replace(/\.js"/g, ".js?v=" + timestamp + '"');

  file.contents = Buffer.from(result, 'utf8');
  return file;
}

// build tasks

gulp.task("build-src", function () {
  return gulp.src("./src/**/*").pipe(gulp.dest("./build/src"));
});

gulp.task("build-sln", function () {
  return gulp.src("./build.sln").pipe(gulp.dest("./build"));
});


gulp.task("build-ss-admin", function () {
  let html = fs.readFileSync("./src/SSCMS.Web/Pages/Shared/_Layout.cshtml", {
    encoding: "utf8",
  });

  return gulp
    .src("./src/SSCMS.Web/Pages/ss-admin/**/*.cshtml")
    .pipe(through2.obj((file, enc, cb) => {
      cb(null, transform(file, html))
    }))
    .pipe(gulp.dest("./build/src/SSCMS.Web/wwwroot/ss-admin"));
});

gulp.task("build-home", function () {
  let html = fs.readFileSync("./src/SSCMS.Web/Pages/Shared/_LayoutHome.cshtml", {
    encoding: "utf8",
  });

  return gulp
    .src("./src/SSCMS.Web/Pages/home/**/*.cshtml")
    .pipe(through2.obj((file, enc, cb) => {
      cb(null, transform(file, html))
    }))
    .pipe(gulp.dest("./build/src/SSCMS.Web/wwwroot/home"));
});

gulp.task("build", async function () {
    console.log("build version: " + version);
    return runSequence(
        "build-src",
        "build-sln",
        "build-ss-admin",
        "build-home"
    );
});

// copy tasks

gulp.task("copy-files", async function () {
  fs.copySync('./appsettings.json', publishDir + '/appsettings.json');
  fs.copySync('./sscms.json', publishDir + '/sscms.json');
  fs.copySync('./web.config', publishDir + '/web.config');
  fs.copySync('./src/SSCMS.Web/assets', publishDir + '/wwwroot/sitefiles/assets');
  fs.copySync('./404.html', publishDir + '/wwwroot/404.html');
  fs.copySync('./favicon.ico', publishDir + '/wwwroot/favicon.ico');
  fs.copySync('./index.html', publishDir + '/wwwroot/index.html');
});

gulp.task("copy-sscms-linux", async function () {
  fs.copySync(publishDir + '/SSCMS.Web', publishDir + '/sscms');
  fs.removeSync(publishDir + '/SSCMS.Web.pdb');
  fs.removeSync(publishDir + '/SSCMS.Web');
});

gulp.task("copy-sscms-win", async function () {
  fs.copySync(publishDir + '/SSCMS.Web.exe', publishDir + '/sscms.exe');
  fs.removeSync(publishDir + '/SSCMS.Web.pdb');
  fs.removeSync(publishDir + '/SSCMS.Web.exe');
});

gulp.task("copy-css", function () {
  return gulp
    .src(["./src/SSCMS.Web/assets/**/*.css"])
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
    .pipe(gulp.dest(publishDir + "/wwwroot/sitefiles/assets"));
});

gulp.task("copy-js", function () {
  const f = filter(['**/*-min.js']);
  return gulp
    .src(["./src/SSCMS.Web/assets/**/*.js"])
    .pipe(minify())
    .pipe(f)
    .pipe(rename(function (path) {
      path.basename = path.basename.substring(0, path.basename.length - 4);
    }))
      .pipe(gulp.dest(publishDir + "/wwwroot/sitefiles/assets"));
});

gulp.task("copy-osx-x64", async function (callback) {
  publishDir = './publish/sscms-' + version + '-osx-x64';
  console.log("publish dir: " + publishDir);

  return runSequence(
    "copy-files",
    "copy-sscms-linux",
    "copy-css",
    "copy-js"
  );
});

gulp.task("copy-linux-x64", async function (callback) {
  publishDir = './publish/sscms-' + version + '-linux-x64';
  console.log("publish dir: " + publishDir);

  return runSequence(
    "copy-files",
    "copy-sscms-linux",
    "copy-css",
    "copy-js"
  );
});

gulp.task("copy-win-x64", async function (callback) {
  publishDir = './publish/sscms-' + version + '-win-x64';
  console.log("publish dir: " + publishDir);

  return runSequence(
    "copy-files",
    "copy-sscms-win",
    "copy-css",
    "copy-js"
  );
});

gulp.task("copy-win-x86", async function (callback) {
  publishDir = './publish/sscms-' + version + '-win-x86';
  console.log("publish dir: " + publishDir);

  return runSequence(
    "copy-files",
    "copy-sscms-win",
    "copy-css",
    "copy-js"
  );
});