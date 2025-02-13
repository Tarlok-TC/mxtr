; (function ($, window, document, undefined) {
    $.fn.calendar360 = function (options) {
        var defaults = {
            //start: moment().subtract('months', 1),
            start: _filterGraph.startdate != "" ? _filterGraph.startdate : (moment().subtract(1, 'months')),
            end: _filterGraph.enddate != "" ? _filterGraph.enddate : moment(),
            today: moment(),
            dateFormat: 'MM/DD/YYYY',
            titleFormat: 'MMM. DD, YYYY',
            presets: {
                'custom': {
                    text: 'Custom',
                    isDefault: true,
                    timeshift: function () { }
                }
            },
            post: function () { }
        };



        var settings = $.extend({}, defaults, options);
        settings.presets.pastThirtyDays.isDefault = false;
        settings.presets = $.extend({}, defaults.presets, options.presets);

        // Make sure dates are moment objects
        settings.start = createMomentObj(settings.start);
        settings.end = createMomentObj(settings.end);
        settings.today = createMomentObj(settings.today);

        // Keep track of active date range for cleanup / etc
        // Make sure they are momemnt objects too
        var activeDateRange = {
            start: settings.start,
            end: settings.end,
            today: settings.today
        };

        // Keep track of clicks for selecting days
        // Starts as true because it will be first click on render
        var isFirstClick = true;

        // SETUP VAR FOR SELECTED PRESET
        var activePreset = '';

        $(this).html(init(activeDateRange)).addClass('m-daterange');


        ///////////
        // CLICK FUNCTIONS
        // SETUP ACTIONS

        var actions = {
            open: function () {
                openCalendar(this);
            },
            close: function () {
                closeCalendar(this);
            },
            prev: function () {
                slideMonths(true);
            },
            next: function () {
                slideMonths(false);
            },
            selectDay: function () {
                selectDay(this);
            },
            apply: function () {
                setNewDateRange(settings.post);
            }
        };


        // BIND ACTIONS TO ELEMENTS
        // CLICK ACTIONS
        $(this).on('click', 'a[data-action], button[data-action]', function (event) {

            var link = $(this),
                action = link.data('action');

            event.preventDefault();

            // If there's an action with the given name, call it
            if (typeof actions[action] === "function") {
                actions[action].call(link, event);
            }
        });

        //BLUR ACTIONS - move out of date range input
        $(this).on('focusout', 'input.daterange', function (event) {

            if ($(this).val() != '') {

                $('#DateRangePresets').val('custom').trigger('chosen:updated');

                var dateRange = {
                    start: createMomentObj($('#DateRangeStart').val()),
                    end: createMomentObj($('#DateRangeEnd').val())
                };

                updateDateInputs(dateRange);

            }
        });


        // CHANGE ACTIONS - when drop down value changes
        $(this).on('change', '#DateRangePresets', function (event) {

            var selectedVal = $(this).val();
            var selectedPreset = settings.presets[selectedVal];

            if (selectedVal != 'custom') {
                var dateRange = {
                    start: selectedPreset.timeshift(settings.today.clone()),
                    end: settings.today
                };

                if (dateRange.start && dateRange.end) {

                    updateDateInputs(dateRange);

                }

            } else {
                $('a.m-day').removeClass('m-day-selected-last m-day-selected-first m-day-selected-same m-day-selected');
                $('#DateRangeStart').val('');
                $('#DateRangeEnd').val('');
            }
        });


        // CLICK OUTSIDE CALENDAR, CLOSE/CANCEL
        $(document).click(function (event) {
            var target = $(event.target);
            // SEE IF CLICK IS OUTSIDE PARENT
            if (!target.parents('.m-daterange').length) {
                closeCalendar();
            }
        });

        $(document).on('focus', 'input.daterange', function (e) {
            $(this).keyup(function (e) {
                if (e.keyCode == 13) {
                    setNewDateRange(settings.post);
                }
            });
        });

        $(document).keyup(function (e) {
            if (e.keyCode == 27) { closeCalendar(); }   // esc
        });




        ///////////
        // UI FUNCTIONS - SHOW / HIDE / SLIDE ETC
        // showCalendar
        function openCalendar(element, event) {
            var html = buildCalendar();

            $('.m-daterange-slider').html(html);

            $('.m-daterange-menu').fadeIn(200);
            $('#DateRangePresets').chosen({ disable_search_threshold: 10 });
            $(this).data('open', 'true');
            $('.m-daterange-select').addClass('is-active').data('action', 'close');
        }


        function closeCalendar(event) {
            $('.m-daterange-menu').fadeOut(200, function () {
                $('#DateRangePresets').val(activePreset).trigger('chosen:updated');
                $('#DateRangeStart').val(activeDateRange.start.format(settings.dateFormat));
                $('#DateRangeEnd').val(activeDateRange.end.format(settings.dateFormat));
                hideErrors();
            });
            $(this).data('open', 'false');
            $('.m-daterange-select').removeClass('is-active').data('action', 'open');
        }


        function slideMonths(isPast) {
            var item_width = 235;
            var newMonth = '';

            if (isPast) {
                var left_indent = parseInt($('ul.m-daterange-months').css('left')) + item_width;
                newMonth = createMomentObj($('ul.m-daterange-months li:first').data('moment'));
                //newMonth.subtract('months', 1);
                newMonth.subtract(1, 'months');
            } else {
                var left_indent = parseInt($('ul.m-daterange-months').css('left')) - item_width;
                newMonth = createMomentObj($('ul.m-daterange-months li:last').data('moment'));
                //newMonth.add('months', 1);
                newMonth.add(1, 'months');
            }

            $('ul.m-daterange-months').animate(
                { 'left': left_indent }, 50,
                function () {

                    if (isPast) {
                        $('ul.m-daterange-months').prepend(buildMonthHTML(newMonth));
                        $('ul.m-daterange-months li:last-child').remove();
                    } else {
                        $('ul.m-daterange-months').append(buildMonthHTML(newMonth));
                        $('ul.m-daterange-months li:first-child').remove();
                    }

                    var left = parseInt($('ul.m-daterange-months').css('left'));
                    if (isPast) {
                        $('ul.m-daterange-months').css({ 'left': (left - item_width) });
                    } else {
                        $('ul.m-daterange-months').css({ 'left': (left + item_width) });
                    }
                }
            );
        }


        // CLICKING DAYS
        function selectDay(element) {


            var isFirstClick = whichClick();
            var clickDate = createMomentObj(element.data('date'));

            if (clickDate.isAfter(moment())) {
                return false;
            }

            $('#DateRangePresets').val('custom').trigger('chosen:updated');

            var isStartDateValid = isNotNullorInvalid(createMomentObj($('#DateRangeStart').val()));
            var isEndDateValid = isNotNullorInvalid(createMomentObj($('#DateRangeEnd').val()));

            if (isFirstClick) {

                if (!isStartDateValid && isEndDateValid) {
                    var dateRange = {
                        start: clickDate,
                        end: getEndDate()
                    };

                    if (clickDate.isBefore(dateRange.end) || clickDate.isSame(dateRange.end)) {

                        $('a.m-day').removeClass('m-day-selected-first m-day-selected m-day-selected-same');
                        element.addClass('m-day-selected-first');

                        if (clickDate.isSame(dateRange.end)) {
                            element.addClass('m-day-selected-same');
                        }

                        updateDateInputs(dateRange, true);

                        updateSelectedDays();
                    }
                } else {

                    var dateRange = {
                        start: clickDate,
                        end: null
                    };

                    $('a.m-day').removeClass('m-day-selected-first m-day-selected-last m-day-selected m-day-selected-same');
                    element.addClass('m-day-selected-first');
                    updateDateInputs(dateRange, true);
                }

            } else {

                var dateRange = {
                    start: getStartDate(),
                    end: clickDate
                };

                if (clickDate.isAfter(dateRange.start) || clickDate.isSame(dateRange.start)) {

                    element.addClass('m-day-selected-last');

                    if (clickDate.isSame(dateRange.start)) {
                        element.addClass('m-day-selected-same');
                    }

                    updateDateInputs(dateRange, true);
                    updateSelectedDays();

                }
            }
        }




        ///////////
        // BUILD FUNCTIONS - to output HTML
        // UPDATE DATE RANGE TRIGGER TEXT
        function updateDisplayRange(dateRangeObj) {
            var dates = dateRangeObj;
            var html = '<a class="m-report-filter-click btn-datepicker btn btn-default m-daterange-select" data-action="open"><span class="m-daterange-select-date">' + dates.start.format(settings.titleFormat) + ' - ' + dates.end.format(settings.titleFormat) + '</span><span class="fa fa-caret-down grey-down-icon"></span></a>';
            return html;
        }


        // BUILD OUT PRESENTS DROPDOWN
        function buildPresetsDropDown(presets) {
            var html = '<select id="DateRangePresets" name="DateRangePresets" tabindex="1">';
            var items = presets;

            // LOOP THRU PRESETS TO BUILD
            $.each(items, function (key, value) {
                if (value.isDefault) {
                    html += '<option value="' + key + '" selected="selected">' + value.text + '</option>';
                    activePreset = key;
                } else {
                    html += '<option value="' + key + '">' + value.text + '</option>';
                }
            });

            //close html tags & add label
            html += '</select><label for="DateRangePresets">Date Range</label>';

            return html;
        }


        // BUILD OUT DATE RANGE INPUTS - start /end
        function buildDateRangeInputHTML(dateRange) {

            var inputStartHTML = '<input type="text" id="DateRangeStart" tabindex="2" name="DateRangeStart" value="' + dateRange.start.format(settings.dateFormat) + '" class="daterange" /><label for="DateRangeStart">Starting</label>';
            var inputEndHTML = '<input type="text" id="DateRangeEnd" tabindex="3" name="DateRangeEnd" value="' + dateRange.end.format(settings.dateFormat) + '"  class="daterange"/><label for="DateRangeEnd">Ending</label>';
            var html = '<div class="m-daterange-start">' + inputStartHTML + '</div><span class="m-daterange-divider">to</span><div class="m-daterange-end">' + inputEndHTML + '</div>';

            return html;
        }

        // BUILD CALENDER
        function buildCalendar() {
            var html = '<ul class="m-daterange-months">';
            var displayMonths = createDisplayMonths();

            $.each(displayMonths, function (key, value) {
                html += buildMonthHTML(value);
            });

            html += '</ul>';

            return html;
        }


        // Generate a Month HTML
        function buildMonthHTML(mDate) {
            var monthTitle = mDate.format('MMMM YYYY');
            var totalDaysInMonth = mDate.daysInMonth();
            var firstDayOfMonth = mDate.startOf('month').day();

            // START MONTH CONTAINER AND OUTPUT MONTH TITLE [Month Year]
            var html = '<li class="m-daterange-month-container" data-moment="' + mDate.format(settings.dateFormat) + '"><table class="m-month">';
            html += '<tr><th colspan="7">' + monthTitle + '</th></tr>';

            var week = 0;

            // CREATE EMPTY TDs FOR BEGINING OF MONTH
            if (firstDayOfMonth != 0) {
                html += '<tr>';
                for (var i = 0; i < firstDayOfMonth; i++) {
                    html += '<td> </td>';
                    week++;
                }
            }

            // CREATE DAYS OF MONTH
            for (var i = 1; i <= totalDaysInMonth; i++) {
                var dayCSS = getDayCss(mDate.date(i));

                if (week == 0) {
                    html += '<tr>';
                }

                html += '<td><a class="' + dayCSS + '" data-action="selectDay" data-date="' + mDate.date(i).format(settings.dateFormat) + '">' + i + '</a></td>';
                week++

                if (week == 7) {
                    html += '</tr>';
                    week = 0;
                }
            }

            // CLOSE TABLE & MONTH
            html += '</table></li>';

            return html;
        }




        ///////////
        // UTILITY FUNCTIONS

        // Create a moment object
        function createMomentObj(date) {
            if (date) {
                return moment(date, settings.dateFormat);
            } else {
                return null;
            }
        }


        // Create display months
        function createDisplayMonths() {
            var months = new Array();
            var mEndDate = getEndDate();
            //var mMonth = mEndDate.clone().add('months', 1);
            var mMonth = mEndDate.clone().add(1, 'months');

            for (var i = 0; i < 5; i++) {
                months.unshift(mMonth.clone());
                //mMonth = mMonth.subtract('months', 1);
                mMonth = mMonth.subtract(1, 'months');
            }

            return months;
        }

        function getEndDate() {
            var mEndDate = createMomentObj($('#DateRangeEnd').val());
            if (mEndDate) {
                if (mEndDate.isValid()) {
                    return mEndDate;
                } else {
                    return activeDateRange.end;
                }
            } else {
                return null;
            }
        }

        function getStartDate() {
            var mStartDate = createMomentObj($('#DateRangeStart').val());
            if (mStartDate) {
                if (mStartDate.isValid()) {
                    return mStartDate;
                } else {
                    return activeDateRange.start;
                }
            } else {
                return null;
            }
        }

        function getDayCss(mDate) {
            var css = 'm-day';
            var dateRange = {
                start: getStartDate(),
                end: getEndDate(),
                today: moment()
            };

            var isCurrentMonthAndYear = false;

            if (settings.today.month() == mDate.month() && settings.today.year() == mDate.year()) {
                isCurrentMonthAndYear = true;
            }

            if (isCurrentMonthAndYear && settings.today.date() == mDate.date()) {
                css += ' m-today';
            }

            if (mDate.isAfter(settings.today.clone())) {
                css += ' is-disabled';
            } else {

                if (dateRange.start && dateRange.end) {

                    if (dateRange.end.isSame(mDate) && dateRange.start.isSame(mDate)) {
                        css += ' m-day-selected-same';
                    }

                    if (mDate.isAfter(dateRange.start) && mDate.isBefore(dateRange.end)) {
                        css += ' m-day-selected';
                    }
                }

                if (dateRange.start) {

                    if (dateRange.start.isSame(mDate)) {
                        css += ' m-day-selected-first';
                    }

                }

                if (dateRange.end) {

                    if (dateRange.end.isSame(mDate)) {
                        css += ' m-day-selected-last';
                    }

                }
            }


            return css;
        }

        function whichClick() {
            var mStartDate = createMomentObj($('#DateRangeStart').val());
            var mEndDate = createMomentObj($('#DateRangeEnd').val());

            var isStartDate = isNotNullorInvalid(mStartDate);
            var isEndDate = isNotNullorInvalid(mEndDate);

            if ((isStartDate && isEndDate) || (!isStartDate && !isEndDate)) {
                return true;
            } else if (!isStartDate && isEndDate) {
                return true;
            } else if (isStartDate && !isEndDate) {
                return false;
            } else {
                return true;
            }
        }

        function isNotNullorInvalid(mDate) {
            if (mDate && mDate.isValid()) {
                return true;
            } else {
                return false;
            }
        }

        // SETUP ERROR TRACKING FOR MESSAGES
        var isStartError = false,
            isEndError = false;

        function updateDateInputs(dateRange, isClickEvent) {

            var isValidStartDate = isNotNullorInvalid(dateRange.start);
            var isValidEndDate = isNotNullorInvalid(dateRange.end);

            // IF BOTH ARE VALID DATES, DETERMINE IF VALID DATE RANGE, AND NEITHER DATE IS IN THE FUTURE
            if (isValidStartDate && isValidEndDate) {

                // EVALUATE START DATE
                if ((dateRange.start.isBefore(settings.today) || dateRange.start.isSame(settings.today)) && (dateRange.start.isBefore(dateRange.end) || dateRange.end.isSame(dateRange.start))) {

                    isStartError = false;
                    $('#DateRangeStart').val(dateRange.start.format(settings.dateFormat));

                } else {

                    isStartError = true;

                }

                //EVALUATE END DATE
                if ((dateRange.end.isBefore(settings.today) || dateRange.end.isSame(settings.today)) && (dateRange.start.isBefore(dateRange.end) || dateRange.start.isSame(dateRange.end))) {

                    isEndError = false;
                    $('#DateRangeEnd').val(dateRange.end.format(settings.dateFormat));

                } else {

                    isEndError = true;

                }

                // CHECK FOR CLICK EVENT, ERRORS, UPDATE CALENDAR
                if (!isClickEvent && !isStartError && !isEndError) {

                    $('.m-daterange-slider').html(buildCalendar());

                }

            } else { //BOTH DATES AREN'T VALID, EVALUATE EACH INVDIVIDUALLY

                if (isValidStartDate) {

                    if (isClickEvent) {

                        isStartError = false;
                        $('#DateRangeStart').val(dateRange.start.format(settings.dateFormat));
                        $('#DateRangeEnd').val('');

                    } else {

                        if (dateRange.start.isBefore(settings.today)) {

                            isStartError = false;
                            $('#DateRangeStart').val(dateRange.start.format(settings.dateFormat));

                        } else {

                            isStartError = true;

                        }
                    }

                } else {

                    isStartError = true;

                }

                if (isValidEndDate && !isClickEvent) {

                    if (dateRange.end.isBefore(settings.today) || dateRange.end.isSame(settings.today)) {
                        isEndError = false;
                        $('DateRangeEnd').val(dateRange.end.format(settings.dateFormat));

                    } else {

                        isEndError = true;

                    }
                } else if (!isValidEndDate && !isClickEvent) {

                    isEndError = true;
                }

            }

            displayErrors();
        }



        function updateSelectedDays() {
            $('a.m-day').each(function () {
                var mDate = createMomentObj($(this).data('date'));
                var css = getDayCss(mDate);
                $(this).prop('class', css);
            });
        }


        function setNewDateRange(callback) {
            var start = isNotNullorInvalid(createMomentObj($('#DateRangeStart').val()));
            var end = isNotNullorInvalid(createMomentObj($('#DateRangeEnd').val()));

            if (start && end) {

                if (!isStartError && !isEndError) {
                    hideErrors();
                    activeDateRange.start = getStartDate();
                    activeDateRange.end = getEndDate();
                    activePreset = $('#DateRangePresets').val();
                    $('.m-daterange-select').replaceWith(updateDisplayRange(activeDateRange));
                    closeCalendar();
                    callback(activeDateRange);
                }

            } else {

                if (!start) {

                    isStartError = true;

                }

                if (!end) {

                    isEndError = true;

                }
            }

            displayErrors();
        }



        function displayErrors() {

            if (!isStartError && !isEndError) {
                $('.m-daterange-error').fadeOut(200);
                $('#DateRangeStart').removeClass('is-error').next('label').removeClass('is-error');
                $('#DateRangeEnd').removeClass('is-error').next('label').removeClass('is-error');

            } else {
                if (isStartError && isEndError) {

                    $('.m-daterange-error').text('Sorry, invalid start and end dates. (MM/DD/YYYY)').fadeIn(200);
                    $('#DateRangeStart').addClass('is-error').next('label').addClass('is-error');
                    $('#DateRangeEnd').addClass('is-error').next('label').addClass('is-error');

                } else {

                    if (isStartError) {

                        $('.m-daterange-error').text('Sorry, invalid start date. (MM/DD/YYYY)').fadeIn(200);
                        $('#DateRangeStart').addClass('is-error').next('label').addClass('is-error');

                    } else {

                        $('#DateRangeStart').removeClass('is-error').next('label').removeClass('is-error');

                    }

                    if (isEndError) {

                        $('.m-daterange-error').text('Sorry, invalid end date. (MM/DD/YYYY)').fadeIn(200);
                        $('#DateRangeEnd').addClass('is-error').next('label').addClass('is-error');

                    } else {

                        $('#DateRangeEnd').removeClass('is-error').next('label').removeClass('is-error');

                    }

                }
            }

            //isStartError = false;
            //isEndError = false;
        }



        function hideErrors() {
            $('.m-daterange-error').fadeOut(200);
            $('#DateRangeStart').removeClass('is-error').next('label').removeClass('is-error');
            $('#DateRangeEnd').removeClass('is-error').next('label').removeClass('is-error');
            isStartError = false;
            isEndError = false;
        }



        ///////////////
        // INIT()
        // Used to Build Initial HTML
        function init(activeDateRangeObj) {

            // create var, build 'select' UI to show daterange / trigger
            var selectHTML = updateDisplayRange(activeDateRange);

            // BUILD SLIDER HTML CONTAINER
            // DON't OUTPUT <ul> BECAUSE BUILD CALENDAR WILL DO THAT
            var sliderHTML = '<div class="m-daterange-month-slider-container"><div class="m-daterange-slider"></div><a class="m-daterange-prev btn btn-default" data-action="prev"><span class="fa fa-caret-left"></span></a><a class="m-daterange-next btn btn-default" data-action="next"><span class="fa fa-caret-right"></span></a></div>';

            // BUILD CONTROLS CONTAINER AND ITEMS
            var dropDownHTML = '<div class="m-daterange-presets">';
            dropDownHTML += buildPresetsDropDown(settings.presets);
            dropDownHTML += '</div>';

            var dateRangeInputHTML = '<div class="m-daterange-inputs">';
            dateRangeInputHTML += buildDateRangeInputHTML(activeDateRange);
            dateRangeInputHTML += '</div>';

            var controlsHTML = '<div class="m-daterange-controls cf"><div class="m-daterange-error"><</div>' + dropDownHTML + dateRangeInputHTML +
            '<div class="m-daterange-btns"><button class="btn btn-info" data-action="apply" tabindex="4">Apply</button><button class="btn btn-default" data-action="close" tabindex="5">Cancel</button></div>';

            // BUILD WRAPPER HTML
            var wrapperHTML = '<div class="m-daterange-menu">' + sliderHTML + controlsHTML + '</div>';

            var html = selectHTML + wrapperHTML;

            return html;
        }

        // returns the jQuery object to allow for chainability.
        return this;
    }

})(jQuery, window, document);