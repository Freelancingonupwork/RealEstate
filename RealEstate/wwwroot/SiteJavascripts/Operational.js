
function __glb_fnIsNullOrEmpty(SourceValue) 
{
    
    if (SourceValue == null)
        return true;

    // It's not null (something) and it's not of type 'string'. 
    if (typeof (SourceValue) != "string") 
        return false;

    // It's string.
    return (SourceValue == '');
}

function __glb_validateEmail(email)
{
    const re = /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/;
    return re.test(String(email).toLowerCase());
}

function __glb_validatePhoneNumber(phonenumber)
{
    var re = /^\d{10}$/;
    return re.test(phonenumber);
}



function __glb_fnIUDOperation(formData, ControlURL)
{
    var result;
 
    $.ajax({
        type: "POST",
        url: ControlURL,
        async: false,
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (response) {
            result = response;
            
        },
        error: function () {
            result = { "success": false, "message": "Error!" };
        }
    });
    
    return result;
}



function __glb_fnGetValidGsonDateTime(strDate)
{
    var format = "dd/mm/yyyy hh:MM TT";
    return strDate.format2(format);
}



