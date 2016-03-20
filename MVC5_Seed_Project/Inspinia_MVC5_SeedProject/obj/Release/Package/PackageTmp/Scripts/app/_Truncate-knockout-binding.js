ko.bindingHandlers.truncatedText = {
    update: function (element, valueAccessor, allBindingsAccessor) {
        var originalText = ko.utils.unwrapObservable(valueAccessor()),
            // 10 is a default maximum length
            length = ko.utils.unwrapObservable(allBindingsAccessor().maxTextLength) || 20,
            truncatedText = originalText.length > length ? originalText.substring(0, length) + " ..." : originalText;
        // updating text binding handler to show truncatedText
        ko.bindingHandlers.text.update(element, function () {
            return truncatedText;
        });
    }
};