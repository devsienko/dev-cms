﻿(function ($) {
    $(function() {
        setActiveTab();
        setDeleteConfirmations();

        function setDeleteConfirmations() {
            $('.delete-link').click(function(e) {
                e.preventDefault();
                var addressValue = $(this).attr("href");
                if (confirm("Remove выбранный элемент?")) {
                    window.location.href = addressValue;
                }
            });
        }

        function setActiveTab() {
            var currentPageUrl = getCurrentPageRoute();
            var isMenuItemFound;
            var menuItems = $('.main-menu-admin>a');
            menuItems.each(function(index, menuItem) {
                var linkHref = $(menuItem).attr('href').substr(1); //remove '/'
                if (linkHref === currentPageUrl) {
                    $(menuItem).addClass('active');
                    isMenuItemFound = true;
                }
            });

            if (!isMenuItemFound && currentPageUrl == 'Admin') {
                $(".main-menu-admin>a[href='/Admin/Notifications']").addClass('active');
            }
        }

        function getCurrentPageRoute() {
            var getUrl = window.location;
            var baseUrl = getUrl.protocol + "//" + getUrl.host + "/"; // + getUrl.pathname.split('/')[1]

            var resultLength = window.location.href.length - baseUrl.length;
            var result = window.location.href.substr(baseUrl.length, resultLength);
            return result;
        }
    });
})(jQuery);