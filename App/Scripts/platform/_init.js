/*/////////////////////////////////////
Initialize Kandu Platform
/////////////////////////////////////*/

// Window Events ////////////////////////////////////////////////////////////////////////////////////
$(document).on('ready', function () { S.events.doc.ready(); });
$(window).on('resize', function () { S.events.doc.resize.trigger(); });
$(window).on('scroll', function () { S.events.doc.scroll.trigger(); });

//raise event after document is loaded
S.events.doc.load();
