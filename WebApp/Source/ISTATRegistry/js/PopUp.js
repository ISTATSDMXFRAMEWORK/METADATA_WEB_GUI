var glbUsingCallback = null;
var glbCallback = null;
var currentActiveIds = new Array();

$(document).ready(function () {

    //Close Popups and Fade Layer
    $('a.close').live('click', function () {
        //When clicking on the close or fade layer...
        if (currentActiveIds.length > 0) {
            var currentId = currentActiveIds.pop().toString();

            $('#fade , #' + currentId).fadeOut(function () {
                $('#fade, #' + currentId + "CloseButton").remove();
                if (glbUsingCallback) {
                    glbCallback();
                }
            });
        }
        $.unblockUI();
        //ResetBeforeUnload();

        return false;
    });

});

function openPopUp(popID, popWidth, usingCallback, callback) {
    if ($.inArray(popID, currentActiveIds) == -1) {
        currentActiveIds.push(popID);
    }
    //console.log( "apro " + popID );
    if (popWidth === undefined) popWidth = 350;

    //Fade in the Popup and add close button
    $('#' + popID).fadeIn().css({ 'width': Number(popWidth) }).prepend('<a href="#" class="close" id= "' + popID + 'CloseButton"><img src="images/close_pop.png" class="btn_close" title="Close Window" alt="Close" /></a>');

    //Define margin for center alignment (vertical + horizontal) - we add 80 to the height/width to accomodate for the padding + border width defined in the css
    var popMargTop = ($('#' + popID).height() + 80) / 2;
    var popMargLeft = ($('#' + popID).width() + 80) / 2;

    //Apply Margin to Popup
    $('#' + popID).css({
        'margin-top': -popMargTop,
        'margin-left': -popMargLeft
    });

    //Fade in Background
    $('body').append('<div id="fade"></div>'); //Add the fade layer to bottom of the body tag.
    $('#fade').css({ 'filter': 'alpha(opacity=80)' }).fadeIn(); //Fade in the fade layer

    if (usingCallback) 
    {
        glbUsingCallback = true;
        glbCallback = callback;
    }
    else 
    {
        glbUsingCallback = false;
        glbCallback = "";
    }

    return false;
}

function closePopup() {
    $('#fade , .popup_block').fadeOut(function () {
        $('#fade, a.close').remove();
    });

    return false;
}