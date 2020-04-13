const fs = require('fs-extra')
const gulp = require("gulp");
const minifier = require("gulp-minifier");
const minify = require("gulp-minify");
const rename = require("gulp-rename");
const replace = require('gulp-string-replace');
const filter = require("gulp-filter");
const runSequence = require("gulp4-run-sequence");

const version = process.env.PRODUCTVERSION || (new Date().toISOString());
let publishDir = '';

// build tasks

gulp.task("build-src", function () {
  return gulp.src("./src/**/*").pipe(gulp.dest("./build/src"));
});

gulp.task("build-tests", function () {
  return gulp.src("./tests/**/*").pipe(gulp.dest("./build/tests"));
});

gulp.task("build-sln", function () {
  return gulp.src("./sscms.sln").pipe(gulp.dest("./build"));
});

gulp.task("build-cshtml", function () {
  return gulp
      .src("./src/SSCMS.Web/Pages/**/*.cshtml")
      .pipe(replace(/\.css"/g, '.css?v=' + version + '"'))
      .pipe(replace(/\.js"/g, '.js?v=' + version + '"'))
      .pipe(gulp.dest("./build/src/SSCMS.Web/Pages"));
});

gulp.task("build", async function () {
    console.log("build version: " + version);
    return runSequence(
        "build-src",
        "build-tests",
        "build-sln",
        "build-cshtml"
    );
});

// copy tasks

gulp.task("copy-files", async function () {
  fs.copySync('./appsettings.json', publishDir + '/appsettings.json');
  fs.copySync('./sscms.json', publishDir + '/sscms.json');
  fs.copySync('./web.config', publishDir + '/web.config');
  fs.copySync('./src/SSCMS.Web/Pages/ss-admin/assets', publishDir + '/wwwroot/SiteFiles/assets');
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
    .src(["./src/SSCMS.Web/Pages/ss-admin/assets/**/*.css"])
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
    .pipe(gulp.dest(publishDir + "/wwwroot/SiteFiles/assets"));
});

gulp.task("copy-js", function () {
  const f = filter(['**/*-min.js']);
  return gulp
    .src(["./src/SSCMS.Web/Pages/ss-admin/assets/**/*.js"])
    .pipe(minify())
    .pipe(f)
    .pipe(rename(function (path) {
      path.basename = path.basename.substring(0, path.basename.length - 4);
    }))
    .pipe(gulp.dest(publishDir + "/wwwroot/SiteFiles/assets"));
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