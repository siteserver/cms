const fs = require("fs");
const gulp = require("gulp");
const minifier = require("gulp-minifier");
const minify = require("gulp-minify");
const rename = require("gulp-rename");
const replace = require('gulp-string-replace');
const filter = require("gulp-filter");
const runSequence = require("gulp4-run-sequence");

const version = process.env.PRODUCTVERSION || Math.random();
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

gulp.task("copy-sscms", function () {
  fs.copyFileSync(publishDir + '/SSCMS.Web', publishDir + '/sscms');
  fs.unlinkSync(publishDir + '/SSCMS.Web.pdb');
  fs.unlinkSync(publishDir + '/SSCMS.Web');
});

gulp.task("copy-root", function () {
  return gulp.src(["./appsettings.html"]).pipe(gulp.dest(publishDir));
});

gulp.task("copy-assets", function () {
  return gulp.src(["./src/SSCMS.Web/assets"]).pipe(gulp.dest(publishDir + "/assets"));
});

gulp.task("copy-wwwroot", function () {
  return gulp.src(["./404.html", "./favicon.ico", "./index.html"]).pipe(gulp.dest(publishDir + "/wwwroot"));
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

gulp.task("copy-linux-x64", async function (callback) {
  publishDir = './publish/sscms-' + version + '-linux-x64';
  console.log("publish dir: " + publishDir);

  return runSequence(
      "copy-sscms",
      "copy-root",
      "copy-assets",
      "copy-wwwroot",
      "copy-css",
      "copy-js"
  );
});