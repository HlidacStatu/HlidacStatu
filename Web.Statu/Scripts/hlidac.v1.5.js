var persons;

var param = 'fbclid';
if (location.search.indexOf(param + '=') !== -1) {
    var replace = '';
    try {
        var url = new URL(location);
        url.searchParams.delete(param);
        replace = url.pathname + url.search + url.hash;
    } catch (ex) {
        var regExp = new RegExp('[?&]' + param + '=.*$');
        replace = location.search.replace(regExp, '');
        replace = location.pathname + replace + location.hash;
    }
    history.replaceState(null, '', replace);
}

function getRandom(min, max) {
    return Math.random() * (max - min) + min;
}

function timeDayDifference(endDate, startDate) {
    var difference = endDate.getTime() - startDate.getTime();
    difference = (Math.floor(difference / 1000 / 60 / 60 / 24) * 1000 * 60 * 60 * 24);
    return difference;
}

function createCookie(name, value, days) {
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        var expires = "; expires=" + date.toGMTString();
    }
    else var expires = "";
    document.cookie = name + "=" + value + expires + "; path=/";
}

function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function eraseCookie(name) {
    createCookie(name, "", -1);
}


var trackInLink = function (obj, source) {
    var url = obj.getAttribute("href");
    ga('send', 'event', 'insideLink', source, url, {
        'transport': 'beacon'
    });
    return true;
}
var trackOutLink = function (obj, source) {
    var url = obj.getAttribute("href");
    ga('send', 'event', 'outbound', source, url, {
        'transport': 'beacon'
    });
    return true;
}


var filesadded = "" //list of files already added

function loadjscssfile(filename, filetype) {
    if (filetype == "js") { //if filename is a external JavaScript file
        var fileref = document.createElement('script')
        fileref.setAttribute("type", "text/javascript")
        fileref.setAttribute("src", filename)
    }
    else if (filetype == "css") { //if filename is an external CSS file
        var fileref = document.createElement("link")
        fileref.setAttribute("rel", "stylesheet")
        fileref.setAttribute("type", "text/css")
        fileref.setAttribute("href", filename)
    }
    if (typeof fileref != "undefined")
        document.getElementsByTagName("head")[0].appendChild(fileref)
}
function checkloadjscssfile(filename, filetype) {
    if (filesadded.indexOf("[" + filename + "]") == -1) {
        loadjscssfile(filename, filetype)
        filesadded += "[" + filename + "]" //List of files added in the form "[filename1],[filename2],etc"
    }
}

function filterReviewDom(obj) {
    var o = obj.clone()
        .find("script,noscript,style,form,.modal,.toReviewBtn")
        .remove()
        .end();
    return o;
}

function AddReview(btn, formid) {
    var data = "?";
    $('#' + formid + ' input,form textarea').each(function () { var t = $(this); data = data + t.attr('name') + '=' + encodeURI(t.val()) + "&"; });

    var btn1 = $(btn);
    var url = "/manage/addReview" + data;
    var jqxhr = $.ajax(url)
        .done(function (data, textStatus, jqXHR) {
            if (data == "1") {
                btn1.attr("disabled", "disabled");
                btn1.removeClass("btn-default");
                btn1.addClass("btn-success");
                btn1.text("Posláno. Děkujeme za pomoc, co nejdříve změnu provedeme.");
            };
            if (data == "0") {
                btn1.removeClass("btn-default");
                btn1.addClass("btn-danger");
                btn1.text("Chyba. Opravu jsme nedostali. Zkuste to za chvili znovu.");
            };

        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            btn1.removeClass("btn-default");
            btn1.addClass("btn-danger");
            btn1.text("Chyba volání. Hlídání se neuložilo. Zkuste to za chvíli znovu.");

        })
}

function AddNewWD(query, datatype, name, period, focus, btn) {
    var btn1 = $(btn);
    var url = "/manage/addWd?name=" + encodeURIComponent(name) + "&datatype=" + encodeURIComponent(datatype) + "&period=" + period + "&focus=" + focus + "&query=" + encodeURIComponent(query);
    var jqxhr = $.ajax(url)
        .done(function (data, textStatus, jqXHR) {
            if (data == "1") {
                btn1.attr("disabled", "disabled");
                btn1.removeClass("btn-default");
                btn1.addClass("btn-success");
                btn1.text("Uloženo. V noci začínáme hlídat.");
            };
            if (data == "0") {
                btn1.removeClass("btn-default");
                btn1.addClass("btn-danger");
                btn1.text("Chyba. Hlídání se neuložilo. Zkuste to za chvíli znovu.");
            };
            if (data == "2") {
                //window.location = "http://www.yoururl.com";
                btn1.removeClass("btn-default");
                btn1.addClass("btn-danger");
                btn1.attr("href", "/cenik");
                btn1.text("Problém. Hlídání se uložilo, ale nemůžeme ho spustit. Klikněte sem pro více informací.");
                btn1.prop('onclick', null).off('click');
            };

        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            btn1.removeClass("btn-default");
            btn1.addClass("btn-danger");
            btn1.text("Chyba volání. Hlídání se neuložilo. Zkuste to za chvíli znovu.");

        })
}


function ResendConfirmationMail(calledBy) {
    var btn1 = $(calledBy);
    btn1.attr("disabled", "disabled");
    btn1.removeClass("btn-default");
    btn1.addClass("btn-success disabled");
    var url = "/api/v1/ResendConfirmationMail";
    var jqxhr = $.ajax(url)
        .done(function (data, textStatus, jqXHR) {
            if (data == "ok") {
                btn1.removeClass("btn-default");
                btn1.addClass("btn-success");
                btn1.text("Email odeslán. Zkontrolujte si poštu, i složku s nevyžádanou poštou.");
            }
            else  {
                btn1.removeClass("btn-default");
                btn1.addClass("btn-danger");
                btn1.text("Nastala nějaká chyba, víme o ni a budeme ji řešit.");
            };
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            btn1.removeClass("btn-default");
            btn1.addClass("btn-danger");
            btn1.text("Nastala nějaká chyba, víme o ni a budeme ji řešit.");

        })
}
function ChangeBookmark(btn) {
    var btn1 = $(btn);
    if (btn1.hasClass("bookmarkOn")) {
        //remove bookmark
        var bid = encodeURIComponent(btn1.attr("bmid"));
        var btype = btn1.attr("bmtype");
        var url = "/manage/removeBookmark?id=" + bid + "&type=" + btype;
    }
    else {
        //set bookmark
        var bname = encodeURIComponent(btn1.attr("bmname"));
        var burl = encodeURIComponent(btn1.attr("bmurl"));
        var bid = encodeURIComponent(btn1.attr("bmid"));
        var btype = btn1.attr("bmtype");
        var url = "/manage/setBookmark?name=" + bname + "&url=" + burl + "&id=" + bid + "&type=" + btype;
    }

    var jqxhr = $.ajax(url)
        .done(function (data, textStatus, jqXHR) {
            if (data == "1") {
                btn1.removeClass("bookmarkOff")
                btn1.addClass("bookmarkOn")
            };
            if (data == "0") {
                btn1.removeClass("bookmarkOn")
                btn1.addClass("bookmarkOff")
            };
            if (data == "-1") {
                window.alert("Nastala chyba na serveru. Už o tom víme a řešíme to. Omlouváme se.")
            };

        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            window.alert("Nastala chyba na serveru. Už o tom víme a řešíme to. Omlouváme se.")
        });
}


var campaignN = "bookmark";
$(function () {

    $(".low-box").each(function () {
        var t = $(this);
        var more = t.find(".low-box-line:first");
        var actheight = t.outerHeight();
        var cssheight = parseInt(t.css("max-height"),10);
        if (actheight < cssheight) {
            more.hide();
        }
    });
    $(".low-box .low-box-line .more").click(function () {
        var totalHeight = 0;
        $el = $(this); $p = $el.parent(); $up = $p.parent();
        $ps = $up.find(".low-box-content:first");
        // measure how tall inside should be by adding together heights of all inside paragraphs (except read-more paragraph)
        $ps.each(function () {
            totalHeight += $(this).outerHeight();
            // FAIL totalHeight += $(this).css("margin-bottom");
        });

        $up.css({
            // Set height to prevent instant jumpdown when max height is removed
            "height": $up.height(),
            "max-height": 9999
        })
            .animate({
                "height": totalHeight
            });

        // fade out read-more
        $p.fadeOut();
        // prevent jump-down
        return false;
    });

    //Vertical tabs
    $("div.verticaltab-menu>div.list-group>a").click(function (e) {
        e.preventDefault();
        var t = $(this);
        var par = t.parents("div.verticaltab-container").first();
        var index = $(this).index();
        t.siblings('a.active').removeClass("active");
        t.addClass("active");
        par.find("div.verticaltab>div.verticaltab-content").removeClass("active");
        par.find("div.verticaltab>div.verticaltab-content").eq(index).addClass("active");
    });

    // ADS

    var darLast = parseInt(readCookie("darLast" + campaignN));
    if (isNaN(darLast))
        darLast = 1;
    var darNum = parseInt(readCookie("darNum" + campaignN));
    if (isNaN(darNum))
        darNum = 0;
    var now = new Date();
    var darDatDiff = timeDayDifference(now, new Date(darLast));



    if (false && darDatDiff > 5 && darNum < 5) {

        $('#dar-footer-msg').fadeIn();
        ga('send', 'event', 'darujAdfooter', 'shown', { nonInteraction: true });
        createCookie("darLast" + campaignN, now.getTime(), 25);
        createCookie("darNum" + campaignN, darNum + 1, 360);


        //setTimeout(function () { $('#ads').fadeIn(); }, 500);

        var zavri = $("#dar-footer-msg #fund-zavri-btn");
        var info = $("#dar-footer-msg #fund-info-btn");
        var daruj = $("#dar-footer-msg #fund-daruj-btn");

        daruj.click(function () {
            createCookie("darLast" + campaignN, now.getTime(), 25);
            createCookie("darNum" + campaignN, darNum + 3, 360);
            $('#dar-footer-msg').fadeOut();

        });
        info.click(function () {
            createCookie("darLast" + campaignN, now.getTime(), 11);
            createCookie("darNum" + campaignN, darNum + 1, 360);
            $('#dar-footer-msg').fadeOut();
            return true;
        });

        zavri.click(function () {
            createCookie("darLast" + campaignN, now.getTime(), 5);
            createCookie("darNum" + campaignN, darNum + 1, 360);
            $('#dar-footer-msg').fadeOut();
            return false;
        });
    }
    else {
        //if (_showFAD) {
        //setTimeout(function () { $('#fads').fadeIn(800); }, 900);
        //$('#ads').fadeIn(250);
        //}
    }




    $('[data-toggle="popover"]').popover();

    function Hamburger() {
        var $control = $('.hamburger');
        var $body = $('body');

        function toggleNav() {
            $body.toggleClass('nav-open');
        }

        function bindEvents() {
            $control.on('click', toggleNav);
        }

        function init() {
            bindEvents();
        }

        init();
    }

    $('.social-fb, .social-tw').on('click', function (event) {
        if (bowser.mobile || bowser.tablet) {
            return true;
        }

        event.preventDefault();

        var wWidth = $(window).width();
        var wHeight = $(window).height();

        var w = 580;
        var h = 325;

        if (w > wWidth) {
            w = wWidth;
        }

        if (h > wWidth) {
            h = wHeight;
        }

        var wLeft = wWidth / 2 - w / 2;
        var wTop = wHeight / 2 - h / 2;

        window.open($(this).attr('href'), 'sharer', 'top=' + wTop + ',left=' + wLeft + ',toolbar=0,status=0,width=580,height=325');
    });

    Hamburger();

    $("#VZInfoAnon").on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget); // Button that triggered the modal
        var url = encodeURI(button.data('url')); // Extract info from data-* attributes
        // If necessary, you could initiate an AJAX request here (and then do the updating in a callback).
        // Update the modal's content. We'll use jQuery here, but you could use a data binding library or other methods instead.
        var modal = $(this);
        modal.find('#VZInfoAnonRegister').attr("href", "/account/Register?returnUrl=" + url);// text('New message to ' + recipient);
        modal.find('#VZInfoAnonLogin').attr("href", "/account/Login?returnUrl=" + url);
    })

});

