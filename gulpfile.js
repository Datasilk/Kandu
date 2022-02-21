'use strict';

//includes
var gulp = require('gulp'),
    concat = require('gulp-concat'),
    uglify = require('gulp-uglify'),
    cleancss = require('gulp-clean-css'),
    less = require('gulp-less'),
    rename = require('gulp-rename'),
    merge = require('merge-stream'),
    config = require('./App/config.json');
    
//get config variables from config.json
var environment = config.environment;

//determine environment
var prod = false;
if (environment != 'dev' && environment != 'development' && environment != null) {
    //using staging or production environment
    prod = true;
}

//paths
var paths = {
    scripts: 'App/Scripts/',
    css: 'App/CSS/',
    app: 'App/',
    webroot: 'App/wwwroot/',
};

//working paths
paths.working = {
    js: {
        platform: [
            paths.webroot + 'js/selector.js',
            paths.scripts + 'utility/velocity.min.js',
            paths.scripts + "platform/_super.js",
            paths.scripts + "platform/accordion.js",
            paths.scripts + "platform/ajax.js",
            paths.scripts + "platform/drag.js",
            paths.scripts + "platform/loader.js",
            paths.scripts + "platform/message.js",
            paths.scripts + "platform/polyfill.js",
            paths.scripts + "platform/popup.js",
            paths.scripts + "platform/view.js",
            paths.scripts + "platform/scrollbar.js",
            paths.scripts + "platform/svg.js",
            paths.scripts + "platform/util.js",
            paths.scripts + "platform/util.color.js",
            paths.scripts + "platform/util.text.js",
            paths.scripts + "platform/util.datetime.js",
            paths.scripts + "platform/validate.js",
            paths.scripts + "platform/window.js",
            paths.scripts + 'utility/remarkable.min.js',
            paths.scripts + 'utility/highlight.min.js'
        ],
        app: paths.app + '**/*.js',
        utility: [
            paths.scripts + 'utility/*.js',
            paths.scripts + 'utility/**/*.js'
        ]
    },

    less:{
        platform: paths.css + 'platform.less',
        app: [
            paths.app + '**/*.less'
        ],
        themes: paths.css + 'themes/*.less',
        tapestry: [
            paths.css + 'tapestry/tapestry.less',
            paths.css + 'tapestry/less/theme.less',
            paths.css + 'tapestry/less/util.less'
        ],
        utility: paths.css + 'utility/*.less'
    },

    css: {
        utility: paths.css + 'utility/**/*.css',
        app: paths.app + '**/*.css'
    },

    exclude: {
        app: [
            '!' + paths.app + 'wwwroot/**/*',
            '!' + paths.app + 'Content/**/*',
            '!' + paths.app + 'CSS/**/*',
            '!' + paths.app + 'CSS/*',
            '!' + paths.app + 'Scripts/**/*',
            '!' + paths.app + 'obj/**/*',
            '!' + paths.app + 'bin/**/*'
        ]
    },

    dashboard: {
        js: [
            paths.scripts + 'utility.js',
            paths.app + 'views/organizations/organizations.js',
            paths.app + 'views/teams/teams.js',
            paths.app + 'views/members/members.js',
            paths.app + 'views/security/security.js',
            paths.app + 'views/user/user.js',
            paths.app + 'views/boards/boards.js',
            paths.app + 'views/shared/header.js'
        ],
        css: [
            paths.webroot + 'css/views/organizations/organizations.css',
            paths.webroot + 'css/views/teams/teams.css',
            paths.webroot + 'css/views/members/members.css',
            paths.webroot + 'css/views/security/security.css',
            paths.webroot + 'css/views/user/user.css',
            paths.webroot + 'css/views/boards/boards.css',
            paths.webroot + 'css/views/shared/header.css'
        ]
    }
};

//compiled paths
paths.compiled = {
    platform: paths.webroot + 'js/platform.js',
    js: paths.webroot + 'js/',
    css: paths.webroot + 'css/',
    themes: paths.webroot + 'css/themes/'
};

//tasks for compiling javascript //////////////////////////////////////////////////////////////
gulp.task('js:app', function () {
    var pathlist = paths.working.exclude.app.slice(0);
    pathlist.unshift(paths.working.js.app);
    var p = gulp.src(pathlist)
        .pipe(rename(function (path) {
            path.dirname = path.dirname.toLowerCase();
            path.basename = path.basename.toLowerCase();
            path.extname = path.extname.toLowerCase();
        }));

    if (prod == true) { p = p.pipe(uglify()); }
    return p.pipe(gulp.dest(paths.compiled.js, { overwrite: true }));
});

gulp.task('js:selector', function () {
    var p = gulp.src(paths.scripts + 'selector/selector.js', { base: '.' })
            .pipe(concat('selector.js'));
    if (prod == true) { 
        p = p.pipe(uglify());
    }
    return p.pipe(gulp.dest(paths.compiled.js, { overwrite: true }));
});

gulp.task('js:platform', function () {
    var p = gulp.src(paths.working.js.platform, { base: '.' })
        .pipe(concat(paths.compiled.platform));
    if (prod == true) { p = p.pipe(uglify()); }
    return p.pipe(gulp.dest('.', { overwrite: true }));
});

gulp.task('js:utility', function () {
    var p = gulp.src(paths.working.js.utility)
        .pipe(rename(function (path) {
            path.dirname = path.dirname.toLowerCase();
            path.basename = path.basename.toLowerCase();
            path.extname = path.extname.toLowerCase();
        }));

    if (prod == true) { p = p.pipe(uglify()); }
    return p.pipe(gulp.dest(paths.compiled.js + 'utility', { overwrite: true }));
});

/* custom js compiling */
gulp.task('js:dashboard', function () {
    var p = gulp.src(paths.working.dashboard.js, { base: '.' })
        .pipe(concat(paths.compiled.js + 'dashboard.js'));
    if (prod == true) { p = p.pipe(uglify()); }
    return p.pipe(gulp.dest('.', { overwrite: true }));
});

gulp.task('js', gulp.series(
    'js:app',
    'js:selector',
    'js:platform',
    'js:utility',
    'js:dashboard'
));

//tasks for compiling LESS & CSS /////////////////////////////////////////////////////////////////////
gulp.task('less:app', function () {
    var pathlist = paths.working.exclude.app.slice(0);
    for (var x = paths.working.less.app.length - 1; x >= 0; x--) {
        pathlist.unshift(paths.working.less.app[x]);
    }
    var p = gulp.src(pathlist)
        .pipe(less())
        .pipe(rename(function (path) {
            path.dirname = path.dirname.toLowerCase();
            path.basename = path.basename.toLowerCase();
            path.extname = path.extname.toLowerCase();
        }));
    if(prod == true){ p = p.pipe(cleancss({compatibility: 'ie8'})); }
    return p.pipe(gulp.dest(paths.compiled.css, { overwrite: true }));
});

gulp.task('less:platform', function () {
    var p = gulp.src(paths.working.less.platform)
        .pipe(less());
    if (prod == true) { p = p.pipe(cleancss({ compatibility: 'ie8' })); }
    return p.pipe(gulp.dest(paths.compiled.css, { overwrite: true }));
});

gulp.task('less:themes', function () {
    var p = gulp.src(paths.working.less.themes)
        .pipe(less());
    if (prod == true) { p = p.pipe(cleancss({ compatibility: 'ie8' })); }
    return p.pipe(gulp.dest(paths.compiled.css + 'themes', { overwrite: true }));
});

gulp.task('less:utility', function () {
    var p = gulp.src(paths.working.less.utility)
        .pipe(less());
    if (prod == true) { p = p.pipe(cleancss({ compatibility: 'ie8' })); }
    return p.pipe(gulp.dest(paths.compiled.css + 'themes', { overwrite: true }));
});

gulp.task('css:app', function () {
    var pathlist = paths.working.exclude.app.slice(0);
    pathlist.unshift(paths.working.css.app);
    var p = gulp.src(pathlist)
        .pipe(rename(function (path) {
            path.dirname = path.dirname.toLowerCase();
            path.basename = path.basename.toLowerCase();
            path.extname = path.extname.toLowerCase();
        }));
    if (prod == true) { p = p.pipe(cleancss({ compatibility: 'ie8' })); }
    return p.pipe(gulp.dest(paths.compiled.css, { overwrite: true }));
});

gulp.task('css:utility', function () {
    var p = gulp.src(paths.working.css.utility)
        .pipe(rename(function (path) {
            path.dirname = path.dirname.toLowerCase();
            path.basename = path.basename.toLowerCase();
            path.extname = path.extname.toLowerCase();
        }));
    if (prod == true) { p = p.pipe(cleancss({ compatibility: 'ie8' })); }
    return p.pipe(gulp.dest(paths.compiled.css + 'utility', { overwrite: true }));
});

/* custom css compiling */
gulp.task('less:dashboard', function () {
    var p = gulp.src(paths.working.dashboard.css, { base: '.' })
        .pipe(concat(paths.compiled.css + 'dashboard.css'));
    if (prod == true) { p = p.pipe(uglify()); }
    return p.pipe(gulp.dest('.', { overwrite: true }));
});

gulp.task('less', gulp.series(
    'less:platform',
    'less:app',
    'less:themes',
    'less:utility',
    'less:dashboard'
));

gulp.task('css', gulp.series(
    'css:app',
    'css:utility'
));

//tasks for compiling vendor app dependencies /////////////////////////////////////////////////


//default task
gulp.task('default', gulp.series('js', 'less', 'css'));

//watch task
gulp.task('watch', function () {
    //watch platform JS
    gulp.watch(paths.working.js.platform, gulp.series('js:platform'));
    
    //watch app JS
    var pathjs = [paths.working.js.app, ...paths.working.exclude.app.map(a => a + '*.js')];
    var watchAppJs = gulp.watch(pathjs);
    watchAppJs.on('change', (path) => {
        //only copy JS files that were changed in the app folder
        path = path.replace(/\\/g, '/');
        var tasks = [];
        var p = gulp.src(path, { base: 'App' });
        if (prod == true) { p = p.pipe(uglify()); }
        p.pipe(gulp.dest(paths.compiled.js, { overwrite: true }));
        tasks.push(p);
        var newpath = path.toLowerCase().replace(/\\/g, '/');
        console.log('copying ' + path + ' to ' + newpath);
        if (paths.working.dashboard.js.filter(a => a.toLowerCase() == newpath).length > 0) {
            //recompile dashboard.js since a related js file was updated
            console.log('updating dashboard.js');
            var p2 = gulp.src(paths.working.dashboard.js, { base: '.' })
                .pipe(concat(paths.compiled.js + 'dashboard.js'));
            if (prod == true) { p2 = p2.pipe(uglify()); }
            p2.pipe(gulp.dest('.', { overwrite: true }));
            tasks.push(p2);
        }
        return merge(tasks);
    });

    //watch app LESS
    var pathless = [...paths.working.less.app, ...paths.working.exclude.app.map(a => a + '*.less')];
    var watchAppLess = gulp.watch(pathless);
    watchAppLess.on('change', (path) => {
        //only copy LESS files that were changed in the app folder
        path = path.replace(/\\/g, '/');
        var tasks = [];
        var p = gulp.src(path, { base: 'App' })
            .pipe(less())
            .pipe(rename(function (path) {
                path.dirname = path.dirname.toLowerCase();
                path.basename = path.basename.toLowerCase();
                path.extname = path.extname.toLowerCase();
            }));
        if (prod == true) { p = p.pipe(cleancss({ compatibility: 'ie8' })); }
        p.pipe(gulp.dest(paths.compiled.css, { overwrite: true }));
        tasks.push(p);

        var newpath = path.toLowerCase().replace(paths.app.toLowerCase(), paths.webroot.toLowerCase() + 'css/').replace('.less', '.css');
        console.log('copying ' + path + ' to ' + newpath);
        if (paths.working.dashboard.css.filter(a => a.toLowerCase() == newpath).length > 0) {
            //recompile dashboard.css since a related CSS file was updated
            console.log('updating dashboard.css');
            var p2 = gulp.src(paths.working.dashboard.css, { base: '.' })
                .pipe(concat(paths.compiled.css + 'dashboard.css'));
            if (prod == true) { p2 = p2.pipe(uglify()); }
            p2.pipe(gulp.dest('.', { overwrite: true }));
            tasks.push(p2);
        }
        return merge(tasks);
    });

    //watch platform LESS
    gulp.watch([
        paths.working.less.platform,
        ...paths.working.less.tapestry
    ], gulp.series('less:platform'));

    //watch themes LESS
    gulp.watch([
        paths.working.less.themes
    ], gulp.series('less:themes', 'less:platform'));

    //watch utility LESS
    gulp.watch([
        paths.working.less.utility
    ], gulp.series('less:utility'));

    //watch app CSS
    //var pathcss = [paths.working.css.app, ...paths.working.exclude.app.map(a => a + '*.css')];
    //gulp.watch(pathcss, gulp.series('css:app'));
    
    //watch utility CSS
    gulp.watch([
        paths.working.css.utility
    ], gulp.series('css:utility'));
});