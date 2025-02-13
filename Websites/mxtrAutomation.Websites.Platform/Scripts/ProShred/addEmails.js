$(document).ready(function () {
    var max_fields = 3; //maximum input boxes allowed
    var wrapper = $(".input_fields_wrap"); //Fields wrapper
    var add_button = $(".add_field_button"); //Add button ID

    var x = 1; //initlal text box count
    $(add_button).click(function (e) { //on add input button click
        e.preventDefault();
        var email = "Email" + x;
        if (x < max_fields) { //max input box allowed
            x++; //text box increment
            $(wrapper).append('<div class="form-group" id="divEmail_' + x + '"><div class="input-group"><input class="form-control" type="text" name="Email[' + x + ']"  id="Email_' + x + '"/><span class="input-group-btn"> <button class="btn btn-primary remove_field" type="button">X</button></span></div></div>'); //add input box  
        } else if (x === 3) {
            alert("You can only add 3 emails!");
        }
    });

    $(wrapper).on("click",
        ".remove_field",
        function (e) { //user click on remove text
            e.preventDefault();
            $(this).parent().parent().parent().remove();
            x--;
        });
});