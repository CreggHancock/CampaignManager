var gulp = require("gulp");
var elm = require("gulp-elm");

function elmBundle() {
  return gulp
    .src("Views/**/Main.elm")
    .pipe(elm.bundle("elm.js", { optimize: true }))
    .pipe(gulp.dest("wwwroot/js/"));
}

gulp.task("elm-bundle", elmBundle);
exports.default = elmBundle;
