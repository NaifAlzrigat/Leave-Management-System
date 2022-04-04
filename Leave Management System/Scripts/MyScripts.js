function ShowImagePreview(ImageUpload, ImagePreview) {
    if (ImageUpload.files && ImageUpload.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $(ImagePreview).attr('src', e.target.result);
        }
        reader.readAsDataURL(ImageUpload.files[0]);
    }
}

//function jQuryAjaxPost(form) {
//    $.validator.unobtrusive.parse(form);
//    if ($(form).valid()) {
//        var ajaxConfig = {
//            type: 'Post',
//            url: form.action,
//            data: new FormData(form),
//            success: function (response) {
//                $('#tab1').html(response);
//                refreshAddNewTab($(form).attr('data-restUrl'), true);
//            }
//        }
//        if ($(form).attr('enctype') == "multipart/form-data") {
//            ajaxConfig['contentType'] = false;
//            ajaxConfig['processData'] = false;
//        }
//        $.ajax(ajaxConfig);
//    }
//    return false;
//}

//function refreshAddNewTab(restUrl, showViewTab) {
//    $.ajax({
//        type: 'Get',
//        url: restUrl,
//        success: function (respnse) {
//            $("#tab2").html(respnse);
//            $('ul.nav.nav-tabs a:eq(1)').html('Add New');
//            if (showViewTab) {
//                $('ul.nav.nav-tabs a:eq(0)').tab('show');

//            }
//        }
//    })
//}


//$(function () {
//    $("#datepicker1").datepicker();
//});

//$(function () {
//    $("#datepicker2").datepicker();
//});

//$("#TestCondition").click(function () {
//    if ($("#datepicker2").val() < $("#datepicker1").val()) {
//        alert("End Date must come after Start Date!");
//    }
//});

//function test () {
//    if ($("#datepicker2").val() < $("#datepicker1").val()) {
//        $("TestCondition").hide();
//    } else {

//        $("TestCondition").hide();
//    }

//};
//var d1 = null;
//var d2 = null;
//$('#From_Date').change(function () {
//    d1=$('#From_Date').val();
//});
//$('#To_Date').change(function () {
//    d2 = $('#From_Date').val();

//});

//if (d1 < Date.now() || d1 > d2) {
//    $("TestCondition").hide();
//}