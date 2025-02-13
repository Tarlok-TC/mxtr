function ShowMessage(message) {
    //swal(message);
    swal({
        html: message,
        allowOutsideClick: false
    });
}

function SuccessAlert(title, message) {
    //swal(title, message, 'success');
    swal({
        title: title,
        html: message,
        type: 'success',
        allowOutsideClick: false
    });
}

function WarningAlert(title, message) {
    //swal(title, message, 'warning');
    swal({
        title: title,
        html: message,
        type: 'warning',
        allowOutsideClick: false
    });
}

function ErrorAlert(title, Message) {
    //swal(title, Message, 'error');
    swal({
        title: title,
        html: Message,
        type: 'error',
        allowOutsideClick: false
    });
}

function InfoAlert(title, Message) {
    //swal(title, Message, 'info');
    swal({
        title: title,
        html: Message,
        type: 'info',
        allowOutsideClick: false
    });
}

function QuestionAlert(title, Message, SuccessFunction, CancelFunction, confirmText, cancelText) {
    if (typeof confirmText == 'undefined')
        confirmText = 'Yes';
    if (typeof cancelText == 'undefined')
        cancelText = 'No';
    swal({
        title: title,
        html: Message,
        type: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: confirmText,
        cancelButtonText: cancelText,
        allowOutsideClick: false
    }).then(function () {
        SuccessFunction();
    }, CancelFunction)
}

function SubmitAjaxAlert(title, inputType, inputValue,message, preConfirmFunction, SuccessFunction) {
    swal({
        title: title,
        input: inputType,
        inputValue: inputValue,
        text:message,
        showCancelButton: true,
        confirmButtonText: 'Submit',
        showLoaderOnConfirm: true,
        preConfirm: function (userInput) {
            return new Promise(function (resolve, reject) {
                preConfirmFunction(resolve, reject, userInput)
            })
        },
        allowOutsideClick: false
    }).then(function (userInput) {
        SuccessFunction(userInput);
    })
}