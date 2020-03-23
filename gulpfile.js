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

function getDependencies() {
  var str = "";

  var dirs = fs.readdirSync("./packages").filter(function (file) {
    return fs.statSync("./packages/" + file).isDirectory();
  });
  for (var dir of dirs) {
    var items = dir.split(".");
    var index = 0;
    for (var i = 0; i < items.length; i++) {
      var isNumber = !isNaN(parseFloat(items[i])) && isFinite(items[i]);
      if (isNumber) {
        index = i;
        break;
      }
    }
    var id = items.slice(0, index).join(".");
    var version = items.slice(index).join(".");
    str += '<dependency id="' + id + '" version="' + version + '" />';
  }
  str = "<dependencies>" + str + "</dependencies>";
  return str;
}

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

gulp.task("build-nuspec", function () {
  var dependencies = getDependencies();
  return gulp
      .src("./SS.CMS.nuspec")
      .pipe(replace("$version$", version))
      .pipe(replace("</metadata>", dependencies + "</metadata>"))
      .pipe(gulp.dest("./build"));
});

gulp.task("build", async function (callback) {
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
  fs.copySync('./src/SSCMS.Web/assets', publishDir + '/assets');
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
    .pipe(gulp.dest(publishDir + "/assets"));
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
    .pipe(gulp.dest(publishDir + "/assets"));
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