var Swiper = function (f, b) {
    function g(a) {
        return document.querySelectorAll ? document.querySelectorAll(a) : jQuery(a);
    }
    function h() {
        var c = y - l;
        b.freeMode && (c = y - l);
        b.slidesPerView > a.slides.length && (c = 0);
        0 > c && (c = 0);
        return c;
    }
    function n() {
        function c(c) {
            var d = new Image;
            d.onload = function () {
                a.imagesLoaded++;
                if (a.imagesLoaded == a.imagesToLoad.length && (a.reInit(), b.onImagesReady)) {
                    b.onImagesReady(a);
                }
            };
            d.src = c;
        }
        a.browser.ie10 ? (a.h.addEventListener(a.wrapper, a.touchEvents.touchStart, z, !1), a.h.addEventListener(document, a.touchEvents.touchMove, A, !1), a.h.addEventListener(document, a.touchEvents.touchEnd, B, !1)) : (a.support.touch && (a.h.addEventListener(a.wrapper, "touchstart", z, !1), a.h.addEventListener(a.wrapper, "touchmove", A, !1), a.h.addEventListener(a.wrapper, "touchend", B, !1)), b.simulateTouch && (a.h.addEventListener(a.wrapper, "mousedown", z, !1), a.h.addEventListener(document, "mousemove", A, !1), a.h.addEventListener(document, "mouseup", B, !1)));
        b.autoResize && a.h.addEventListener(window, "resize", a.resizeFix, !1);
        t();
        a._wheelEvent = !1;
        if (b.mousewheelControl) {
            void 0 !== document.onmousewheel && (a._wheelEvent = "mousewheel");
            try {
                WheelEvent("wheel"), a._wheelEvent = "wheel";
            } catch (d) { }
            a._wheelEvent || (a._wheelEvent = "DOMMouseScroll");
            a._wheelEvent && a.h.addEventListener(a.container, a._wheelEvent, N, !1);
        }
        b.keyboardControl && a.h.addEventListener(document, "keydown", O, !1);
        if (b.updateOnImagesReady) {
            document.querySelectorAll ? a.imagesToLoad = a.container.querySelectorAll("img") : window.jQuery && (a.imagesToLoad = g(a.container).find("img"));
            for (var e = 0; e < a.imagesToLoad.length; e++) {
                c(a.imagesToLoad[e].getAttribute("src"));
            }
        }
    }
    function t() {
        if (b.preventLinks) {
            var c = [];
            document.querySelectorAll ? c = a.container.querySelectorAll("a") : window.jQuery && (c = g(a.container).find("a"));
            for (var d = 0; d < c.length; d++) {
                a.h.addEventListener(c[d], "click", P, !1);
            }
        }
        if (b.releaseFormElements) {
            for (c = document.querySelectorAll ? a.container.querySelectorAll("input, textarea, select") : g(a.container).find("input, textarea, select"), d = 0; d < c.length; d++) {
                a.h.addEventListener(c[d], a.touchEvents.touchStart, Q, !0);
            }
        }
        if (b.onSlideClick) {
            for (d = 0; d < a.slides.length; d++) {
                a.h.addEventListener(a.slides[d], "click", R, !1);
            }
        }
        if (b.onSlideTouch) {
            for (d = 0; d < a.slides.length; d++) {
                a.h.addEventListener(a.slides[d], a.touchEvents.touchStart, S, !1);
            }
        }
    }
    function v() {
        if (b.onSlideClick) {
            for (var c = 0; c < a.slides.length; c++) {
                a.h.removeEventListener(a.slides[c], "click", R, !1);
            }
        }
        if (b.onSlideTouch) {
            for (c = 0; c < a.slides.length; c++) {
                a.h.removeEventListener(a.slides[c], a.touchEvents.touchStart, S, !1);
            }
        }
        if (b.releaseFormElements) {
            for (var d = document.querySelectorAll ? a.container.querySelectorAll("input, textarea, select") : g(a.container).find("input, textarea, select"), c = 0; c < d.length; c++) {
                a.h.removeEventListener(d[c], a.touchEvents.touchStart, Q, !0);
            }
        }
        if (b.preventLinks) {
            for (d = [], document.querySelectorAll ? d = a.container.querySelectorAll("a") : window.jQuery && (d = g(a.container).find("a")), c = 0; c < d.length; c++) {
                a.h.removeEventListener(d[c], "click", P, !1);
            }
        }
    }
    function O(c) {
        var b = c.keyCode || c.charCode;
        if (37 == b || 39 == b || 38 == b || 40 == b) {
            for (var e = !1, f = a.h.getOffset(a.container), h = a.h.windowScroll().left, g = a.h.windowScroll().top, m = a.h.windowWidth(), l = a.h.windowHeight(), f = [
                [f.left, f.top],
                [f.left + a.width, f.top],
                [f.left, f.top + a.height],
                [f.left + a.width, f.top + a.height]
            ], p = 0; p < f.length; p++) {
                var r = f[p];
                r[0] >= h && (r[0] <= h + m && r[1] >= g && r[1] <= g + l) && (e = !0);
            }
            if (!e) {
                return;
            }
        }
        if (k) {
            if (37 == b || 39 == b) {
                c.preventDefault ? c.preventDefault() : c.returnValue = !1;
            }
            39 == b && a.swipeNext();
            37 == b && a.swipePrev();
        } else {
            if (38 == b || 40 == b) {
                c.preventDefault ? c.preventDefault() : c.returnValue = !1;
            }
            40 == b && a.swipeNext();
            38 == b && a.swipePrev();
        }
    }
    function N(c) {
        var d = a._wheelEvent,
            e;
        c.detail ? e = -c.detail : "mousewheel" == d ? e = c.wheelDelta : "DOMMouseScroll" == d ? e = -c.detail : "wheel" == d && (e = Math.abs(c.deltaX) > Math.abs(c.deltaY) ? -c.deltaX : -c.deltaY);
        b.freeMode ? (k ? a.getWrapperTranslate("x") : a.getWrapperTranslate("y"), k ? (d = a.getWrapperTranslate("x") + e, e = a.getWrapperTranslate("y"), 0 < d && (d = 0), d < -h() && (d = -h())) : (d = a.getWrapperTranslate("x"), e = a.getWrapperTranslate("y") + e, 0 < e && (e = 0), e < -h() && (e = -h())), a.setWrapperTransition(0), a.setWrapperTranslate(d, e, 0), k ? a.updateActiveSlide(d) : a.updateActiveSlide(e)) : 0 > e ? a.swipeNext() : a.swipePrev();
        b.autoplay && a.stopAutoplay();
        c.preventDefault ? c.preventDefault() : c.returnValue = !1;
        return !1;
    }
    function T(a) {
        for (var d = !1; !d;) {
            -1 < a.className.indexOf(b.slideClass) && (d = a), a = a.parentElement;
        }
        return d;
    }
    function R(c) {
        a.allowSlideClick && (c.target ? (a.clickedSlide = this, a.clickedSlideIndex = a.slides.indexOf(this)) : (a.clickedSlide = T(c.srcElement), a.clickedSlideIndex = a.slides.indexOf(a.clickedSlide)), b.onSlideClick(a));
    }
    function S(c) {
        a.clickedSlide = c.target ? this : T(c.srcElement);
        a.clickedSlideIndex = a.slides.indexOf(a.clickedSlide);
        b.onSlideTouch(a);
    }
    function P(b) {
        if (!a.allowLinks) {
            return b.preventDefault ? b.preventDefault() : b.returnValue = !1, !1;
        }
    }
    function Q(a) {
        a.stopPropagation ? a.stopPropagation() : a.returnValue = !1;
        return !1;
    }
    function z(c) {
        b.preventLinks && (a.allowLinks = !0);
        if (a.isTouched || b.onlyExternal) {
            return !1;
        }
        var d;
        if (d = b.noSwiping) {
            if (d = c.target || c.srcElement) {
                d = c.target || c.srcElement;
                var e = !1;
                do {
                    -1 < d.className.indexOf(b.noSwipingClass) && (e = !0), d = d.parentElement;
                } while (!e && d.parentElement && -1 == d.className.indexOf(b.wrapperClass));
                !e && (-1 < d.className.indexOf(b.wrapperClass) && -1 < d.className.indexOf(b.noSwipingClass)) && (e = !0);
                d = e;
            }
        }
        if (d) {
            return !1;
        }
        G = !1;
        a.isTouched = !0;
        u = "touchstart" == c.type;
        if (!u || 1 == c.targetTouches.length) {
            b.loop && a.fixLoop();
            a.callPlugins("onTouchStartBegin");
            u || (c.preventDefault ? c.preventDefault() : c.returnValue = !1);
            d = u ? c.targetTouches[0].pageX : c.pageX || c.clientX;
            c = u ? c.targetTouches[0].pageY : c.pageY || c.clientY;
            a.touches.startX = a.touches.currentX = d;
            a.touches.startY = a.touches.currentY = c;
            a.touches.start = a.touches.current = k ? d : c;
            a.setWrapperTransition(0);
            a.positions.start = a.positions.current = k ? a.getWrapperTranslate("x") : a.getWrapperTranslate("y");
            k ? a.setWrapperTranslate(a.positions.start, 0, 0) : a.setWrapperTranslate(0, a.positions.start, 0);
            a.times.start = (new Date).getTime();
            x = void 0;
            0 < b.moveStartThreshold && (M = !1);
            if (b.onTouchStart) {
                b.onTouchStart(a);
            }
            a.callPlugins("onTouchStartEnd");
        }
    }
    function A(c) {
        if (a.isTouched && !b.onlyExternal && (!u || "mousemove" != c.type)) {
            var d = u ? c.targetTouches[0].pageX : c.pageX || c.clientX,
                e = u ? c.targetTouches[0].pageY : c.pageY || c.clientY;
            "undefined" === typeof x && k && (x = !!(x || Math.abs(e - a.touches.startY) > Math.abs(d - a.touches.startX)));
            "undefined" !== typeof x || k || (x = !!(x || Math.abs(e - a.touches.startY) < Math.abs(d - a.touches.startX)));
            if (x) {
                a.isTouched = !1;
            } else {
                if (c.assignedToSwiper) {
                    a.isTouched = !1;
                } else {
                    if (c.assignedToSwiper = !0, a.isMoved = !0, b.preventLinks && (a.allowLinks = !1), b.onSlideClick && (a.allowSlideClick = !1), b.autoplay && a.stopAutoplay(), !u || 1 == c.touches.length) {
                        a.callPlugins("onTouchMoveStart");
                        c.preventDefault ? c.preventDefault() : c.returnValue = !1;
                        a.touches.current = k ? d : e;
                        a.positions.current = (a.touches.current - a.touches.start) * b.touchRatio + a.positions.start;
                        if (0 < a.positions.current && b.onResistanceBefore) {
                            b.onResistanceBefore(a, a.positions.current);
                        }
                        if (a.positions.current < -h() && b.onResistanceAfter) {
                            b.onResistanceAfter(a, Math.abs(a.positions.current + h()));
                        }
                        b.resistance && "100%" != b.resistance && (0 < a.positions.current && (c = 1 - a.positions.current / l / 2, a.positions.current = 0.5 > c ? l / 2 : a.positions.current * c), a.positions.current < -h() && (d = (a.touches.current - a.touches.start) * b.touchRatio + (h() + a.positions.start), c = (l + d) / l, d = a.positions.current - d * (1 - c) / 2, e = -h() - l / 2, a.positions.current = d < e || 0 >= c ? e : d));
                        b.resistance && "100%" == b.resistance && (0 < a.positions.current && (!b.freeMode || b.freeModeFluid) && (a.positions.current = 0), a.positions.current < -h() && (!b.freeMode || b.freeModeFluid) && (a.positions.current = -h()));
                        if (b.followFinger) {
                            b.moveStartThreshold ? Math.abs(a.touches.current - a.touches.start) > b.moveStartThreshold || M ? (M = !0, k ? a.setWrapperTranslate(a.positions.current, 0, 0) : a.setWrapperTranslate(0, a.positions.current, 0)) : a.positions.current = a.positions.start : k ? a.setWrapperTranslate(a.positions.current, 0, 0) : a.setWrapperTranslate(0, a.positions.current, 0);
                            (b.freeMode || b.watchActiveIndex) && a.updateActiveSlide(a.positions.current);
                            b.grabCursor && (a.container.style.cursor = "move", a.container.style.cursor = "grabbing", a.container.style.cursor = "-moz-grabbin", a.container.style.cursor = "-webkit-grabbing");
                            D || (D = a.touches.current);
                            H || (H = (new Date).getTime());
                            a.velocity = (a.touches.current - D) / ((new Date).getTime() - H) / 2;
                            2 > Math.abs(a.touches.current - D) && (a.velocity = 0);
                            D = a.touches.current;
                            H = (new Date).getTime();
                            a.callPlugins("onTouchMoveEnd");
                            if (b.onTouchMove) {
                                b.onTouchMove(a);
                            }
                            return !1;
                        }
                    }
                }
            }
        }
    }
    function B(c) {
        x && a.swipeReset();
        if (!b.onlyExternal && a.isTouched) {
            a.isTouched = !1;
            b.grabCursor && (a.container.style.cursor = "move", a.container.style.cursor = "grab", a.container.style.cursor = "-moz-grab", a.container.style.cursor = "-webkit-grab");
            a.positions.current || 0 === a.positions.current || (a.positions.current = a.positions.start);
            b.followFinger && (k ? a.setWrapperTranslate(a.positions.current, 0, 0) : a.setWrapperTranslate(0, a.positions.current, 0));
            a.times.end = (new Date).getTime();
            a.touches.diff = a.touches.current - a.touches.start;
            a.touches.abs = Math.abs(a.touches.diff);
            a.positions.diff = a.positions.current - a.positions.start;
            a.positions.abs = Math.abs(a.positions.diff);
            var d = a.positions.diff,
                e = a.positions.abs;
            c = a.times.end - a.times.start;
            5 > e && (300 > c && !1 == a.allowLinks) && (b.freeMode || 0 == e || a.swipeReset(), b.preventLinks && (a.allowLinks = !0), b.onSlideClick && (a.allowSlideClick = !0));
            setTimeout(function () {
                b.preventLinks && (a.allowLinks = !0);
                b.onSlideClick && (a.allowSlideClick = !0);
            }, 100);
            if (a.isMoved) {
                a.isMoved = !1;
                var f = h();
                if (0 < a.positions.current) {
                    a.swipeReset();
                } else {
                    if (a.positions.current < -f) {
                        a.swipeReset();
                    } else {
                        if (b.freeMode) {
                            if (b.freeModeFluid) {
                                var e = 1000 * b.momentumRatio,
                                    d = a.positions.current + a.velocity * e,
                                    g = !1,
                                    F, m = 20 * Math.abs(a.velocity) * b.momentumBounceRatio;
                                d < -f && (b.momentumBounce && a.support.transitions ? (d + f < -m && (d = -f - m), F = -f, G = g = !0) : d = -f);
                                0 < d && (b.momentumBounce && a.support.transitions ? (d > m && (d = m), F = 0, G = g = !0) : d = 0);
                                0 != a.velocity && (e = Math.abs((d - a.positions.current) / a.velocity));
                                k ? a.setWrapperTranslate(d, 0, 0) : a.setWrapperTranslate(0, d, 0);
                                a.setWrapperTransition(e);
                                b.momentumBounce && g && a.wrapperTransitionEnd(function () {
                                    if (G) {
                                        if (b.onMomentumBounce) {
                                            b.onMomentumBounce(a);
                                        }
                                        k ? a.setWrapperTranslate(F, 0, 0) : a.setWrapperTranslate(0, F, 0);
                                        a.setWrapperTransition(300);
                                    }
                                });
                                a.updateActiveSlide(d);
                            } (!b.freeModeFluid || 300 <= c) && a.updateActiveSlide(a.positions.current);
                        } else {
                            E = 0 > d ? "toNext" : "toPrev";
                            "toNext" == E && 300 >= c && (30 > e || !b.shortSwipes ? a.swipeReset() : a.swipeNext(!0));
                            "toPrev" == E && 300 >= c && (30 > e || !b.shortSwipes ? a.swipeReset() : a.swipePrev(!0));
                            f = 0;
                            if ("auto" == b.slidesPerView) {
                                for (var d = Math.abs(k ? a.getWrapperTranslate("x") : a.getWrapperTranslate("y")), q = g = 0; q < a.slides.length; q++) {
                                    if (m = k ? a.slides[q].getWidth(!0) : a.slides[q].getHeight(!0), g += m, g > d) {
                                        f = m;
                                        break;
                                    }
                                }
                                f > l && (f = l);
                            } else {
                                f = p * b.slidesPerView;
                            }
                            "toNext" == E && 300 < c && (e >= 0.5 * f ? a.swipeNext(!0) : a.swipeReset());
                            "toPrev" == E && 300 < c && (e >= 0.5 * f ? a.swipePrev(!0) : a.swipeReset());
                        }
                    }
                }
                if (b.onTouchEnd) {
                    b.onTouchEnd(a);
                }
                a.callPlugins("onTouchEnd");
            } else {
                a.isMoved = !1;
                if (b.onTouchEnd) {
                    b.onTouchEnd(a);
                }
                a.callPlugins("onTouchEnd");
                a.swipeReset();
            }
        }
    }
    function I(c, d, e) {
        function f() {
            g += m;
            if (p = "toNext" == l ? g > c : g < c) {
                k ? a.setWrapperTranslate(Math.round(g), 0) : a.setWrapperTranslate(0, Math.round(g)), a._DOMAnimating = !0, window.setTimeout(function () {
                    f();
                }, 1000 / 60);
            } else {
                if (b.onSlideChangeEnd) {
                    b.onSlideChangeEnd(a);
                }
                k ? a.setWrapperTranslate(c, 0) : a.setWrapperTranslate(0, c);
                a._DOMAnimating = !1;
            }
        }
        if (a.support.transitions || !b.DOMAnimation) {
            k ? a.setWrapperTranslate(c, 0, 0) : a.setWrapperTranslate(0, c, 0);
            var h = "to" == d && 0 <= e.speed ? e.speed : b.speed;
            a.setWrapperTransition(h);
        } else {
            var g = k ? a.getWrapperTranslate("x") : a.getWrapperTranslate("y"),
                h = "to" == d && 0 <= e.speed ? e.speed : b.speed,
                m = Math.ceil((c - g) / h * (1000 / 60)),
                l = g > c ? "toNext" : "toPrev",
                p = "toNext" == l ? g > c : g < c;
            if (a._DOMAnimating) {
                return;
            }
            f();
        }
        a.updateActiveSlide(c);
        if (b.onSlideNext && "next" == d) {
            b.onSlideNext(a, c);
        }
        if (b.onSlidePrev && "prev" == d) {
            b.onSlidePrev(a, c);
        }
        if (b.onSlideReset && "reset" == d) {
            b.onSlideReset(a, c);
        } ("next" == d || "prev" == d || "to" == d && !0 == e.runCallbacks) && W();
    }
    function W() {
        a.callPlugins("onSlideChangeStart");
        if (b.onSlideChangeStart) {
            if (b.queueStartCallbacks && a.support.transitions) {
                if (a._queueStartCallbacks) {
                    return;
                }
                a._queueStartCallbacks = !0;
                b.onSlideChangeStart(a);
                a.wrapperTransitionEnd(function () {
                    a._queueStartCallbacks = !1;
                });
            } else {
                b.onSlideChangeStart(a);
            }
        }
        b.onSlideChangeEnd && (a.support.transitions ? b.queueEndCallbacks ? a._queueEndCallbacks || (a._queueEndCallbacks = !0, a.wrapperTransitionEnd(b.onSlideChangeEnd)) : a.wrapperTransitionEnd(b.onSlideChangeEnd) : b.DOMAnimation || setTimeout(function () {
            b.onSlideChangeEnd(a);
        }, 10));
    }
    function U() {
        for (var b = a.paginationButtons, d = 0; d < b.length; d++) {
            a.h.removeEventListener(b[d], "click", V, !1);
        }
    }
    function V(b) {
        var d;
        b = b.target || b.srcElement;
        for (var e = a.paginationButtons, f = 0; f < e.length; f++) {
            b === e[f] && (d = f);
        }
        a.swipeTo(d);
    }
    if (document.body.__defineGetter__ && HTMLElement) {
        var s = HTMLElement.prototype;
        s.__defineGetter__ && s.__defineGetter__("outerHTML", function () {
            return (new XMLSerializer).serializeToString(this);
        });
    }
    window.getComputedStyle || (window.getComputedStyle = function (a, b) {
        this.el = a;
        this.getPropertyValue = function (b) {
            var d = /(\-([a-z]){1})/g;
            "float" === b && (b = "styleFloat");
            d.test(b) && (b = b.replace(d, function (a, b, c) {
                return c.toUpperCase();
            }));
            return a.currentStyle[b] ? a.currentStyle[b] : null;
        };
        return this;
    });
    Array.prototype.indexOf || (Array.prototype.indexOf = function (a, b) {
        for (var e = b || 0, f = this.length; e < f; e++) {
            if (this[e] === a) {
                return e;
            }
        }
        return -1;
    });
    if ((document.querySelectorAll || window.jQuery) && "undefined" !== typeof f && (f.nodeType || 0 !== g(f).length)) {
        var a = this;
        a.touches = {
            start: 0,
            startX: 0,
            startY: 0,
            current: 0,
            currentX: 0,
            currentY: 0,
            diff: 0,
            abs: 0
        };
        a.positions = {
            start: 0,
            abs: 0,
            diff: 0,
            current: 0
        };
        a.times = {
            start: 0,
            end: 0
        };
        a.id = (new Date).getTime();
        a.container = f.nodeType ? f : g(f)[0];
        a.isTouched = !1;
        a.isMoved = !1;
        a.activeIndex = 0;
        a.activeLoaderIndex = 0;
        a.activeLoopIndex = 0;
        a.previousIndex = null;
        a.velocity = 0;
        a.snapGrid = [];
        a.slidesGrid = [];
        a.imagesToLoad = [];
        a.imagesLoaded = 0;
        a.wrapperLeft = 0;
        a.wrapperRight = 0;
        a.wrapperTop = 0;
        a.wrapperBottom = 0;
        var J, p, y, E, x, l, s = {
            mode: "horizontal",
            touchRatio: 1,
            speed: 300,
            freeMode: !1,
            freeModeFluid: !1,
            momentumRatio: 1,
            momentumBounce: !0,
            momentumBounceRatio: 1,
            slidesPerView: 1,
            slidesPerGroup: 1,
            simulateTouch: !0,
            followFinger: !0,
            shortSwipes: !0,
            moveStartThreshold: !1,
            autoplay: !1,
            onlyExternal: !1,
            createPagination: !0,
            pagination: !1,
            paginationElement: "span",
            paginationClickable: !1,
            paginationAsRange: !0,
            resistance: !0,
            scrollContainer: !1,
            preventLinks: !0,
            noSwiping: !1,
            noSwipingClass: "swiper-no-swiping",
            initialSlide: 0,
            keyboardControl: !1,
            mousewheelControl: !1,
            mousewheelDebounce: 600,
            useCSS3Transforms: !0,
            loop: !1,
            loopAdditionalSlides: 0,
            calculateHeight: !1,
            updateOnImagesReady: !0,
            releaseFormElements: !0,
            watchActiveIndex: !1,
            visibilityFullFit: !1,
            offsetPxBefore: 0,
            offsetPxAfter: 0,
            offsetSlidesBefore: 0,
            offsetSlidesAfter: 0,
            centeredSlides: !1,
            queueStartCallbacks: !1,
            queueEndCallbacks: !1,
            autoResize: !0,
            resizeReInit: !1,
            DOMAnimation: !0,
            loader: {
                slides: [],
                slidesHTMLType: "inner",
                surroundGroups: 1,
                logic: "reload",
                loadAllSlides: !1
            },
            slideElement: "div",
            slideClass: "swiper-slide",
            slideActiveClass: "swiper-slide-active",
            slideVisibleClass: "swiper-slide-visible",
            wrapperClass: "swiper-wrapper",
            paginationElementClass: "swiper-pagination-switch",
            paginationActiveClass: "swiper-active-switch",
            paginationVisibleClass: "swiper-visible-switch"
        };
        b = b || {};
        for (var q in s) {
            if (q in b && "object" === typeof b[q]) {
                for (var C in s[q]) {
                    C in b[q] || (b[q][C] = s[q][C]);
                }
            } else {
                q in b || (b[q] = s[q]);
            }
        }
        a.params = b;
        b.scrollContainer && (b.freeMode = !0, b.freeModeFluid = !0);
        b.loop && (b.resistance = "100%");
        var k = "horizontal" === b.mode;
        a.touchEvents = {
            touchStart: a.support.touch || !b.simulateTouch ? "touchstart" : a.browser.ie10 ? "MSPointerDown" : "mousedown",
            touchMove: a.support.touch || !b.simulateTouch ? "touchmove" : a.browser.ie10 ? "MSPointerMove" : "mousemove",
            touchEnd: a.support.touch || !b.simulateTouch ? "touchend" : a.browser.ie10 ? "MSPointerUp" : "mouseup"
        };
        for (q = a.container.childNodes.length - 1; 0 <= q; q--) {
            if (a.container.childNodes[q].className) {
                for (C = a.container.childNodes[q].className.split(" "), s = 0; s < C.length; s++) {
                    C[s] === b.wrapperClass && (J = a.container.childNodes[q]);
                }
            }
        }
        a.wrapper = J;
        a._extendSwiperSlide = function (c) {
            c.append = function () {
                b.loop ? (c.insertAfter(a.slides.length - a.loopedSlides), a.removeLoopedSlides(), a.calcSlides(), a.createLoop()) : a.wrapper.appendChild(c);
                a.reInit();
                return c;
            };
            c.prepend = function () {
                b.loop ? (a.wrapper.insertBefore(c, a.slides[a.loopedSlides]), a.removeLoopedSlides(), a.calcSlides(), a.createLoop()) : a.wrapper.insertBefore(c, a.wrapper.firstChild);
                a.reInit();
                return c;
            };
            c.insertAfter = function (d) {
                if ("undefined" === typeof d) {
                    return !1;
                }
                b.loop ? (d = a.slides[d + 1 + a.loopedSlides], a.wrapper.insertBefore(c, d), a.removeLoopedSlides(), a.calcSlides(), a.createLoop()) : (d = a.slides[d + 1], a.wrapper.insertBefore(c, d));
                a.reInit();
                return c;
            };
            c.clone = function () {
                return a._extendSwiperSlide(c.cloneNode(!0));
            };
            c.remove = function () {
                a.wrapper.removeChild(c);
                a.reInit();
            };
            c.html = function (a) {
                if ("undefined" === typeof a) {
                    return c.innerHTML;
                }
                c.innerHTML = a;
                return c;
            };
            c.index = function () {
                for (var b, e = a.slides.length - 1; 0 <= e; e--) {
                    c === a.slides[e] && (b = e);
                }
                return b;
            };
            c.isActive = function () {
                return c.index() === a.activeIndex ? !0 : !1;
            };
            c.swiperSlideDataStorage || (c.swiperSlideDataStorage = {});
            c.getData = function (a) {
                return c.swiperSlideDataStorage[a];
            };
            c.setData = function (a, b) {
                c.swiperSlideDataStorage[a] = b;
                return c;
            };
            c.data = function (a, b) {
                return b ? (c.setAttribute("data-" + a, b), c) : c.getAttribute("data-" + a);
            };
            c.getWidth = function (b) {
                return a.h.getWidth(c, b);
            };
            c.getHeight = function (b) {
                return a.h.getHeight(c, b);
            };
            c.getOffset = function () {
                return a.h.getOffset(c);
            };
            return c;
        };
        a.calcSlides = function (c) {
            var d = a.slides ? a.slides.length : !1;
            a.slides = [];
            a.displaySlides = [];
            for (var e = 0; e < a.wrapper.childNodes.length; e++) {
                if (a.wrapper.childNodes[e].className) {
                    for (var f = a.wrapper.childNodes[e].className.split(" "), g = 0; g < f.length; g++) {
                        f[g] === b.slideClass && a.slides.push(a.wrapper.childNodes[e]);
                    }
                }
            }
            for (e = a.slides.length - 1; 0 <= e; e--) {
                a._extendSwiperSlide(a.slides[e]);
            }
            d && (d !== a.slides.length || c) && (v(), t(), a.updateActiveSlide(), b.createPagination && a.params.pagination && a.createPagination(), a.callPlugins("numberOfSlidesChanged"));
        };
        a.createSlide = function (c, d, e) {
            d = d || a.params.slideClass;
            e = e || b.slideElement;
            e = document.createElement(e);
            e.innerHTML = c || "";
            e.className = d;
            return a._extendSwiperSlide(e);
        };
        a.appendSlide = function (b, d, e) {
            if (b) {
                return b.nodeType ? a._extendSwiperSlide(b).append() : a.createSlide(b, d, e).append();
            }
        };
        a.prependSlide = function (b, d, e) {
            if (b) {
                return b.nodeType ? a._extendSwiperSlide(b).prepend() : a.createSlide(b, d, e).prepend();
            }
        };
        a.insertSlideAfter = function (b, d, e, f) {
            return "undefined" === typeof b ? !1 : d.nodeType ? a._extendSwiperSlide(d).insertAfter(b) : a.createSlide(d, e, f).insertAfter(b);
        };
        a.removeSlide = function (c) {
            if (a.slides[c]) {
                if (b.loop) {
                    if (!a.slides[c + a.loopedSlides]) {
                        return !1;
                    }
                    a.slides[c + a.loopedSlides].remove();
                    a.removeLoopedSlides();
                    a.calcSlides();
                    a.createLoop();
                } else {
                    a.slides[c].remove();
                }
                return !0;
            }
            return !1;
        };
        a.removeLastSlide = function () {
            return 0 < a.slides.length ? (b.loop ? (a.slides[a.slides.length - 1 - a.loopedSlides].remove(), a.removeLoopedSlides(), a.calcSlides(), a.createLoop()) : a.slides[a.slides.length - 1].remove(), !0) : !1;
        };
        a.removeAllSlides = function () {
            for (var b = a.slides.length - 1; 0 <= b; b--) {
                a.slides[b].remove();
            }
        };
        a.getSlide = function (b) {
            return a.slides[b];
        };
        a.getLastSlide = function () {
            return a.slides[a.slides.length - 1];
        };
        a.getFirstSlide = function () {
            return a.slides[0];
        };
        a.activeSlide = function () {
            return a.slides[a.activeIndex];
        };
        var K = [],
            L;
        for (L in a.plugins) {
            b[L] && (q = a.plugins[L](a, b[L])) && K.push(q);
        }
        a.callPlugins = function (a, b) {
            b || (b = {});
            for (var e = 0; e < K.length; e++) {
                if (a in K[e]) {
                    K[e][a](b);
                }
            }
        };
        a.browser.ie10 && !b.onlyExternal && (k ? a.wrapper.classList.add("swiper-wp8-horizontal") : a.wrapper.classList.add("swiper-wp8-vertical"));
        b.freeMode && (a.container.className += " swiper-free-mode");
        a.initialized = !1;
        a.init = function (c, d) {
            var e = a.h.getWidth(a.container),
                f = a.h.getHeight(a.container);
            if (e !== a.width || f !== a.height || c) {
                a.width = e;
                a.height = f;
                l = k ? e : f;
                e = a.wrapper;
                c && a.calcSlides(d);
                if ("auto" === b.slidesPerView) {
                    var g = 0,
                        h = 0;
                    0 < b.slidesOffset && (e.style.paddingLeft = "", e.style.paddingRight = "", e.style.paddingTop = "", e.style.paddingBottom = "");
                    e.style.width = "";
                    e.style.height = "";
                    0 < b.offsetPxBefore && (k ? a.wrapperLeft = b.offsetPxBefore : a.wrapperTop = b.offsetPxBefore);
                    0 < b.offsetPxAfter && (k ? a.wrapperRight = b.offsetPxAfter : a.wrapperBottom = b.offsetPxAfter);
                    b.centeredSlides && (k ? (a.wrapperLeft = (l - this.slides[0].getWidth(!0)) / 2, a.wrapperRight = (l - a.slides[a.slides.length - 1].getWidth(!0)) / 2) : (a.wrapperTop = (l - a.slides[0].getHeight(!0)) / 2, a.wrapperBottom = (l - a.slides[a.slides.length - 1].getHeight(!0)) / 2));
                    k ? (0 <= a.wrapperLeft && (e.style.paddingLeft = a.wrapperLeft + "px"), 0 <= a.wrapperRight && (e.style.paddingRight = a.wrapperRight + "px")) : (0 <= a.wrapperTop && (e.style.paddingTop = a.wrapperTop + "px"), 0 <= a.wrapperBottom && (e.style.paddingBottom = a.wrapperBottom + "px"));
                    var m = 0,
                        q = 0;
                    a.snapGrid = [];
                    a.slidesGrid = [];
                    for (var n = 0, r = 0; r < a.slides.length; r++) {
                        var f = a.slides[r].getWidth(!0),
                            s = a.slides[r].getHeight(!0);
                        b.calculateHeight && (n = Math.max(n, s));
                        var t = k ? f : s;
                        if (b.centeredSlides) {
                            var u = r === a.slides.length - 1 ? 0 : a.slides[r + 1].getWidth(!0),
                                w = r === a.slides.length - 1 ? 0 : a.slides[r + 1].getHeight(!0),
                                u = k ? u : w;
                            if (t > l) {
                                for (w = 0; w <= Math.floor(t / (l + a.wrapperLeft)) ; w++) {
                                    0 === w ? a.snapGrid.push(m + a.wrapperLeft) : a.snapGrid.push(m + a.wrapperLeft + l * w);
                                }
                                a.slidesGrid.push(m + a.wrapperLeft);
                            } else {
                                a.snapGrid.push(q), a.slidesGrid.push(q);
                            }
                            q += t / 2 + u / 2;
                        } else {
                            if (t > l) {
                                for (w = 0; w <= Math.floor(t / l) ; w++) {
                                    a.snapGrid.push(m + l * w);
                                }
                            } else {
                                a.snapGrid.push(m);
                            }
                            a.slidesGrid.push(m);
                        }
                        m += t;
                        g += f;
                        h += s;
                    }
                    b.calculateHeight && (a.height = n);
                    k ? (y = g + a.wrapperRight + a.wrapperLeft, e.style.width = g + "px", e.style.height = a.height + "px") : (y = h + a.wrapperTop + a.wrapperBottom, e.style.width = a.width + "px", e.style.height = h + "px");
                } else {
                    if (b.scrollContainer) {
                        e.style.width = "", e.style.height = "", n = a.slides[0].getWidth(!0), g = a.slides[0].getHeight(!0), y = k ? n : g, e.style.width = n + "px", e.style.height = g + "px", p = k ? n : g;
                    } else {
                        if (b.calculateHeight) {
                            g = n = 0;
                            k || (a.container.style.height = "");
                            e.style.height = "";
                            for (r = 0; r < a.slides.length; r++) {
                                a.slides[r].style.height = "", n = Math.max(a.slides[r].getHeight(!0), n), k || (g += a.slides[r].getHeight(!0));
                            }
                            s = n;
                            a.height = s;
                            k ? g = s : (l = s, a.container.style.height = l + "px");
                        } else {
                            s = k ? a.height : a.height / b.slidesPerView, g = k ? a.height : a.slides.length * s;
                        }
                        f = k ? a.width / b.slidesPerView : a.width;
                        n = k ? a.slides.length * f : a.width;
                        p = k ? f : s;
                        0 < b.offsetSlidesBefore && (k ? a.wrapperLeft = p * b.offsetSlidesBefore : a.wrapperTop = p * b.offsetSlidesBefore);
                        0 < b.offsetSlidesAfter && (k ? a.wrapperRight = p * b.offsetSlidesAfter : a.wrapperBottom = p * b.offsetSlidesAfter);
                        0 < b.offsetPxBefore && (k ? a.wrapperLeft = b.offsetPxBefore : a.wrapperTop = b.offsetPxBefore);
                        0 < b.offsetPxAfter && (k ? a.wrapperRight = b.offsetPxAfter : a.wrapperBottom = b.offsetPxAfter);
                        b.centeredSlides && (k ? (a.wrapperLeft = (l - p) / 2, a.wrapperRight = (l - p) / 2) : (a.wrapperTop = (l - p) / 2, a.wrapperBottom = (l - p) / 2));
                        k ? (0 < a.wrapperLeft && (e.style.paddingLeft = a.wrapperLeft + "px"), 0 < a.wrapperRight && (e.style.paddingRight = a.wrapperRight + "px")) : (0 < a.wrapperTop && (e.style.paddingTop = a.wrapperTop + "px"), 0 < a.wrapperBottom && (e.style.paddingBottom = a.wrapperBottom + "px"));
                        y = k ? n + a.wrapperRight + a.wrapperLeft : g + a.wrapperTop + a.wrapperBottom;
                        e.style.width = n + "px";
                        e.style.height = g + "px";
                        m = 0;
                        a.snapGrid = [];
                        a.slidesGrid = [];
                        for (r = 0; r < a.slides.length; r++) {
                            a.snapGrid.push(m), a.slidesGrid.push(m), m += p, a.slides[r].style.width = f + "px", a.slides[r].style.height = s + "px";
                        }
                    }
                }
                if (a.initialized) {
                    if (a.callPlugins("onInit"), b.onFirstInit) {
                        b.onInit(a);
                    }
                } else {
                    if (a.callPlugins("onFirstInit"), b.onFirstInit) {
                        b.onFirstInit(a);
                    }
                }
                a.initialized = !0;
            }
        };
        a.reInit = function (b) {
            a.init(!0, b);
        };
        a.resizeFix = function (c) {
            a.callPlugins("beforeResizeFix");
            a.init(b.resizeReInit || c);
            if (!b.freeMode) {
                b.loop ? a.swipeTo(a.activeLoopIndex, 0, !1) : a.swipeTo(a.activeIndex, 0, !1);
            } else {
                if ((k ? a.getWrapperTranslate("x") : a.getWrapperTranslate("y")) < -h()) {
                    c = k ? -h() : 0;
                    var d = k ? 0 : -h();
                    a.setWrapperTransition(0);
                    a.setWrapperTranslate(c, d, 0);
                }
            }
            a.callPlugins("afterResizeFix");
        };
        a.destroy = function (c) {
            a.browser.ie10 ? (a.h.removeEventListener(a.wrapper, a.touchEvents.touchStart, z, !1), a.h.removeEventListener(document, a.touchEvents.touchMove, A, !1), a.h.removeEventListener(document, a.touchEvents.touchEnd, B, !1)) : (a.support.touch && (a.h.removeEventListener(a.wrapper, "touchstart", z, !1), a.h.removeEventListener(a.wrapper, "touchmove", A, !1), a.h.removeEventListener(a.wrapper, "touchend", B, !1)), b.simulateTouch && (a.h.removeEventListener(a.wrapper, "mousedown", z, !1), a.h.removeEventListener(document, "mousemove", A, !1), a.h.removeEventListener(document, "mouseup", B, !1)));
            b.autoResize && a.h.removeEventListener(window, "resize", a.resizeFix, !1);
            v();
            b.paginationClickable && U();
            b.mousewheelControl && a._wheelEvent && a.h.removeEventListener(a.container, a._wheelEvent, N, !1);
            b.keyboardControl && a.h.removeEventListener(document, "keydown", O, !1);
            b.autoplay && a.stopAutoplay();
            a.callPlugins("onDestroy");
            a = null;
        };
        b.grabCursor && (a.container.style.cursor = "move", a.container.style.cursor = "grab", a.container.style.cursor = "-moz-grab", a.container.style.cursor = "-webkit-grab");
        a.allowSlideClick = !0;
        a.allowLinks = !0;
        var u = !1,
            M, G = !0,
            D, H;
        a.swipeNext = function (c) {
            !c && b.loop && a.fixLoop();
            a.callPlugins("onSwipeNext");
            var d = c = k ? a.getWrapperTranslate("x") : a.getWrapperTranslate("y");
            if ("auto" == b.slidesPerView) {
                for (var e = 0; e < a.snapGrid.length; e++) {
                    if (-c >= a.snapGrid[e] && -c < a.snapGrid[e + 1]) {
                        d = -a.snapGrid[e + 1];
                        break;
                    }
                }
            } else {
                d = p * b.slidesPerGroup, d = -(Math.floor(Math.abs(c) / Math.floor(d)) * d + d);
            }
            d < -h() && (d = -h());
            if (d == c) {
                return !1;
            }
            I(d, "next");
            return !0;
        };
        a.swipePrev = function (c) {
            !c && b.loop && a.fixLoop();
            !c && b.autoplay && a.stopAutoplay();
            a.callPlugins("onSwipePrev");
            c = Math.ceil(k ? a.getWrapperTranslate("x") : a.getWrapperTranslate("y"));
            var d;
            if ("auto" == b.slidesPerView) {
                d = 0;
                for (var e = 1; e < a.snapGrid.length; e++) {
                    if (-c == a.snapGrid[e]) {
                        d = -a.snapGrid[e - 1];
                        break;
                    }
                    if (-c > a.snapGrid[e] && -c < a.snapGrid[e + 1]) {
                        d = -a.snapGrid[e];
                        break;
                    }
                }
            } else {
                d = p * b.slidesPerGroup, d *= -(Math.ceil(-c / d) - 1);
            }
            0 < d && (d = 0);
            if (d == c) {
                return !1;
            }
            I(d, "prev");
            return !0;
        };
        a.swipeReset = function () {
            a.callPlugins("onSwipeReset");
            var c = k ? a.getWrapperTranslate("x") : a.getWrapperTranslate("y"),
                d = p * b.slidesPerGroup;
            h();
            if ("auto" == b.slidesPerView) {
                for (var e = d = 0; e < a.snapGrid.length; e++) {
                    if (-c === a.snapGrid[e]) {
                        return;
                    }
                    if (-c >= a.snapGrid[e] && -c < a.snapGrid[e + 1]) {
                        d = 0 < a.positions.diff ? -a.snapGrid[e + 1] : -a.snapGrid[e];
                        break;
                    }
                } -c >= a.snapGrid[a.snapGrid.length - 1] && (d = -a.snapGrid[a.snapGrid.length - 1]);
                c <= -h() && (d = -h());
            } else {
                d = 0 > c ? Math.round(c / d) * d : 0;
            }
            b.scrollContainer && (d = 0 > c ? c : 0);
            d < -h() && (d = -h());
            b.scrollContainer && l > p && (d = 0);
            if (d == c) {
                return !1;
            }
            I(d, "reset");
            return !0;
        };
        a.swipeTo = function (c, d, e) {
            c = parseInt(c, 10);
            a.callPlugins("onSwipeTo", {
                index: c,
                speed: d
            });
            b.loop && (c += a.loopedSlides);
            var f = k ? a.getWrapperTranslate("x") : a.getWrapperTranslate("y");
            if (!(c > a.slides.length - 1 || 0 > c)) {
                var g;
                g = "auto" == b.slidesPerView ? -a.slidesGrid[c] : -c * p;
                g < -h() && (g = -h());
                if (g == f) {
                    return !1;
                }
                I(g, "to", {
                    index: c,
                    speed: d,
                    runCallbacks: !1 === e ? !1 : !0
                });
                return !0;
            }
        };
        a._queueStartCallbacks = !1;
        a._queueEndCallbacks = !1;
        a.updateActiveSlide = function (c) {
            if (a.initialized && 0 != a.slides.length) {
                a.previousIndex = a.activeIndex;
                0 < c && (c = 0);
                "undefined" == typeof c && (c = k ? a.getWrapperTranslate("x") : a.getWrapperTranslate("y"));
                if ("auto" == b.slidesPerView) {
                    if (a.activeIndex = a.slidesGrid.indexOf(-c), 0 > a.activeIndex) {
                        for (var d = 0; d < a.slidesGrid.length - 1 && !(-c > a.slidesGrid[d] && -c < a.slidesGrid[d + 1]) ; d++) { }
                        var e = Math.abs(a.slidesGrid[d] + c),
                            f = Math.abs(a.slidesGrid[d + 1] + c);
                        a.activeIndex = e <= f ? d : d + 1;
                    }
                } else {
                    a.activeIndex = b.visibilityFullFit ? Math.ceil(-c / p) : Math.round(-c / p);
                }
                a.activeIndex == a.slides.length && (a.activeIndex = a.slides.length - 1);
                0 > a.activeIndex && (a.activeIndex = 0);
                if (a.slides[a.activeIndex]) {
                    a.calcVisibleSlides(c);
                    e = RegExp("\\s*" + b.slideActiveClass);
                    f = RegExp("\\s*" + b.slideVisibleClass);
                    for (d = 0; d < a.slides.length; d++) {
                        a.slides[d].className = a.slides[d].className.replace(e, "").replace(f, ""), 0 <= a.visibleSlides.indexOf(a.slides[d]) && (a.slides[d].className += " " + b.slideVisibleClass);
                    }
                    a.slides[a.activeIndex].className += " " + b.slideActiveClass;
                    b.loop ? (d = a.loopedSlides, a.activeLoopIndex = a.activeIndex - d, a.activeLoopIndex >= a.slides.length - 2 * d && (a.activeLoopIndex = a.slides.length - 2 * d - a.activeLoopIndex), 0 > a.activeLoopIndex && (a.activeLoopIndex = a.slides.length - 2 * d + a.activeLoopIndex)) : a.activeLoopIndex = a.activeIndex;
                    b.pagination && a.updatePagination(c);
                }
            }
        };
        a.createPagination = function (c) {
            b.paginationClickable && a.paginationButtons && U();
            var d = "",
                e = a.slides.length;
            b.loop && (e -= 2 * a.loopedSlides);
            for (var f = 0; f < e; f++) {
                d += "<" + b.paginationElement + ' class="' + b.paginationElementClass + '"></' + b.paginationElement + ">";
            }
            a.paginationContainer = b.pagination.nodeType ? b.pagination : g(b.pagination)[0];
            a.paginationContainer.innerHTML = d;
            a.paginationButtons = [];
            document.querySelectorAll ? a.paginationButtons = a.paginationContainer.querySelectorAll("." + b.paginationElementClass) : window.jQuery && (a.paginationButtons = g(a.paginationContainer).find("." + b.paginationElementClass));
            c || a.updatePagination();
            a.callPlugins("onCreatePagination");
            if (b.paginationClickable) {
                for (c = a.paginationButtons, d = 0; d < c.length; d++) {
                    a.h.addEventListener(c[d], "click", V, !1);
                }
            }
        };
        a.updatePagination = function (c) {
            if (b.pagination && !(1 > a.slides.length)) {
                if (document.querySelectorAll) {
                    var d = a.paginationContainer.querySelectorAll("." + b.paginationActiveClass);
                } else {
                    window.jQuery && (d = g(a.paginationContainer).find("." + b.paginationActiveClass));
                }
                if (d && (d = a.paginationButtons, 0 != d.length)) {
                    for (var e = 0; e < d.length; e++) {
                        d[e].className = b.paginationElementClass;
                    }
                    var f = b.loop ? a.loopedSlides : 0;
                    if (b.paginationAsRange) {
                        a.visibleSlides || a.calcVisibleSlides(c);
                        c = [];
                        for (e = 0; e < a.visibleSlides.length; e++) {
                            var h = a.slides.indexOf(a.visibleSlides[e]) - f;
                            b.loop && 0 > h && (h = a.slides.length - 2 * a.loopedSlides + h);
                            b.loop && h >= a.slides.length - 2 * a.loopedSlides && (h = a.slides.length - 2 * a.loopedSlides - h, h = Math.abs(h));
                            c.push(h);
                        }
                        for (e = 0; e < c.length; e++) {
                            d[c[e]] && (d[c[e]].className += " " + b.paginationVisibleClass);
                        }
                        b.loop ? d[a.activeLoopIndex].className += " " + b.paginationActiveClass : d[a.activeIndex].className += " " + b.paginationActiveClass;
                    } else {
                        b.loop ? d[a.activeLoopIndex].className += " " + b.paginationActiveClass + " " + b.paginationVisibleClass : d[a.activeIndex].className += " " + b.paginationActiveClass + " " + b.paginationVisibleClass;
                    }
                }
            }
        };
        a.calcVisibleSlides = function (c) {
            var d = [],
                e = 0,
                f = 0,
                g = 0;
            k && 0 < a.wrapperLeft && (c += a.wrapperLeft);
            !k && 0 < a.wrapperTop && (c += a.wrapperTop);
            for (var h = 0; h < a.slides.length; h++) {
                var e = e + f,
                    f = "auto" == b.slidesPerView ? k ? a.h.getWidth(a.slides[h], !0) : a.h.getHeight(a.slides[h], !0) : p,
                    g = e + f,
                    m = !1;
                b.visibilityFullFit ? (e >= -c && g <= -c + l && (m = !0), e <= -c && g >= -c + l && (m = !0)) : (g > -c && g <= -c + l && (m = !0), e >= -c && e < -c + l && (m = !0), e < -c && g > -c + l && (m = !0));
                m && d.push(a.slides[h]);
            }
            0 == d.length && (d = [a.slides[a.activeIndex]]);
            a.visibleSlides = d;
        };
        a.autoPlayIntervalId = void 0;
        a.startAutoplay = function () {
            if ("undefined" !== typeof a.autoPlayIntervalId) {
                return !1;
            }
            b.autoplay && !b.loop && (a.autoPlayIntervalId = setInterval(function () {
                a.swipeNext(!0) || a.swipeTo(0);
            }, b.autoplay));
            b.autoplay && b.loop && (a.autoPlayIntervalId = setInterval(function () {
                a.swipeNext();
            }, b.autoplay));
            a.callPlugins("onAutoplayStart");
        };
        a.stopAutoplay = function () {
            a.autoPlayIntervalId && clearInterval(a.autoPlayIntervalId);
            a.autoPlayIntervalId = void 0;
            a.callPlugins("onAutoplayStop");
        };
        a.loopCreated = !1;
        a.removeLoopedSlides = function () {
            if (a.loopCreated) {
                for (var b = 0; b < a.slides.length; b++) {
                    !0 === a.slides[b].getData("looped") && a.wrapper.removeChild(a.slides[b]);
                }
            }
        };
        a.createLoop = function () {
            if (0 != a.slides.length) {
                a.loopedSlides = b.slidesPerView + b.loopAdditionalSlides;
                for (var c = "", d = "", e = 0; e < a.loopedSlides; e++) {
                    c += a.slides[e].outerHTML;
                }
                for (e = a.slides.length - a.loopedSlides; e < a.slides.length; e++) {
                    d += a.slides[e].outerHTML;
                }
                J.innerHTML = d + J.innerHTML + c;
                a.loopCreated = !0;
                a.calcSlides();
                for (e = 0; e < a.slides.length; e++) {
                    (e < a.loopedSlides || e >= a.slides.length - a.loopedSlides) && a.slides[e].setData("looped", !0);
                }
                a.callPlugins("onCreateLoop");
            }
        };
        a.fixLoop = function () {
            if (a.activeIndex < a.loopedSlides) {
                var c = a.slides.length - 3 * a.loopedSlides + a.activeIndex;
                a.swipeTo(c, 0, !1);
            } else {
                a.activeIndex > a.slides.length - 2 * b.slidesPerView && (c = -a.slides.length + a.activeIndex + a.loopedSlides, a.swipeTo(c, 0, !1));
            }
        };
        a.loadSlides = function () {
            var c = "";
            a.activeLoaderIndex = 0;
            for (var d = b.loader.slides, e = b.loader.loadAllSlides ? d.length : b.slidesPerView * (1 + b.loader.surroundGroups), f = 0; f < e; f++) {
                c = "outer" == b.loader.slidesHTMLType ? c + d[f] : c + ("<" + b.slideElement + ' class="' + b.slideClass + '" data-swiperindex="' + f + '">' + d[f] + "</" + b.slideElement + ">");
            }
            a.wrapper.innerHTML = c;
            a.calcSlides(!0);
            b.loader.loadAllSlides || a.wrapperTransitionEnd(a.reloadSlides, !0);
        };
        a.reloadSlides = function () {
            var c = b.loader.slides,
                d = parseInt(a.activeSlide().data("swiperindex"), 10);
            if (!(0 > d || d > c.length - 1)) {
                a.activeLoaderIndex = d;
                var e = Math.max(0, d - b.slidesPerView * b.loader.surroundGroups),
                    f = Math.min(d + b.slidesPerView * (1 + b.loader.surroundGroups) - 1, c.length - 1);
                0 < d && (d = -p * (d - e), k ? a.setWrapperTranslate(d, 0, 0) : a.setWrapperTranslate(0, d, 0), a.setWrapperTransition(0));
                if ("reload" === b.loader.logic) {
                    for (var g = a.wrapper.innerHTML = "", d = e; d <= f; d++) {
                        g += "outer" == b.loader.slidesHTMLType ? c[d] : "<" + b.slideElement + ' class="' + b.slideClass + '" data-swiperindex="' + d + '">' + c[d] + "</" + b.slideElement + ">";
                    }
                    a.wrapper.innerHTML = g;
                } else {
                    for (var g = 1000, h = 0, d = 0; d < a.slides.length; d++) {
                        var l = a.slides[d].data("swiperindex");
                        l < e || l > f ? a.wrapper.removeChild(a.slides[d]) : (g = Math.min(l, g), h = Math.max(l, h));
                    }
                    for (d = e; d <= f; d++) {
                        d < g && (e = document.createElement(b.slideElement), e.className = b.slideClass, e.setAttribute("data-swiperindex", d), e.innerHTML = c[d], a.wrapper.insertBefore(e, a.wrapper.firstChild)), d > h && (e = document.createElement(b.slideElement), e.className = b.slideClass, e.setAttribute("data-swiperindex", d), e.innerHTML = c[d], a.wrapper.appendChild(e));
                    }
                }
                a.reInit(!0);
            }
        };
        a.calcSlides();
        0 < b.loader.slides.length && 0 == a.slides.length && a.loadSlides();
        b.loop && a.createLoop();
        a.init();
        n();
        b.pagination && b.createPagination && a.createPagination(!0);
        b.loop || 0 < b.initialSlide ? a.swipeTo(b.initialSlide, 0, !1) : a.updateActiveSlide(0);
        b.autoplay && a.startAutoplay();
    }
};
Swiper.prototype = {
    plugins: {},
    wrapperTransitionEnd: function (f, b) {
        function g() {
            f(h);
            h.params.queueEndCallbacks && (h._queueEndCallbacks = !1);
            if (!b) {
                for (var v = 0; v < t.length; v++) {
                    h.h.removeEventListener(n, t[v], g, !1);
                }
            }
        }
        var h = this,
			n = h.wrapper,
			t = ["webkitTransitionEnd", "transitionend", "oTransitionEnd", "MSTransitionEnd", "msTransitionEnd"];
        if (f) {
            for (var v = 0; v < t.length; v++) {
                h.h.addEventListener(n, t[v], g, !1);
            }
        }
    },
    getWrapperTranslate: function (f) {
        var b = this.wrapper,
			g, h, n = window.WebKitCSSMatrix ? new WebKitCSSMatrix(window.getComputedStyle(b, null).webkitTransform) : window.getComputedStyle(b, null).MozTransform || window.getComputedStyle(b, null).OTransform || window.getComputedStyle(b, null).MsTransform || window.getComputedStyle(b, null).msTransform || window.getComputedStyle(b, null).transform || window.getComputedStyle(b, null).getPropertyValue("transform").replace("translate(", "matrix(1, 0, 0, 1,");
        g = n.toString().split(",");
        this.params.useCSS3Transforms ? ("x" == f && (h = 16 == g.length ? parseFloat(g[12]) : window.WebKitCSSMatrix ? n.m41 : parseFloat(g[4])), "y" == f && (h = 16 == g.length ? parseFloat(g[13]) : window.WebKitCSSMatrix ? n.m42 : parseFloat(g[5]))) : ("x" == f && (h = parseFloat(b.style.left, 10) || 0), "y" == f && (h = parseFloat(b.style.top, 10) || 0));
        return h || 0;
    },
    setWrapperTranslate: function (f, b, g) {
        var h = this.wrapper.style;
        f = f || 0;
        b = b || 0;
        g = g || 0;
        this.params.useCSS3Transforms ? this.support.transforms3d ? h.webkitTransform = h.MsTransform = h.msTransform = h.MozTransform = h.OTransform = h.transform = "translate3d(" + f + "px, " + b + "px, " + g + "px)" : (h.webkitTransform = h.MsTransform = h.msTransform = h.MozTransform = h.OTransform = h.transform = "translate(" + f + "px, " + b + "px)", this.support.transforms || (h.left = f + "px", h.top = b + "px")) : (h.left = f + "px", h.top = b + "px");
        this.callPlugins("onSetWrapperTransform", {
            x: f,
            y: b,
            z: g
        });
    },
    setWrapperTransition: function (f) {
        var b = this.wrapper.style;
        b.webkitTransitionDuration = b.MsTransitionDuration = b.msTransitionDuration = b.MozTransitionDuration = b.OTransitionDuration = b.transitionDuration = f / 1000 + "s";
        this.callPlugins("onSetWrapperTransition", {
            duration: f
        });
    },
    h: {
        getWidth: function (f, b) {
            var g = window.getComputedStyle(f, null).getPropertyValue("width"),
				h = parseFloat(g);
            if (isNaN(h) || 0 < g.indexOf("%")) {
                h = f.offsetWidth - parseFloat(window.getComputedStyle(f, null).getPropertyValue("padding-left")) - parseFloat(window.getComputedStyle(f, null).getPropertyValue("padding-right"));
            }
            b && (h += parseFloat(window.getComputedStyle(f, null).getPropertyValue("padding-left")) + parseFloat(window.getComputedStyle(f, null).getPropertyValue("padding-right")));
            return h;
        },
        getHeight: function (f, b) {
            if (b) {
                return f.offsetHeight;
            }
            var g = window.getComputedStyle(f, null).getPropertyValue("height"),
				h = parseFloat(g);
            if (isNaN(h) || 0 < g.indexOf("%")) {
                h = f.offsetHeight - parseFloat(window.getComputedStyle(f, null).getPropertyValue("padding-top")) - parseFloat(window.getComputedStyle(f, null).getPropertyValue("padding-bottom"));
            }
            b && (h += parseFloat(window.getComputedStyle(f, null).getPropertyValue("padding-top")) + parseFloat(window.getComputedStyle(f, null).getPropertyValue("padding-bottom")));
            return h;
        },
        getOffset: function (f) {
            var b = f.getBoundingClientRect(),
				g = document.body,
				h = f.clientTop || g.clientTop || 0,
				g = f.clientLeft || g.clientLeft || 0,
				n = window.pageYOffset || f.scrollTop;
            f = window.pageXOffset || f.scrollLeft;
            document.documentElement && !window.pageYOffset && (n = document.documentElement.scrollTop, f = document.documentElement.scrollLeft);
            return {
                top: b.top + n - h,
                left: b.left + f - g
            };
        },
        windowWidth: function () {
            if (window.innerWidth) {
                return window.innerWidth;
            }
            if (document.documentElement && document.documentElement.clientWidth) {
                return document.documentElement.clientWidth;
            }
        },
        windowHeight: function () {
            if (window.innerHeight) {
                return window.innerHeight;
            }
            if (document.documentElement && document.documentElement.clientHeight) {
                return document.documentElement.clientHeight;
            }
        },
        windowScroll: function () {
            if ("undefined" != typeof pageYOffset) {
                return {
                    left: window.pageXOffset,
                    top: window.pageYOffset
                };
            }
            if (document.documentElement) {
                return {
                    left: document.documentElement.scrollLeft,
                    top: document.documentElement.scrollTop
                };
            }
        },
        addEventListener: function (f, b, g, h) {
            f.addEventListener ? f.addEventListener(b, g, h) : f.attachEvent && f.attachEvent("on" + b, g);
        },
        removeEventListener: function (f, b, g, h) {
            f.removeEventListener ? f.removeEventListener(b, g, h) : f.detachEvent && f.detachEvent("on" + b, g);
        }
    },
    setTransform: function (f, b) {
        var g = f.style;
        g.webkitTransform = g.MsTransform = g.msTransform = g.MozTransform = g.OTransform = g.transform = b;
    },
    setTranslate: function (f, b) {
        var g = f.style,
			h = b.x || 0,
			n = b.y || 0,
			t = b.z || 0;
        g.webkitTransform = g.MsTransform = g.msTransform = g.MozTransform = g.OTransform = g.transform = this.support.transforms3d ? "translate3d(" + h + "px," + n + "px," + t + "px)" : "translate(" + h + "px," + n + "px)";
        this.support.transforms || (g.left = h + "px", g.top = n + "px");
    },
    setTransition: function (f, b) {
        var g = f.style;
        g.webkitTransitionDuration = g.MsTransitionDuration = g.msTransitionDuration = g.MozTransitionDuration = g.OTransitionDuration = g.transitionDuration = b + "ms";
    },
    support: {
        touch: window.Modernizr && !0 === Modernizr.touch ||
		function () {
		    return !!("ontouchstart" in window || window.DocumentTouch && document instanceof DocumentTouch);
		}(),
        transforms3d: window.Modernizr && !0 === Modernizr.csstransforms3d ||
		function () {
		    var f = document.createElement("div");
		    return "webkitPerspective" in f.style || "MozPerspective" in f.style || "OPerspective" in f.style || "MsPerspective" in f.style || "perspective" in f.style;
		}(),
        transforms: window.Modernizr && !0 === Modernizr.csstransforms ||
		function () {
		    var f = document.createElement("div").style;
		    return "transform" in f || "WebkitTransform" in f || "MozTransform" in f || "msTransform" in f || "MsTransform" in f || "OTransform" in f;
		}(),
        transitions: window.Modernizr && !0 === Modernizr.csstransitions ||
		function () {
		    var f = document.createElement("div").style;
		    return "transition" in f || "WebkitTransition" in f || "MozTransition" in f || "msTransition" in f || "MsTransition" in f || "OTransition" in f;
		}()
    },
    browser: {
        ie8: function () {
            var f = -1;
            "Microsoft Internet Explorer" == navigator.appName && null != /MSIE ([0-9]{1,}[.0-9]{0,})/.exec(navigator.userAgent) && (f = parseFloat(RegExp.$1));
            return -1 != f && 9 > f;
        }(),
        ie10: window.navigator.msPointerEnabled
    }
};
(window.jQuery || window.Zepto) &&
function (f) {
    f.fn.swiper = function (b) {
        b = new Swiper(f(this)[0], b);
        f(this).data("swiper", b);
        return b;
    };
}(window.jQuery || window.Zepto);
"undefined" !== typeof module && (module.exports = Swiper);
/*
 * iScroll v4.2.5 ~ Copyright (c) 2012 Matteo Spinelli, http://cubiq.org
 * Released under MIT license, http://cubiq.org/license
 */
(function (window, doc) {
    var m = Math,
		dummyStyle = doc.createElement("div").style,
		vendor = (function () {
		    var vendors = "t,webkitT,MozT,msT,OT".split(","),
				t, i = 0,
				l = vendors.length;
		    for (; i < l; i++) {
		        t = vendors[i] + "ransform";
		        if (t in dummyStyle) {
		            return vendors[i].substr(0, vendors[i].length - 1);
		        }
		    }
		    return false;
		})(),
		cssVendor = vendor ? "-" + vendor.toLowerCase() + "-" : "",
		transform = prefixStyle("transform"),
		transitionProperty = prefixStyle("transitionProperty"),
		transitionDuration = prefixStyle("transitionDuration"),
		transformOrigin = prefixStyle("transformOrigin"),
		transitionTimingFunction = prefixStyle("transitionTimingFunction"),
		transitionDelay = prefixStyle("transitionDelay"),
		isAndroid = (/android/gi).test(navigator.appVersion),
		isIDevice = (/iphone|ipad/gi).test(navigator.appVersion),
		isTouchPad = (/hp-tablet/gi).test(navigator.appVersion),
		has3d = prefixStyle("perspective") in dummyStyle,
		hasTouch = "ontouchstart" in window && !isTouchPad,
		hasTransform = vendor !== false,
		hasTransitionEnd = prefixStyle("transition") in dummyStyle,
		RESIZE_EV = "onorientationchange" in window ? "orientationchange" : "resize",
		START_EV = hasTouch ? "touchstart" : "mousedown",
		MOVE_EV = hasTouch ? "touchmove" : "mousemove",
		END_EV = hasTouch ? "touchend" : "mouseup",
		CANCEL_EV = hasTouch ? "touchcancel" : "mouseup",
		TRNEND_EV = (function () {
		    if (vendor === false) {
		        return false;
		    }
		    var transitionEnd = {
		        "": "transitionend",
		        "webkit": "webkitTransitionEnd",
		        "Moz": "transitionend",
		        "O": "otransitionend",
		        "ms": "MSTransitionEnd"
		    };
		    return transitionEnd[vendor];
		})(),
		nextFrame = (function () {
		    return window.requestAnimationFrame || window.webkitRequestAnimationFrame || window.mozRequestAnimationFrame || window.oRequestAnimationFrame || window.msRequestAnimationFrame ||
			function (callback) {
			    return setTimeout(callback, 1);
			};
		})(),
		cancelFrame = (function () {
		    return window.cancelRequestAnimationFrame || window.webkitCancelAnimationFrame || window.webkitCancelRequestAnimationFrame || window.mozCancelRequestAnimationFrame || window.oCancelRequestAnimationFrame || window.msCancelRequestAnimationFrame || clearTimeout;
		})(),
		translateZ = has3d ? " translateZ(0)" : "",
		iScroll = function (el, options) {
		    var that = this,
				i;
		    that.wrapper = typeof el == "object" ? el : doc.getElementById(el);
		    that.wrapper.style.overflow = "hidden";
		    that.scroller = that.wrapper.children[0];
		    that.options = {
		        hScroll: true,
		        vScroll: true,
		        x: 0,
		        y: 0,
		        bounce: true,
		        bounceLock: false,
		        momentum: true,
		        lockDirection: true,
		        useTransform: true,
		        useTransition: false,
		        topOffset: 0,
		        checkDOMChanges: false,
		        handleClick: true,
		        hScrollbar: true,
		        vScrollbar: true,
		        fixedScrollbar: isAndroid,
		        hideScrollbar: isIDevice,
		        fadeScrollbar: isIDevice && has3d,
		        scrollbarClass: "",
		        zoom: false,
		        zoomMin: 1,
		        zoomMax: 4,
		        doubleTapZoom: 2,
		        wheelAction: "scroll",
		        snap: false,
		        snapThreshold: 1,
		        onRefresh: null,
		        onBeforeScrollStart: function (e) {
		            e.preventDefault();
		        },
		        onScrollStart: null,
		        onBeforeScrollMove: null,
		        onScrollMove: null,
		        onBeforeScrollEnd: null,
		        onScrollEnd: null,
		        onTouchEnd: null,
		        onDestroy: null,
		        onZoomStart: null,
		        onZoom: null,
		        onZoomEnd: null
		    };
		    for (i in options) {
		        that.options[i] = options[i];
		    }
		    that.x = that.options.x;
		    that.y = that.options.y;
		    that.options.useTransform = hasTransform && that.options.useTransform;
		    that.options.hScrollbar = that.options.hScroll && that.options.hScrollbar;
		    that.options.vScrollbar = that.options.vScroll && that.options.vScrollbar;
		    that.options.zoom = that.options.useTransform && that.options.zoom;
		    that.options.useTransition = hasTransitionEnd && that.options.useTransition;
		    if (that.options.zoom && isAndroid) {
		        translateZ = "";
		    }
		    that.scroller.style[transitionProperty] = that.options.useTransform ? cssVendor + "transform" : "top left";
		    that.scroller.style[transitionDuration] = "0";
		    that.scroller.style[transformOrigin] = "0 0";
		    if (that.options.useTransition) {
		        that.scroller.style[transitionTimingFunction] = "cubic-bezier(0.33,0.66,0.66,1)";
		    }
		    if (that.options.useTransform) {
		        that.scroller.style[transform] = "translate(" + that.x + "px," + that.y + "px)" + translateZ;
		    } else {
		        that.scroller.style.cssText += ";position:absolute;top:" + that.y + "px;left:" + that.x + "px";
		    }
		    if (that.options.useTransition) {
		        that.options.fixedScrollbar = true;
		    }
		    that.refresh();
		    that._bind(RESIZE_EV, window);
		    that._bind(START_EV);
		    if (!hasTouch) {
		        if (that.options.wheelAction != "none") {
		            that._bind("DOMMouseScroll");
		            that._bind("mousewheel");
		        }
		    }
		    if (that.options.checkDOMChanges) {
		        that.checkDOMTime = setInterval(function () {
		            that._checkDOMChanges();
		        }, 500);
		    }
		};
    iScroll.prototype = {
        enabled: true,
        x: 0,
        y: 0,
        steps: [],
        scale: 1,
        currPageX: 0,
        currPageY: 0,
        pagesX: [],
        pagesY: [],
        aniTime: null,
        wheelZoomCount: 0,
        handleEvent: function (e) {
            var that = this;
            switch (e.type) {
                case START_EV:
                    if (!hasTouch && e.button !== 0) {
                        return;
                    }
                    that._start(e);
                    break;
                case MOVE_EV:
                    that._move(e);
                    break;
                case END_EV:
                case CANCEL_EV:
                    that._end(e);
                    break;
                case RESIZE_EV:
                    that._resize();
                    break;
                case "DOMMouseScroll":
                case "mousewheel":
                    that._wheel(e);
                    break;
                case TRNEND_EV:
                    that._transitionEnd(e);
                    break;
            }
        },
        _checkDOMChanges: function () {
            if (this.moved || this.zoomed || this.animating || (this.scrollerW == this.scroller.offsetWidth * this.scale && this.scrollerH == this.scroller.offsetHeight * this.scale)) {
                return;
            }
            this.refresh();
        },
        _scrollbar: function (dir) {
            var that = this,
				bar;
            if (!that[dir + "Scrollbar"]) {
                if (that[dir + "ScrollbarWrapper"]) {
                    if (hasTransform) {
                        that[dir + "ScrollbarIndicator"].style[transform] = "";
                    }
                    that[dir + "ScrollbarWrapper"].parentNode.removeChild(that[dir + "ScrollbarWrapper"]);
                    that[dir + "ScrollbarWrapper"] = null;
                    that[dir + "ScrollbarIndicator"] = null;
                }
                return;
            }
            if (!that[dir + "ScrollbarWrapper"]) {
                bar = doc.createElement("div");
                if (that.options.scrollbarClass) {
                    bar.className = that.options.scrollbarClass + dir.toUpperCase();
                } else {
                    bar.style.cssText = "position:absolute;z-index:100;" + (dir == "h" ? "height:7px;bottom:1px;left:2px;right:" + (that.vScrollbar ? "7" : "2") + "px" : "width:7px;bottom:" + (that.hScrollbar ? "7" : "2") + "px;top:2px;right:1px");
                }
                bar.style.cssText += ";pointer-events:none;" + cssVendor + "transition-property:opacity;" + cssVendor + "transition-duration:" + (that.options.fadeScrollbar ? "350ms" : "0") + ";overflow:hidden;opacity:" + (that.options.hideScrollbar ? "0" : "1");
                that.wrapper.appendChild(bar);
                that[dir + "ScrollbarWrapper"] = bar;
                bar = doc.createElement("div");
                if (!that.options.scrollbarClass) {
                    bar.style.cssText = "position:absolute;z-index:100;background:rgba(0,0,0,0.5);border:1px solid rgba(255,255,255,0.9);" + cssVendor + "background-clip:padding-box;" + cssVendor + "box-sizing:border-box;" + (dir == "h" ? "height:100%" : "width:100%") + ";" + cssVendor + "border-radius:3px;border-radius:3px";
                }
                bar.style.cssText += ";pointer-events:none;" + cssVendor + "transition-property:" + cssVendor + "transform;" + cssVendor + "transition-timing-function:cubic-bezier(0.33,0.66,0.66,1);" + cssVendor + "transition-duration:0;" + cssVendor + "transform: translate(0,0)" + translateZ;
                if (that.options.useTransition) {
                    bar.style.cssText += ";" + cssVendor + "transition-timing-function:cubic-bezier(0.33,0.66,0.66,1)";
                }
                that[dir + "ScrollbarWrapper"].appendChild(bar);
                that[dir + "ScrollbarIndicator"] = bar;
            }
            if (dir == "h") {
                that.hScrollbarSize = that.hScrollbarWrapper.clientWidth;
                that.hScrollbarIndicatorSize = m.max(m.round(that.hScrollbarSize * that.hScrollbarSize / that.scrollerW), 8);
                that.hScrollbarIndicator.style.width = that.hScrollbarIndicatorSize + "px";
                that.hScrollbarMaxScroll = that.hScrollbarSize - that.hScrollbarIndicatorSize;
                that.hScrollbarProp = that.hScrollbarMaxScroll / that.maxScrollX;
            } else {
                that.vScrollbarSize = that.vScrollbarWrapper.clientHeight;
                that.vScrollbarIndicatorSize = m.max(m.round(that.vScrollbarSize * that.vScrollbarSize / that.scrollerH), 8);
                that.vScrollbarIndicator.style.height = that.vScrollbarIndicatorSize + "px";
                that.vScrollbarMaxScroll = that.vScrollbarSize - that.vScrollbarIndicatorSize;
                that.vScrollbarProp = that.vScrollbarMaxScroll / that.maxScrollY;
            }
            that._scrollbarPos(dir, true);
        },
        _resize: function () {
            var that = this;
            setTimeout(function () {
                that.refresh();
            }, isAndroid ? 200 : 0);
        },
        _pos: function (x, y) {
            if (this.zoomed) {
                return;
            }
            x = this.hScroll ? x : 0;
            y = this.vScroll ? y : 0;
            if (this.options.useTransform) {
                this.scroller.style[transform] = "translate(" + x + "px," + y + "px) scale(" + this.scale + ")" + translateZ;
            } else {
                x = m.round(x);
                y = m.round(y);
                this.scroller.style.left = x + "px";
                this.scroller.style.top = y + "px";
            }
            this.x = x;
            this.y = y;
            this._scrollbarPos("h");
            this._scrollbarPos("v");
        },
        _scrollbarPos: function (dir, hidden) {
            var that = this,
				pos = dir == "h" ? that.x : that.y,
				size;
            if (!that[dir + "Scrollbar"]) {
                return;
            }
            pos = that[dir + "ScrollbarProp"] * pos;
            if (pos < 0) {
                if (!that.options.fixedScrollbar) {
                    size = that[dir + "ScrollbarIndicatorSize"] + m.round(pos * 3);
                    if (size < 8) {
                        size = 8;
                    }
                    that[dir + "ScrollbarIndicator"].style[dir == "h" ? "width" : "height"] = size + "px";
                }
                pos = 0;
            } else {
                if (pos > that[dir + "ScrollbarMaxScroll"]) {
                    if (!that.options.fixedScrollbar) {
                        size = that[dir + "ScrollbarIndicatorSize"] - m.round((pos - that[dir + "ScrollbarMaxScroll"]) * 3);
                        if (size < 8) {
                            size = 8;
                        }
                        that[dir + "ScrollbarIndicator"].style[dir == "h" ? "width" : "height"] = size + "px";
                        pos = that[dir + "ScrollbarMaxScroll"] + (that[dir + "ScrollbarIndicatorSize"] - size);
                    } else {
                        pos = that[dir + "ScrollbarMaxScroll"];
                    }
                }
            }
            that[dir + "ScrollbarWrapper"].style[transitionDelay] = "0";
            that[dir + "ScrollbarWrapper"].style.opacity = hidden && that.options.hideScrollbar ? "0" : "1";
            that[dir + "ScrollbarIndicator"].style[transform] = "translate(" + (dir == "h" ? pos + "px,0)" : "0," + pos + "px)") + translateZ;
        },
        _start: function (e) {
            var that = this,
				point = hasTouch ? e.touches[0] : e,
				matrix, x, y, c1, c2;
            if (!that.enabled) {
                return;
            }
            if (that.options.onBeforeScrollStart) {
                that.options.onBeforeScrollStart.call(that, e);
            }
            if (that.options.useTransition || that.options.zoom) {
                that._transitionTime(0);
            }
            that.moved = false;
            that.animating = false;
            that.zoomed = false;
            that.distX = 0;
            that.distY = 0;
            that.absDistX = 0;
            that.absDistY = 0;
            that.dirX = 0;
            that.dirY = 0;
            if (that.options.zoom && hasTouch && e.touches.length > 1) {
                c1 = m.abs(e.touches[0].pageX - e.touches[1].pageX);
                c2 = m.abs(e.touches[0].pageY - e.touches[1].pageY);
                that.touchesDistStart = m.sqrt(c1 * c1 + c2 * c2);
                that.originX = m.abs(e.touches[0].pageX + e.touches[1].pageX - that.wrapperOffsetLeft * 2) / 2 - that.x;
                that.originY = m.abs(e.touches[0].pageY + e.touches[1].pageY - that.wrapperOffsetTop * 2) / 2 - that.y;
                if (that.options.onZoomStart) {
                    that.options.onZoomStart.call(that, e);
                }
            }
            if (that.options.momentum) {
                if (that.options.useTransform) {
                    matrix = getComputedStyle(that.scroller, null)[transform].replace(/[^0-9\-.,]/g, "").split(",");
                    x = +(matrix[12] || matrix[4]);
                    y = +(matrix[13] || matrix[5]);
                } else {
                    x = +getComputedStyle(that.scroller, null).left.replace(/[^0-9-]/g, "");
                    y = +getComputedStyle(that.scroller, null).top.replace(/[^0-9-]/g, "");
                }
                if (x != that.x || y != that.y) {
                    if (that.options.useTransition) {
                        that._unbind(TRNEND_EV);
                    } else {
                        cancelFrame(that.aniTime);
                    }
                    that.steps = [];
                    that._pos(x, y);
                    if (that.options.onScrollEnd) {
                        that.options.onScrollEnd.call(that);
                    }
                }
            }
            that.absStartX = that.x;
            that.absStartY = that.y;
            that.startX = that.x;
            that.startY = that.y;
            that.pointX = point.pageX;
            that.pointY = point.pageY;
            that.startTime = e.timeStamp || Date.now();
            if (that.options.onScrollStart) {
                that.options.onScrollStart.call(that, e);
            }
            that._bind(MOVE_EV, window);
            that._bind(END_EV, window);
            that._bind(CANCEL_EV, window);
        },
        _move: function (e) {
            var that = this,
				point = hasTouch ? e.touches[0] : e,
				deltaX = point.pageX - that.pointX,
				deltaY = point.pageY - that.pointY,
				newX = that.x + deltaX,
				newY = that.y + deltaY,
				c1, c2, scale, timestamp = e.timeStamp || Date.now();
            if (that.options.onBeforeScrollMove) {
                that.options.onBeforeScrollMove.call(that, e);
            }
            if (that.options.zoom && hasTouch && e.touches.length > 1) {
                c1 = m.abs(e.touches[0].pageX - e.touches[1].pageX);
                c2 = m.abs(e.touches[0].pageY - e.touches[1].pageY);
                that.touchesDist = m.sqrt(c1 * c1 + c2 * c2);
                that.zoomed = true;
                scale = 1 / that.touchesDistStart * that.touchesDist * this.scale;
                if (scale < that.options.zoomMin) {
                    scale = 0.5 * that.options.zoomMin * Math.pow(2, scale / that.options.zoomMin);
                } else {
                    if (scale > that.options.zoomMax) {
                        scale = 2 * that.options.zoomMax * Math.pow(0.5, that.options.zoomMax / scale);
                    }
                }
                that.lastScale = scale / this.scale;
                newX = this.originX - this.originX * that.lastScale + this.x;
                newY = this.originY - this.originY * that.lastScale + this.y;
                this.scroller.style[transform] = "translate(" + newX + "px," + newY + "px) scale(" + scale + ")" + translateZ;
                if (that.options.onZoom) {
                    that.options.onZoom.call(that, e);
                }
                return;
            }
            that.pointX = point.pageX;
            that.pointY = point.pageY;
            if (newX > 0 || newX < that.maxScrollX) {
                newX = that.options.bounce ? that.x + (deltaX / 2) : newX >= 0 || that.maxScrollX >= 0 ? 0 : that.maxScrollX;
            }
            if (newY > that.minScrollY || newY < that.maxScrollY) {
                newY = that.options.bounce ? that.y + (deltaY / 2) : newY >= that.minScrollY || that.maxScrollY >= 0 ? that.minScrollY : that.maxScrollY;
            }
            that.distX += deltaX;
            that.distY += deltaY;
            that.absDistX = m.abs(that.distX);
            that.absDistY = m.abs(that.distY);
            if (that.absDistX < 6 && that.absDistY < 6) {
                return;
            }
            if (that.options.lockDirection) {
                if (that.absDistX > that.absDistY + 5) {
                    newY = that.y;
                    deltaY = 0;
                } else {
                    if (that.absDistY > that.absDistX + 5) {
                        newX = that.x;
                        deltaX = 0;
                    }
                }
            }
            that.moved = true;
            that._pos(newX, newY);
            that.dirX = deltaX > 0 ? -1 : deltaX < 0 ? 1 : 0;
            that.dirY = deltaY > 0 ? -1 : deltaY < 0 ? 1 : 0;
            if (timestamp - that.startTime > 300) {
                that.startTime = timestamp;
                that.startX = that.x;
                that.startY = that.y;
            }
            if (that.options.onScrollMove) {
                that.options.onScrollMove.call(that, e);
            }
        },
        _end: function (e) {
            if (hasTouch && e.touches.length !== 0) {
                return;
            }
            var that = this,
				point = hasTouch ? e.changedTouches[0] : e,
				target, ev, momentumX = {
				    dist: 0,
				    time: 0
				},
				momentumY = {
				    dist: 0,
				    time: 0
				},
				duration = (e.timeStamp || Date.now()) - that.startTime,
				newPosX = that.x,
				newPosY = that.y,
				distX, distY, newDuration, snap, scale;
            that._unbind(MOVE_EV, window);
            that._unbind(END_EV, window);
            that._unbind(CANCEL_EV, window);
            if (that.options.onBeforeScrollEnd) {
                that.options.onBeforeScrollEnd.call(that, e);
            }
            if (that.zoomed) {
                scale = that.scale * that.lastScale;
                scale = Math.max(that.options.zoomMin, scale);
                scale = Math.min(that.options.zoomMax, scale);
                that.lastScale = scale / that.scale;
                that.scale = scale;
                that.x = that.originX - that.originX * that.lastScale + that.x;
                that.y = that.originY - that.originY * that.lastScale + that.y;
                that.scroller.style[transitionDuration] = "200ms";
                that.scroller.style[transform] = "translate(" + that.x + "px," + that.y + "px) scale(" + that.scale + ")" + translateZ;
                that.zoomed = false;
                that.refresh();
                if (that.options.onZoomEnd) {
                    that.options.onZoomEnd.call(that, e);
                }
                return;
            }
            if (!that.moved) {
                if (hasTouch) {
                    if (that.doubleTapTimer && that.options.zoom) {
                        clearTimeout(that.doubleTapTimer);
                        that.doubleTapTimer = null;
                        if (that.options.onZoomStart) {
                            that.options.onZoomStart.call(that, e);
                        }
                        that.zoom(that.pointX, that.pointY, that.scale == 1 ? that.options.doubleTapZoom : 1);
                        if (that.options.onZoomEnd) {
                            setTimeout(function () {
                                that.options.onZoomEnd.call(that, e);
                            }, 200);
                        }
                    } else {
                        if (this.options.handleClick) {
                            that.doubleTapTimer = setTimeout(function () {
                                that.doubleTapTimer = null;
                                target = point.target;
                                while (target.nodeType != 1) {
                                    target = target.parentNode;
                                }
                                if (target.tagName != "SELECT" && target.tagName != "INPUT" && target.tagName != "TEXTAREA") {
                                    ev = doc.createEvent("MouseEvents");
                                    ev.initMouseEvent("click", true, true, e.view, 1, point.screenX, point.screenY, point.clientX, point.clientY, e.ctrlKey, e.altKey, e.shiftKey, e.metaKey, 0, null);
                                    ev._fake = true;
                                    target.dispatchEvent(ev);
                                }
                            }, that.options.zoom ? 250 : 0);
                        }
                    }
                }
                that._resetPos(400);
                if (that.options.onTouchEnd) {
                    that.options.onTouchEnd.call(that, e);
                }
                return;
            }
            if (duration < 300 && that.options.momentum) {
                momentumX = newPosX ? that._momentum(newPosX - that.startX, duration, -that.x, that.scrollerW - that.wrapperW + that.x, that.options.bounce ? that.wrapperW : 0) : momentumX;
                momentumY = newPosY ? that._momentum(newPosY - that.startY, duration, -that.y, (that.maxScrollY < 0 ? that.scrollerH - that.wrapperH + that.y - that.minScrollY : 0), that.options.bounce ? that.wrapperH : 0) : momentumY;
                newPosX = that.x + momentumX.dist;
                newPosY = that.y + momentumY.dist;
                if ((that.x > 0 && newPosX > 0) || (that.x < that.maxScrollX && newPosX < that.maxScrollX)) {
                    momentumX = {
                        dist: 0,
                        time: 0
                    };
                }
                if ((that.y > that.minScrollY && newPosY > that.minScrollY) || (that.y < that.maxScrollY && newPosY < that.maxScrollY)) {
                    momentumY = {
                        dist: 0,
                        time: 0
                    };
                }
            }
            if (momentumX.dist || momentumY.dist) {
                newDuration = m.max(m.max(momentumX.time, momentumY.time), 10);
                if (that.options.snap) {
                    distX = newPosX - that.absStartX;
                    distY = newPosY - that.absStartY;
                    if (m.abs(distX) < that.options.snapThreshold && m.abs(distY) < that.options.snapThreshold) {
                        that.scrollTo(that.absStartX, that.absStartY, 200);
                    } else {
                        snap = that._snap(newPosX, newPosY);
                        newPosX = snap.x;
                        newPosY = snap.y;
                        newDuration = m.max(snap.time, newDuration);
                    }
                }
                that.scrollTo(m.round(newPosX), m.round(newPosY), newDuration);
                if (that.options.onTouchEnd) {
                    that.options.onTouchEnd.call(that, e);
                }
                return;
            }
            if (that.options.snap) {
                distX = newPosX - that.absStartX;
                distY = newPosY - that.absStartY;
                if (m.abs(distX) < that.options.snapThreshold && m.abs(distY) < that.options.snapThreshold) {
                    that.scrollTo(that.absStartX, that.absStartY, 200);
                } else {
                    snap = that._snap(that.x, that.y);
                    if (snap.x != that.x || snap.y != that.y) {
                        that.scrollTo(snap.x, snap.y, snap.time);
                    }
                }
                if (that.options.onTouchEnd) {
                    that.options.onTouchEnd.call(that, e);
                }
                return;
            }
            that._resetPos(200);
            if (that.options.onTouchEnd) {
                that.options.onTouchEnd.call(that, e);
            }
        },
        _resetPos: function (time) {
            var that = this,
				resetX = that.x >= 0 ? 0 : that.x < that.maxScrollX ? that.maxScrollX : that.x,
				resetY = that.y >= that.minScrollY || that.maxScrollY > 0 ? that.minScrollY : that.y < that.maxScrollY ? that.maxScrollY : that.y;
            if (resetX == that.x && resetY == that.y) {
                if (that.moved) {
                    that.moved = false;
                    if (that.options.onScrollEnd) {
                        that.options.onScrollEnd.call(that);
                    }
                }
                if (that.hScrollbar && that.options.hideScrollbar) {
                    if (vendor == "webkit") {
                        that.hScrollbarWrapper.style[transitionDelay] = "300ms";
                    }
                    that.hScrollbarWrapper.style.opacity = "0";
                }
                if (that.vScrollbar && that.options.hideScrollbar) {
                    if (vendor == "webkit") {
                        that.vScrollbarWrapper.style[transitionDelay] = "300ms";
                    }
                    that.vScrollbarWrapper.style.opacity = "0";
                }
                return;
            }
            that.scrollTo(resetX, resetY, time || 0);
        },
        _wheel: function (e) {
            var that = this,
				wheelDeltaX, wheelDeltaY, deltaX, deltaY, deltaScale;
            if ("wheelDeltaX" in e) {
                wheelDeltaX = e.wheelDeltaX / 12;
                wheelDeltaY = e.wheelDeltaY / 12;
            } else {
                if ("wheelDelta" in e) {
                    wheelDeltaX = wheelDeltaY = e.wheelDelta / 12;
                } else {
                    if ("detail" in e) {
                        wheelDeltaX = wheelDeltaY = -e.detail * 3;
                    } else {
                        return;
                    }
                }
            }
            if (that.options.wheelAction == "zoom") {
                deltaScale = that.scale * Math.pow(2, 1 / 3 * (wheelDeltaY ? wheelDeltaY / Math.abs(wheelDeltaY) : 0));
                if (deltaScale < that.options.zoomMin) {
                    deltaScale = that.options.zoomMin;
                }
                if (deltaScale > that.options.zoomMax) {
                    deltaScale = that.options.zoomMax;
                }
                if (deltaScale != that.scale) {
                    if (!that.wheelZoomCount && that.options.onZoomStart) {
                        that.options.onZoomStart.call(that, e);
                    }
                    that.wheelZoomCount++;
                    that.zoom(e.pageX, e.pageY, deltaScale, 400);
                    setTimeout(function () {
                        that.wheelZoomCount--;
                        if (!that.wheelZoomCount && that.options.onZoomEnd) {
                            that.options.onZoomEnd.call(that, e);
                        }
                    }, 400);
                }
                return;
            }
            deltaX = that.x + wheelDeltaX;
            deltaY = that.y + wheelDeltaY;
            if (deltaX > 0) {
                deltaX = 0;
            } else {
                if (deltaX < that.maxScrollX) {
                    deltaX = that.maxScrollX;
                }
            }
            if (deltaY > that.minScrollY) {
                deltaY = that.minScrollY;
            } else {
                if (deltaY < that.maxScrollY) {
                    deltaY = that.maxScrollY;
                }
            }
            if (that.maxScrollY < 0) {
                that.scrollTo(deltaX, deltaY, 0);
            }
        },
        _transitionEnd: function (e) {
            var that = this;
            if (e.target != that.scroller) {
                return;
            }
            that._unbind(TRNEND_EV);
            that._startAni();
        },
        _startAni: function () {
            var that = this,
				startX = that.x,
				startY = that.y,
				startTime = Date.now(),
				step, easeOut, animate;
            if (that.animating) {
                return;
            }
            if (!that.steps.length) {
                that._resetPos(400);
                return;
            }
            step = that.steps.shift();
            if (step.x == startX && step.y == startY) {
                step.time = 0;
            }
            that.animating = true;
            that.moved = true;
            if (that.options.useTransition) {
                that._transitionTime(step.time);
                that._pos(step.x, step.y);
                that.animating = false;
                if (step.time) {
                    that._bind(TRNEND_EV);
                } else {
                    that._resetPos(0);
                }
                return;
            }
            animate = function () {
                var now = Date.now(),
					newX, newY;
                if (now >= startTime + step.time) {
                    that._pos(step.x, step.y);
                    that.animating = false;
                    if (that.options.onAnimationEnd) {
                        that.options.onAnimationEnd.call(that);
                    }
                    that._startAni();
                    return;
                }
                now = (now - startTime) / step.time - 1;
                easeOut = m.sqrt(1 - now * now);
                newX = (step.x - startX) * easeOut + startX;
                newY = (step.y - startY) * easeOut + startY;
                that._pos(newX, newY);
                if (that.animating) {
                    that.aniTime = nextFrame(animate);
                }
            };
            animate();
        },
        _transitionTime: function (time) {
            time += "ms";
            this.scroller.style[transitionDuration] = time;
            if (this.hScrollbar) {
                this.hScrollbarIndicator.style[transitionDuration] = time;
            }
            if (this.vScrollbar) {
                this.vScrollbarIndicator.style[transitionDuration] = time;
            }
        },
        _momentum: function (dist, time, maxDistUpper, maxDistLower, size) {
            var deceleration = 0.0006,
				speed = m.abs(dist) / time,
				newDist = (speed * speed) / (2 * deceleration),
				newTime = 0,
				outsideDist = 0;
            if (dist > 0 && newDist > maxDistUpper) {
                outsideDist = size / (6 / (newDist / speed * deceleration));
                maxDistUpper = maxDistUpper + outsideDist;
                speed = speed * maxDistUpper / newDist;
                newDist = maxDistUpper;
            } else {
                if (dist < 0 && newDist > maxDistLower) {
                    outsideDist = size / (6 / (newDist / speed * deceleration));
                    maxDistLower = maxDistLower + outsideDist;
                    speed = speed * maxDistLower / newDist;
                    newDist = maxDistLower;
                }
            }
            newDist = newDist * (dist < 0 ? -1 : 1);
            newTime = speed / deceleration;
            return {
                dist: newDist,
                time: m.round(newTime)
            };
        },
        _offset: function (el) {
            var left = -el.offsetLeft,
				top = -el.offsetTop;
            while (el = el.offsetParent) {
                left -= el.offsetLeft;
                top -= el.offsetTop;
            }
            if (el != this.wrapper) {
                left *= this.scale;
                top *= this.scale;
            }
            return {
                left: left,
                top: top
            };
        },
        _snap: function (x, y) {
            var that = this,
				i, l, page, time, sizeX, sizeY;
            page = that.pagesX.length - 1;
            for (i = 0, l = that.pagesX.length; i < l; i++) {
                if (x >= that.pagesX[i]) {
                    page = i;
                    break;
                }
            }
            if (page == that.currPageX && page > 0 && that.dirX < 0) {
                page--;
            }
            x = that.pagesX[page];
            sizeX = m.abs(x - that.pagesX[that.currPageX]);
            sizeX = sizeX ? m.abs(that.x - x) / sizeX * 500 : 0;
            that.currPageX = page;
            page = that.pagesY.length - 1;
            for (i = 0; i < page; i++) {
                if (y >= that.pagesY[i]) {
                    page = i;
                    break;
                }
            }
            if (page == that.currPageY && page > 0 && that.dirY < 0) {
                page--;
            }
            y = that.pagesY[page];
            sizeY = m.abs(y - that.pagesY[that.currPageY]);
            sizeY = sizeY ? m.abs(that.y - y) / sizeY * 500 : 0;
            that.currPageY = page;
            time = m.round(m.max(sizeX, sizeY)) || 200;
            return {
                x: x,
                y: y,
                time: time
            };
        },
        _bind: function (type, el, bubble) {
            (el || this.scroller).addEventListener(type, this, !!bubble);
        },
        _unbind: function (type, el, bubble) {
            (el || this.scroller).removeEventListener(type, this, !!bubble);
        },
        destroy: function () {
            var that = this;
            that.scroller.style[transform] = "";
            that.hScrollbar = false;
            that.vScrollbar = false;
            that._scrollbar("h");
            that._scrollbar("v");
            that._unbind(RESIZE_EV, window);
            that._unbind(START_EV);
            that._unbind(MOVE_EV, window);
            that._unbind(END_EV, window);
            that._unbind(CANCEL_EV, window);
            if (!that.options.hasTouch) {
                that._unbind("DOMMouseScroll");
                that._unbind("mousewheel");
            }
            if (that.options.useTransition) {
                that._unbind(TRNEND_EV);
            }
            if (that.options.checkDOMChanges) {
                clearInterval(that.checkDOMTime);
            }
            if (that.options.onDestroy) {
                that.options.onDestroy.call(that);
            }
        },
        refresh: function () {
            var that = this,
				offset, i, l, els, pos = 0,
				page = 0;
            if (that.scale < that.options.zoomMin) {
                that.scale = that.options.zoomMin;
            }
            that.wrapperW = that.wrapper.clientWidth || 1;
            that.wrapperH = that.wrapper.clientHeight || 1;
            that.minScrollY = -that.options.topOffset || 0;
            that.scrollerW = m.round(that.scroller.offsetWidth * that.scale);
            that.scrollerH = m.round((that.scroller.offsetHeight + that.minScrollY) * that.scale);
            that.maxScrollX = that.wrapperW - that.scrollerW;
            that.maxScrollY = that.wrapperH - that.scrollerH + that.minScrollY;
            that.dirX = 0;
            that.dirY = 0;
            if (that.options.onRefresh) {
                that.options.onRefresh.call(that);
            }
            that.hScroll = that.options.hScroll && that.maxScrollX < 0;
            that.vScroll = that.options.vScroll && (!that.options.bounceLock && !that.hScroll || that.scrollerH > that.wrapperH);
            that.hScrollbar = that.hScroll && that.options.hScrollbar;
            that.vScrollbar = that.vScroll && that.options.vScrollbar && that.scrollerH > that.wrapperH;
            offset = that._offset(that.wrapper);
            that.wrapperOffsetLeft = -offset.left;
            that.wrapperOffsetTop = -offset.top;
            if (typeof that.options.snap == "string") {
                that.pagesX = [];
                that.pagesY = [];
                els = that.scroller.querySelectorAll(that.options.snap);
                for (i = 0, l = els.length; i < l; i++) {
                    pos = that._offset(els[i]);
                    pos.left += that.wrapperOffsetLeft;
                    pos.top += that.wrapperOffsetTop;
                    that.pagesX[i] = pos.left < that.maxScrollX ? that.maxScrollX : pos.left * that.scale;
                    that.pagesY[i] = pos.top < that.maxScrollY ? that.maxScrollY : pos.top * that.scale;
                }
            } else {
                if (that.options.snap) {
                    that.pagesX = [];
                    while (pos >= that.maxScrollX) {
                        that.pagesX[page] = pos;
                        pos = pos - that.wrapperW;
                        page++;
                    }
                    if (that.maxScrollX % that.wrapperW) {
                        that.pagesX[that.pagesX.length] = that.maxScrollX - that.pagesX[that.pagesX.length - 1] + that.pagesX[that.pagesX.length - 1];
                    }
                    pos = 0;
                    page = 0;
                    that.pagesY = [];
                    while (pos >= that.maxScrollY) {
                        that.pagesY[page] = pos;
                        pos = pos - that.wrapperH;
                        page++;
                    }
                    if (that.maxScrollY % that.wrapperH) {
                        that.pagesY[that.pagesY.length] = that.maxScrollY - that.pagesY[that.pagesY.length - 1] + that.pagesY[that.pagesY.length - 1];
                    }
                }
            }
            that._scrollbar("h");
            that._scrollbar("v");
            if (!that.zoomed) {
                that.scroller.style[transitionDuration] = "0";
                that._resetPos(400);
            }
        },
        scrollTo: function (x, y, time, relative) {
            var that = this,
				step = x,
				i, l;
            that.stop();
            if (!step.length) {
                step = [{
                    x: x,
                    y: y,
                    time: time,
                    relative: relative
                }];
            }
            for (i = 0, l = step.length; i < l; i++) {
                if (step[i].relative) {
                    step[i].x = that.x - step[i].x;
                    step[i].y = that.y - step[i].y;
                }
                that.steps.push({
                    x: step[i].x,
                    y: step[i].y,
                    time: step[i].time || 0
                });
            }
            that._startAni();
        },
        scrollToElement: function (el, time) {
            var that = this,
				pos;
            el = el.nodeType ? el : that.scroller.querySelector(el);
            if (!el) {
                return;
            }
            pos = that._offset(el);
            pos.left += that.wrapperOffsetLeft;
            pos.top += that.wrapperOffsetTop;
            pos.left = pos.left > 0 ? 0 : pos.left < that.maxScrollX ? that.maxScrollX : pos.left;
            pos.top = pos.top > that.minScrollY ? that.minScrollY : pos.top < that.maxScrollY ? that.maxScrollY : pos.top;
            time = time === undefined ? m.max(m.abs(pos.left) * 2, m.abs(pos.top) * 2) : time;
            that.scrollTo(pos.left, pos.top, time);
        },
        scrollToPage: function (pageX, pageY, time) {
            var that = this,
				x, y;
            time = time === undefined ? 400 : time;
            if (that.options.onScrollStart) {
                that.options.onScrollStart.call(that);
            }
            if (that.options.snap) {
                pageX = pageX == "next" ? that.currPageX + 1 : pageX == "prev" ? that.currPageX - 1 : pageX;
                pageY = pageY == "next" ? that.currPageY + 1 : pageY == "prev" ? that.currPageY - 1 : pageY;
                pageX = pageX < 0 ? 0 : pageX > that.pagesX.length - 1 ? that.pagesX.length - 1 : pageX;
                pageY = pageY < 0 ? 0 : pageY > that.pagesY.length - 1 ? that.pagesY.length - 1 : pageY;
                that.currPageX = pageX;
                that.currPageY = pageY;
                x = that.pagesX[pageX];
                y = that.pagesY[pageY];
            } else {
                x = -that.wrapperW * pageX;
                y = -that.wrapperH * pageY;
                if (x < that.maxScrollX) {
                    x = that.maxScrollX;
                }
                if (y < that.maxScrollY) {
                    y = that.maxScrollY;
                }
            }
            that.scrollTo(x, y, time);
        },
        disable: function () {
            this.stop();
            this._resetPos(0);
            this.enabled = false;
            this._unbind(MOVE_EV, window);
            this._unbind(END_EV, window);
            this._unbind(CANCEL_EV, window);
        },
        enable: function () {
            this.enabled = true;
        },
        stop: function () {
            if (this.options.useTransition) {
                this._unbind(TRNEND_EV);
            } else {
                cancelFrame(this.aniTime);
            }
            this.steps = [];
            this.moved = false;
            this.animating = false;
        },
        zoom: function (x, y, scale, time) {
            var that = this,
				relScale = scale / that.scale;
            if (!that.options.useTransform) {
                return;
            }
            that.zoomed = true;
            time = time === undefined ? 200 : time;
            x = x - that.wrapperOffsetLeft - that.x;
            y = y - that.wrapperOffsetTop - that.y;
            that.x = x - x * relScale + that.x;
            that.y = y - y * relScale + that.y;
            that.scale = scale;
            that.refresh();
            that.x = that.x > 0 ? 0 : that.x < that.maxScrollX ? that.maxScrollX : that.x;
            that.y = that.y > that.minScrollY ? that.minScrollY : that.y < that.maxScrollY ? that.maxScrollY : that.y;
            that.scroller.style[transitionDuration] = time + "ms";
            that.scroller.style[transform] = "translate(" + that.x + "px," + that.y + "px) scale(" + scale + ")" + translateZ;
            that.zoomed = false;
        },
        isReady: function () {
            return !this.moved && !this.zoomed && !this.animating;
        }
    };

    function prefixStyle(style) {
        if (vendor === "") {
            return style;
        }
        style = style.charAt(0).toUpperCase() + style.substr(1);
        return vendor + style;
    }
    dummyStyle = null;
    if (typeof exports !== "undefined") {
        exports.iScroll = iScroll;
    } else {
        window.iScroll = iScroll;
    }
})(window, document);
var code = [
	["#ff5252", "#22ebd6"],
	["#ffd21d", "#8a6fff"],
	["#fe702b", "#296aff"]
];
var timer = function (index) {
    index < 3 ? ($(".cube1").css("backgroundColor", code[index][0]), $(".cube2").css("backgroundColor", code[index][1]), index++) : index = 0, setTimeout(function () {
        timer(index);
    }, 450);
};
var myScroll;

function loaded() {
    myScroll = new iScroll("wrapper", {
        checkDOMChanges: true,
        hScrollbar: false,
        hScroll: false,
        useTransform: false,
        onBeforeScrollStart: function (e) {
            var target = e.target;
            while (target.nodeType != 1) {
                target = target.parentNode;
            }
            if (target.tagName != "SELECT" && target.tagName != "INPUT" && target.tagName != "TEXTAREA") {
                e.preventDefault();
            }
        }
    });
}
timer(0);
document.addEventListener("WeixinJSBridgeReady", function () {
    WeixinJSBridge.invoke("hideToolbar");
});
loaded();
$(".spinner").css({
    left: ($(window).width() - 32) / 2,
    top: ($(window).height() - 32) / 2
});
var current = "";
var hasmove = false;

function showicontips() {
    if (mySwiper.activeIndex > 0) {
        hasmove = true;
    }
    if (!hasmove) {
        $(".icontips").show().addClass("animate3 fadeInRight");
        setTimeout(function () {
            $(".icontips").removeClass("animate3 fadeInRight");
            $(".icontips").fadeOut();
        }, 1500);
    }
}
var mySwiper;

function hideSpinner() {
    $(".spinner").hide();
    $(".swiper-container").show().addClass("animate0 fadeInRight");
    mySwiper = new Swiper(".swiper-container", {
        pagination: ".pagination",
        paginationClickable: true,
        slidesPerView: "auto"
    });
    document.addEventListener("touchmove", function (e) {
        e.preventDefault();
    }, false);
    $("#wrapper").fadeIn();
    $("#section-sumary").show().addClass("animate3 bounceIn");
    setInterval(showicontips, 2000);
    current = "section-sumary";
    $(".swiper-item").click(function () {
        var o = $(this);
        var id = o.attr("attr");
        var effect = o.attr("effect");
        if (id != current) {
            if (current == "section-place") {
                $("#" + current).hide();
            } else {
                $("#" + current).fadeOut();
            }
            $("#" + id).show().addClass("animate3 " + effect);
            myScroll.scrollTo(0, 0, 100);
            current = id;
        }
    });
}
setTimeout(hideSpinner, 200);

function gotoapply() {
    $("#" + current).fadeOut();
    $("#section-apply").show().addClass("animate3 bounceInRight");
    myScroll.scrollTo(0, 0, 100);
    current = "section-apply";
}
function initguestface() {
    $("img.guest-avatar").each(function () {
        var img = $(this);
        var src = img.attr("_src");
        img.attr("src", src);
    });
}
function checkform(f) {
    if (f.username.value == "") {
        alert("姓名没有填写哦！");
        f.username.focus();
        myScroll.scrollTo(0, 0, 100);
        return false;
    } else {
        if (f.email.value == "") {
            alert("邮箱没有填写哦！");
            f.email.focus();
            myScroll.scrollTo(0, 0, 100);
            return false;
        } else {
            if (f.phone.value == "") {
                alert("手机没有填写哦！");
                f.phone.focus();
                myScroll.scrollTo(0, 0, 100);
                return false;
            }
        }
    }
}
setTimeout(initguestface, 2000);
//(function () {
//    var onBridgeReady = function () {
//        WeixinJSBridge.on("menu:share:appmessage", function (argv) {
//            WeixinJSBridge.invoke("sendAppMessage", {
//                "appid": dataForWeixin.appId,
//                "img_url": dataForWeixin.MsgImg,
//                "img_width": "120",
//                "img_height": "120",
//                "link": dataForWeixin.url,
//                "desc": dataForWeixin.desc,
//                "title": dataForWeixin.title
//            }, function (res) {
//                (dataForWeixin.callback)();
//            });
//        });
//        WeixinJSBridge.on("menu:share:timeline", function (argv) {
//            (dataForWeixin.callback)();
//            WeixinJSBridge.invoke("shareTimeline", {
//                "img_url": dataForWeixin.MsgImg,
//                "img_width": "120",
//                "img_height": "120",
//                "link": dataForWeixin.url,
//                "desc": dataForWeixin.desc,
//                "title": dataForWeixin.title
//            }, function (res) {
//                (dataForWeixin.callback)();
//            });
//        });
//        WeixinJSBridge.on("menu:share:weibo", function (argv) {
//            WeixinJSBridge.invoke("shareWeibo", {
//                "content": dataForWeixin.weibodesc + " " + dataForWeixin.url,
//                "url": dataForWeixin.url
//            }, function (res) {
//                (dataForWeixin.callback)();
//            });
//        });
//        WeixinJSBridge.on("menu:share:facebook", function (argv) {
//            WeixinJSBridge.invoke("shareFB", {
//                "img_url": dataForWeixin.MsgImg,
//                "img_width": "120",
//                "img_height": "120",
//                "link": dataForWeixin.url,
//                "desc": dataForWeixin.desc,
//                "title": dataForWeixin.title
//            }, function (res) { });
//        });
//    };
//    if (document.addEventListener) {
//        document.addEventListener("WeixinJSBridgeReady", onBridgeReady, false);
//    } else {
//        if (document.attachEvent) {
//            document.attachEvent("WeixinJSBridgeReady", onBridgeReady);
//            document.attachEvent("onWeixinJSBridgeReady", onBridgeReady);
//        }
//    }
//})();