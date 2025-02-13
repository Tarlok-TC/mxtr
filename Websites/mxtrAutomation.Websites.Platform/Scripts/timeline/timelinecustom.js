// Timeline functions
var timeline;
var timeline_control = new Object();
var eventToItemMapping = {};
var lastEventId = 1;
var prevSelectedDotDiv = null;
/**
 * Zoom
 * @param zoomVal
 */
timeline_control.zoom = function (zoomVal) {
    timeline.zoom(zoomVal);
    timeline.trigger("rangechange");
    timeline.trigger("rangechanged");
}

/**
 * Adjust the visible time range such that all events are visible.
 */
function adjustVisibleTimeRangeToAccommodateAllEvents() {
    timeline.setVisibleChartRangeAuto();
}

/**
 * Move
 * @param moveVal
 */
timeline_control.move = function (moveVal) {
    timeline.move(moveVal);
    timeline.trigger("rangechange");
    timeline.trigger("rangechanged");
}

/**
 * Move the visible range such that the current time is located in the center of the timeline.
 */
function moveToCurrentTime() {
    timeline.setVisibleChartRangeNow();
}
function bindEventHoverEvent() {
    $('.event').on("mouseenter", function (event) {
        $('.timeline-event-dot').removeClass("hovered");
        var id = $(this).attr('id').replace(/[^\d]+/img, '');

        $('.timeline-event-dot-copied').removeClass("hovered");
        var id = $(this).attr('id').replace(/[^\d]+/img, '');

        //console.log(event, ' - ', id);
        eventToItemMapping[id].forEach(function (el) {
            $(el).addClass('hovered');
        });
    });

    $('.event').on("mouseleave", function (event) {
        var id = $(this).attr('id').replace(/[^\d]+/img, '');
        eventToItemMapping[id].forEach(function (el) {
            $(el).removeClass('hovered');
        });
    });
}

function mouseoverItemEventCallback(eventId) {
    $('.event').removeClass("highlighted");
    $('#event_id_' + eventId).addClass("highlighted");
}

function clickItemEventCallback(eventId, dotDiv) {
    $('.timeline-event-dot').removeClass("selected");
    $('.timeline-event-dot-copied').removeClass("selected");
    if ($(prevSelectedDotDiv)) {
        $(prevSelectedDotDiv).removeClass("selected");
    }
    $(dotDiv).addClass("selected");
    prevSelectedDotDiv = dotDiv;
    $('.events').scrollTo('#event_id_' + eventId, { duration: 'slow' });
}

function itemToEventMapCallback(eventId, dotDiv, eventIds) {
    eventIds.push(eventId);
    eventIds.forEach(function (evtId) {

        if (eventToItemMapping.hasOwnProperty(evtId)) {
            eventToItemMapping[evtId].push(dotDiv);
        } else {
            eventToItemMapping[evtId] = [dotDiv];
        }

    });
    //console.log("eventToItemMapping : ", eventToItemMapping);
}

function itemArialLabelCallback(divDot, className, eventDate, content) {
    var eventType = (className == 'green') ? "Ok" :
        (
            (className == 'green-m') ? "Maintenance" :
                (
                    (className == 'yellow') ? "Warning" : "Error"
                )
        );
    eventType = className;
    divDot.setAttribute("aria-label", "Event type " + eventType + " on " + moment(eventDate).format("DD/MM/YYYY"));
}

function init_timeline(eventData) {
    // alert(divBox.className);
    // console.log("#########Test", eventData);
    if ($('#mytimeline').length) {
        var timelinedata = [];
        var htmlcontent = "";
        $.each(eventData, function (index, event) {
            // alert(event.IsCopied);
            var eventDate = new Date(event.CreateTimestamp);
            //var dd = eventDate.getDate();
            //var mm = eventDate.getMonth() + 1;
            //var yyyy = eventDate.getFullYear();
            var formattedDate = moment(event.CreateTimestamp).fromNow();
            event.EventID = event.EventID ? event.EventID : index + 1;
            timelinedata[index] = {
                'start': eventDate,
                'content': event.EventDescription,
                'className': event.IsCopied == true ? event.EventType + "-copied" : event.EventType,
                'event_id': event.EventID,
                'IsCopied': event.IsCopied
            }
            var detail = '';
            var eventType = event.EventType;
            if (eventType == 'pageVisit') {
                if (event.IsCopied) {
                    eventType = 'pageVisit-copied'
                }
                detail = ' ' + '<span class="visitTo"><a target="_blank" href="http://' + event.WhatID + '">' + event.WhatID + '</a></span>';
            }
            if (!eventType || (eventType != 'pageVisit' && eventType != 'form' && eventType != 'email')) {
                if (event.IsCopied) {
                    eventType = eventType ? "timeline-event-dot-copied " + eventType : "timeline-event-dot-copied";
                }
                else {
                    eventType = eventType ? "timeline-event-dot " + eventType : "timeline-event-dot";
                }
            }
            else {
                if (event.IsCopied) {
                    eventType = eventType + "-copied";
                }
            }
            //htmlcontent += '<a href="javascript:void(0)">  <div class="mail_list event"  id="event_id_' + event.EventID + '" >    <div class="left">    <div class="' + eventType + ' label pull-left">&nbsp;&nbsp;</div>    </div>    <div class="right">    <h3>' + event.EventDescription + ' <small>' + formattedDate + '</small></h3>' + detail + ' </div> </div></a>';
            //htmlcontent += '<div class="timeline-detail">  <div class="mail_list event"  id="event_id_' + event.EventID + '" >    <div class="left">    <div class="' + eventType + ' label pull-left">&nbsp;&nbsp;</div>    </div>    <div class="right">    <span>' + event.EventDescription + '  <small>' + formattedDate + '</small></span><span class=text-right>' + event.LeadAccountName + '</span>' + detail + ' </div> </div></div>';
            var fulltextClass = '';
            if ($.trim(detail) == '') {
                fulltextClass = 'full-detailswidth';
            }
            if (!String.prototype.endsWith) {
                String.prototype.endsWith = function (suffix) {
                    return this.indexOf(suffix, this.length - suffix.length) !== -1;
                };
            }
            if (event.EventDescription.endsWith(".")) {
                event.EventDescription = event.EventDescription.substring(0, event.EventDescription.length - 1);
            }
            htmlcontent += '<div class="timeline-detail">  <div class="mail_list event"  id="event_id_' + event.EventID + '" >    <div class="left">    <div class="' + eventType + ' label pull-left">&nbsp;&nbsp;</div>    </div>    <div class="right">    <span class=' + fulltextClass + '>' + event.EventDescription + ' ' + formattedDate + '</span>' + detail + '<span class="text-right pull-right">' + event.LeadAccountName + '</span></div> </div></div>';


        });
        document.getElementById('eventcontainer').innerHTML = htmlcontent;
        // specify options
        var options = {
            'width': '100%',
            'height': '125px',
            'start': moment().subtract(30, 'days').calendar(),
            'end': new Date(),
            'cluster': true,
            'clusterMaxItems': 1
        };
        // Instantiate our timeline object.
        timeline = new links.Timeline(document.getElementById('mytimeline'), options);

        // Draw our timeline with the created data and options
        timeline.draw(timelinedata);
        bindEventHoverEvent();
        $(window).resize(function () {
            timeline.redraw();
        });
        //alert(55);
        //setTimeout(function () {
        //    adjustVisibleTimeRangeToAccommodateAllEvents();
        //}, 3000);
    }
}