﻿(function (cheroes, $) {

    'use strict';

    var cssOpen = 'open';

    $.fn.dropdownify = function (options) {

        return this.each(function () {

            var trigger = $(this);
            var parent = trigger.parent();

            var eventData = {
                parent: parent,
                trigger: trigger
            };

            parent
                .on('click', eventData, toggle)
                .on('keydown', eventData, onParentKeyDown);

            $(document)
                .on('click', eventData, onClick)
                .on('keydown', eventData, onDocKeyDown);
        });

    };

    function closeMenu(parent, trigger) {
        parent.removeClass(cssOpen);
        trigger.attr('aria-expanded', false);
    }

    function onClick(event) {
        if ($.contains(event.data.parent[0], event.target)) {
            return;
        }
        closeMenu(event.data.parent, event.data.trigger);
    }

    function onDocKeyDown(event) {
        if (event.which == cheroes.KEYS.ESC) {
            return closeMenu(event.data.parent, event.data.trigger);
        }
    }

    function onParentKeyDown(event) {

        if (event.which == cheroes.KEYS.UP || event.which == cheroes.KEYS.DOWN) {

            event.preventDefault();

            var items = event.data.parent.find('.dropdown-menu li:not(.disabled):visible a');
            var index = items.index(event.target);

            if (event.which == cheroes.KEYS.UP && index > 0) {
                index--;
            }

            if (event.which == cheroes.KEYS.DOWN && (index < items.length - 1)) {
                index++;
            }

            if (index < 0) {
                index = 0;
            }

            items.eq(index).trigger('focus');

        }

    }

    function toggle(event) {
        event.data.trigger.attr('aria-expanded', !event.data.parent.hasClass(cssOpen));
        event.data.parent.toggleClass(cssOpen);
        event.data.trigger.trigger('focus');
    }

}(this.cheroes = this.cheroes || {}, jQuery));
