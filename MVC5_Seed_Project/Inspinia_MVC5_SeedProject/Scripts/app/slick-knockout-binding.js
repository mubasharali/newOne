ko.bindingHandlers.slick = (function () {
    var createImageDiv = function (imgUrl) {

        $a = $('<a>');
        $a.attr('href', imgUrl);
        //$a.attr('data-gallery', '#blueimp-gallery');
        $a.attr('target', '_blank');
        $div = $('<div>');
        $image = $('<img>');
        $image.attr('src', imgUrl);
        // $div.append($image);
        $a.append($image);
        $div.append($a);
        return $div;
    };

    // Initializes the carousel
    var init = function (element, valueAccessor, allBindingsAccessor) {
        // Clears the div
        $(element).empty();
        // Creates the inner divs with images
        var images = ko.unwrap(valueAccessor());
        if (images) {
            images.forEach(function (imgUrl) {
                $(element).append(createImageDiv(imgUrl));
            });
        }

        // try to recover slickOptions
        var options = allBindingsAccessor().slickOptions || {};

        // Initialize slick on the div, with provided options
        $(element).slick(options);

        //handle disposal, if KO removes the element
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            $(element).slick('unslick');
        });

        // check if there is slickIndex
        var slickIndex = allBindingsAccessor().slickIndex;
        if (slickIndex) {
            $(element).slick('slickGoTo', ko.unwrap(slickIndex));
        }
        // If it's obervable, subscribe to its changes
        if (ko.isObservable(slickIndex)) {
            slickIndex.subscribe(function (idx) {
                $(element).slick('slickGoTo', idx);
            });
        }
        // It if's writable observable, update when slick current index changes
        if (ko.isWritableObservable(slickIndex)) {
            $(element).on('afterChange', function (evt, slick, pos) {
                slickIndex(pos);
            })
        }

        // Check if the array of images is an observable array
        var imagesArray = valueAccessor();
        // If it's observable array, subscribe to changes
        if (ko.isObservable(imagesArray) && 'destroyAll' in imagesArray) {
            imagesArray.subscribe(function (changes) {
                console.log(changes);
                if (changes) {
                    changes.forEach(function (change) {
                        if (change.status == 'added') {
                            // Add new img div at index
                            var imgDiv = createImageDiv(change.value)[0];
                            var index = change.index;
                            var slideCount = $(element).slick('getSlick').slideCount;
                            // if the index is out of range (which can happen
                            // with push, and splice), include at the end
                            var addBefore = true;
                            if (index >= slideCount) {
                                index = slideCount - 1;
                                addBefore = false;
                            }
                            $(element).slick('slickAdd', imgDiv, index, addBefore);
                        } else if (change.status == 'deleted') {
                            // delete img div at index
                            $(element).slick('slickRemove', change.index);
                        }
                    });
                }
            }, null, 'arrayChange');
        } // if observable array
    }

    //update the control when the view model changes
    var update = function (element, valueAccessor) {
        var images = ko.unwrap(valueAccessor());
        // Do something to update the content
        console.log('update');
    }

    return { init: init, update: update };
})();