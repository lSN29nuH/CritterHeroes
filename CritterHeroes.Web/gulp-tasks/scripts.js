﻿'use strict';

module.exports = function (gulp, plugins, common) {

    var distScripts = common.distPath + '/js';
    var libScripts = common.srcPath + '/lib';
    var appScripts = common.srcPath + '/js';
    var appBundle = appScripts + '/bundled';
    var stagingScripts = common.srcPath + '/staging';

    var bowerBase = 'bower_components';

    var hashes = {};

    var getHashes = function () {

        var collect = function (file, enc, cb) {
            if (file.revHash) {
                hashes[common.path.basename(file.revOrigPath, common.path.extname(file.revOrigPath))] = file.revHash;
            }
            this.push(file);
            return cb();
        };

        return plugins.through2.obj(collect);
    };

    gulp.task('clean-scripts', function () {
        return plugins.del([distScripts + '/**', '!' + common.distPath, libScripts, '!' + common.srcPath, './versioned-js.json']);
    });

    gulp.task('clean-staging', ['app-scripts'], function () {
        return plugins.del(stagingScripts);
    });

    gulp.task('copy-scripts-src', ['clean-scripts'], function () {

        var sources = [
            bowerBase + '/jquery/dist/jquery.js',
            bowerBase + '/jquery/dist/jquery.min.js',
            bowerBase + '/jquery/dist/jquery.min.map',
            bowerBase + '/jquery-validation/dist/jquery.validate.js',
            bowerBase + '/jquery-validation/dist/jquery.validate.min.js',
            bowerBase + '/jquery-ajax-unobtrusive/jquery.unobtrusive-ajax.js',
            bowerBase + '/jquery-ajax-unobtrusive/jquery.unobtrusive-ajax.min.js',
            bowerBase + '/jquery-validation-unobtrusive/jquery.validate.unobtrusive.js',
            bowerBase + '/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js',
            bowerBase + '/dropzone//dist/dropzone.js',
            bowerBase + '/dropzone/dist/min/dropzone.min.js'
    ];

        return gulp.src(sources, { base: bowerBase })
            .pipe(plugins.flatten())
            .pipe(gulp.dest(libScripts));

    });

    gulp.task('copy-scripts-dist', ['clean-scripts', 'copy-scripts-src'], function () {

        return gulp.src([libScripts + '/*.js', '!' + libScripts + '/*.min.js'])
            .pipe(plugins.flatten())
            .pipe(plugins.rev())
            .pipe(getHashes())
            .pipe(gulp.dest(distScripts))
            .pipe(plugins.rev.manifest({ path: "versioned-lib.json" }))
            .pipe(gulp.dest('./'));

    });

    gulp.task('copy-scripts-dist-min', ['clean-scripts', 'copy-scripts-src', 'copy-scripts-dist'], function () {

        return gulp.src([libScripts + '/*.min.js'])
            .pipe(plugins.rename(function (path) {
                var fileKey = common.path.basename(path.basename, '.min');
                path.basename = fileKey + '-' + hashes[fileKey] + '.min';
            }))
            .pipe(plugins.flatten())
            .pipe(gulp.dest(distScripts));

    });

    gulp.task('stage-app-scripts', ['clean-scripts'], function () {

        var bundleTask = gulp.src(appBundle + '/*.js')
            .pipe(plugins.concat('cheroes.js'))
            .pipe(plugins.rev())
            .pipe(gulp.dest(stagingScripts))
            .pipe(plugins.rev.manifest({ path: "versioned-js.json", merge: true }))
            .pipe(gulp.dest('./'));

        var remainingTask = gulp.src(appScripts + '/*.js')
            .pipe(plugins.rev())
            .pipe(gulp.dest(stagingScripts))
            .pipe(plugins.rev.manifest({ path: "versioned-js.json", merge: true }))
            .pipe(gulp.dest('./'));

        return plugins.mergeStream(bundleTask, remainingTask);

    });

    gulp.task('app-scripts', ['clean-scripts', 'stage-app-scripts'], function () {

        return gulp.src(stagingScripts + '/*.js')
            .pipe(gulp.dest(distScripts))
            .pipe(plugins.uglify())
            .pipe(plugins.rename({ extname: '.min.js' }))
            .pipe(gulp.dest(distScripts));

    });

    return ['clean-scripts', 'copy-scripts-src', 'copy-scripts-dist', 'copy-scripts-dist-min', 'stage-app-scripts', 'app-scripts', 'clean-staging'];

}